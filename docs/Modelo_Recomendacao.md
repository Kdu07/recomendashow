# Modelo de Recomendação — Portal Aventura

## Sumário

- [1. Visão Geral e Arquitetura](#1-visão-geral-e-arquitetura)
- [2. Funk SVD — o que é e por que não KNN](#2-funk-svd--o-que-é-e-por-que-não-knn)
- [3. Pipeline C# — Treino, Inferência e Persistência](#3-pipeline-c--treino-inferência-e-persistência)
- [4. Hiperparâmetros do Modelo](#4-hiperparâmetros-do-modelo)
- [5. Scoring Híbrido (SVD + Afinidade de Categoria)](#5-scoring-híbrido-svd--afinidade-de-categoria)
- [6. Estratégia de Cold-start](#6-estratégia-de-cold-start)
- [7. Métricas de Avaliação](#7-métricas-de-avaliação)
- [8. Riscos, Limitações e Pendências](#8-riscos-limitações-e-pendências)

---

## 1. Visão Geral e Arquitetura

O sistema de recomendação do Portal Aventura é construído sobre dados de compra do Sympla. Não há avaliações explícitas — ninguém deu nota para *A Noviça Rebelde*. O sinal de interesse é o número de dias distintos em que um comprador adquiriu ingressos para o mesmo espetáculo, armazenado na coluna `ranking` (escala 1–5).

| Dado | Valor |
|---|---|
| Interações no dataset | 109.536 |
| Usuários únicos | 68.313 |
| Espetáculos únicos | 447 |
| Esparsidade da matriz usuário-item | 99,64% |
| Distribuição do ranking | rank=1: ~94% · rank=2: ~5% · rank≥3: <1% |

O motor é **único e híbrido**: **Funk SVD** (via `Microsoft.ML.Recommender` / `MatrixFactorizationTrainer`) combinado com **afinidade de categoria**. Há um projeto de **treino** e um **host de inferência**; o contrato com o backend é a **tabela `recomendacoes`** — o backend não carrega modelo.

### Fluxo ponta a ponta

```
data/raw/database_sympla.csv
   └─[src/ml: dotnet run preprocess]──► data/dataset/dataset.csv
        └─[src/ml: dotnet run train]──► data/models/model.zip
                                   + data/models/{cold_start,item_categories,
                                                  category_affinity,cold_start_by_category}.json
                                        └─[src/inference-service (host ASP.NET)]
                                             ├─ HybridModelLoader: valida/carrega artefatos no startup
                                             ├─ RecommendationBatchJob (24h): para cada usuário do dataset
                                             │    → HybridRecommendationService.RecommendAsync
                                             │       (delega a AventuraML.Recommender — fórmula híbrida)
                                             │    → RecommendationWriteRepository.SaveAsync
                                             │         └─► tabela `recomendacoes` (DELETE+INSERT por usuário)
                                             └─ Endpoints HTTP: /api/metrics e /api/diagnostics
                                                  └─[src/backend] lê a tabela `recomendacoes`
                                                       (GET /api/recomendacao/{usuarioId}; fallback por
                                                        gêneros favoritos quando não há linhas)
```

### Pré-processamento (raw → dataset.csv)

O comando `dotnet run preprocess` (`src/ml/Preprocessor.cs`) transforma as transações brutas do Sympla no dataset de interações. As regras são as do pipeline original (nascido nos notebooks de AED e portado para C# na migração):

1. **Leitura**: colunas usadas do raw — `ID COMPRADOR.1.ID USUÁRIO`, `NOME ESPETÁCULO`, `DATA DA APRESENTAÇÃO`, `DATA DA COMPRA`, `VALOR DO INGRESSO`, `TIPO`.
2. **Limpeza**: linhas com data inválida são descartadas; **lead time** (apresentação − compra) deve estar entre 0 e 180 dias; preço aceita vírgula decimal (`"19,5"` → 19.5; não numérico → 0).
3. **Dedupe**: compras do mesmo usuário, mesmo espetáculo e mesmo dia contam como **uma** interação (mantém a primeira linha).
4. **Outliers**: usuários com volume de interações acima do **percentil 99,9** são removidos (compradores institucionais/revendedores distorceriam o sinal de afinidade).
5. **Agregação** por (usuário, espetáculo): `ranking` = nº de dias distintos de compra, limitado a 5; `valor_medio_ingresso` = média dos preços (2 casas); `tipo_evento` = primeiro `TIPO` observado.

Os nomes de espetáculo são mantidos como estão no raw (sem trim) — `DataPipeline` e `DatasetRepository` normalizam com `Trim()` na leitura.

### Estrutura dos projetos

```
data/
├── dataset/dataset.csv                  ← dados do Sympla (mesmo arquivo p/ treino e serving)
└── models/                              ← diretório canônico de artefatos
    ├── model.zip
    ├── cold_start.json
    ├── item_categories.json
    ├── category_affinity.json
    └── cold_start_by_category.json
src/
├── ml/                                  ← treino e avaliação (console, .NET 9) — AventuraML
│   ├── Preprocessor.cs                  ← pré-processamento raw → dataset.csv
│   ├── DataPipeline.cs                  ← leitura e transformação do CSV
│   ├── Trainer.cs                       ← treino + export dos 5 artefatos
│   ├── Recommender.cs                   ← fórmula híbrida + PredictRating (fonte única)
│   ├── Evaluator.cs                     ← avaliação 80/20 (usada pelo console e pelo host)
│   └── Benchmarker.cs                   ← benchmark de latência
├── inference-service/                   ← host de inferência/persistência (ASP.NET, .NET 9)
│   ├── Services/HybridModelLoader.cs    ← fail-fast: carrega artefatos no startup
│   ├── Services/HybridRecommendationService.cs ← adaptador fino sobre o Recommender
│   ├── Services/RecommendationBatchJob.cs      ← batch 24h → tabela recomendacoes
│   ├── Services/MetricsService.cs       ← métricas via Evaluator de src/ml
│   ├── Data/RecommendationWriteRepository.cs   ← writer Npgsql (score normalizado [0,1])
│   └── InferenceService.Tests/          ← suíte xunit (modelo real pequeno via fixture)
└── backend/                             ← API do produto: apenas LÊ a tabela recomendacoes
```

### Artefatos de modelo

| Arquivo | Gerado por | Conteúdo |
|---|---|---|
| `data/models/model.zip` | `Trainer.cs` | Pesos do Funk SVD (ML.NET) |
| `data/models/cold_start.json` | `Trainer.cs` | Lista de `item_id` ordenada por número de compradores distintos |
| `data/models/item_categories.json` | `Trainer.cs` | Dicionário `item_id → tipo_evento` |
| `data/models/category_affinity.json` | `Trainer.cs` | Dicionário `userId → { tipo_evento → score normalizado }` |
| `data/models/cold_start_by_category.json` | `Trainer.cs` | Dicionário `tipo_evento → [items por popularidade]` |

### Como executar o pipeline

```bash
# 1. Gerar data/dataset/dataset.csv a partir do raw do Sympla
cd src/ml
dotnet run preprocess

# 2. Treinar e gerar todos os artefatos em data/models/
dotnet run train

# 3. Testar inferência pelo CLI
dotnet run recommend 1 --n 10

# 4. Avaliar SVD puro vs. híbrido (split 80/20 por usuário)
dotnet run evaluate --k 10 --alpha 0.7

# 5. Subir o host de inferência (batch 24h + métricas/diagnóstico)
cd ../inference-service
dotnet run
# → GET http://localhost:5000/api/diagnostics/health
# → GET http://localhost:5000/api/diagnostics/model-info
# → GET http://localhost:5000/api/metrics?k=10&rankingUsers=200
# → GET http://localhost:5000/api/metrics/charts/{error-histogram|real-vs-predicted|svd-vs-hybrid}
```

> **Não há endpoint HTTP de recomendação.** A recomendação chega ao produto exclusivamente
> via batch → tabela `recomendacoes` → `GET /api/recomendacao/{usuarioId}` do backend.

---

## 2. Funk SVD — o que é e por que não KNN

### O problema de base: 99,64% de células vazias

Imagine uma planilha com 68.313 linhas (usuários) e 447 colunas (espetáculos). A maioria das células está vazia — quem comprou *Elis, a Musical* provavelmente nunca comprou *Thiago Ventura - Modo Efetivo*, e vice-versa. A tarefa do recomendador é estimar o interesse do usuário em espetáculos que ele ainda não viu.

### SVD clássico vs Funk SVD

A Decomposição em Valores Singulares da álgebra linear decompõe uma matriz $R$ em $U \Sigma V^T$. Ela exige que todos os valores sejam conhecidos. Com 99,64% de entradas ausentes, aplicar a SVD diretamente é inviável — não existe como decompor o que não foi observado.

Simon Funk resolveu isso em 2006, durante o Netflix Prize: em vez de decompor a matriz cheia, o **Funk SVD aprende representações compactas de usuários e espetáculos a partir apenas das interações existentes**, via gradiente descendente estocástico (SGD). Nenhuma imputação, nenhuma matriz densa.

Cada usuário $u$ ganha um vetor $p_u \in \mathbb{R}^k$ e cada espetáculo $i$ ganha um vetor $q_i \in \mathbb{R}^k$. Esses vetores são os **fatores latentes** — representações abstratas que o modelo infere dos padrões de compra, sem rótulos predefinidos. A predição do ranking de $u$ para $i$ é:

$$\hat{r}_{ui} = \mu + b_u + b_i + p_u \cdot q_i^T$$

| Componente | O que representa neste dataset |
|---|---|
| $\mu$ | Média global de ranking — aproximadamente 1,08, reflexo de que ~94% das interações têm rank=1 |
| $b_u$ | Viés do usuário — captura quem tende a comprar o mesmo show em mais dias distintos |
| $b_i$ | Viés do espetáculo — *A Noviça Rebelde* tem milhares de compradores distintos, o que eleva seu $b_i$ |
| $p_u \cdot q_i^T$ | Afinidade personalizada — produto interno entre o perfil latente do usuário e as características latentes do espetáculo |

O modelo minimiza o erro quadrático com regularização L2:

$$\min_{P,Q,b} \sum_{(u,i) \in \mathcal{K}} (r_{ui} - \hat{r}_{ui})^2 + \lambda \left(||p_u||^2 + ||q_i||^2 + b_u^2 + b_i^2\right)$$

A regularização $\lambda$ é essencial aqui: com dados tão esparsos, sem penalização os vetores latentes tendem a memorizar os poucos casos observados em vez de generalizar padrões.


### Atualização por SGD

O modelo é otimizado por gradiente descendente estocástico. A cada par observado $(u, i) \in \mathcal{K}$, o erro residual é calculado e os parâmetros atualizados na direção oposta ao gradiente:

$$e_{ui} = r_{ui} - \hat{r}_{ui}$$

$$b_u \leftarrow b_u + \gamma \left(e_{ui} - \lambda b_u\right)$$

$$b_i \leftarrow b_i + \gamma \left(e_{ui} - \lambda b_i\right)$$

$$p_u \leftarrow p_u + \gamma \left(e_{ui}\, q_i - \lambda p_u\right)$$

$$q_i \leftarrow q_i + \gamma \left(e_{ui}\, p_u - \lambda q_i\right)$$

onde $\gamma$ é a taxa de aprendizado (`LearningRate = 0.01`) e $\lambda$ o coeficiente de regularização L2 (`Lambda = 0.01`). Os termos $-\lambda \theta$ atuam como *weight decay*, impedindo que os vetores latentes cresçam indefinidamente e controlando overfitting em dados tão esparsos. Cada iteração SGD percorre o dataset embaralhado e aplica as quatro atualizações acima para cada interação observada.
### Comparação com KNN

#### KNNWithMeans item-based

O `KNNWithMeans` item-based não aprende parâmetros globais. Para estimar o interesse de um usuário $u$ no espetáculo $i$, busca os $k$ itens mais similares a $i$ que $u$ já consumiu ($N_k(i;u)$) e calcula uma média ponderada corrigida pela média de cada item:

$$\hat{r}_{ui} = \bar{r}_i + \frac{\displaystyle\sum_{j \in N_k(i;\,u)} \mathrm{sim}(i,j)\,(r_{uj} - \bar{r}_j)}{\displaystyle\sum_{j \in N_k(i;\,u)} |\mathrm{sim}(i,j)|}$$

onde $\bar{r}_i$ é o ranking médio do espetáculo $i$ entre todos os seus compradores e $\mathrm{sim}(i,j)$ é a similaridade de cosseno calculada sobre os vetores de interação. É uma abordagem *memory-based* — toda a matriz de interações precisa estar disponível em tempo de inferência.

#### KNNWithMeans user-based

A variante user-based busca os $k$ usuários mais similares a $u$ que tenham consumido $i$ ($N_k(u;i)$) e agrega seus rankings:

$$\hat{r}_{ui} = \bar{r}_u + \frac{\displaystyle\sum_{v \in N_k(u;\,i)} \mathrm{sim}(u,v)\,(r_{vi} - \bar{r}_v)}{\displaystyle\sum_{v \in N_k(u;\,i)} |\mathrm{sim}(u,v)|}$$

Com 68.313 usuários e 99,64 % de esparsidade, o conjunto $N_k(u;i)$ fica vazio para a maioria dos pares: dois usuários raramente compraram o mesmo espetáculo. A predição colapsa para $\bar{r}_u$ (média pessoal) ou é indefinida, tornando o user-based ainda mais instável que o item-based neste dataset.

#### Por que o SVD vence

| Critério | Funk SVD | KNN item-based | KNN user-based |
|---|---|---|---|
| Parâmetros aprendidos | $P$, $Q$, $b_u$, $b_i$ (offline) | Nenhum | Nenhum |
| Dados em inferência | Apenas os vetores aprendidos | Toda a matriz de interações | Toda a matriz de interações |
| Alta esparsidade | Lida bem — aprende de cada par observado | Problemático — pares sem compradores em comum geram cosseno indefinido | Muito problemático — vizinhos de usuário raramente existem |
| Cauda longa | Generaliza via fatores latentes | Espetáculos raros têm vizinhos instáveis | Usuários com histórico curto têm vizinhos instáveis |
| Memória em inferência | $O(N_u k + N_i k)$ | $O(N_u N_i)$ | $O(N_u N_i)$ |
| RMSE (Python, 80/20) | **0,2953** | 0,3402 | não avaliado (colapso por esparsidade) |
| Explicabilidade | Baixa (fatores sem rótulo) | Alta ("porque você viu X, veja Y") | Alta ("usuários similares a você gostaram de Y") |

Com ~450 espetáculos e 99,64 % de esparsidade, a maioria dos pares de itens compartilha zero compradores em comum — o cosseno item-based é ruidoso. O SVD generaliza porque aprende padrões globais: um usuário que foi a shows de Afonso Padilha e Thiago Ventura tem $p_u$ orientado para o "fator stand-up", mesmo sem jamais ter comprado *Bruna Louise - Ela Tá Correndo Atrás*.

O KNN item-based ainda ganha em interpretabilidade — se o parceiro precisar justificar uma recomendação, ele responde com vizinhos similares. O SVD responde com vetores sem rótulo. O KNN foi avaliado na fase de pesquisa (Python) e descartado; não é entregue no pipeline.

---

## 3. Pipeline C# — Treino, Inferência e Persistência

### Tecnologias

| Componente | Biblioteca | Função |
|---|---|---|
| Leitura do CSV | `CsvHelper 33.x` | Parse do `dataset.csv` com mapeamento por atributo `[Name]` |
| Treino | `Microsoft.ML.Recommender` — `MatrixFactorizationTrainer` | Fatoração de matrizes (equivalente ao Funk SVD) com SGD |
| Serialização do modelo | `Microsoft.ML` — `mlContext.Model.Save` | Salva pesos treinados em `model.zip` |
| Inferência | `Microsoft.ML` — `PredictionEngine<T>` | Pontuação de pares (usuário, espetáculo) via modelo treinado |
| Persistência | `Npgsql` | Escrita das recomendações na tabela `recomendacoes` (PostgreSQL) |
| Gráficos | `ScottPlot 5.x` | PNGs de métricas no host de inferência |
| Artefatos auxiliares | `System.Text.Json` | `cold_start.json`, `item_categories.json`, `category_affinity.json`, `cold_start_by_category.json` |

### Fluxo de treino (`src/ml/Trainer.cs`)

Além de treinar o modelo, o `Trainer` pré-computa todos os artefatos auxiliares necessários para inferência e cold-start:

```csharp
// 1. Carregar dataset (incluindo tipo_evento)
var rawRows = DataPipeline.LoadRaw(datasetPath);  // List<CsvRatingRow>

// 2. Cold-start global: itens ordenados por compradores distintos
var popularItems = rows.GroupBy(r => r.ItemId)
    .OrderByDescending(g => g.Count())
    .Select(g => g.Key).ToList();

// 3. Mapeamento item → categoria (item_categories.json)
// 4. Afinidade de categoria por usuário (category_affinity.json)
// 5. Cold-start por categoria (cold_start_by_category.json)

// 6. Treinar Funk SVD
var pipeline = mlContext.Transforms.Conversion
    .MapValueToKey("UserIdEncoded", nameof(RatingData.UserId))
    .Append(mlContext.Transforms.Conversion.MapValueToKey("ItemIdEncoded", nameof(RatingData.ItemId)))
    .Append(mlContext.Recommendation().Trainers.MatrixFactorization(options));
var model = pipeline.Fit(data);
mlContext.Model.Save(model, data.Schema, modelOutputPath);
```

### Fonte única da fórmula (`src/ml/Recommender.cs`)

A fórmula híbrida vive **apenas** no `Recommender` de `src/ml`. O host de inferência referencia o projeto `AventuraML` e delega — proibido reimplementar a fórmula em outro lugar:

```csharp
// 1. Score SVD bruto para cada item não consumido (PredictRating — sob lock)
// 2. Normalizar SVD para [0, 1] no batch de candidatos
var normSvd = svdRange > 1e-6f ? (svdScore - minSvd) / svdRange : 0.5f;
// 3. Afinidade de categoria: fração do histórico do usuário na categoria do item
var affinity = _categoryAffinity[userId][itemCategory];
// 4. Score híbrido (α padrão = 0.7)
var finalScore = _alpha * normSvd + (1f - _alpha) * affinity;
// 5. Ordenar por score híbrido e retornar top-N
```

O `PredictionEngine` não é thread-safe; o acesso a `_engine.Predict` é serializado por `lock` **dentro do próprio `Recommender`**, protegendo tanto o batch quanto o endpoint de métricas do host.

### Host de inferência (`src/inference-service`)

- **`HybridModelLoader`** (`IHostedService`): valida e carrega `model.zip` + JSONs + `dataset.csv` no startup. Se faltar artefato, a aplicação **não sobe** (fail-fast antes da primeira requisição).
- **`HybridRecommendationService : IRecommendationService`**: adaptador fino, `ModelType = "hibrido"`; delega a `Recommender.Recommend(userId, n)`.
- **`RecommendationBatchJob`** (`BackgroundService`): a cada 24h percorre todos os `user_id` do dataset, gera o top-10 híbrido e grava via `RecommendationWriteRepository` (DELETE+INSERT por usuário, match por `espetaculos.nome`, score em `[0,1]`, `motivo` descritivo). Falhas por usuário são logadas e não interrompem o batch.
- **Métricas e diagnóstico** (HTTP): ver seção 7.

### Contrato com o backend

O `src/backend` **não** conhece o modelo. `RecomendacaoRepository` lê `recomendacoes (usuario_id, espetaculo_id, score, motivo, data_geracao)` ordenando por `score DESC`. Se o usuário não tem linhas, o backend aplica fallback próprio por gêneros favoritos. Trocar o motor de recomendação não exige mudar o backend.

---

## 4. Hiperparâmetros do Modelo

Os hiperparâmetros da versão Python foram determinados por `GridSearchCV` com 3-fold cross-validation sobre 54 combinações. Esses valores orientaram a configuração do `MatrixFactorizationTrainer` no pipeline C#, adaptada à API disponível:

| Hiperparâmetro | Valor | Correspondência Python | Justificativa |
|---|---|---|---|
| `NumberOfIterations` | 20 | `n_epochs=50` | Iterações SGD; valor menor compensa o treino no dataset completo sem CV |
| `ApproximationRank` | 50 | `n_factors=10` | Dimensão dos vetores latentes; aumentado para explorar mais padrões com a API ML.NET |
| `LearningRate` | 0.01 | `lr_all=0.001` | Taxa de atualização dos pesos no SGD |
| `Lambda` | 0.01 | `reg_all=0.2` | Regularização L2 — controla o crescimento dos vetores latentes |
| `Alpha` (`Recommendation:Alpha`) | 0.7 | — | Peso do SVD no score híbrido; configurável no `appsettings.json` do `inference-service` |

**Métricas de referência da versão Python** (mantidas como baseline histórico):

| Métrica | SVD (Python) | KNN (Python) |
|---|---|---|
| RMSE ↓ | **0.2953** | 0.3402 |
| MAE ↓ | **0.1210** | 0.1366 |

> Para comparação: o modelo híbrido atual (`model.zip`), avaliado pelo host sobre o split 80/20
> do mesmo dataset, apresenta RMSE ≈ 0.29 / MAE ≈ 0.15 na predição de rating (score SVD bruto).
> Executar `dotnet run evaluate` em `src/ml` reproduz a avaliação offline completa.

---

## 5. Scoring Híbrido (SVD + Afinidade de Categoria)

O campo `tipo_evento` existe no dataset com oito categorias — *Show Musical*, *Stand-Up*, *Espetáculo Teatral*, *Espetáculo Musical*, entre outras. Ele é utilizado ativamente no scoring de recomendações.

### Como a afinidade de categoria é calculada

Para cada usuário, o `Trainer` computa a proporção do seu histórico de compras (medida em soma de `ranking`) dedicada a cada `tipo_evento`:

```
categoryAffinity[userId][tipo] = Σranking(tipo) / Σranking(todos)
```

Por exemplo, um usuário que somou `ranking=5` em Stand-Up e `ranking=5` em Teatro terá afinidade 0,5 para cada categoria.

### Fórmula do score híbrido

$$\text{score}_{ui} = \alpha \cdot \text{SVD}_{ui}^{\text{norm}} + (1 - \alpha) \cdot \text{affinity}(u, \text{tipo}(i))$$

onde $\text{SVD}_{ui}^{\text{norm}}$ é o score SVD bruto normalizado para $[0, 1]$ dentro do batch de candidatos e $\alpha = 0.7$ por padrão (configurável via `Recommendation:Alpha` no `appsettings.json` do `inference-service`).

O score híbrido resultante já está em $[0,1]$ e é gravado assim na tabela `recomendacoes` (com clamp defensivo e arredondamento a 2 casas no writer).

### Efeito prático

Com $\alpha = 0.7$, o SVD ainda domina o ordenamento, mas a afinidade de categoria funciona como desempate e boost para espetáculos alinhados às preferências de gênero do usuário. Dois espetáculos com score SVD similar terão o da categoria preferida do usuário na frente.

A métrica **Category Precision@K** quantifica isso (seção 7): na avaliação amostrada do host, o híbrido elevou a Category Precision@10 de ~0.25 (SVD puro) para ~0.61, com Hit Rate@10 também superior.

---

## 6. Estratégia de Cold-start

Uma usuária nova — sem nenhuma compra registrada no Sympla — não tem `UserId` no conjunto de treino do modelo. O `Recommender` detecta esse caso pelo histórico carregado do `dataset.csv` e aplica fallback por **popularidade global**: os `n` espetáculos com maior número de compradores distintos (`cold_start.json`), com `Score = null` sinalizando recomendação não personalizada.

No produto, há ainda um segundo nível independente: se a tabela `recomendacoes` não tem linhas para o usuário, o **backend** aplica fallback próprio por gêneros favoritos declarados no onboarding.

O artefato `cold_start_by_category.json` (espetáculos mais populares por `tipo_evento`) é gerado no treino e fica disponível para evoluir o cold-start do recomendador para "popularidade dentro das categorias preferidas" — hoje ele não é consumido pelo host.

**Transição para personalização.** Um usuário com histórico recebe predições do híbrido no próximo ciclo de batch. Um usuário novo só sai do cold-start após um ciclo completo: coleta de novos dados do Sympla → atualização do `dataset.csv` → retreinamento (`dotnet run train` em `src/ml/`) → reinício do `inference-service`.

---

## 7. Métricas de Avaliação

### Avaliação offline (`src/ml/Evaluator.cs`)

O `Evaluator` implementa uma divisão 80/20 **por usuário** (mínimo 2 interações para entrar no teste, seed fixa). O comando de console treina um SVD apenas no conjunto de treino e compara **SVD puro** × **híbrido**:

```bash
cd src/ml
dotnet run evaluate --k 10 --alpha 0.7
```

```
══════════════════════════════════════════════════════
  MÉTRICAS DE AVALIAÇÃO (split 80/20 por usuário)
══════════════════════════════════════════════════════

  Predição de rating (SVD puro, N pares):
    RMSE : ...
    MAE  : ...

  Ranking (top-10, M usuários avaliados):

  Métrica                      SVD puro    Híbrido       Δ
  ────────────────────────────────────────────────────────
  Hit Rate@10                    ...         ...       +...
  Precision@10                   ...         ...       +...
  Recall@10                      ...         ...       +...
  Category Precision@10          ...         ...       +...
```

### Métricas no host (`GET /api/metrics`)

O `inference-service` expõe as **mesmas métricas, calculadas pelo mesmo código** (`Evaluator.EvaluateWithPredictor`, de `src/ml`) — porém sobre o **`model.zip` em produção**, sem retreinar:

- **RMSE/MAE** usam o **score SVD bruto** do `model.zip` (`Recommender.PredictRating`, escala 1–5) contra o `ranking` real de todos os pares de teste.
- **Hit Rate@K, Precision@K, Recall@K, Category Precision@K** comparam o ranking SVD puro × híbrido sobre uma **amostra** de usuários de teste (`?rankingUsers=200` por padrão) — a avaliação de ranking custa ~1 predição por item candidato por usuário.
- Resultados são cacheados por parâmetro (o modelo é estático no processo).

Como o `model.zip` foi treinado no dataset completo, os pares de "teste" foram vistos no treino — as métricas do host são de **acompanhamento/sanidade**, otimistas por construção. A avaliação honesta de generalização é a do console (`dotnet run evaluate`), que retreina só no split de treino.

Gráficos PNG (ScottPlot):

| Endpoint | Conteúdo |
|---|---|
| `/api/metrics/charts/error-histogram` | Histograma dos erros (real − predito) |
| `/api/metrics/charts/real-vs-predicted` | Scatter ranking real × predito |
| `/api/metrics/charts/svd-vs-hybrid` | Barras comparando as métricas de ranking SVD puro × híbrido |

### Definição das métricas

**RMSE** mede a raiz do erro quadrático médio entre o `ranking` real e o previsto pelo SVD. Penaliza erros grandes desproporcionalmente.

**MAE** é o desvio absoluto médio entre `ranking` real e previsto.

**Hit Rate@K**: fração dos usuários de teste para os quais pelo menos um dos top-K recomendados estava no conjunto de teste.

**Precision@K**: média da proporção de itens relevantes nos top-K recomendados (por usuário).

**Recall@K**: média da proporção dos itens do conjunto de teste que aparecem nos top-K recomendados (por usuário).

**Category Precision@K**: fração dos top-K que pertence às duas categorias com maior soma de `ranking` no histórico de treino do usuário. Mede se o recomendador respeita as preferências de gênero do usuário.

### Por que as métricas de classificação binária enganam aqui

O modelo também poderia ser avaliado com acurácia/precisão binária usando limiar `ranking >= 2`. O SVD tende a obter precisão e F1 baixos nessa métrica — consequência do dataset desbalanceado. A média global de ranking é ~1,08 e ~94% dos rankings reais são 1. O modelo otimiza o erro numérico e converge para predições conservadoras, todas abaixo do limiar arbitrário de 2. RMSE e as métricas Top-K são os indicadores mais honestos para feedback implícito deste tipo.

---

## 8. Riscos, Limitações e Pendências

### Mapeamento usuário do dataset ↔ usuário do banco (D4 — resolvido para a demo via `ml_id`)

O writer resolve `usuarios WHERE ml_id = @userId`. A coluna `ml_id` (`SERIAL UNIQUE`) nasce com um auto-incremento por ordem de inserção, **sem relação** com o `user_id` do Sympla no `dataset.csv` — por isso, sozinha, ela não bastava. O mapeamento foi então **definido explicitamente**: no fim de `src/database/02_sprint4_improvements.sql`, três `UPDATE ... WHERE email = ...` associam os usuários do seed a `user_id` do dataset com histórico rico (`alex@email.com → 63261`, `maria@email.com → 14888`, `joao@email.com → 55251`). Com isso o batch passa a **gravar recomendações reais** para esses três usuários (em banco recriado do zero; ver `Deploy_Recomendacao.md` §8). O writer já suporta colunas alternativas (`dataset_user_id`, `external_user_id`, ...) caso o mapeamento mude.

**Limitação:** o `dataset.csv` é anônimo (`user_id` inteiro, sem PII), então isto é uma **atribuição de persona** — cada usuário do seed herda o histórico de um comprador parecido —, não um casamento de identidade real. O mapeamento cobre apenas os usuários do seed; usuários autenticados em geral ainda não recebem `ml_id` mapeado (evolução futura: mapear por identidade real em escala).

### Thread-safety do PredictionEngine

O `PredictionEngine<T>` do ML.NET **não é thread-safe**. O acesso é serializado por `lock` dentro do `Recommender` (src/ml), cobrindo o batch e o endpoint de métricas. Para escalar, a alternativa é `Microsoft.Extensions.ML` com `PredictionEnginePool<T>`.

### Performance do scoring do batch

O batch pontua ~68 mil usuários × ~446 itens com 1 predição por par (sequencial). É aceitável para a janela de 24h, mas é dívida técnica conhecida — a otimização natural é scoring em lote (matriz) em vez de `Predict` unitário.

### Modelo estático

O modelo não aprende com novas compras em tempo real. Toda nova interação do Sympla só impacta as recomendações após um ciclo completo:

```
coleta de dados → atualização de data/dataset/dataset.csv
→ dotnet run train (src/ml/)            ← gera os 5 artefatos em data/models/
→ reinício do inference-service         ← batch 24h repersiste a tabela recomendacoes
```

A atualização periódica citada no fluxo BPMN (`docs/Projeto.md`) é implementada pelo `RecommendationBatchJob` (24h) para a **persistência**; o **retreinamento** continua manual.

### Feedback implícito como proxy de interesse

O `ranking` mede dias distintos de compra, não satisfação com o espetáculo. Um usuário que comprou *A Noviça Rebelde* em três dias distintos pode ter comprado ingressos para familiares — não necessariamente assistiu três vezes. Esse ruído é inerente à fonte de dados do Sympla e limita a qualidade dos fatores latentes aprendidos.

### Cold-start de espetáculos novos

O cold-start atual cobre **usuários novos**. **Espetáculos novos** não são cobertos: um show adicionado ao catálogo após o treino não tem vetor $q_i$ e não aparece nas predições do modelo. O fallback de popularidade também não inclui espetáculos sem histórico de compra. Peças em pré-estreia são invisíveis para o recomendador até o próximo retreinamento.

### Hiperparâmetros não otimizados (pipeline C#)

Os hiperparâmetros do `MatrixFactorizationTrainer` e o valor de `alpha` foram definidos com base nos valores obtidos pelo `GridSearchCV` da versão Python e em testes manuais. Ainda não foi realizada uma busca formal de hiperparâmetros no pipeline C#. Recomenda-se usar `dotnet run evaluate` com variações de `--alpha` e dos parâmetros do trainer para confirmar ou ajustar `ApproximationRank`, `LearningRate`, `Lambda` e `Alpha`.
