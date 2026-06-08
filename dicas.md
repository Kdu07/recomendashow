# Orientações para a finalização do projeto — Semana 8

Este documento reúne orientações para a conclusão do projeto nesta semana, com
ênfase no módulo de recomendação e no motor de recomendação (*engine*). Não se
trata de uma correção nem de atribuição de nota; é um conjunto de recomendações
técnicas derivado do estado atual do repositório em 8 de junho de 2026,
considerando o ramo `main`, o ramo `develop` e as *feature branches* em aberto.

## Síntese

Diferentemente de outros grupos, o motor de recomendação já se encontra integrado à
linha principal (`main`). A solução adota Fatoração de Matrizes (Funk SVD) com
separação adequada entre treino *offline* e *serving online*: o `training-service`
treina o modelo e exporta os pesos (`svd_weights.json`); o `recommendation-service`
carrega esses pesos na inicialização (`ModelLoaderService`, registrado como
`IHostedService`) e atende às requisições por meio do `SvdRecommendationService`,
que calcula r̂(u,i) = μ + b_u + b_i + p_u · q_iᵀ, filtra os itens já consumidos e
retorna o *top-N*. O tratamento de *cold-start* por popularidade, os controladores
de métricas e diagnóstico e a geração de gráficos (`ScottPlotChartService`) estão
presentes. Trata-se de uma base consistente.

Sendo assim, o foco da semana não é integração, e sim **maturidade**: reduzir o
acoplamento a arquivos estáticos, cobrir o motor com testes, consolidar as frentes
de modelagem e fechar os itens de encerramento. Observa-se, ainda, que o ramo
`develop` está atrás de `main` (três *commits*), o que convém reconciliar para
manter a coerência do fluxo de trabalho.

## Acoplamento entre o motor e a base de dados

Atualmente, o `recommendation-service` e o `training-service` não possuem cliente de
banco de dados: tanto o modelo (`svd_weights.json`) quanto as interações utilizadas
para o filtro de itens consumidos e para o *cold-start* (`dataset.csv`) são lidos de
arquivos estáticos carregados na inicialização. Em consequência, novas interações
registradas na base (avaliações e eventos de uso) não se refletem nas recomendações
sem uma nova exportação manual.

Recomenda-se aproximar o motor da base de dados, que é a fonte de verdade do
sistema: as interações consumidas pelo `DatasetRepository` deveriam derivar do banco
(diretamente ou por meio do *backend*), e o procedimento de treino deveria consumir
os dados a partir da mesma origem. Caso a leitura direta do banco em tempo de
requisição não seja viável no prazo, convém ao menos documentar com clareza o
procedimento de atualização dos artefatos (exportação dos pesos e do *dataset*) e a
periodicidade pretendida, deixando explícita a limitação atual de *freshness*.

## Consolidação das frentes de modelagem

Coexistem três bases de código relacionadas à modelagem:

- `training-service` — treino do Funk SVD que efetivamente gera os pesos servidos
  em produção;
- `src/ml` — projeto de exploração e *benchmark* em ML.NET (`Benchmarker`,
  `Evaluator`, `Recommender`), referenciado apenas em `docs/Modelo_Recomendacao.md`;
- referência em Python (`inference.py`, `benchmark.py`), da qual o serviço em C# é
  uma porta direta.

A existência de três artefatos com propósitos próximos tende a gerar ambiguidade
sobre qual é a fonte de verdade do modelo. Recomenda-se declarar explicitamente, na
documentação, que o `training-service` é o pipeline de produção e que `src/ml` e o
código Python têm finalidade de exploração e validação comparativa, descrevendo
brevemente como os resultados de *benchmark* sustentaram a escolha do Funk SVD.

## Cobertura de testes do motor

O conjunto de testes existente concentra-se no *backend* (`BackEnd.Aventura.Tests`,
com testes de autenticação, serviços, controladores e *guards*) e no *frontend*. O
motor de recomendação — `recommendation-service`, `training-service` e `src/ml` — não
possui testes automatizados, embora seja o componente de maior complexidade e valor
do projeto.

Recomenda-se priorizar testes de unidade para esse módulo, conforme previsto no
barema (cobertura mínima nos *services*). São candidatos diretos e de alta
testabilidade:

- `SvdRecommendationService` — verificação do cálculo de *score*, da ordenação do
  *top-N*, da exclusão de itens consumidos e do caminho de *cold-start*;
- `DatasetRepository` — construção das estruturas de *lookup* e da lista de
  popularidade;
- `ModelLoaderService` — carregamento e validação dos pesos.

## Integração contínua e orquestração

