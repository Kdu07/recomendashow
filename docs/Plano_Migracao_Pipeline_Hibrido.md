# Plano de Migração — Consolidar no Pipeline Híbrido (Opção 2)

> **Objetivo:** fazer o **Pipeline B** (Funk SVD híbrido via ML.NET, hoje em `src/ml`) ser o
> **único** motor de recomendação, persistindo no banco da **mesma forma** que o Pipeline A
> persiste hoje (batch 24h → tabela `recomendacoes`). Depois, **remover** o Pipeline A
> (`training-service` + `recommendation-service` na forma atual) e o Pipeline C (sobras Python).
>
> **Abordagem escolhida (Opção 2):** reaproveitar o "casco" de persistência já testado do
> `recommendation-service` (BackgroundService 24h + writer Npgsql) e **trocar apenas o miolo de
> inferência**: SVD puro → híbrido (SVD + afinidade de categoria).

---

## 1. Contexto

### 1.1 Estado atual (três pipelines coexistindo)

| Pipeline | Papel hoje | Onde |
|---|---|---|
| **A** | Treino (`training-service`) + serviço de inferência SVD puro + batch 24h → DB (`recommendation-service`) | `src/training-service`, `src/recommendation-service` |
| **B** | Console: treina modelo **híbrido** (ML.NET) e recomenda no terminal. **Não** persiste no banco | `src/ml` |
| **C** | Sobras do treino original em Python (Surprise) | `data/models/final_model.joblib`, `data/models/__pycache__/`, `src/ml/benchmark.py` |

O **contrato real** entre o motor e o app **é a tabela `recomendacoes`**: o `src/backend`
(`RecomendacaoService` → `RecomendacaoRepository`) apenas **lê** essa tabela e, se vazia, cai num
fallback por gêneros favoritos. O backend **não** carrega modelo. Isso é o que torna a troca de
motor possível sem mexer no backend.

### 1.2 Estado-alvo

```
data/raw/database_sympla.csv
   └─[src/ml: dotnet run train]──► data/dataset/dataset.csv
                                   + data/models/model.zip
                                   + data/models/{cold_start,item_categories,category_affinity,cold_start_by_category}.json
                                        └─[host de inferência (rec-service repurposed)]
                                             └─ BackgroundService (24h): para cada usuário
                                                  → HybridRecommendationService.RecommendAsync
                                                  → RecommendationWriteRepository.SaveAsync
                                                       └─► tabela `recomendacoes`
                                                            └─[src/backend] lê a tabela (inalterado)
```

Resultado: **um treinador** (`src/ml`, console, modelo híbrido) + **um host de inferência/persistência**
(o atual `recommendation-service`, com o miolo trocado) + **backend inalterado**.

---

## 2. Escopo

### 2.1 Dentro do escopo
- Adicionar referências ML.NET ao projeto de host de inferência.
- Criar um adaptador `HybridRecommendationService : IRecommendationService` que **delega** para a
  lógica híbrida de `src/ml` (fonte única da fórmula).
- Carregar os artefatos do modelo híbrido (`model.zip` + JSONs) no startup do host.
- Trocar o registro de DI: `IRecommendationService` passa a resolver para o híbrido.
- Padronizar o diretório canônico de artefatos em `data/models/`.
- Adaptar a suíte de testes do host (SVD → híbrido) mantendo a cobertura do batch e do writer.
- **Portar** as métricas/gráficos para o modelo híbrido (**D2**) — sobre o `model.zip`, não sobre `svd_weights.json`.
- **Remover** os endpoints HTTP de recomendação; persistência só via batch (**D3**).
- **Renomear** o projeto/pasta do host para `inference-service` (**D1**).
- Verificação ponta-a-ponta (treino → host → linhas em `recomendacoes` → `GET` do backend).
- **Pré-requisito de banco bloqueante** (ver §5.1): corrigir o conflito de merge no SQL.
- **Pré-requisito de banco adiado** (ver §5.2, **D4**): mapeamento `user_id (dataset) ↔ usuarios.id (UUID)`.
- Remover Pipeline A (training-service) e C (Python) **após** o gate de verificação.
- Atualizar a documentação (`docs/Modelo_Recomendacao.md`) para refletir o fluxo real.

