# Gestão e configuração

Este documento descreve as práticas de gerenciamento de configuração adotadas pela equipe para controle de versões, organização de branches e manutenção da integridade do repositório. Também define diretrizes para criação, uso e integração de branches, garantindo consistência e colaboração no desenvolvimento.

# 1 - Gitflow

## 1.1 - Branches Principais

### Main
Contém código estável e pronto para produção.  
Só é atualizada por meio de merges de release ou hotfix.

### Develop
Branch de integração para desenvolvimento.  
Todas as branches de feature são mescladas aqui.

## 1.2 - Branches de Suporte

### feature
Utilizada para o desenvolvimento de novas funcionalidades.

**Exemplo:**

```bash
feature/user-authentication
feature/api-endpoints
```

### release
Utilizada para preparar uma nova versão para produção.

**Exemplo:**

```bash
release/v1.0.0
```

### hotfix
Utilizada para corrigir problemas críticos em produção.

**Exemplo:**

```bash
hotfix/login-error
```

### docs
Utilizada para criação e atualização de documentação.

**Exemplo:**

```bash
docs/atualizacao-readme
```

### chore
Utilizada para tarefas de manutenção que não impactam diretamente o código da aplicação, como configurações, dependências, scripts e organização do repositório.

**Exemplo:**

```bash
chore/atualizacao-dependencias
```

**Abaixo segue diagrama que simplifica o fluxo:**

<img src="../assets/mermaid-diagram-Wsk9JrR0UmTfhp85LXe6k-low (1).png">

# 2 - Convenções para nomeação

| Tipo de Branch | Formato |
|------|------|
| Feature | feature/descricao-curta |
| Release | release/numero-da-versao |
| Hotfix | hotfix/descricao-curta |
| Fix | fix/descricao-curta |
| Docs | docs/descricao-curta |
| Chore | chore/descricao-curta |

**Exemplo de criação:**

```bash
git checkout develop
git pull origin develop
git checkout -b feature/autenticacao-de-usuario
```

# 3 - Fluxo de branch

## 3.1 - Fluxo de Desenvolvimento de Feature

1. Criar a branch a partir da develop  
2. Implementar a funcionalidade  
3. Realizar commits seguindo a política de commits  
4. Abrir Pull Request para a develop  
5. Realizar code review  
6. Fazer merge na develop  
7. Deletar branch  

## 3.2 - Fluxo de Desenvolvimento de Release

1. Criar a branch de release a partir da develop  
2. Realizar ajustes finais  
3. Atualizar número da versão  
4. Fazer merge da release na main  
5. Criar tag da versão  
6. Fazer merge de volta na develop  
7. Deletar branch  

## 3.3 - Fluxo de desenvolvimento de Hotfix

1. Criar a branch de hotfix a partir da main  
2. Implementar a correção  
3. Abrir Pull Request para a main  
4. Fazer merge na main  
5. Criar tag da versão  
6. Fazer merge na develop  
7. Deletar branch  

## 3.4 - Fluxo de Documentação (docs)

1. Criar a branch a partir da develop  
2. Realizar alterações na documentação  
3. Realizar commits seguindo a política de commits  
4. Abrir Pull Request para a develop  
5. Realizar revisão, quando aplicável  
6. Fazer merge na develop  
7. Deletar branch  

## 3.5 - Fluxo de Manutenção (chore)

1. Criar a branch a partir da develop  
2. Realizar ajustes de configuração/manutenção  
3. Realizar commits seguindo a política de commits  
4. Abrir Pull Request para a develop  
5. Realizar code review  
6. Fazer merge na develop  
7. Deletar branch  

# 4 - Estratégias de merge e pull request

- Todos os merges devem ser realizados via Pull Request (PR)
- Nenhuma branch pode ser mergeada diretamente, sem PR
- Todo PR deve passar por code review antes da aprovação
- O autor não pode aprovar o próprio PR
- O PR deve conter uma descrição objetiva do que foi alterado
- Quando aplicável, o PR deve referenciar a issue relacionada
- Antes do merge, a branch deve estar atualizada com a develop ou com a branch de destino

# 5 - Ciclo de Vida das Branches

## Criação

- feature: criada ao iniciar uma issue relacionada a uma funcionalidade
- release: criada ao final da sprint ou antes de uma entrega estável
- hotfix: criada ao identificar erro crítico em produção
- docs: criada quando a alteração for focada em documentação
- chore: criada para ajustes de manutenção, configuração ou organização

## Finalização

- Após merge, todas as branches devem ser deletadas
- Nenhuma branch deve permanecer aberta após conclusão
- Branches antigas devem ser revisadas periodicamente para evitar acúmulo no repositório

# 6 - Relação com issues

- Toda feature deve estar vinculada a uma issue
- O nome da branch deve refletir a issue associada
- Pull Requests devem referenciar a issue correspondente
- Commits devem estar relacionados ao escopo da branch

# 7 - Políticas operacionais de Git

