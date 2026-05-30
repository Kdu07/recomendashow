# Modelo de Recomendação — Portal Aventura

## Sumário

- [1. Visão Geral](#1-visão-geral)
- [2. Funk SVD — o que é e por que não KNN](#2-funk-svd--o-que-é-e-por-que-não-knn)
- [3. Pipeline C# — Treino e Inferência](#3-pipeline-c--treino-e-inferência)
- [4. Hiperparâmetros do Modelo](#4-hiperparâmetros-do-modelo)
- [5. Scoring Híbrido (SVD + Afinidade de Categoria)](#5-scoring-híbrido-svd--afinidade-de-categoria)
- [6. Estratégia de Cold-start](#6-estratégia-de-cold-start)
- [7. Métricas de Avaliação e Comparação SVD vs Híbrido](#7-métricas-de-avaliação-e-comparação-svd-vs-híbrido)
- [8. Riscos e Limitações Técnicas](#8-riscos-e-limitações-técnicas)

---

## 1. Visão Geral

O sistema de recomendação do Portal Aventura é construído sobre dados de compra do Sympla. Não há avaliações explícitas — ninguém deu nota para *A Noviça Rebelde*. O sinal de interesse é o número de dias distintos em que um comprador adquiriu ingressos para o mesmo espetáculo, armazenado na coluna `ranking` (escala 1–5).

| Dado | Valor |
|---|---|
| Interações no dataset | 109.916 |
| Usuários únicos | 68.499 |
| Espetáculos únicos | 449 |
| Esparsidade da matriz usuário-item | 99,64% |
| Distribuição do ranking | rank=1: 94,2% · rank=2: 5,1% · rank≥3: 0,7% |

O modelo em produção é o **Funk SVD** (implementado via `Microsoft.ML.Recommender` / `MatrixFactorizationTrainer`), combinado com **afinidade de categoria** para gerar um score híbrido. Os artefatos são carregados pelo `RecommendationService` da API C# na inicialização. Para usuários sem histórico, o sistema recorre a listas pré-computadas por categoria ou, como fallback final, aos 449 espetáculos ordenados por popularidade global.

### Artefatos de modelo

| Arquivo | Gerado por | Conteúdo |
|---|---|---|
| `src/models/model.zip` | `Trainer.cs` | Pesos do Funk SVD (ML.NET) |
| `src/models/cold_start.json` | `Trainer.cs` | Lista de `item_id` ordenada por número de compradores distintos |
| `src/models/item_categories.json` | `Trainer.cs` | Dicionário `item_id → tipo_evento` |
| `src/models/category_affinity.json` | `Trainer.cs` | Dicionário `userId → { tipo_evento → score normalizado }` |
| `src/models/cold_start_by_category.json` | `Trainer.cs` | Dicionário `tipo_evento → [items por popularidade]` |

### Estrutura do Pipeline

```
src/
├── dataset/
│   └── dataset.csv                      ← dados do Sympla
├── ml/                                  ← projeto de treino (console, .NET 9)
│   ├── DataPipeline.cs                  ← leitura e transformação do CSV
│   ├── Trainer.cs                       ← treino + export dos 5 artefatos
│   ├── Recommender.cs                   ← inferência standalone (CLI)
│   ├── Evaluator.cs                     ← avaliação 80/20 SVD puro vs. híbrido
│   └── AventuraML.csproj
├── models/
│   ├── model.zip
│   ├── cold_start.json
│   ├── item_categories.json
│   ├── category_affinity.json
│   └── cold_start_by_category.json
└── backend/                             ← API ASP.NET Core
    └── services/
        ├── RecommendationService.cs     ← carrega artefatos na inicialização
        └── PreferenceService.cs         ← preferências de categoria do usuário
```

### Como executar o pipeline

```bash
# 1. Treinar e gerar todos os artefatos (a partir de src/ml/)
cd src/ml
dotnet run train

# 2. Testar inferência pelo CLI
dotnet run recommend 1 --n 10

# 3. Avaliar SVD puro vs. híbrido (split 80/20 por usuário)
dotnet run evaluate --k 10 --alpha 0.7

# 4. Subir a API
cd ../backend
dotnet run
# → GET http://localhost:5000/api/recommendation/{userId}?n=10
```

---

## 2. Funk SVD — o que é e por que não KNN

### O problema de base: 99,64% de células vazias

Imagine uma planilha com 68.499 linhas (usuários) e 449 colunas (espetáculos). A maioria das células está vazia — quem comprou *Elis, a Musical* provavelmente nunca comprou *Thiago Ventura - Modo Efetivo*, e vice-versa. A tarefa do recomendador é estimar o interesse do usuário em espetáculos que ele ainda não viu.

### SVD clássico vs Funk SVD

A Decomposição em Valores Singulares da álgebra linear decompõe uma matriz $R$ em $U \Sigma V^T$. Ela exige que todos os valores sejam conhecidos. Com 99,64% de entradas ausentes numa matriz de 68.499 × 449, aplicar a SVD diretamente é inviável — não existe como decompor o que não foi observado.

Simon Funk resolveu isso em 2006, durante o Netflix Prize: em vez de decompor a matriz cheia, o **Funk SVD aprende representações compactas de usuários e espetáculos a partir apenas das interações existentes**, via gradiente descendente estocástico (SGD). Nenhuma imputação, nenhuma matriz densa.

Cada usuário $u$ ganha um vetor $p_u \in \mathbb{R}^k$ e cada espetáculo $i$ ganha um vetor $q_i \in \mathbb{R}^k$. Esses vetores são os **fatores latentes** — representações abstratas que o modelo infere dos padrões de compra, sem rótulos predefinidos. A predição do ranking de $u$ para $i$ é:

$$\hat{r}_{ui} = \mu + b_u + b_i + p_u \cdot q_i^T$$

| Componente | O que representa neste dataset |
|---|---|
| $\mu$ | Média global de ranking — aproximadamente 1,08, reflexo de que 94% das interações têm rank=1 |
| $b_u$ | Viés do usuário — captura quem tende a comprar o mesmo show em mais dias distintos |
| $b_i$ | Viés do espetáculo — *A Noviça Rebelde* tem 8.636 compradores distintos, o que eleva seu $b_i$ |
| $p_u \cdot q_i^T$ | Afinidade personalizada — produto interno entre o perfil latente do usuário e as características latentes do espetáculo |

O modelo minimiza o erro quadrático com regularização L2:

$$\min_{P,Q,b} \sum_{(u,i) \in \mathcal{K}} (r_{ui} - \hat{r}_{ui})^2 + \lambda \left(||p_u||^2 + ||q_i||^2 + b_u^2 + b_i^2\right)$$

A regularização $\lambda$ é essencial aqui: com dados tão esparsos, sem penalização os vetores latentes tendem a memorizar os poucos casos observados em vez de generalizar padrões.

### Comparação com KNN

O `KNNWithMeans` item-based não aprende parâmetros globais. Para recomendar algo a um usuário, calcula a similaridade de cosseno entre espetáculos usando os vetores de interação e busca os `k` mais próximos. É uma abordagem *memory-based* — os dados de treino precisam estar disponíveis em tempo de inferência.

| Critério | Funk SVD | KNN item-based |
|---|---|---|
| Parâmetros aprendidos | $P$, $Q$, $b_u$, $b_i$ (offline) | Nenhum (similaridades calculadas ao treinar) |
| Dados em inferência | Apenas os vetores aprendidos | Toda a matriz de interações |
| Alta esparsidade | Lida bem — aprende de cada par observado | Problemático — pares com zero compradores em comum geram similaridade indefinida |
| Cauda longa | Generaliza via fatores latentes | Espetáculos raros têm vizinhos instáveis |
| Explicabilidade | Baixa (fatores sem rótulo) | Alta ("porque você viu X, veja Y") |

Com 449 espetáculos e 99,64% de esparsidade, a maioria dos pares compartilha zero ou um comprador em comum. O cosseno fica ruidoso nesses casos. O SVD generaliza porque aprende padrões de nível global — um usuário que foi a shows de Afonso Padilha e Thiago Ventura tem $p_u$ orientado para o "fator stand-up", mesmo que nunca tenha comprado *Bruna Louise - Ela Tá Correndo Atrás*.

O KNN ainda ganha em um ponto: interpretabilidade. Se o parceiro precisar justificar uma recomendação ("por que esse espetáculo foi sugerido?"), o KNN responde com uma lista de vizinhos similares. O SVD responde com vetores sem rótulo.

---

## 3. Pipeline C# — Treino e Inferência

### Tecnologias

| Componente | Biblioteca | Função |
|---|---|---|
| Leitura do CSV | `CsvHelper 33.x` | Parse do `dataset.csv` com mapeamento por atributo `[Name]` |
| Treino | `Microsoft.ML.Recommender` — `MatrixFactorizationTrainer` | Fatoração de matrizes (equivalente ao Funk SVD) com SGD |
| Serialização do modelo | `Microsoft.ML` — `mlContext.Model.Save` | Salva pesos treinados em `model.zip` |
| Inferência | `Microsoft.ML` — `PredictionEngine<T>` | Pontuação de pares (usuário, espetáculo) via modelo treinado |
| Artefatos auxiliares | `System.Text.Json` | `cold_start.json`, `item_categories.json`, `category_affinity.json`, `cold_start_by_category.json` |

### Fluxo de treino (`Trainer.cs`)

Além de treinar o modelo, o `Trainer` pré-computa todos os artefatos auxiliares necessários para inferência e cold-start:

```csharp
// 1. Carregar dataset (incluindo tipo_evento)
var rawRows = DataPipeline.LoadRaw(datasetPath);  // List<CsvRatingRow>

// 2. Cold-start global: itens ordenados por compradores distintos
var popularItems = rows.GroupBy(r => r.ItemId)
    .OrderByDescending(g => g.Count())
    .Select(g => g.Key).ToList();
File.WriteAllText(coldStartOutputPath, JsonSerializer.Serialize(popularItems));

// 3. Mapeamento item → categoria
var itemCategories = rawRows.GroupBy(r => r.ItemId)
    .ToDictionary(g => g.Key, g => g.First().TipoEvento);
File.WriteAllText(itemCategoriesOutputPath, JsonSerializer.Serialize(itemCategories));

// 4. Afinidade de categoria por usuário (score normalizado pela soma de rankings)
var categoryAffinity = rawRows.GroupBy(r => r.UserId).ToDictionary(
    g => g.Key.ToString(),
    g => { var byCategory = g.GroupBy(r => r.TipoEvento)
                             .ToDictionary(cat => cat.Key, cat => cat.Sum(r => r.Ranking));
           var total = byCategory.Values.Sum();
           return byCategory.ToDictionary(kvp => kvp.Key,
               kvp => total > 0f ? kvp.Value / total : 0f); });
File.WriteAllText(categoryAffinityOutputPath, JsonSerializer.Serialize(categoryAffinity));

// 5. Cold-start por categoria: tipo_evento → [itens por popularidade]
var coldStartByCategory = rawRows.GroupBy(r => r.TipoEvento).ToDictionary(
    g => g.Key,
    g => g.GroupBy(r => r.ItemId).OrderByDescending(item => item.Count())
          .Select(item => item.Key).ToList());
File.WriteAllText(coldStartByCategoryOutputPath, JsonSerializer.Serialize(coldStartByCategory));

// 6. Treinar Funk SVD
var pipeline = mlContext.Transforms.Conversion
    .MapValueToKey("UserIdEncoded", nameof(RatingData.UserId))
    .Append(mlContext.Transforms.Conversion.MapValueToKey("ItemIdEncoded", nameof(RatingData.ItemId)))
    .Append(mlContext.Recommendation().Trainers.MatrixFactorization(options));
var model = pipeline.Fit(data);
mlContext.Model.Save(model, data.Schema, modelOutputPath);
```

### Fluxo de inferência (`RecommendationService.cs`)

Para usuários com histórico, o score final de cada espetáculo candidato é um **score híbrido** que combina o score SVD normalizado com a afinidade de categoria do usuário:

```csharp
// 1. Score SVD bruto para cada item não consumido
var pred = _engine.Predict(new RatingInput { UserId = uid, ItemId = item });

// 2. Normalizar SVD para [0, 1] no batch de candidatos
var normSvd = svdRange > 1e-6f ? (svdScore - minSvd) / svdRange : 0.5f;

// 3. Afinidade de categoria: fração do histórico do usuário na categoria do item
var affinity = _categoryAffinity[userId][itemCategory];

// 4. Score híbrido (α padrão = 0.7)
var finalScore = _alpha * normSvd + (1f - _alpha) * affinity;

// 5. Ordenar por score híbrido e retornar top-N
return scored.OrderByDescending(x => x.FinalScore).Take(n)
    .Select(x => new RecommendationResult(x.Item, x.FinalScore)).ToList();
```

O `PredictionEngine` não é thread-safe; o acesso ao `_engine.Predict` é protegido por `lock` no `RecommendationService`, que é registrado como **singleton** no DI do ASP.NET Core. O restante da lógica (normalização, combinação híbrida) ocorre fora do lock.

---

## 4. Hiperparâmetros do Modelo

Os hiperparâmetros da versão Python foram determinados por `GridSearchCV` com 3-fold cross-validation sobre 54 combinações. Esses valores orientaram a configuração do `MatrixFactorizationTrainer` no pipeline C#, adaptada à API disponível:

| Hiperparâmetro | Valor | Correspondência Python | Justificativa |
|---|---|---|---|
| `NumberOfIterations` | 20 | `n_epochs=50` | Iterações SGD; valor menor compensa o treino no dataset completo sem CV |
| `ApproximationRank` | 50 | `n_factors=10` | Dimensão dos vetores latentes; aumentado para explorar mais padrões com a API ML.NET |
| `LearningRate` | 0.01 | `lr_all=0.001` | Taxa de atualização dos pesos no SGD |
| `Lambda` | 0.01 | `reg_all=0.2` | Regularização L2 — controla o crescimento dos vetores latentes |
| `Alpha` (`ML:Alpha`) | 0.7 | — | Peso do SVD no score híbrido; configurável em `appsettings.json` |

**Métricas de referência da versão Python** (mantidas como baseline):

| Métrica | SVD (Python) | KNN (Python) |
|---|---|---|
| RMSE ↓ | **0.2953** | 0.3402 |
| MAE ↓ | **0.1210** | 0.1366 |
| Precision@1 ↑ | **0.8926** | 0.8846 |
| Recall@1 ↑ | **0.8766** | 0.8694 |
| F1@1 ↑ | **0.8845** | 0.8770 |

> Executar `dotnet run evaluate` após o retreinamento produz as métricas atualizadas do pipeline C# (SVD puro e híbrido), permitindo comparação direta com este baseline Python.

---

## 5. Scoring Híbrido (SVD + Afinidade de Categoria)

O campo `tipo_evento` existe no dataset com oito categorias — *Show Musical*, *Stand-Up*, *Espetáculo Teatral*, *Espetáculo Musical*, entre outras. No pipeline atual, ele é utilizado ativamente no scoring de recomendações, não apenas preservado no CSV.

### Como a afinidade de categoria é calculada

Para cada usuário, o `Trainer` computa a proporção do seu histórico de compras (medida em soma de `ranking`) dedicada a cada `tipo_evento`:

```
categoryAffinity[userId][tipo] = Σranking(tipo) / Σranking(todos)
```

Por exemplo, um usuário que somou `ranking=5` em Stand-Up e `ranking=5` em Teatro terá afinidade 0,5 para cada categoria.

### Fórmula do score híbrido

$$\text{score}_{ui} = \alpha \cdot \text{SVD}_{ui}^{\text{norm}} + (1 - \alpha) \cdot \text{affinity}(u, \text{tipo}(i))$$

onde $\text{SVD}_{ui}^{\text{norm}}$ é o score SVD bruto normalizado para $[0, 1]$ dentro do batch de candidatos e $\alpha = 0.7$ por padrão (configurável via `ML:Alpha` em `appsettings.json`).

### Efeito prático

Com $\alpha = 0.7$, o SVD ainda domina o ordenamento, mas a afinidade de categoria funciona como desempate e boost para espetáculos alinhados às preferências de gênero do usuário. Dois espetáculos com score SVD similar terão o da categoria preferida do usuário na frente.

A métrica **Category Precision@K** no `Evaluator` quantifica isso: a fração dos top-K recomendados que pertence às duas categorias mais frequentes no histórico do usuário.

---

## 6. Estratégia de Cold-start

Uma usuária nova — como Ana Luiza, que acabou de criar conta sem nenhuma compra registrada no Sympla — não tem `UserId` no conjunto de treino do modelo. O `RecommendationService` detecta esse caso verificando o histórico carregado do `dataset.csv` e aplica o fallback em dois níveis:

### Nível 1 — Cold-start por categoria (usuários com preferências declaradas)

Se o usuário declarou preferências de `tipo_evento` via `PreferenceService` (por exemplo, selecionou *Stand-Up* e *Espetáculo Musical* no onboarding), o sistema usa `cold_start_by_category.json` para retornar os espetáculos mais populares nessas categorias:

```csharp
var prefs = _preferenceService.GetPreferences(userId);  // List<string> de tipo_evento
if (prefs.Count > 0)
{
    var categoryItems = prefs
        .Where(_coldStartByCategory.ContainsKey)
        .SelectMany(tipo => _coldStartByCategory[tipo])
        .Distinct()
        .Take(n)
        .Select(item => new RecommendationResult(item, null))
        .ToList();
    if (categoryItems.Count > 0) return categoryItems;
}
```

### Nível 2 — Popularidade global (fallback final)

Se o usuário não tem histórico nem preferências declaradas, o sistema retorna os `n` espetáculos com maior número de compradores distintos, carregados de `cold_start.json`:

```csharp
return _popularItems.Take(n)
    .Select(item => new RecommendationResult(item, null))
    .ToList();
```

Em ambos os casos, o `Score` retornado é `null`, sinalizando ao consumidor da API que a recomendação é por popularidade, não personalizada. *A Noviça Rebelde* encabeça a lista global — 8.636 compradores distintos.

**Transição para personalização.** Marcos Abrantes, que já tem histórico de compras, recebe predições via SVD híbrido imediatamente. Ana Luiza só sai do cold-start após um ciclo completo: coleta de novos dados do Sympla → atualização do `dataset.csv` → retreinamento (`dotnet run train` em `src/ml/`) → reinício do backend.

---

## 7. Métricas de Avaliação e Comparação SVD vs Híbrido

### Avaliação C# (`Evaluator.cs`)

O `Evaluator` implementa uma divisão 80/20 **por usuário** diretamente em C#, sem dependências externas. Apenas usuários com pelo menos 2 interações entram no conjunto de teste.

```bash
dotnet run evaluate --k 10 --alpha 0.7
```

O resultado compara **SVD puro** (ranking pelo score bruto do modelo) com o **híbrido** (SVD normalizado + afinidade de categoria):

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

  α = 0.7  (SVD weight)
══════════════════════════════════════════════════════
```

### Definição das métricas

**RMSE** mede a raiz do erro quadrático médio entre o `ranking` real e o previsto pelo SVD. Penaliza erros grandes desproporcionalmente.

**MAE** é o desvio absoluto médio entre `ranking` real e previsto.

**Hit Rate@K**: fração dos usuários de teste para os quais pelo menos um dos top-K recomendados estava no conjunto de teste.

**Precision@K**: média da proporção de itens relevantes nos top-K recomendados (por usuário).

**Recall@K**: média da proporção dos itens do conjunto de teste que aparecem nos top-K recomendados (por usuário).

**Category Precision@K**: fração dos top-K que pertence às duas categorias com maior soma de `ranking` no histórico de treino do usuário. Mede se o recomendador respeita as preferências de gênero do usuário.

### Baseline da versão Python

A comparação original Python (SVD vs. KNN) está registrada na seção 4. O delta do modelo híbrido C# em relação ao SVD puro C# é o indicador que deve ser monitorado após ajustes em `alpha` ou nos hiperparâmetros do treino.

### Por que as métricas de classificação binária enganam aqui

O modelo também pode ser avaliado com acurácia/precisão binária usando limiar `ranking >= 2`. O SVD tende a obter precisão e F1 baixos nessa métrica — consequência do dataset desbalanceado. A média global de ranking é 1,08 e 94% dos rankings reais são 1. O modelo otimiza o erro numérico e converge para predições conservadoras, todas abaixo do limiar arbitrário de 2. RMSE e as métricas Top-K são os indicadores mais honestos para feedback implícito deste tipo.

---

## 8. Riscos e Limitações Técnicas

### Thread-safety do PredictionEngine

O `PredictionEngine<T>` do ML.NET **não é thread-safe**. O `RecommendationService` usa um `Lock` para serializar o acesso ao `_engine.Predict` em ambiente multi-thread. A normalização e a combinação híbrida ocorrem fora do lock. Para escalar, a alternativa é usar `Microsoft.Extensions.ML` e seu `PredictionEnginePool<T>`, que mantém um pool de engines e elimina o lock global.

### Modelo estático

O modelo não aprende com novas compras em tempo real. Toda nova interação do Sympla só impacta as recomendações após um ciclo completo de atualização:

```
coleta de dados → atualização de dataset.csv
→ dotnet run train (src/ml/)      ← gera os 5 artefatos
→ reinício do backend (src/backend/)
```

A ausência de um pipeline automatizado para esse ciclo é um ponto de atenção operacional. O fluxo BPMN em `docs/Projeto.md` menciona atualização a cada 24 horas — verificar se e onde este pipeline está implementado.

### Feedback implícito como proxy de interesse

O `ranking` mede dias distintos de compra, não satisfação com o espetáculo. Um usuário que comprou *A Noviça Rebelde* em três dias distintos pode ter comprado ingressos para familiares — não necessariamente assistiu três vezes. Esse ruído é inerente à fonte de dados do Sympla e limita a qualidade dos fatores latentes aprendidos.

### Cold-start de espetáculos novos

O cold-start atual cobre **usuários novos** e usa preferências declaradas quando disponíveis. **Espetáculos novos** não são cobertos: um show adicionado ao catálogo após o treino não tem vetor $q_i$ e não aparece nas predições do modelo. O fallback de popularidade (incluindo `cold_start_by_category.json`) também não inclui espetáculos sem histórico de compra. Peças em pré-estreia são invisíveis para o recomendador até o próximo retreinamento.

### Hiperparâmetros não otimizados (pipeline C#)

Os hiperparâmetros do `MatrixFactorizationTrainer` e o valor de `alpha` foram definidos com base nos valores obtidos pelo `GridSearchCV` da versão Python e em testes manuais. Ainda não foi realizada uma busca formal de hiperparâmetros no pipeline C#. Recomenda-se usar `dotnet run evaluate` com variações de `--alpha` e dos parâmetros do trainer para confirmar ou ajustar os valores de `ApproximationRank`, `LearningRate`, `Lambda` e `Alpha`.