- **Integração contínua.** O repositório não possui `.gitlab-ci.yml`. Para um
  projeto que adota TDD, recomenda-se um *pipeline* mínimo que execute
  `dotnet build` e `dotnet test` a cada *merge request*, fornecendo evidência
  objetiva de não regressão — especialmente relevante após a inclusão dos testes do
  motor.
- **Orquestração.** O arquivo de composição existente (`docker-compose-db.yml`)
  cobre apenas o banco de dados. Recomenda-se uma composição que suba, de forma
  integrada, o banco, o *backend* e o `recommendation-service`, facilitando a
  execução local completa e a demonstração do sistema.

## Procedimento de retreino e promoção de modelo

A exportação dos pesos é hoje um passo manual. Como o projeto já dispõe de
`MetricsService` e da geração de gráficos de avaliação, recomenda-se formalizar um
procedimento de retreino que registre as métricas de cada versão e condicione a
promoção de um novo modelo à superação de um critério mínimo de qualidade,
preservando o modelo anterior em caso de reprovação. Ainda que executado
manualmente, esse procedimento documentado evidencia maturidade na operação do
modelo e vincula-se aos requisitos não funcionais de confiabilidade.

## Encerramento do projeto

- **Reconciliação de ramos.** Há *feature branches* recentes ainda não integradas
  (integração *front-back* de *login*, padronização de telas e diagrama de
  implantação). Recomenda-se concluir sua integração e alinhar `develop` a `main`
  antes do encerramento.
- **Vídeo demonstrativo.** Há link de vídeo referenciado no `Projeto.md`. Convém
  confirmar que a demonstração evidencia o caminho completo da recomendação (com o
  tratamento de *cold-start*) e a execução do sistema com a tela dividida entre a
  aplicação e o console, conforme solicitado.
- **Documentação Swagger.** Confirmar que as interfaces Swagger do *backend* e do
  `recommendation-service` estão acessíveis e referenciadas no `Projeto.md`.
- **`Projeto.md`.** Recomenda-se que a seção de arquitetura descreva de forma
  explícita a separação treino *offline* / *serving online* e o relacionamento entre
  os três artefatos de modelagem, refletindo a solução efetivamente implementada.

## Arquitetura de referência da recomendação

```
        ┌──────────────────── TREINO (offline) ────────────────────┐
        │  training-service                                          │
        │   DataProcessor → SvdTrainer (Funk SVD) → WeightsExporter  │
        │                                  └── svd_weights.json ──┐  │
        └─────────────────────────────────────────────────────────┼──┘
                                                                   ▼
                                                        ┌────────────────────┐
                                                        │  artefatos servidos │
                                                        │ svd_weights.json +  │
                                                        │     dataset.csv     │
                                                        └────────────────────┘
                                                                   ▲ carga no startup
        ┌──────────────────── SERVING (online) ─────────────────────┼──┐
        │  recommendation-service                                    │  │
        │   ModelLoaderService (IHostedService) ─────────────────────┘  │
        │   SvdRecommendationService: r̂ = μ + b_u + b_i + p_u·q_iᵀ       │
        │     → filtra consumidos → top-N (cold-start: popularidade)     │
        │   MetricsController / DiagnosticsController / gráficos          │
        └────────────────────────────────────────────────────────────────┘

   Recomendação a evoluir: que "artefatos servidos" derivem do banco de dados
   (fonte de verdade), e não de arquivos estáticos desacoplados.
```

## Sequência de atividades sugerida

1. Aproximar o `DatasetRepository` e o treino da base de dados, ou documentar
   formalmente o procedimento de atualização dos artefatos e a limitação de
   *freshness*.
2. Declarar na documentação o `training-service` como pipeline de produção e
   `src/ml`/Python como exploração e *benchmark*.
3. Implementar os testes de unidade do motor (`SvdRecommendationService`,
   `DatasetRepository`, `ModelLoaderService`).
4. Adicionar um *pipeline* mínimo de integração contínua (`dotnet build` e
   `dotnet test`).
5. Estender a composição *Docker* para subir banco, *backend* e
   `recommendation-service` de forma integrada.
6. Formalizar o procedimento de retreino com registro de métricas e critério de
   promoção do modelo.
7. Integrar as *feature branches* pendentes e reconciliar `develop` com `main`.
8. Revisar o vídeo, a documentação Swagger e a seção de arquitetura do `Projeto.md`.

A orientação central é que o grupo já dispõe de um motor de recomendação consistente
e integrado à linha principal. O que determinará a qualidade da entrega nesta semana
é elevar a sua maturidade — reduzindo o acoplamento a arquivos estáticos, cobrindo o
módulo com testes e consolidando a documentação — em vez de iniciar novas frentes de
desenvolvimento.