Esta seção formaliza os procedimentos de criação, atualização, integração e remoção de branches no repositório do projeto.

## 7.1 - Criação de branch

Toda nova branch deve ser criada a partir da branch base correta, evitando desenvolvimento em branches desatualizadas.

Para funcionalidades, documentação e tarefas de manutenção, a branch base deve ser a `develop`:

```bash
git checkout develop
git pull origin develop
git checkout -b feature/nome-da-funcionalidade
```

Para correções críticas em produção, a branch base deve ser a `main`:

```bash
git checkout main
git pull origin main
git checkout -b hotfix/correcao-critica
```

## 7.2 - Atualização da branch

Antes de abrir um Pull Request, a branch deve ser atualizada com a branch de destino para reduzir conflitos.

```bash
git checkout nome-da-branch
git pull origin develop
```

Caso existam conflitos, eles devem ser resolvidos localmente antes do envio final da branch.

## 7.3 - Commits

Os commits devem ser pequenos, objetivos e relacionados ao escopo da branch. Recomenda-se utilizar mensagens claras, indicando o tipo de alteração realizada.

**Exemplos:**

```bash
docs: update configuration management policies
feat: add user authentication flow
fix: adjust login validation
chore: update project dependencies
```

## 7.4 - Merge

O merge deve ocorrer somente por Pull Request, após revisão do código ou da documentação. O objetivo é garantir rastreabilidade, revisão por pares e controle das alterações integradas ao projeto.

Fluxo recomendado:

1. Finalizar a implementação local
2. Enviar a branch para o repositório remoto
3. Abrir Pull Request para a branch correta
4. Solicitar revisão
5. Ajustar comentários, se necessário
6. Realizar merge após aprovação

```bash
git push origin feature/nome-da-funcionalidade
```

## 7.5 - Exclusão da branch

Após o merge, a branch deve ser excluída para manter o repositório organizado.

Exclusão remota:

```bash
git push origin --delete feature/nome-da-funcionalidade
```

Exclusão local:

```bash
git branch -d feature/nome-da-funcionalidade
```

# 8 - Exemplos práticos de aplicação no projeto

## 8.1 - Exemplo de fluxo para documentação

Uma alteração neste próprio documento deve seguir o fluxo abaixo:

1. Criar a branch a partir da `develop`

```bash
git checkout develop
git pull origin develop
git checkout -b docs/configuration-management-sprint-planning
```

2. Editar o arquivo localizado em `docs/Gestao_Projeto_Configuracao.md`
3. Realizar commits objetivos durante a evolução do documento

```bash
git add docs/Gestao_Projeto_Configuracao.md
git commit -m "docs: formalize git procedures and examples"
git commit -m "docs: add sprint 1 retrospective"
git commit -m "docs: add sprint 2 planning"
```

4. Enviar a branch para o repositório remoto

```bash
git push origin docs/configuration-management-sprint-planning
```

5. Abrir Pull Request para `develop`
6. Solicitar revisão de outro integrante
7. Realizar ajustes, caso necessário
8. Fazer merge após aprovação
9. Excluir a branch após o merge

## 8.2 - Exemplo de fluxo para funcionalidade

Caso a equipe precise implementar uma nova tela de login, o fluxo recomendado seria:

```bash
git checkout develop
git pull origin develop
git checkout -b feature/tela-login
```

Após a implementação, o desenvolvedor realiza commits relacionados apenas à tela de login:

```bash
git add .
git commit -m "feat: add login screen structure"
git commit -m "feat: connect login form validation"
```

Em seguida, a branch é enviada ao remoto e integrada por Pull Request:

```bash
git push origin feature/tela-login
```

O Pull Request deve ser aberto para a `develop`, revisado por outro integrante e, após aprovação, a branch deve ser mesclada e excluída.

## 8.3 - Evidência de uso das políticas

A equipe aplica as políticas de gestão de configuração por meio das seguintes práticas:

- Uso da `main` como branch estável
- Uso da `develop` como branch de integração
- Criação de branches específicas para cada tipo de atividade
- Abertura de Pull Requests para integração de alterações
- Revisão antes do merge
- Exclusão de branches finalizadas
- Separação dos commits por escopo de alteração

Essas práticas reduzem conflitos, aumentam a rastreabilidade das entregas e tornam o histórico do projeto mais organizado.

# 9 - Retrospectiva da Sprint 1

A retrospectiva da Sprint 1 tem como objetivo analisar o desempenho da equipe, identificando pontos positivos, dificuldades encontradas e ações de melhoria para a próxima sprint.

## 9.1 - Pontos fortes

| Ponto forte | Descrição |
|------|------|
| Organização inicial do repositório | A equipe estruturou o repositório com separação entre documentação, código e arquivos de apoio, facilitando a navegação e manutenção. |
| Definição inicial do fluxo de branches | Foram estabelecidas branches principais e de suporte, permitindo maior controle sobre o desenvolvimento. |
| Colaboração entre integrantes | Houve divisão de responsabilidades e participação dos membros na construção dos artefatos da sprint. |
| Registro das práticas de configuração | A equipe iniciou a documentação das políticas de Git e controle de versão, atendendo à necessidade de padronização. |
| Entrega incremental | O artefato foi construído progressivamente, permitindo ajustes ao longo da sprint. |

