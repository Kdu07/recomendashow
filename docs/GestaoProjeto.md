# Gestão do Projeto
----

## Grupo 2 - Aventureiros

## Projeto - RecomendaShow

## Integrantes

- [Alexsander da Silva Barbosa](https://www.linkedin.com/in/alexsander-dsbarbosa/)
- [Carlos Eduardo Serrano Quaglia](https://www.linkedin.com/in/carlos-quaglia-309088357/)
- [Catarina Sayuri Arashiro Braga Felipe](https://www.linkedin.com/in/catarina-sayuri/)
- [Débora Pereira Nogueira](https://www.linkedin.com/in/debora-pereira-nogueira/)
- [Isaac Souza Santos](https://www.linkedin.com/in/isaac-souza-santos/)
- [Nicholas Anantha Krishnan Millani](https://www.linkedin.com/in/nicholas-millani-83a950357/)
- [Paulo Henrique Bueno Fernandes](https://www.linkedin.com/in/paulo-henrique0601/)

## Objetivo do Documento

Este documento registra a gestão do projeto ao longo das sprints, reunindo retrospectivas, planejamentos, pontos fortes, pontos de melhoria, problemas identificados, ações propostas e evolução entre ciclos. Seu objetivo é apresentar de forma organizada o acompanhamento do trabalho da equipe, preservando o histórico das decisões e melhorias realizadas durante o desenvolvimento do projeto.

## Sumário

 - [Sprint 1](#1-sprint-1)
 - [Sprint 2](#2-sprint-2)
 - [Sprint 3](#3-sprint-3)
 - [Sprint 4 - Gestão Evolutiva](#4-sprint-4---gestão-evolutiva)
 - [Planejamento da Sprint 5](#5-planejamento-da-sprint-5)
 - [Atendimento aos Critérios do Escritório de Projetos](#6-atendimento-aos-critérios-do-escritório-de-projetos)
 - [Uso das Políticas de Gestão de Configuração](#7-uso-das-políticas-de-gestão-de-configuração)
 - [Referências](#8-referências)

---

## 1. Sprint 1

### 1.1 Retrospectiva da Sprint 1

A retrospectiva da Sprint 1 tem como objetivo analisar o desempenho da equipe, identificando pontos positivos, dificuldades encontradas e ações de melhoria para a próxima sprint.

#### 1.1.1 Pontos fortes

- Boa priorização de tarefas, garantindo fluxo contínuo de trabalho sem ociosidade da equipe
- Quebra adequada das tarefas, evitando commits muito grandes e facilitando revisão
- Colaboração entre áreas, com membros atuando tanto em dados quanto em documentação
- Uso consistente de issues e commits seguindo o padrão Conventional Commits
- Estruturação inicial organizada do repositório, facilitando o entendimento do projeto

#### 1.1.2 Pontos fracos

- Parte dos artefatos foi validada apenas próximo da entrega final, gerando retrabalho em diagramas e ajustes estruturais após feedback recebido
- Algumas Merge Requests permaneceram abertas por longos períodos, impactando o fluxo de integração e atrasando tarefas dependentes

#### 1.1.3 Ações de melhoria

| Problema | Ação | Indicador de Sucesso |
|---|---|---|
| Falta de validação contínua | Planejar checkpoints com professores antes das entregas | Redução de retrabalho |
| Demora na aprovação de PRs | Estabelecer prazo máximo de revisão de 24h | Redução do tempo de merge |
| Feedback tardio | Abrir PRs parciais para revisão antecipada | Revisões mais rápidas |
| Tasks muito amplas | Quebrar tarefas grandes em subtarefas menores | Aumento da granularidade |

### 1.2 Planejamento da Sprint 2

O planejamento da Sprint 2 organiza as entregas com base nos artefatos definidos, estruturando as tarefas de forma detalhada, priorizada e com estimativas realistas.

#### 1.2.1 Critério de estimativa

| Tamanho | Tempo estimado |
|------|------|
| PP | Até 15 minutos |
| P | Até 30 minutos |
| M | Até 1 hora |
| G | Até 2 horas |
| GG | Acima de 2 horas |

### 1.3 Evolução entre Sprint 1 e Sprint 2

| Aspecto | Sprint 1 | Sprint 2 | Evolução |
|---|---|---|---|
| Planejamento | Tasks muito amplas | Maior granularidade | Melhorando |
| Pull Requests | Revisões lentas | Revisões mais frequentes | Parcial |
| Organização Git | Estrutura inicial | Fluxo consolidado | Melhorou |
| Conventional Commits | Uso parcial | Uso padronizado | Melhorou |
| Validação contínua | Próxima da entrega | Checkpoints definidos | Melhorando |

---

## 2. Sprint 2

### 2.1 Retrospectiva da Sprint 2

A retrospectiva da Sprint 2 tem como objetivo analisar o desempenho da equipe ao longo da sprint, identificando os pontos que contribuíram positivamente para o desenvolvimento do projeto, as principais dificuldades encontradas durante o processo e as ações de melhoria que podem ser aplicadas nas próximas entregas, em comparação à Sprint 1.

---

### 2.2 Pontos Fortes

* **Proatividade:** Boa iniciativa da equipe na resolução de problemas e suporte mútuo entre os integrantes.
* **Granularidade de Tarefas:** Manutenção de uma boa divisão das tasks, facilitando o acompanhamento e evitando sobrecarga.
* **Eficiência em Code Review:** Melhora significativa no tempo de revisão e aprovação de Pull Requests em relação à sprint anterior.
* **Alinhamento:** Realização de reuniões para garantir a sincronia sobre prioridades e mudanças de rumo no projeto.
* **Padronização Técnica:** Uso organizado de issues, branches e commits padronizados, garantindo rastreabilidade e organização.
* **Colaboração Multidisciplinar:** Apoio mútuo entre as frentes de documentação, desenvolvimento e modelagem.

---

### 2.3 Pontos Fracos

* **Restrição de Tempo Útil:** O tempo de desenvolvimento foi reduzido por ponderadas de sala que se estenderam além do esperado.
* **Interdependência entre Tasks:** Atrasos em atividades críticas impactaram diretamente o início ou conclusão de tarefas relacionadas.
* **Ruído na Comunicação (Dailies):** Falhas pontuais no repasse de informações sobre o andamento e, principalmente, sobre impedimentos enfrentados.
* **Subestimação de Prazos:** Algumas atividades demandaram mais tempo que o previsto, agravado pela interdependência citada.
* **Concentração de Tasks Grandes:** Apesar da melhora na granularidade em relação à Sprint 1, ainda houve predominância de tarefas classificadas como G, dificultando a previsibilidade da sprint.

---

### 2.4 Impactos dos Pontos Fracos no Projeto

* **Gargalo no Fluxo de Trabalho (Efeito Cascata):** A interdependência não mapeada gerou ociosidade forçada ou "mudança de contexto" constante, onde membros precisaram abandonar tarefas pela metade para trabalhar em itens secundários enquanto aguardavam liberações.
* **Assimetria de Informação:** O ruído nas *Dailies* dificultou a compreensão sobre o que estava sendo feito pelos membros.
* **Sobrecarga:** A subestimação de prazos aliada à perda de tempo de dev gasto em ponderadas resultou em uma aceleração não planejada no final da sprint, prejudicando a sustentabilidade do ritmo de trabalho.
* **Baixa Previsibilidade:** A existência de tasks muito amplas dificultou estimativas mais precisas e acompanhamento detalhado do progresso.

#### Tabela de Consequências

| Problema Raiz | Impacto Gerado | Risco Associado |
| :--- | :--- | :--- |
| **Reuniões Longas** | Perda de foco e redução do *throughput*. | Atraso nas entregas principais. |
| **Interdependência** | Bloqueio de tarefas e tempo de espera. | Ineficiência no uso da mão de obra. |
| **Ruído na Daily** | Desalinhamento sobre impedimentos. | Retrabalho por falta de orientação. |
| **Subestimação** | Entrega no limite do prazo. | Comprometimento da estabilidade do código. |
| **Tasks Grandes** | Dificuldade de acompanhamento detalhado. | Redução da previsibilidade da sprint. |

---

### 2.5 Plano de Ação para a Sprint 3 (Melhorias)

Para garantir que as melhorias sejam rastreáveis e menos superficiais, a equipe adotará as seguintes ações:

| Ação de Melhoria | Descrição Detalhada (O "Como") | Indicador de Sucesso / Rastreabilidade |
| :--- | :--- | :--- |
| **Mapeamento de Dependências** | Durante a Planning, utilizaremos a técnica de "Dependency Mapping". Identificaremos quais tarefas bloqueiam outras e sinalizaremos via links de "bloqueado por" no Board. | Redução no tempo de espera (*idle time*) de tarefas dependentes. |
| **Sincronização de Fluxo (Daily)** | Adotar o padrão de 3 perguntas focado no fluxo: 1. O que concluí? 2. O que pretendo concluir hoje? 3. **Existe algum impedimento?**. | Atualização em tempo real do board e ausência de surpresas sobre bloqueios. |
| **Refinamento de Estimativas** | Durante a Planning, tasks consideradas muito amplas serão quebradas em subtarefas menores, reduzindo concentração de tasks G. | Aumento da quantidade de tasks P e M na Sprint 3. |
| **Timeboxing de Reuniões** | Definir limite de tempo para ponderadas e alinhamentos internos, evitando impacto excessivo no tempo útil de desenvolvimento. | Maior tempo efetivo dedicado ao desenvolvimento técnico. |

---

### 2.6 Evolução em Relação à Sprint 1

| Aspecto | Sprint 1 | Sprint 2 | Evolução |
| :--- | :--- | :--- | :--- |
| Granularidade das Tasks | Predomínio de tasks médias/grandes | Inclusão de tasks menores | Melhorou |
| Code Review | Revisões lentas | Revisões mais rápidas | Melhorou |
| Organização Técnica | Estrutura inicial | Fluxo mais consolidado | Melhorou |
| Comunicação | Alinhamento básico | Melhor frequência de reuniões | Parcial |
| Estimativas | Pouca previsibilidade | Melhor divisão, mas ainda com subestimação | Parcial |

---

### 2.7 Conclusão

A Sprint 2 mostrou uma evolução na organização técnica e na colaboração interna. Em comparação à Sprint 1, houve melhora na granularidade das tarefas, na velocidade de revisão de Pull Requests e na consolidação das práticas de organização técnica.

O foco da Sprint 3 será o refinamento da comunicação, a antecipação de bloqueios e o aumento da rastreabilidade das tarefas, garantindo que a interdependência entre atividades não comprometa o cronograma final nem a qualidade técnica da entrega.

---

## 3. Sprint 3

### 3.1 Retrospectiva da Sprint 3

A retrospectiva da Sprint 3 tem como objetivo analisar o desempenho da equipe ao longo da sprint, identificando os pontos que contribuíram positivamente para o desenvolvimento do projeto, as principais dificuldades encontradas durante o processo e as ações de melhoria que podem ser aplicadas nas próximas entregas, em comparação à Sprint anterior.

Durante esta sprint, a equipe apresentou avanços importantes relacionados à comunicação e à colaboração entre os integrantes. Entretanto, ainda foram observadas dificuldades relacionadas à organização das tarefas, estimativas de esforço e granularização das atividades, impactando diretamente a previsibilidade do desenvolvimento.

---

### 3.2 Pontos Fortes

| Ponto Forte | Evidência Observada | Impacto Positivo |
| :--- | :--- | :--- |
| **Proatividade da equipe** | Os integrantes demonstraram maior autonomia na resolução de problemas técnicos e apoio mútuo durante o desenvolvimento das tarefas. | Redução de bloqueios e maior continuidade do desenvolvimento. |
| **Melhoria na comunicação** | Houve maior frequência de alinhamentos durante a sprint e melhor compartilhamento de informações entre os membros. | Menor ocorrência de dúvidas não resolvidas e melhor integração da equipe. |
| **Maior colaboração entre os integrantes** | Integrantes auxiliaram colegas em tarefas técnicas e revisões de implementação. | Melhor distribuição do conhecimento técnico do projeto. |
| **Maior acompanhamento das atividades** | A equipe realizou checkpoints mais frequentes durante o andamento da sprint. | Melhor visibilidade sobre o progresso das tarefas. |
| **Comprometimento com as entregas** | Mesmo diante de dificuldades, a equipe buscou concluir as entregas prioritárias planejadas. | Garantia da continuidade do projeto dentro da sprint. |

---

### 3.3 Pontos Fracos

| Ponto Fraco | Evidência Observada | Consequência |
| :--- | :--- | :--- |
| **Demora na execução das tarefas** | Algumas atividades permaneceram abertas por períodos maiores do que o inicialmente planejado. | Impacto no cronograma e atraso em entregas dependentes. |
| **Estimativas inconsistentes de esforço** | A soma das horas previstas nas tarefas ficou abaixo das horas reais de desenvolvimento da sprint. | Dificuldade em prever corretamente a capacidade da equipe. |
| **Tarefas pouco granularizadas** | Algumas tasks continham múltiplas responsabilidades ou escopo amplo. | Dificuldade no acompanhamento do progresso real da sprint. |
| **Planejamento técnico insuficiente** | Algumas implementações exigiram ajustes posteriores devido à ausência de detalhamento inicial. | Ocorrência de retrabalho e aumento do tempo de desenvolvimento. |

---

### 3.4 Impactos dos Pontos Fracos no Projeto

Os problemas identificados durante a Sprint 3 impactaram principalmente a previsibilidade e o acompanhamento do desenvolvimento. A baixa granularização das tarefas dificultou a visualização clara do andamento da sprint, enquanto as estimativas inconsistentes afetaram o planejamento da carga de trabalho da equipe.

Além disso, a demora na conclusão de algumas atividades gerou dependências entre tarefas e aumento no risco de atrasos em entregas prioritárias. Em determinados casos, a falta de detalhamento técnico inicial também resultou em retrabalho e necessidade de ajustes durante o desenvolvimento.

#### Tabela de Consequências

| Problema Raiz | Impacto Gerado | Risco Associado |
| :--- | :--- | :--- |
| **Demora na execução das tarefas** | Atraso no fluxo de desenvolvimento | Não conclusão de tarefas dentro da sprint |
| **Estimativas inconsistentes** | Planejamento desalinhado da capacidade da equipe | Sobrecarga de integrantes |
| **Tarefas pouco granularizadas** | Baixa visibilidade do progresso | Dificuldade de acompanhamento da sprint |
| **Planejamento técnico insuficiente** | Necessidade de retrabalho | Redução da produtividade |
| **Dependência entre tarefas** | Bloqueio de atividades subsequentes | Acúmulo de tarefas pendentes |

---

### 3.5 Plano de Ação para a Sprint 4 (Melhorias)

Com base nas dificuldades identificadas na Sprint 3, a equipe definiu ações de melhoria diretamente relacionadas aos problemas encontrados, buscando aumentar a rastreabilidade e garantir evolução contínua entre as sprints.

| Problema Identificado | Ação de Melhoria | Como Será Aplicado | Status Esperado / Indicador |
| :--- | :--- | :--- | :--- |
| **Tarefas pouco granularizadas** | Melhor divisão das tasks | Dividir tarefas grandes em subtarefas menores e mais específicas antes do início da sprint. | Tasks menores e com menor tempo médio de conclusão. |
| **Estimativas inconsistentes** | Revisão das estimativas de esforço | Utilizar como referência o tempo real gasto nas tarefas das sprints anteriores. | Maior compatibilidade entre horas previstas e realizadas. |
| **Demora na execução das tarefas** | Checkpoints frequentes | Realizar acompanhamentos periódicos para identificar bloqueios rapidamente. | Redução de tarefas atrasadas ao final da sprint. |
| **Planejamento técnico insuficiente** | Refinamento técnico prévio | Definir previamente requisitos técnicos e critérios de implementação das tarefas. | Redução de retrabalho durante o desenvolvimento. |
| **Dependências entre atividades** | Mapeamento de dependências | Identificar tarefas bloqueadoras antes do início da sprint. | Redução de gargalos no fluxo de desenvolvimento. |

#### Relação entre as Ações da Sprint Anterior e os Resultados Obtidos

| Ação da Sprint Anterior | Objetivo | Resultado Obtido | Situação |
| :--- | :--- | :--- | :--- |
| **Melhorar comunicação da equipe** | Reduzir falhas de alinhamento | Houve melhora perceptível na troca de informações entre integrantes. | **Concluído parcialmente com sucesso** |
| **Maior acompanhamento das tarefas** | Melhorar visibilidade do progresso | Os checkpoints ajudaram no acompanhamento, porém ainda existem dificuldades com tasks amplas. | **Parcialmente concluído** |
| **Reduzir bloqueios técnicos** | Aumentar fluidez do desenvolvimento | Houve evolução na colaboração da equipe, mas ainda ocorreram dependências entre tarefas. | **Parcialmente concluído** |
| **Melhorar planejamento das tarefas** | Tornar sprint mais previsível | Ainda foram identificadas inconsistências nas estimativas e na granularização. | **Não concluído completamente** |

### 3.6 Relação entre Problemas Identificados e Issues da Sprint

Para garantir maior rastreabilidade das dificuldades encontradas durante a Sprint 3, os problemas identificados foram associados diretamente às issues e tarefas relacionadas no board da sprint.

| Problema Identificado | Issue / Task Relacionada | Evidência Observada | Impacto Gerado |
| :--- | :--- | :--- | :--- |
| **Demora na execução das tarefas** | **ISSUE #-123** — Implementação do schema do banco de dados | Task permaneceu vários dias em andamento sem atualização. | Atraso em tarefas dependentes. |
| **Estimativas inconsistentes** | **ISSUE #-147** — Desenvolvimento dos testes de integração | Tempo real ultrapassou significativamente o estimado inicialmente. | Planejamento desalinhado da sprint. |
| **Tarefas pouco granularizadas** | **ISSUE #-147** — Desenvolvimento dos testes de integração | Essa task concentrava trabalho demais, o que poderia ser dividido em tarefas menores. | Dificuldade no acompanhamento do progresso. |

#### Objetivo da Relação com Issues

O relacionamento entre problemas e issues permite:

- Identificar exatamente onde os problemas ocorreram;
- Facilitar análises futuras entre sprints;
- Melhorar a rastreabilidade das ações corretivas;
- Medir se as melhorias propostas realmente reduziram os problemas identificados;
- Criar histórico comparativo de evolução do processo da equipe.

---

### 3.7 Evolução em Relação à Sprint Anterior

| Aspecto | Sprint Anterior | Sprint Atual | Evolução |
| :--- | :--- | :--- | :--- |
| **Comunicação da equipe** | Comunicação menos frequente | Maior alinhamento e troca de informações | Evolução positiva |
| **Colaboração entre integrantes** | Trabalho mais individualizado | Maior apoio entre membros da equipe | Evolução positiva |
| **Acompanhamento das tarefas** | Menor visibilidade do progresso | Checkpoints mais frequentes | Evolução moderada |
| **Planejamento de tempo** | Estimativas imprecisas | Persistência de divergências entre tempo previsto e real | Sem evolução significativa |
| **Granularização das tasks** | Tarefas amplas e pouco específicas | Problema ainda presente em parte da sprint | Necessita melhoria |

---

### 3.8 Conclusão

A Sprint 3 apresentou avanços importantes principalmente na comunicação, colaboração e acompanhamento das atividades da equipe. Os integrantes demonstraram maior proatividade durante o desenvolvimento e houve melhora no alinhamento interno em relação às sprints anteriores.

Por outro lado, ainda persistem desafios relacionados à estimativa de esforço, granularização das tarefas e planejamento técnico inicial, fatores que impactaram diretamente a previsibilidade da sprint e o controle das entregas.

Para a Sprint 4, o foco principal será melhorar a organização e divisão das tarefas, tornar as estimativas mais próximas da realidade e reduzir dependências entre atividades, garantindo maior eficiência, rastreabilidade e previsibilidade no fluxo de desenvolvimento do projeto.

---

## 4. Sprint 4 - Gestão Evolutiva

### 4.1 Objetivo da Sprint 4

A Sprint 4 teve como foco consolidar entregas, revisar artefatos já produzidos, melhorar a organização documental e preparar o projeto para a etapa final de refinamento. A análise da sprint considera observações feitas em plannings, dailies, acompanhamento do Kanban e revisão dos documentos versionados no repositório.

### 4.2 Pontos Fortes

| Ponto forte | Evidência concreta | Impacto no projeto |
|---|---|---|
| Comunicação mais constante entre integrantes | Observado nas dailies e alinhamentos da sprint; pendência: anexar registro do board ou ata de acompanhamento. | Melhorou a visibilidade sobre impedimentos, prioridades e andamento das tasks. |
| Revisão documental mais cuidadosa | Arquivos como `docs/GestaoProjeto.md`, `docs/Projeto.md`, `docs/Index.md`, `docs/Gestao_Projeto_Configuracao.md` e `docs/projeto_visual.md` passaram por revisão de estrutura, cabeçalho, sumário e padronização. | Reduziu inconsistências entre documentos e deixou a entrega mais alinhada aos critérios de avaliação. |
| Colaboração entre frentes de trabalho | A sprint envolveu ajustes em gestão do projeto, documentação técnica, documentação visual e organização de referências internas. | Diminuiu a dependência de uma única frente e ajudou na continuidade das entregas mesmo com ajustes acumulados. |
| Uso do Kanban para acompanhamento | O andamento das tasks foi acompanhado no Kanban; pendência: registrar evidência exportada ou link público do board. | Facilitou a identificação de pendências, tarefas em andamento e itens que precisavam de revisão. |
| Preparação para o refinamento final | A seção de planejamento da Sprint 5 foi estruturada com métrica t-shirt e campos para priorização, dependências e critérios de aceite. | Criou uma base objetiva para transformar os problemas da Sprint 4 em ações planejadas na Sprint 5. |

### 4.3 Pontos Fracos

| Ponto fraco | Evidência concreta | Consequência |
|---|---|---|
| Revisões acumuladas de sprints anteriores | A Sprint 4 precisou corrigir e padronizar documentos já existentes, incluindo sumários, cabeçalhos e organização de conteúdo. | Reduziu o tempo disponível para novas entregas e aumentou o volume de retrabalho documental. |
| Demora para concluir tasks | Observado no acompanhamento do Kanban e nas dailies; pendência: registrar quais cards permaneceram mais tempo em andamento. | Reduziu a previsibilidade da sprint e concentrou ajustes perto do fechamento. |
| Granularidade irregular das tasks | A necessidade de refinamento na Sprint 4 indica que parte das atividades anteriores ainda estava ampla ou pouco detalhada. | Dificultou medir progresso real, estimar esforço e identificar bloqueios rapidamente. |
| Redução de tempo útil por feriado | O calendário da sprint teve menos dias úteis; pendência: registrar no planejamento a data do feriado considerado. | Diminuiu a janela disponível para desenvolvimento, revisão e validação das entregas. |
| Validação final concentrada | Correções finais de documentação e links foram feitas próximas ao fechamento da sprint. | Aumentou o risco de retrabalho e reduziu o tempo para revisão cruzada pela equipe. |

### 4.4 Ações de Melhoria para a Sprint 5

| Problema identificado | Ação de melhoria | Indicador de sucesso | Issue relacionada | Status esperado |
|---|---|---|---|---|
| Revisões acumuladas de sprints anteriores | Priorizar a Sprint 5 como refinamento final dos artefatos principais. | 100% dos documentos principais revisados antes da entrega final. | A definir no board da Sprint 5. | Planejada |
| Demora para concluir tasks | Reduzir o escopo de cada task e acompanhar o avanço diariamente no Kanban. | Pelo menos 80% das tasks concluídas no prazo definido para a sprint. | A definir no board da Sprint 5. | Planejada |
| Granularidade irregular das tasks | Quebrar atividades grandes em tasks menores antes do início da execução. | Pelo menos 70% das tasks classificadas como P ou M pela métrica t-shirt. | A definir no board da Sprint 5. | Planejada |
| Redução de tempo útil por feriado | Considerar a capacidade reduzida no planejamento e priorizar primeiro as tarefas críticas. | 100% das tarefas críticas priorizadas antes das tarefas complementares. | A definir no board da Sprint 5. | Planejada |
| Validação final concentrada | Antecipar revisão documental, build e testes ao longo da sprint. | 100% dos merges para a branch principal com build/testes executados antes do merge, quando aplicável. | A definir no board da Sprint 5. | Planejada |
| Evidências incompletas de acompanhamento | Registrar evidências objetivas do board, dailies e fechamento da sprint. | 100% dos pontos fortes e fracos da Sprint 5 acompanhados por evidência, justificativa ou pendência explícita. | A definir no board da Sprint 5. | Planejada |

### 4.5 Status das Ações da Sprint Anterior

| Ação planejada na sprint anterior | Resultado na Sprint 4 | Status | Evidência ou justificativa |
|---|---|---|---|
| Melhor divisão das tasks | Houve maior atenção à granularidade, mas ainda foram identificadas atividades amplas que precisam ser quebradas na Sprint 5. | Parcialmente concluída | Evidência observada no Kanban; pendência: registrar cards específicos. |
| Revisão das estimativas de esforço | A equipe identificou a necessidade de considerar capacidade reduzida e impacto de calendário no planejamento. | Parcialmente concluída | A Sprint 5 já prevê uso da métrica t-shirt P, M e G para estimativa. |
| Checkpoints frequentes | Dailies e alinhamentos apoiaram a comunicação e o acompanhamento de pendências. | Concluída | Evidência observada nas rotinas de acompanhamento; pendência: anexar registro formal. |
| Refinamento técnico prévio | Parte das revisões técnicas e documentais ainda ocorreu perto do fechamento. | Parcialmente concluída | A Sprint 5 deve antecipar revisão documental, build e testes antes do fechamento. |
| Mapeamento de dependências | As dependências foram discutidas, mas ainda houve impacto de tasks demoradas e ajustes acumulados. | Parcialmente concluída | A Sprint 5 deve registrar dependências diretamente nas tasks planejadas. |

### 4.6 Síntese da Evolução da Sprint

Em relação à Sprint 3, a Sprint 4 evoluiu na comunicação da equipe, na revisão dos documentos e na preparação do refinamento final. Porém, ainda persistiram problemas de granularidade, demora na conclusão de tasks, concentração de validações no fim da sprint e falta de evidências formais para alguns acompanhamentos. O planejamento da Sprint 5 responde a esses pontos ao priorizar refinamento final, uso da métrica t-shirt, registro de dependências, indicadores numéricos e validações antes do fechamento da entrega.

---

## 5. Planejamento da Sprint 5

### 5.1 Objetivo da Sprint 5

A Sprint 5 terá como objetivo realizar o refinamento final do projeto, revisando os artefatos principais, corrigindo lacunas pendentes, validando a consistência técnica e preparando a entrega final com documentação, evidências e organização adequadas.

### 5.2 Critério de Priorização

O detalhamento das tasks da Sprint 5 será definido no board da equipe. A priorização deverá considerar dependência técnica, impacto na nota, risco de entrega, capacidade do time, tempo útil disponível e necessidade de corrigir lacunas apontadas no feedback. A métrica t-shirt adotada para estimativa será:

| Tamanho | Estimativa |
|---|---|
| P | Até 30 minutos |
| M | Entre 30 e 60 minutos |
| G | Entre 60 e 120 minutos |

### 5.3 Lista de Tasks da Sprint 5

A Sprint 5 será voltada ao refinamento final do projeto. As tasks abaixo foram organizadas em uma sequência lógica, priorizando primeiro a correção de lacunas críticas, depois a validação técnica e, por fim, a revisão final da entrega.

| Ordem | Task | Descrição | Prioridade | Tamanho t-shirt | Estimativa | Dependências | Critério de aceite |
|---|---|---|---|---|---|---|---|
| 1 | A preencher | A preencher | Alta / Média / Baixa | P / M / G | Até 30 min / 30-60 min / 60-120 min | A preencher | A preencher |
| 2 | A preencher | A preencher | Alta / Média / Baixa | P / M / G | Até 30 min / 30-60 min / 60-120 min | A preencher | A preencher |
| 3 | A preencher | A preencher | Alta / Média / Baixa | P / M / G | Até 30 min / 30-60 min / 60-120 min | A preencher | A preencher |
| 4 | A preencher | A preencher | Alta / Média / Baixa | P / M / G | Até 30 min / 30-60 min / 60-120 min | A preencher | A preencher |
| 5 | A preencher | A preencher | Alta / Média / Baixa | P / M / G | Até 30 min / 30-60 min / 60-120 min | A preencher | A preencher |

---

## 6. Atendimento aos Critérios do Escritório de Projetos

### 6.1 README

| Nº | Critério | Evidência no repositório/documentação | Status | Ajuste necessário, se houver |
|---|---|---|---|---|
| 1 | Nome do projeto presente? | `README.md` inicia com `# RecomendaShow` e `## Projeto - RecomendaShow`. | Atendido | Nenhum ajuste identificado. |
| 2 | Nome do grupo presente? | `README.md` apresenta `## Grupo 2 - Aventureiros`. | Atendido | Nenhum ajuste identificado. |
| 3 | Integrantes descritos? | Seção `Equipe` do `README.md` lista os sete integrantes. | Atendido | Nenhum ajuste identificado. |
| 4 | Links presentes ou corretos dos integrantes? | Cada integrante está listado com link de LinkedIn no formato `- [Nome](link)`. | Atendido | Nenhum ajuste identificado |
| 5 | Apresenta descrição curta do projeto? | `README.md` apresenta `Recomendação de Espetáculos no Portal Aventura` e a seção `Descrição do Projeto`. | Atendido | Nenhum ajuste identificado. |
| 6 | Apresenta estrutura de pastas conforme o modelo? | `README.md` possui a seção `Estrutura do Repositório`, com `docs/` e `src/`. | Parcialmente atendido | Avaliar se a estrutura deve citar explicitamente contrato/front-end/documentação conforme o modelo da disciplina. |
| 8 | Apresenta histórico de lançamentos? | `README.md` possui a seção `Histórico de Versões`. | Atendido | Nenhum ajuste identificado. |
| 9 | Histórico de lançamentos adequado? | Histórico apresenta versões `0.1.0`, `0.2.0` e `0.3.0-dev`, com datas e descrições. | Atendido | Atualizar caso exista uma nova versão de entrega. |
| 10 | Apresenta licença? | `README.md` possui seção `Licença` com Creative Commons Attribution 4.0. | Atendido | Nenhum ajuste identificado. |
| 11 | Inteli consta na licença? | A licença menciona e linka `Inteli`. | Atendido | Nenhum ajuste identificado. |
| 12 | Nome do grupo ou integrantes presentes na licença? | A licença menciona explicitamente `Grupo 2 - Aventureiros` e lista os integrantes. | Atendido | Nenhum ajuste identificado. |
| 13 | Link dos integrantes ou grupo corretos na licença? | A licença mantém link do repositório e links corretos dos integrantes. | Atendido | Nenhum ajuste identificado. |
| 15 | Não apresenta nenhuma outra imagem além da logo do Inteli? | Não foram identificadas imagens no `README.md`. | Atendido | Caso o critério exija a logo do Inteli, adicionar apenas essa imagem. |
| 16 | Instruções iniciais removidas? | Não foram encontradas instruções iniciais de template no `README.md`. | Atendido | Nenhum ajuste identificado. |

### 6.2 Documentação

| Nº | Critério | Evidência no repositório/documentação | Status | Ajuste necessário, se houver |
|---|---|---|---|---|
| 1 | Apresenta pasta `documentos`, `docs` ou `documents` na raiz do projeto? | Existe pasta `docs` na raiz. | Atendido | Nenhum ajuste identificado. |
| 2 | Apresenta arquivo `Index.md` preenchido que indexa todos os documentos da pasta `docs`? | Existe `docs/Index.md`, indexando os documentos principais encontrados na pasta `docs`. | Atendido | Nenhum ajuste identificado. |
| 3 | Documentação encontrada? | A pasta `docs` contém `Projeto.md`, `GestaoProjeto.md`, `Gestao_Projeto_Configuracao.md`, `Modelo_Recomendacao.md` e `projeto_visual.md`. | Atendido | Nenhum ajuste identificado. |
| 4 | Sumário de todos os documentos `.md` navegáveis? | Documentos principais em `docs` possuem seção `Sumário`. | Atendido | Conferir manualmente se todos os links internos funcionam corretamente. |
| 5 | Instruções e observações dos documentos removidas? | Não foram identificadas instruções iniciais de template nos documentos principais; há templates `A preencher` em seções planejadas. | Parcialmente atendido | Preencher ou remover campos `A preencher` antes da entrega final. |
| 6 | Links acessíveis? | Links locais principais do `docs/Index.md` apontam para documentos existentes na pasta `docs`. | Atendido | Validar manualmente links externos, quando houver. |
| 7 | Não compartilha dados sensíveis? | Documentação não expõe credenciais; o README orienta uso de `.env` e alerta que não deve ser versionado. | Parcialmente atendido | Avaliar a presença do arquivo `.env` versionado/local antes da entrega. |
| 8 | Possui referências bibliográficas, se for o caso? | `docs/Projeto.md` contém seção de referências bibliográficas. | Atendido | Nenhum ajuste identificado. |
| 9 | Formatação adequada? | Documentos principais usam Markdown com títulos, tabelas e sumários. | Atendido | Fazer revisão final de links e quebras de linha antes da entrega. |
| 10 | Apresenta todas as seções preenchidas? | Há campos `A preencher` em `GestaoProjeto.md` e em partes de `Projeto.md`. | Parcialmente atendido | Completar os campos pendentes antes da entrega final. |
| 11 | Arquivos de instrução `.txt` ou não relevantes para o projeto foram removidos? | Não há arquivos `.txt` relevantes na raiz ou em `docs`; há arquivos gerados em ambientes/dependências. | Parcialmente atendido | Ignorar dependências geradas ou garantir que pastas como `.venv311` e `node_modules` não entrem na entrega final. |
| 12 | Links acessíveis sem solicitação de permissão? | Links de documentos locais são relativos; links externos dependem de acesso público. | Parcialmente atendido | Validar links externos e remover links que exijam permissão. |

### 6.3 Aplicação

| Nº | Critério | Evidência no repositório/documentação | Status | Ajuste necessário, se houver |
|---|---|---|---|---|
| 1 | Possui pasta `src`? | Existe pasta `src` na raiz do projeto. | Atendido | Nenhum ajuste identificado. |
| 2 | Possui pasta para a lógica/codificação da aplicação? | `src` contém `backend`, `frontend`, `inference-service`, `ml`, `recommendation-service` e `training-service`. | Atendido | Nenhum ajuste identificado. |
| 3 | Possui arquivo `README.md` com informações sobre como executar a aplicação? | `README.md` possui seção `Configuração e Execução`, com comandos para banco, frontend, backend, ML e inferência. | Atendido | Conferir se os comandos estão atualizados com o estado atual do código. |

---

## 7. Uso das Políticas de Gestão de Configuração

As regras de gestão de configuração utilizadas pelo grupo foram descritas no documento [Gestao_Projeto_Configuracao.md](Gestao_Projeto_Configuracao.md). A tabela abaixo resume como essas políticas orientam o fluxo de versionamento, branches, commits, Pull Requests, issues e releases do projeto.

| Política | Como foi aplicada | Evidência no projeto | Status |
|---|---|---|---|
| Gitflow | O projeto utiliza `develop` como branch de integração e `main` como branch estável, conforme regra descrita em [Gestao_Projeto_Configuracao.md](Gestao_Projeto_Configuracao.md). | O histórico Git registra merges de branches como `fix/reorganizacao-modelo`, `feat/docker-mongodb`, `feat/front-back-login-integration` e `feat/db-lgpd-fields` para `develop`. | Aplicada |
| Padrão de branches | As branches seguem prefixos por tipo de atividade, como `feature/`, `fix/` e `docs/`, conforme convenção definida no documento de gestão de configuração. | Branch atual `docs/standardize-project-management` e histórico com branches `fix/reorganizacao-modelo`, `feat/docker-mongodb`, `feat/front-back-login-integration` e `feat/recommendation-unit-tests`. | Aplicada |
| Fluxo de criação e atualização de branches | Alterações de documentação e desenvolvimento são realizadas em branches específicas antes da integração. | A branch atual concentra ajustes de documentação de gestão; o histórico mostra merges de branches de feature e fix para `develop`. | Aplicada |
| Conventional Commits | Os commits seguem a estrutura `tipo: descrição`, com tipos como `docs`, `feat` e `fix`. | Histórico Git contém commits como `docs: add sprint 4 management and sprint 5 planning #248`, `docs: consolidate project management documentation #248`, `feat: mongodb docker #239` e `fix: trim aplicado no ItemId e TipoEvento #238`. | Aplicada |
| Pull Requests e revisão | As alterações devem ser integradas por Pull Request e revisão antes do merge, conforme política definida no documento de gestão de configuração. | O histórico local registra merges de branches para `develop`; pendência: registrar links ou IDs dos Pull Requests para evidência direta da revisão. | Aplicada |
| Relação com issues | Commits, branches e Pull Requests devem estar relacionados ao escopo das issues sempre que possível. | Histórico Git registra commits vinculados a issues, como `#248`, `#238`, `#239`, `#223` e `#234`. | Aplicada |
| Exclusão de branches finalizadas | Branches finalizadas devem ser removidas após merge para manter o repositório organizado. | A política está definida em [Gestao_Projeto_Configuracao.md](Gestao_Projeto_Configuracao.md); pendência: validar no repositório remoto se branches já mergeadas foram excluídas. | Aplicada |
| Versionamento SemVer | O projeto adota versionamento semântico no formato `MAJOR.MINOR.PATCH`. | O `README.md` possui histórico de versões `0.1.0`, `0.2.0` e `0.3.0-dev`, com descrição das entregas. | Aplicada |
| Aplicação prática no projeto | O fluxo recomendado inclui criação de branch, commits, envio ao remoto, abertura de Pull Request, revisão, merge e exclusão da branch. | O histórico Git apresenta commits e merges compatíveis com o fluxo definido. | Aplicada  |

---

## 8. Referências

As informações deste documento foram registradas a partir das observações realizadas durante as plannings, dailies e acompanhamento do Kanban da equipe ao longo das sprints. Também foram considerados os registros documentais já produzidos anteriormente pelo grupo.
