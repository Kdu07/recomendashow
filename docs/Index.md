# Índice da Documentação

Este índice organiza os documentos do projeto **G2 - Recomendação de Espetáculos no Portal Aventura**. O projeto ainda está em desenvolvimento, portanto alguns artefatos podem evoluir ao longo das próximas sprints.

## Documentos Principais

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
| [Marketing e Captação de Prospects](./Marketing_Captacao_Prospects.md) | Plano de ações de Inbound, Outbound e Omnichannel, roteiro de vendas outbound B2B com matriz de objeções e simulação de ligação, e estratégia de CRM integrada ao app. | Referência de negócio para captação de prospects e vendas corporativas. |

## Navegação Recomendada

1. Comece por [Projeto](./Projeto.md) para compreender o escopo, requisitos e arquitetura.
2. Consulte [Guia de Padrões de Desenvolvimento](./GuiaPadroesDesenvolvimento.md) para verificar os padrões técnicos adotados.
3. Consulte [Projeto Visual](./projeto_visual.md) para entender a proposta de interface.
4. Use [Gestão do Projeto](./GestaoProjeto.md) para acompanhar retrospectivas, gestão evolutiva e planejamento das sprints.
5. Use [Gestão e Configuração](./Gestao_Projeto_Configuracao.md) para verificar padrões de versionamento e organização.

## Observações

- Este arquivo deve ser atualizado sempre que novos documentos forem adicionados à pasta `docs`.
- Os links utilizam caminhos relativos para facilitar a navegação no repositório.
