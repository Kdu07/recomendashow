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

- Boa priorização de tarefas, garantindo fluxo contínuo de trabalho sem ociosidade da equipe  
- Quebra adequada das tarefas, evitando commits muito grandes e facilitando revisão  
- Colaboração entre áreas, com membros atuando tanto em dados quanto em documentação  
- Uso consistente de issues e commits seguindo o padrão Conventional Commits  
- Estruturação inicial organizada do repositório, facilitando o entendimento do projeto  

## 9.2 - Pontos fracos

- Falta de validação contínua com professores devido ao curto tempo da sprint  
- Demora na aprovação de Merge Requests, impactando o fluxo de integração   

## 9.3 - Ações de melhoria

- Planejar validações com professores antes das entregas finais  
- Estabelecer tempo máximo para revisão de Merge Requests (ex: até 24h)  
- Priorizar abertura antecipada de PRs, mesmo que parciais, para acelerar feedback  
- Manter granularidade das tarefas e commits, garantindo continuidade do bom fluxo de trabalho  