### 2.2 Fora do escopo
- Qualquer alteração de comportamento do `src/backend` (contrato = tabela `recomendacoes`).
- Reotimização de hiperparâmetros do modelo.
- Otimização de performance do scoring (anotada como dívida técnica, não bloqueante).
- Mudanças no frontend.

---

## 3. Decisões de arquitetura

### 3.1 Fonte única da fórmula híbrida
A fórmula `score = α·SVDnorm + (1-α)·afinidade` já existe em `src/ml/Recommender.cs`.
**Não reimplementar.** O host deve referenciar o projeto `AventuraML` (`src/ml`) e o
`HybridRecommendationService` deve ser um **adaptador fino** que delega a `Recommender.Recommend`.

> Justificativa: evita divergência silenciosa entre "o modelo avaliado offline" e "o modelo que
> persiste". Um único lugar para a fórmula, um único comportamento.

### 3.2 Separação treino × serviço
- **Treino/avaliação:** permanece como console `src/ml` (`dotnet run train`, `dotnet run evaluate`).
- **Serviço/persistência:** o atual `recommendation-service`, repurposed (miolo híbrido).

> Justificativa: treino offline pesado e serving online têm ciclos de vida diferentes. Manter o
> console de treino é coerente com "usar o Pipeline B" e evita reescrever o treinador.

### 3.3 Itens SVD-acoplados — destino decidido
`MetricsService`, `MetricsController`, `ScottPlotChartService`/`IChartService`,
`DiagnosticsController`/`ModelInfoResponse` e `RecommendationController` dependem hoje de
`SvdWeights`/`SvdRecommendationService` — não compilam após a remoção do SVD. Destinos:

| Componente | Decisão | Observação |
|---|---|---|
| `MetricsService` + `MetricsController` + charts (`ScottPlot`) | **Portar** (D2) | Passam a pontuar via `model.zip` |
| `DiagnosticsController`/`ModelInfoResponse` | **Portar** (D2) | Expor metadados do modelo híbrido (nº usuários/itens/fatores) |
| `RecommendationController` (`/api/recommendation/*`) | **Remover** (D3) | Recomendação só via batch → DB |
| `SvdRecommendationService`, `SvdWeights`, loader SVD, refs KNN | **Remover** | Substituídos pelo híbrido |

> **Como as métricas são portadas (constraint inviolável):** RMSE/MAE comparam *rating previsto*
> (escala 1–5) com o `ranking` real, então usam o **score SVD bruto** do `model.zip`
> (`PredictionEngine.Predict().Score`), **não** o score híbrido de ranking `[0,1]`. As métricas de
> ranking (Precision@K, Category Precision@K) podem usar o ranking híbrido. Para respeitar a fonte
> única (§3.1), o cálculo de avaliação deve vir da camada `src/ml` (ex.: expor o que o
> `Evaluator.cs` já calcula), e não ser reimplementado no host.

### 3.4 O host continua web app
Mesmo com D3 (recomendação só via batch), o host **permanece** ASP.NET por causa dos endpoints de
métricas/diagnóstico (D2). O `RecommendationBatchJob` segue como `BackgroundService` dentro dele.

---

## 4. Guardrails (invioláveis)

1. **Backend intocável.** Nenhuma mudança em `src/backend`. O schema lido por
   `RecomendacaoRepository` (`recomendacoes`: `usuario_id, espetaculo_id, score, motivo, data_geracao`)
   é o contrato. Não alterá-lo.
2. **Fórmula híbrida em um só lugar.** Reusar `src/ml/Recommender.cs` via referência de projeto.
   Proibido copiar/colar a fórmula no host.
3. **Diretório canônico de artefatos = `data/models/`.** Atualizar defaults de `src/ml/Program.cs`
   e a config do host para `data/models/...`. Nada de `src/models/` (não existe).
4. **Mesmo `dataset.csv` em treino e serving.** O treino e o `DatasetRepository` devem ler
   **o mesmo** `data/dataset/dataset.csv`. Datasets divergentes ⇒ índices/itens incoerentes.