## 9.2 - Pontos fracos

| Ponto fraco | Impacto |
|------|------|
| Falta de detalhamento em alguns procedimentos | Algumas práticas estavam descritas de forma geral, sem explicitar comandos e etapas completas de execução. |
| Pouca evidência prática do uso das políticas | O documento apresentava conceitos, mas ainda não demonstrava com clareza como o time aplicava essas políticas no projeto. |
| Planejamento inicial pouco detalhado | Algumas atividades não estavam suficientemente quebradas em tarefas menores, dificultando acompanhamento e priorização. |
| Risco de inconsistência nos commits | A ausência de exemplos mais claros poderia gerar mensagens de commit e branches fora do padrão definido. |
| Retrospectiva não consolidada desde o início | Os aprendizados da Sprint 1 precisaram ser organizados posteriormente, em vez de serem registrados continuamente. |

## 9.3 - Ações de melhoria

| Problema identificado | Ação de melhoria |
|------|------|
| Procedimentos pouco explícitos | Criar uma seção formal com comandos para criação, atualização, merge e exclusão de branches. |
| Falta de exemplos reais | Adicionar exemplos práticos conectados ao projeto, incluindo fluxo de documentação e desenvolvimento. |
| Tasks amplas demais | Quebrar as entregas da Sprint 2 em tarefas menores e estimadas com t-shirt sizing. |
| Risco de baixa rastreabilidade | Padronizar commits e exigir referência à issue ou atividade relacionada no Pull Request. |
| Registro tardio de aprendizados | Registrar pontos de melhoria ao longo da sprint, facilitando a retrospectiva final. |

# 10 - Planejamento da Sprint 2

O planejamento da Sprint 2 organiza as próximas entregas em tarefas claras, priorizadas e estimadas com t-shirt sizing.

Critério de estimativa utilizado:

| Tamanho | Tempo estimado |
|------|------|
| P | Até 30 minutos |
| M | Entre 30 e 60 minutos |
| G | Entre 60 e 120 minutos |

## 10.1 - Lista priorizada de tarefas

| Prioridade | Tarefa | Descrição | Estimativa | Dependência |
|------|------|------|------|------|
| 1 | Revisar políticas de Git | Conferir se o fluxo de branches, PRs, merges e deleção está documentado corretamente. | M | Nenhuma |
| 2 | Formalizar procedimentos operacionais | Adicionar comandos e passos explícitos para criar, atualizar, mergear e deletar branches. | M | Revisão das políticas |
| 3 | Adicionar exemplos práticos | Inserir exemplos reais de fluxo de documentação e funcionalidade no contexto do projeto. | M | Procedimentos operacionais |
| 4 | Consolidar retrospectiva da Sprint 1 | Documentar pontos fortes, pontos fracos e ações de melhoria da sprint anterior. | G | Nenhuma |
| 5 | Quebrar entregas da Sprint 2 em tasks | Transformar os objetivos da sprint em tarefas menores e acompanháveis. | M | Retrospectiva |
| 6 | Estimar tasks com t-shirt sizing | Classificar cada tarefa como P, M ou G conforme esforço esperado. | P | Lista de tasks |
| 7 | Sequenciar atividades da Sprint 2 | Ordenar as tarefas de acordo com prioridade e dependências. | P | Estimativas definidas |
| 8 | Validar conformidade com critérios do Escritório de Projetos | Conferir se o documento atende aos critérios exigidos para o artefato. | M | Documento consolidado |
| 9 | Revisar escrita e formatação Markdown | Ajustar clareza, padronização de títulos, tabelas, listas e exemplos. | M | Conteúdo completo |
| 10 | Abrir Pull Request final | Enviar a branch, abrir PR para develop e solicitar revisão de outro integrante. | P | Revisão concluída |

## 10.2 - Sequenciamento lógico

1. Revisar o documento atual e identificar lacunas
2. Completar as políticas operacionais de Git
3. Adicionar exemplos práticos aplicados ao projeto
4. Consolidar a retrospectiva da Sprint 1
5. Planejar a Sprint 2 com tasks, estimativas e prioridades
6. Revisar o Markdown final
7. Abrir Pull Request para validação da equipe

## 10.3 - Critérios de conclusão da Sprint 2

A Sprint 2 será considerada concluída quando:

- O documento `Gestao_Projeto_Configuracao.md` estiver completo
- As políticas de Git estiverem claras e aplicáveis
- A retrospectiva da Sprint 1 estiver registrada
- O planejamento da Sprint 2 estiver priorizado e estimado
- O arquivo estiver armazenado na pasta `docs` do GitLab
- O Pull Request tiver sido aberto, revisado e aprovado
