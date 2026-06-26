# Índice da Documentação — RecomendaShow

Navegação central dos documentos do projeto G02 — Aventureiros.

## Documentos principais

| Documento | Descrição | Uso esperado |
|---|---|---|
| [Projeto](./Projeto.md) | Documento central do projeto, reunindo contexto do parceiro, problema, requisitos, arquitetura, diagramas, planejamento de testes e evolução da solução. | Consulta principal para entendimento técnico e de negócio. |
| [Guia de Padrões de Desenvolvimento](./GuiaPadroesDesenvolvimento.md) | Consolida padrões de pastas, branches, commits, nomenclatura, camadas, erros, testes, padrões de projeto, DoR e DoD. | Referência canônica para desenvolvimento, revisão e avaliação dos padrões do projeto. |
| [Gestão do Projeto](./GestaoProjeto.md) | Consolida retrospectivas, gestão evolutiva, ações de melhoria, evolução das sprints e planejamento revisado da Sprint 5. | Referência para acompanhamento da gestão das sprints. |
| [Gestão e Configuração](./Gestao_Projeto_Configuracao.md) | Define práticas de versionamento, Gitflow, organização de branches, commits e Pull Requests. | Referência para gestão de configuração e colaboração do grupo. |
| [Projeto Visual](./projeto_visual.md) | Documenta decisões visuais, identidade, telas, fluxos e referências de interface do produto. | Apoio para avaliação da experiência do usuário e consistência visual. |
| [Modelo de Recomendação](./Modelo_Recomendacao.md) | Detalha o motor de recomendação híbrido (Funk SVD + afinidade de categoria): pipeline de treino, hiperparâmetros, métricas e contrato com o backend. | Referência técnica do modelo de ML. |
| [Deploy do Ambiente](./Deploy_Ambiente.md) | Documenta a implantação completa do ambiente de produção: arquitetura das duas VMs OCI, decisão arquitetural, limitações conhecidas, procedimento de deploy de cada componente e operação. | Ponto de entrada para qualquer deploy ou troubleshooting de infraestrutura. |
| [Deploy do Motor de Recomendação](./Deploy_Recomendacao.md) | Documenta em detalhes a implantação do serviço de inferência: diagrama, decisões, procedimento via deploy.ps1, operação e troubleshooting. | Runbook específico do inference-service; complemento ao Deploy do Ambiente. |
| [Manual de Implantação](./manual_deploy.md) | Manual simplificado de implantação para uso pelo parceiro Aventura Teatros: pré-requisitos, passo a passo e verificação. | Guia operacional para o parceiro realizar o deploy sem suporte técnico do grupo. |
| [Marketing e Captação de Prospects](./Marketing_Captacao_Prospects.md) | Plano de ações de Inbound, Outbound e Omnichannel, roteiro de vendas outbound B2B com matriz de objeções e simulação de ligação, e estratégia de CRM integrada ao app. | Referência de negócio para captação de prospects e vendas corporativas. |

## Banco de dados

| Documento | Conteúdo |
|---|---|
| [src/database/README.md](../src/database/README.md) | Schema PostgreSQL, dicionário de dados, índices, EXPLAIN ANALYZE, pgbench, MongoDB, Docker, evolução por sprint |

## Guia rápido por tema

| Quero entender… | Ir para |
|---|---|
| O problema e o contexto de negócio | [Projeto.md §1–§8](./Projeto.md) |
| Requisitos funcionais e não-funcionais | [Projeto.md §9–§11](./Projeto.md) |
| Banco de dados e modelagem | [Projeto.md §12–§13](./Projeto.md) e [src/database/README.md](../src/database/README.md) |
| Arquitetura e diagramas UML | [Projeto.md §14–§16](./Projeto.md) |
| Motor de recomendação (SVD, SGD, KNN) | [Modelo_Recomendacao.md](./Modelo_Recomendacao.md) |
| Testes planejados | [Projeto.md §18](./Projeto.md) |
| Testes executados e resultados | [Projeto.md §21](./Projeto.md) |
| Desenvolvimento Sprint 3 | [Projeto.md §19](./Projeto.md) |
| Desenvolvimento Sprint 4 | [Projeto.md §20](./Projeto.md) |
| Desenvolvimento Sprint 5 (MongoDB) | [Projeto.md §23](./Projeto.md) |
| Análise ética IA4People | [Projeto.md §22](./Projeto.md) |
| Gestão e retrospectivas | [GestaoProjeto.md](./GestaoProjeto.md) |
| Padrões de código e commits | [GuiaPadroesDesenvolvimento.md](./GuiaPadroesDesenvolvimento.md) |
| Deploy em produção | [Deploy_Ambiente.md](./Deploy_Ambiente.md) e [Deploy_Recomendacao.md](./Deploy_Recomendacao.md) |
| Manual para o parceiro | [manual_deploy.md](./manual_deploy.md) |
| Design e interface | [projeto_visual.md](./projeto_visual.md) |
| Estratégia de negócio e captação | [Marketing_Captacao_Prospects.md](./Marketing_Captacao_Prospects.md) |