5. **Paridade de persistência.** Manter `RecommendationWriteRepository` como está
   (DELETE+INSERT por usuário, match `espetaculos.nome`, normalização de score → `[0,1]`, `motivo`).
6. **Thread-safety.** `PredictionEngine<T>` do ML.NET **não é thread-safe**. O batch é sequencial
   (OK). Como **a recomendação HTTP foi removida (D3)**, o risco de concorrência migra para o
   **endpoint de métricas (D2)**, que é HTTP e também usa o engine: serializar esse acesso com `lock`
   (ou `PredictionEnginePool`). Não expor o engine a chamadas concorrentes sem proteção.
7. **Gate antes de deletar.** **Não** remover A nem C antes do checklist de verificação (§7) estar
   100% verde. Deleção é a última fase e em commit separado/reversível.
8. **Não declarar sucesso sem linhas no banco.** Os pré-requisitos de banco (§5) são bloqueantes:
   sem mapeamento `user_id ↔ usuarios.id` correto, a persistência grava **zero** linhas (mesmo bug
   do Pipeline A hoje). "Compila" ≠ "persiste".
9. **Cobertura de testes preservada.** Ao substituir `SvdRecommendationServiceTests`, criar
   equivalentes para o híbrido. `RecommendationBatchJobTests` deve seguir verde.
10. **Trabalho em branch.** Migração e remoção em commits distintos; remoção reversível via `git revert`.

---

## 5. Pré-requisitos de banco (bloqueantes)

> Estes são problemas **pré-existentes** herdados do Pipeline A. Persistir o Pipeline B sem
> resolvê-los reproduz o mesmo resultado vazio.

### 5.1 Conflito de merge no SQL
`src/database/02_sprint4_improvements.sql` contém um marcador de conflito não resolvido na
**linha 99** (`>>>>>>> src/database/02_sprint4_improvements.sql`). O script falha ao rodar.
**Ação:** remover o marcador e validar o arquivo ponta a ponta.

### 5.2 Mapeamento `user_id (dataset) ↔ usuarios.id (UUID)`
`RecommendationWriteRepository.ResolveUsuarioIdAsync` resolve `usuarios WHERE ml_id = @userId`,
mas `ml_id SERIAL` é um auto-incremento por ordem de inserção **sem relação** com o `user_id` do
Sympla no `dataset.csv`. Logo, as recomendações são atribuídas a usuários arbitrários/nenhum.

**Ação (decisão de produto — D4) — ADIADA pelo time:** definir um mapeamento real. Opções:
- (a) Coluna `dataset_user_id INT UNIQUE` em `usuarios`, preenchida no vínculo da conta Sympla,
  e usá-la no `FindUserMappingColumnAsync` (já há suporte a nomes alternativos no writer).
- (b) Para ambiente de demonstração, semear `usuarios` com `ml_id`/coluna de mapeamento igual aos
  `user_id` do dataset que se quer demonstrar.
- (c) Persistir por outra chave de identidade acordada com o time de banco.

> **Status: adiado.** Adiar D4 **não bloqueia** as Fases 0–3 (código), o build, os testes unitários
> nem o boot do host. Bloqueia **somente** o critério de aceite "linhas reais gravadas para o
> usuário certo" (a verificação de banco da Fase 4) e a ida para produção. Como o Pipeline A tem o
> **mesmo** defeito hoje, a remoção de A/C (Fase 5) pode prosseguir após a paridade de código estar
> provada, mesmo com D4 pendente — ver §7 (GATE dividido).

---

## 6. Plano de execução por fases

### Fase 0 — Preparação e pré-requisitos
- [ ] Criar branch de trabalho.
- [ ] Resolver §5.1 (conflito SQL) e validar `02_sprint4_improvements.sql`.
- [ ] Decidir e implementar §5.2 (mapeamento de usuário) — ou registrar o ambiente de verificação
      que satisfaz o mapeamento.
- [ ] Padronizar artefatos em `data/models/`: ajustar defaults de `src/ml/Program.cs` e a seção de
      config de paths do host.

### Fase 1 — Plugar o modelo híbrido no host de inferência
- [ ] Adicionar ao `.csproj` do host: `Microsoft.ML` e `Microsoft.ML.Recommender` (mesmas versões de
      `src/ml`: 3.0.1 / 0.21.1).
