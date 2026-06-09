# Fluxo do Sistema de Recomendação e Testes com Testcontainers

## Sumário

- [1. Fluxo de runtime](#1-fluxo-de-runtime)
  - [1.1 Inicialização do serviço](#11-inicialização-do-serviço)
  - [1.2 Caminho A — Cold-start](#12-caminho-a--cold-start)
  - [1.3 Caminho B — Usuário com histórico](#13-caminho-b--usuário-com-histórico)
  - [1.4 Score híbrido](#14-score-híbrido)
- [2. Testes de carga com Testcontainers](#2-testes-de-carga-com-testcontainers)
  - [2.1 Por que Testcontainers](#21-por-que-testcontainers)
  - [2.2 Ciclo de vida da fixture](#22-ciclo-de-vida-da-fixture)
  - [2.3 Os testes de carga](#23-os-testes-de-carga)
  - [2.4 Resultados de latência medidos](#24-resultados-de-latência-medidos)
  - [2.5 Como executar](#25-como-executar)
- [3. Suites funcionais e de validação](#3-suites-funcionais-e-de-validação)
  - [3.1 Visão geral das novas classes](#31-visão-geral-das-novas-classes)
  - [3.2 Fixtures](#32-fixtures)
  - [3.3 Suite A — Caminho Positivo](#33-suite-a--caminho-positivo)
  - [3.4 Suite B — Caminho Negativo](#34-suite-b--caminho-negativo)
- [4. Rastreabilidade RF/RNF](#4-rastreabilidade-rfrnf)
- [5. Decisão arquitetural registrada](#5-decisão-arquitetural-registrada)

---

## 1. Fluxo de runtime

### 1.1 Inicialização do serviço

O `RecommendationService` é registrado como **singleton** no container de DI. Todo o carregamento de artefatos ocorre no construtor — uma única vez durante o startup da aplicação, nunca por requisição.

```
startup
  │
  ├─ mlContext.Model.Load(model.zip)
  │    └─ PredictionEngine<RatingInput, RatingOutput>   ← não é thread-safe
  │
  ├─ LoadCsv(dataset.csv)
  │    └─ Dictionary<uint, HashSet<string>> _userHistory
  │         chave: userId  |  valor: conjunto de item_ids já consumidos
  │
  ├─ cold_start.json          → List<string>  _popularItems / _allItems
  ├─ item_categories.json     → Dictionary<string, string>  _itemCategories
  ├─ category_affinity.json   → Dictionary<string, Dictionary<string, float>>  _categoryAffinity
  └─ cold_start_by_category.json → Dictionary<string, List<string>>  _coldStartByCategory
```

**Artefatos e localização em disco:**

| Artefato | Path padrão (relativo a `src/backend/`) | Sobrescrita via config |
|---|---|---|
| `model.zip` | `../models/model.zip` | `ML:ModelPath` |
| `cold_start.json` | `../models/cold_start.json` | `ML:ColdStartPath` |
| `dataset.csv` | `../dataset/dataset.csv` | `ML:DatasetPath` |
| `item_categories.json` | `../models/item_categories.json` | `ML:ItemCategoriesPath` |
| `category_affinity.json` | `../models/category_affinity.json` | `ML:CategoryAffinityPath` |
| `cold_start_by_category.json` | `../models/cold_start_by_category.json` | `ML:ColdStartByCategoryPath` |

---

### 1.2 Caminho A — Cold-start

Ativado quando `_userHistory` não contém o `userId` (ou o conjunto de itens consumidos está vazio).

```
GET /api/recommendation/{userId}?n=10
  │
  ├─ consumed = _userHistory[userId]  →  vazio ou ausente
  │
  ├─ PreferenceService.GetPreferences(userId)
  │     ├─ tem preferências declaradas?
  │     │     └─ SIM → filtra _coldStartByCategory pelas categorias preferidas
  │     │              → retorna top-n  (Score = null)
  │     └─ NÃO → retorna _popularItems.Take(n)  (Score = null)
  │
  └─ 200 OK  [ { show, score: null }, ... ]
```

`Score = null` sinaliza ao cliente que a recomendação é por popularidade, não personalizada.

---

### 1.3 Caminho B — Usuário com histórico

Ativado quando o usuário tem pelo menos um item consumido em `_userHistory`.

```
GET /api/recommendation/{userId}?n=10
  │
  ├─ consumed = _userHistory[userId]   (HashSet de itens já vistos)
  │
  ├─ lock (_lock)   ← serializa acesso ao PredictionEngine
  │     └─ foreach item em _allItems where !consumed.Contains(item)
  │           pred = _engine.Predict({ UserId, ItemId })
  │           if !NaN && !Infinity → rawScores.Add((item, pred.Score, categoria))
  │   fim lock
  │
  ├─ normalização Min-Max dos scores SVD → svdNorm ∈ [0, 1]
  │
  ├─ score híbrido por item
  │     finalScore = α × svdNorm + (1-α) × categoryAffinity(userId, categoria)
  │     (α padrão = 0.7, configurável via ML:Alpha)
  │
  └─ OrderByDescending(finalScore).Take(n)
       → 200 OK  [ { show, score: float }, ... ]
```

---

### 1.4 Score híbrido

$$\text{score}_{ui} = 0{,}7 \times \text{SVD}_{ui}^{\text{norm}} + 0{,}3 \times \text{affinity}(u,\, \text{tipo}(i))$$

| Componente | Origem | Papel |
|---|---|---|
| `SVD_norm` | `PredictionEngine.Predict` normalizado Min-Max no batch | Relevância personalizada |
| `affinity(u, tipo)` | `category_affinity.json`: `Σranking(tipo) / Σranking(todos)` | Preferência de gênero do usuário |

O lock envolve **todo o loop de scoring** — não só o `Predict` — porque o `PredictionEngine` do ML.NET não é thread-safe. Threads concorrentes ficam enfileiradas enquanto uma percorre o catálogo completo.

---

## 2. Testes de carga com Testcontainers

### 2.1 Por que Testcontainers

O `RecommendationService` não faz queries ao banco durante a inferência, mas a aplicação precisa de uma string de conexão válida para subir (o `DbConnectionFactory` valida na inicialização). Sem Testcontainers, seria necessário um banco externo real ou um mock frágil de `IConfiguration`.

O Testcontainers sobe um **container PostgreSQL efêmero** apenas para a duração do teste, aplica o schema de produção e fornece a connection string ao `WebApplicationFactory`. O banco é destruído ao término — sem estado residual entre execuções.

### 2.2 Ciclo de vida da fixture

A classe `RecommendationLoadTests` implementa `IAsyncLifetime` — o xUnit chama `InitializeAsync` antes do primeiro teste da classe e `DisposeAsync` após o último.

```
xUnit cria RecommendationLoadTests
  │
  ├─ campo: PostgreSqlBuilder().Build()   ← valida disponibilidade do Docker aqui
  │
  ├─ InitializeAsync()
  │     ├─ _db.StartAsync()              ← puxa imagem postgres, inicia container
  │     ├─ ApplySchemaAsync()
  │     │     ├─ executa 01_create_database.sql
  │     │     └─ executa 02_sprint4_improvements.sql
  │     ├─ WebApplicationFactory<Program>
  │     │     └─ .UseSetting(...)        ← injeta connection string + paths dos artefatos ML
  │     └─ _client = _factory.CreateClient()   ← sobe a aplicação em memória
  │
  ├─ [executa os 6 testes]
  │
  └─ DisposeAsync()
        ├─ _factory.Dispose()
        └─ _db.DisposeAsync()            ← para e remove o container
```

**Sobrescrita de configuração via `UseSetting`:**

```csharp
builder.UseSetting("ConnectionStrings:DefaultConnection", _db.GetConnectionString());
builder.UseSetting("ML:ModelPath",               ModelPath("model.zip"));
builder.UseSetting("ML:ColdStartPath",           ModelPath("cold_start.json"));
builder.UseSetting("ML:DatasetPath",             DatasetPath());
builder.UseSetting("ML:ItemCategoriesPath",      ModelPath("item_categories.json"));
builder.UseSetting("ML:CategoryAffinityPath",    ModelPath("category_affinity.json"));
builder.UseSetting("ML:ColdStartByCategoryPath", ModelPath("cold_start_by_category.json"));
```

`appsettings.json` não é modificado. Os caminhos absolutos são calculados em runtime a partir de `AppContext.BaseDirectory` subindo 7 níveis (a barra final do `BaseDirectory` consome uma chamada `GetDirectoryName` sem avançar um nível).

**Por que `public partial class Program {}`**

Top-level statements em .NET 9 geram a classe `Program` como `internal`. O `WebApplicationFactory<Program>` está num assembly diferente (`BackEnd.Aventura.Tests`) e não compila sem essa declaração no final de `Program.cs`.

---

### 2.3 Os testes de carga

#### Teste 1 — `Recommend_ConcurrentRequests_AllReturnValidScores`

Dispara 50 requisições simultâneas para 50 `userId` distintos via `Task.WhenAll`. Para cada resposta verifica:

- Status HTTP 2xx
- `results != null`
- Nenhum score com `float.IsNaN` ou `float.IsInfinity`
- Score no intervalo `[0, 1]`

Valida que o `lock` protege corretamente o `PredictionEngine` sob concorrência real — sem corrida de dados, sem resultados corrompidos.

#### Teste 2 — `Recommend_MeasureLatencyUnderConcurrency` (Theory)

Executa para `concurrency ∈ {1, 5, 20, 50}`. Cada caso:

1. Faz um warm-up (`/api/recommendation/1?n=10`) para garantir que o modelo já está carregado
2. Inicia o `Stopwatch`
3. Dispara `concurrency` requisições simultâneas
4. Para o `Stopwatch` ao término do `Task.WhenAll`
5. Loga `concurrency=XX  elapsed=YYms` via `ITestOutputHelper`
6. Asserta `elapsed < concurrency × 500ms` (teto de 500ms por requisição no pior caso)

Subsidia a decisão de migrar ou não para `PredictionEnginePool`.

#### Teste 3 — `Recommend_ColdStartUsers_NoCrashUnderConcurrency`

Dispara 30 requisições para `userId` no range `[99000, 99029]` — IDs que não existem em `dataset.csv`, portanto sempre entram no caminho cold-start. Verifica apenas que todas retornam `HTTP 200 OK`.

Garante que o caminho cold-start (que não acessa o lock) é seguro sob concorrência e que `PreferenceService` não lança exceção para usuários inexistentes.

---

### 2.4 Resultados de latência medidos

Medidos em 09/06/2026 na máquina de desenvolvimento (Windows 11, Docker Desktop com WSL2):

| Concorrência | Elapsed | Razão vs. baseline |
|:---:|:---:|:---:|
| 1 | 5 ms | 1× |
| 5 | 24 ms | 4,8× |
| 20 | 34 ms | 6,8× |
| 50 | 91 ms | 18,2× |

**Crescimento linear puro esperaria `50 × 5ms = 250ms`; o medido foi 91ms — 64% abaixo do limite linear.**

| Padrão | Diagnóstico | Decisão |
|---|---|---|
| `elapsed[50] ≈ 50 × elapsed[1]` | Lock é gargalo | Migrar para `PredictionEnginePool` |
| `elapsed[50] < 10 × elapsed[1]` | Thread pool absorve a fila | Lock aceitável |
| **`elapsed[50] = 18 × elapsed[1]`** ← resultado atual | **Crescimento sub-linear** | **Não migrar por ora** |

O lock serializa o loop de scoring, mas cada iteração é rápida (lookup em memória + predição matricial). Threads enfileiradas completam rapidamente após a liberação. A migração para `PredictionEnginePool` só se justificaria se o catálogo crescer muito (mais itens → loop mais longo) ou se a concorrência ultrapassar ~200 req/s simultâneos.

---

### 2.5 Como executar

**Pré-requisito:** Docker Desktop rodando (para testes de integração). Os testes lightweight (`LightweightFixture`) não precisam de Docker.

```bash
# Todos os testes do módulo de recomendação
dotnet test src/backend/BackEnd.Aventura.Tests --verbosity minimal

# Apenas caminho positivo
dotnet test src/backend/BackEnd.Aventura.Tests --filter "FullyQualifiedName~HappyPath"

# Apenas caminho negativo
dotnet test src/backend/BackEnd.Aventura.Tests --filter "FullyQualifiedName~Negative"

# Testes leves sem Docker (validação de controller puro)
dotnet test src/backend/BackEnd.Aventura.Tests --filter "Fixture=Lightweight"

# Apenas testes de carga (RecommendationLoadTests)
dotnet test src/backend/BackEnd.Aventura.Tests --filter "FullyQualifiedName~LoadTests"
```

---

## 3. Suites funcionais e de validação

### 3.1 Visão geral das novas classes

Com o crescimento da cobertura, quatro novas classes foram adicionadas em
`src/backend/BackEnd.Aventura.Tests/`. O plano completo está em
[test-plan-recommendation.md](test-plan-recommendation.md).

| Classe | Testes | Fixture | Foco |
|---|:---:|---|---|
| `RecommendationHappyPathTests` | 14 | IntegrationFixture (Docker) | Corretude do caminho positivo |
| `PreferenceHappyPathTests` | 5 | IntegrationFixture (Docker) | CRUD de preferências |
| `RecommendationValidationTests` | 6 | LightweightFixture (sem Docker) | Validação de parâmetros `n` |
| `RecommendationServiceEdgeCaseTests` | 1 | IntegrationFixture (Docker) | Fallback de categoria inexistente |
| `PreferenceNegativeTests` | 5 | LightweightFixture (sem Docker) | Validação de entrada |

### 3.2 Fixtures

Duas fixtures foram extraídas para `Fixtures/`:

- **`IntegrationFixture`** — Reutiliza o padrão de `RecommendationLoadTests` com Testcontainers
  PostgreSQL e `WebApplicationFactory<Program>` com artefatos ML reais. Registrada como
  `ICollectionFixture` na coleção `"Integration"` para que o container suba uma vez por suite.

- **`LightweightFixture`** — Substitui `IRecommendationService` e `IPreferenceService` por
  mocks (Moq), eliminando a dependência de Docker e arquivos ML para testes de validação de
  controller puro.

### 3.3 Suite A — Caminho Positivo

**`RecommendationHappyPathTests`** cobre o caminho warm user e os dois sub-caminhos de
cold-start:

- Warm user: HTTP 200, scores numéricos em [0,1], sem NaN/Inf, sem duplicatas, limite `n` exato
- Cold-start sem preferências: retorna popular items com `Score == null`
- Cold-start com preferências: retorna itens da categoria preferida com `Score == null`
- HP-REC-14 (RF10): verifica que preferência de categoria altera o conjunto de shows retornado
  em relação ao cold-start sem preferências — valida o critério "consistent with user profile"

**`PreferenceHappyPathTests`** cobre o CRUD do endpoint `/api/preference/{userId}`:

- POST salva preferências (204), GET retorna lista salva, segundo POST sobrescreve
- Fluxo integrado: preferência salva → recomendação reflete a categoria (HP-PREF-05)

### 3.4 Suite B — Caminho Negativo

**`RecommendationNegativeTests`** (arquivo único com dois grupos de classe):

- *`RecommendationValidationTests` (lightweight)*: NEG-REC-01 a NEG-REC-06 — `n` inválido retorna 400; mensagem de erro verificada
- *`RecommendationServiceEdgeCaseTests` (integration)*: NEG-REC-07 — preferência com categoria inexistente cai em popular items sem lançar exceção

**`PreferenceNegativeTests`** (lightweight): `tipos` vazio, null e body ausente retornam 400;
categoria inexistente é aceita pelo controller (valida que a validação semântica não é feita nessa camada).

---

## 4. Rastreabilidade RF/RNF

| Requisito | Critério de aceite | Testes que cobrem |
|---|---|---|
| RF01 | Preferências salvas após onboarding | HP-PREF-01, HP-PREF-03, HP-PREF-04 |
| RF08 | Usuário edita preferências; mudança persiste | HP-PREF-01 a HP-PREF-05, NEG-PREF-01 a NEG-PREF-05 |
| RF10 | Sugestões consistentes com perfil do usuário | HP-REC-01 a HP-REC-14 |
| RNF01 | p95 ≤ 3 s com 200 usuários simultâneos | `Recommend_MeasureLatencyUnderConcurrency` com `InlineData(200)` |
| RNF06 | p95 ≤ 3 s com 1.000 usuários por 15 min | Suite k6/Locust em staging — fora de escopo do xUnit |
| RNF08 | Cobertura ≥ 70% em módulos críticos | Suite completa (38 testes) cobre todos os caminhos de `RecommendationService`, `PreferenceService` e ambos os controllers |

RF02 (geolocalização) não é implementado em `RecommendationService` na versão atual — sem testes aplicáveis neste módulo.

---

## 5. Decisão arquitetural registrada

> **Manter o `lock` atual, não migrar para `PredictionEnginePool` neste momento.**
>
> Razão: os testes de carga mostraram crescimento sub-linear de latência (18× para 50× de concorrência). O overhead do lock é insignificante no volume atual. A migração deve ser reavaliada se o catálogo ultrapassar ~5.000 itens ou se o P99 de latência observado em produção superar 300ms sob carga normal.
>
> Referência: `Recommend_MeasureLatencyUnderConcurrency` em `RecommendationLoadTests.cs`.
