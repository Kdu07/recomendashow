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

 feature/user-authentication

feature/api-endpoints

### release
Utilizada para preparar uma nova versão para produção.

**Exemplo:**
release/v1.0.0

### hotfix
Utilizada para corrigir problemas críticos em produção.

**Exemplo:**
hotfix/login-error

### docs
Utilizada para criação e atualização de documentação.

**Exemplo:**
docs/atualizacao-readme  

### chore
Utilizada para tarefas de manutenção que não impactam diretamente o código da aplicação (configurações, dependências, scripts, etc).

**Exemplo:**
chore/atualizacao-dependencias  

**Abaixo segue diagrama que simplifica o fluxo:**

<img src="../assets\mermaid-diagram-Wsk9JrR0UmTfhp85LXe6k-low (1).png">

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
5. Realizar revisão (quando aplicável)
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
- Nenhuma branch pode ser mergeada diretamente (sem PR)
- Todo PR deve passar por code review antes da aprovação
- O autor não pode aprovar o próprio PR


# 5 - Ciclo de Vida das Branches

## Criação

- feature: criada ao iniciar uma issue
- release: criada ao final da sprint
- hotfix: criada ao identificar erro crítico em produção

## Finalização

- Após merge, todas as branches devem ser deletadas
- Nenhuma branch deve permanecer aberta após conclusão

# 6 - Relação com issues
- Toda feature deve estar vinculada a uma issue
- O nome da branch deve refletir a issue associada
- Pull Requests devem referenciar a issue correspondente