- [ ] Adicionar referência de projeto: host → `AventuraML` (`src/ml/AventuraML.csproj`).
- [ ] Criar `HybridModelLoader` (IHostedService) que **valida/carrega** os artefatos no startup
      (`model.zip` + `item_categories.json` + `category_affinity.json` + `cold_start.json`),
      preservando o comportamento "fail-fast antes da primeira requisição" do atual `ModelLoaderService`.
- [ ] Criar `HybridRecommendationService : IRecommendationService`:
  - `ModelType => "hibrido"`.
  - `RecommendAsync(userId, n)` delega a `Recommender.Recommend(userId, n)` e mapeia
    `(string Show, float? Score)` → `RecommendationResult(Espetaculo, Score)`.
  - Cold-start (usuário sem histórico): manter fallback por popularidade.
- [ ] Trocar o registro de DI em `Program.cs` do host: `IRecommendationService` → `HybridRecommendationService`.
- [ ] Apontar a config de paths para `data/models/...` e `data/dataset/dataset.csv`.

### Fase 2 — Tratar os extras acoplados ao SVD  *(D2 portar / D3 remover)*
- [ ] **Remover** `SvdRecommendationService`, `ModelLoaderService` (SVD), `Models/SvdWeights.cs`,
      `IModelLoaderService` (substituídos pela interface/loader do híbrido).
- [ ] **Remover** `RecommendationController` (`/api/recommendation/*` e `/cold-start`) — **D3**, só batch.
- [ ] **Portar** `MetricsService` + `MetricsController` + `ScottPlotChartService`/`IChartService` para
      pontuar via `model.zip` (score SVD bruto p/ RMSE/MAE; ranking híbrido p/ Precision@K), expondo o
      cálculo a partir da camada `src/ml` (não reimplementar). **Manter** o pacote `ScottPlot`.
- [ ] **Portar** `DiagnosticsController`/`ModelInfoResponse` para metadados do modelo híbrido.
- [ ] Proteger o acesso ao `PredictionEngine` no caminho de métricas com `lock` (guardrail 6).
- [ ] **Remover** defaults/menções a KNN (`KnnWeightsJson`, descrição Swagger) — KNN não é entregue.
- [ ] Remover `MathNet.Numerics` do `.csproj` se ficar órfão (o híbrido usa ML.NET).

### Fase 3 — Testes
- [ ] Substituir `SvdRecommendationServiceTests` por `HybridRecommendationServiceTests` (ordenação por
      score híbrido, exclusão de itens consumidos, limite `n`, cold-start por popularidade, `n<=0` lança).
- [ ] Manter `RecommendationBatchJobTests` e `DatasetRepositoryTests` verdes; ajustar mocks se a
      interface do loader mudar.
- [ ] Adicionar teste de mapeamento do writer (resolução de `usuario_id`) se viável com fake/integração.

### Fase 4 — Verificação ponta-a-ponta (GATE)
Ver checklist em §7. **Só avançar para a Fase 5 com o GATE-A (código) verde.** O GATE-B (banco)
fica pendente de D4 e não bloqueia a remoção de A/C (ver §5.2).

### Fase 5 — Remoção dos pipelines A e C
- [ ] Remover `src/training-service` (treino agora é `src/ml`).
- [ ] Remover sobras Python (C): `data/models/final_model.joblib`,
      `data/models/__pycache__/`, `src/ml/benchmark.py`.
- [ ] Remover `data/models/svd_weights.json` (artefato do Pipeline A) e referências.
- [ ] **Renomear** o projeto/pasta do host de `recommendation-service` → `inference-service` (**D1**):
      pasta, `.csproj`, namespace `RecommendationService.*` → `InferenceService.*`, `using`s, e a
      referência no projeto de testes. Validar build após o rename.
- [ ] Limpeza opcional: remover deps ML.NET órfãs e a seção `ML` não usada do `src/backend`
      (`Microsoft.ML*` em `BackEnd.Aventura.csproj`, bloco `"ML"` em `appsettings.json`).

### Fase 6 — Documentação
- [ ] Reescrever `docs/Modelo_Recomendacao.md` para o fluxo real (treino `src/ml` → host de
      inferência → tabela `recomendacoes` → backend), removendo as referências ao
      `RecommendationService.cs` inexistente e aos caminhos `src/models`/`src/dataset`.
- [ ] Atualizar a memória do projeto (`memory/project_svd_notebook.md`) e, se houver, `docs/Index.md`.

---

## 7. Critérios de aceite (GATE dividido — Fase 4)

> O GATE é dividido porque **D4 está adiado**. O **GATE-A (código)** libera a remoção de A/C
> (Fase 5); o **GATE-B (banco)** só fecha quando D4 for resolvido.

### GATE-A — Código e paridade *(não depende de D4; habilita a Fase 5)*
- [ ] `dotnet build` da solução inteira sem erros nem warnings novos.
- [ ] `dotnet test` do host **verde** (testes híbridos + batch + writer).
- [ ] `dotnet run train` (em `src/ml`) gera `data/models/model.zip` + os 4 JSONs em `data/models/`.
- [ ] Host sobe e o loader confirma o carregamento dos artefatos no startup (log "modelo carregado").
- [ ] `GET /api/metrics` retorna métricas calculadas sobre o **modelo híbrido** (`model.zip`); charts respondem PNG.
- [ ] Conferência de que o backend **não** foi alterado (diff vazio em `src/backend`, exceto a limpeza
      opcional explícita da Fase 5).

### GATE-B — Persistência real *(bloqueado por D4)*
- [ ] §5.1 corrigido e mapeamento §5.2 satisfeito (D4 resolvido).
- [ ] Após uma execução do batch, `SELECT count(*) FROM recomendacoes` **> 0** para o usuário esperado.
- [ ] `motivo`/`score` gravados coerentes (score em `[0,1]`).
- [ ] `GET /api/recomendacao/{usuarioId}` do backend retorna itens da tabela (não o fallback) para um
      usuário com recomendações persistidas.

---

## 8. Riscos e mitigação

| Risco | Impacto | Mitigação |
|---|---|---|
| Mapeamento `user_id` não resolvido (§5.2) | Persistência grava 0 linhas | Bloquear o GATE; tratar como pré-requisito de produto |
| `PredictionEngine` não thread-safe | Erro/instabilidade sob concorrência | Batch é sequencial; o endpoint de **métricas** (D2) usa o engine via HTTP → `lock`/pool (guardrail 6) |
| Performance do scoring (≈68k usuários × ~449 itens, 1 predict por par) | Batch lento | Aceitável p/ janela 24h; otimização (scoring em lote) como dívida técnica |
| Divergência treino × serving (dataset/artefatos) | Recomendações incoerentes | Guardrails 3 e 4 (mesmo `data/dataset` e `data/models`) |
| Quebra de compilação dos extras SVD | Build vermelho | Fase 2 remove/porta de forma explícita antes do build final |
| Deleção prematura de A/C | Perda de rollback fácil | Guardrail 7: deletar só pós-GATE, em commit separado |

---

## 9. Plano de rollback

- Toda a migração vive em uma branch; a remoção de A/C é um **commit separado**.
- Se o GATE falhar, reverter para o estado pré-migração (`git revert`/checkout da branch) mantém
  o Pipeline A funcionando como hoje.
- Como o backend lê apenas a tabela `recomendacoes`, reativar o Pipeline A (restaurar
  `recommendation-service`/`training-service`) restabelece a persistência sem tocar no backend.

---

## 10. Decisões (resolvidas)

| ID | Decisão | Resolução |
|---|---|---|
| **D1** | Renomear o projeto/pasta do host? | ✅ **Renomear** `recommendation-service` → `inference-service` (Fase 5) |
| **D2** | Métricas/diagnóstico/gráficos: remover ou portar? | ✅ **Portar** para o modelo híbrido (`model.zip`); manter `ScottPlot` |
| **D3** | Endpoints HTTP de recomendação ou só batch? | ✅ **Só batch**; remover `RecommendationController` |
| **D4** | Mapeamento `user_id ↔ usuarios.id` (§5.2) | ⏸️ **Adiado** — não bloqueia GATE-A; trava só o GATE-B (banco) |
