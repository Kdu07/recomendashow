# Guia de Padrões de Desenvolvimento

## 1. Objetivo do Guia

Este guia consolida os padrões de desenvolvimento adotados no projeto **RecomendaShow**, reunindo convenções de organização, nomenclatura, versionamento, arquitetura, tratamento de erros, testes e critérios de qualidade. O objetivo é servir como referência canônica para novas implementações, revisões de código e refinamentos de documentação.

---

## Sumário

1. [Objetivo do Guia](#1-objetivo-do-guia)
2. [Estrutura de Pastas](#2-estrutura-de-pastas)
3. [Padrão de Branches](#3-padrão-de-branches)
4. [Padrão de Commits](#4-padrão-de-commits)
5. [Padrão de Nomenclatura](#5-padrão-de-nomenclatura)
6. [Arquitetura em Camadas](#6-arquitetura-em-camadas)
7. [Controllers, Services e Repositories](#7-controllers-services-e-repositories)
8. [DTOs e Mappers](#8-dtos-e-mappers)
9. [Tratamento de Erros](#9-tratamento-de-erros)
10. [Testes](#10-testes)
11. [Padrões de Projeto Aplicados](#11-padrões-de-projeto-aplicados)
12. [Definition of Ready e Definition of Done](#12-definition-of-ready-e-definition-of-done)
13. [Referências Internas](#referências-internas)

---

## 2. Estrutura de Pastas

A organização do repositório separa documentação, aplicação, banco de dados, treinamento, inferência e assets.

```text
g02/
|-- assets/
|-- data/
|   |-- raw/
|   `-- models/
|-- docs/
|-- DbRunner/
|-- src/
|   |-- backend/
|   |-- database/
|   |-- frontend/mobile-app-aventura/
|   |-- inference-service/
|   |-- ml/
|   `-- mongodb/
|-- tests/
|-- docker-compose-db.yml
|-- global.json
`-- README.md
```

### Backend

```text
src/backend/
|-- controller/        # Controllers REST
|-- domain/            # Entidades e regras de domínio
|-- dto/               # Objetos de entrada e saída da API
|-- exceptions/        # Exceções customizadas
|-- extensions/        # Registro de dependências
|-- infrastructure/    # Conexão, middleware e persistência auxiliar
|-- mappings/          # Conversão entre entidades e DTOs
|-- repository/        # Contratos e implementações de acesso a dados
`-- services/          # Regras de aplicação e orquestração
```

### Frontend Mobile

```text
src/frontend/mobile-app-aventura/
|-- assets/
|-- components/
|-- constants/
|-- data/
|-- models/
|-- navigation/
|-- repositories/
|-- screens/
|-- services/
|-- tests/
`-- viewmodels/
```

### Serviço de Inferência

```text
src/inference-service/
|-- Controllers/        # Endpoints de métricas e diagnóstico
|-- Data/               # Leitura de dataset e escrita de recomendações
|-- DTOs/               # Contratos de resposta da API de inferência
|-- Infrastructure/     # Middleware e recursos técnicos
|-- Models/             # Modelos internos da inferência
|-- Services/           # Carregamento de modelo, recomendação, métricas e batch
`-- InferenceService.Tests/
```

### Treinamento e Pipeline de ML

```text
src/ml/
|-- Models/
|-- DataPipeline.cs
|-- Preprocessor.cs
|-- Trainer.cs
|-- Recommender.cs
|-- Evaluator.cs
|-- Benchmarker.cs
`-- Program.cs
```

---

## 3. Padrão de Branches

O projeto segue um fluxo baseado em Gitflow, com branches principais e branches de suporte.

| Tipo de branch | Uso | Exemplo |
|---|---|---|
| `main` | Código estável e pronto para entrega | `main` |
| `develop` | Integração das entregas em desenvolvimento | `develop` |
| `feature` | Novas funcionalidades | `feature/autenticacao-usuario` |
| `fix` | Correções de bugs | `fix/validacao-login` |
| `hotfix` | Correções críticas sobre produção | `hotfix/correcao-critica` |
| `release` | Preparação de versão | `release/v1.0.0` |
| `docs` | Alterações de documentação | `docs/guia-padroes-desenvolvimento` |
| `chore` | Manutenção, dependências e configuração | `chore/atualizacao-dependencias` |

Fluxo recomendado:

1. Criar a branch a partir da branch base adequada.
2. Implementar alterações dentro do escopo da branch.
3. Realizar commits pequenos e rastreáveis.
4. Abrir Pull Request para a branch de destino.
5. Solicitar revisão.
6. Realizar merge somente após aprovação.
7. Excluir a branch após o merge.

---

## 4. Padrão de Commits

As mensagens de commit seguem o padrão Conventional Commits.

```text
tipo(escopo): descrição
```

Tipos permitidos:

| Tipo | Uso |
|---|---|
| `feat` | Nova funcionalidade |
| `fix` | Correção de bug |
| `docs` | Alterações em documentação |
| `refactor` | Refatoração sem alteração funcional |
| `test` | Criação ou ajuste de testes |
| `chore` | Manutenção e configuração |
| `style` | Ajustes de formatação |

Exemplos:

```bash
docs: add development standards guide
feat(auth): implement login flow
fix(wishlist): prevent duplicated item
test(recommendation): cover fallback behavior
```

---

## 5. Padrão de Nomenclatura

### C# e .NET

- Classes, interfaces, records, enums, propriedades e métodos usam `PascalCase`.
- Variáveis locais e parâmetros usam `camelCase`.
- Interfaces usam prefixo `I`, como `IUsuarioService` e `IDbConnectionFactory`.
- Classes de teste usam sufixo `Tests`.
- DTOs usam sufixo `Dto`, como `EspetaculoDto`.
- Exceções customizadas usam sufixo `Exception`, como `NotFoundException`.

### JavaScript e React Native

- Componentes React Native usam `PascalCase`, como `HomeScreen` e `EventCard`.
- Hooks e viewmodels usam prefixo `use`, como `useHomeViewModel`.
- Funções, variáveis e instâncias usam `camelCase`.
- Arquivos de teste usam sufixo `.test.js`.
- Constantes compartilhadas ficam em `constants/`.

### Banco de Dados

- Tabelas e colunas seguem o padrão `snake_case`.
- Chaves primárias usam `id`.
- Chaves estrangeiras usam o padrão `entidade_id`.
- Scripts SQL são versionados por ordem de execução, como `01_create_database.sql`, `02_sprint4_improvements.sql` e `03_initial_data.sql`.

---

## 6. Arquitetura em Camadas

O backend utiliza arquitetura em camadas, com separação entre exposição HTTP, regras de aplicação, persistência, domínio e infraestrutura.

```text
Controller -> Service -> Repository -> Infrastructure/Database
                       -> Domain
                       -> DTOs/Mappers
```

Responsabilidades:

| Camada | Responsabilidade |
|---|---|
| Controller | Receber requisições HTTP, validar parâmetros básicos e retornar respostas da API. |
| Service | Orquestrar casos de uso, aplicar regras de negócio e coordenar repositórios. |
| Repository | Isolar consultas SQL, comandos de persistência e leitura do banco. |
| Domain | Representar entidades e regras do domínio. |
| DTO | Definir contratos externos de entrada e saída da API. |
| Mapper | Converter entidades de domínio para DTOs e evitar vazamento do modelo interno. |
| Infrastructure | Concentrar conexão com banco, middleware, DI e recursos técnicos. |

No frontend mobile, a estrutura segue uma organização inspirada em MVVM:

```text
screens -> viewmodels -> repositories/services -> API, storage ou mocks
```

---

## 7. Controllers, Services e Repositories

### Controllers

Controllers devem:

- Expor endpoints REST.
- Delegar regras de negócio para services.
- Retornar códigos HTTP coerentes.
- Evitar acesso direto a banco de dados.
- Evitar lógica extensa de validação ou transformação.

### Services

Services devem:

- Concentrar regras de aplicação.
- Validar fluxos e invariantes antes da persistência.
- Orquestrar chamadas a repositories.
- Lançar exceções de domínio ou aplicação quando necessário.
- Permanecer testáveis de forma isolada.

### Repositories

Repositories devem:

- Encapsular SQL e operações de persistência.
- Receber conexões por meio de `IDbConnectionFactory`.
- Não expor detalhes de banco para controllers.
- Manter queries organizadas e parametrizadas.
- Retornar entidades ou dados necessários para a camada de service.


---

## 8. DTOs e Mappers

DTOs isolam o contrato externo da API e evitam que entidades de domínio sejam expostas diretamente ao frontend.

Regras adotadas:

- Responses da API devem usar DTOs.
- Entidades de domínio não devem ser serializadas diretamente como contrato público.
- Mapeamentos ficam centralizados em `mappings/DtoMapper.cs` ou em mappers específicos de infraestrutura quando necessário.
- Alterações internas no domínio devem preservar compatibilidade dos DTOs sempre que possível.

Exemplos no projeto:

- `EspetaculoDto`
- `TeatroDto`
- `PreferenciasDto`
- `RecomendacaoItemDto`
- `DtoMapper`
- `EspetaculoMapper`

---

## 9. Tratamento de Erros

O backend utiliza exceções customizadas e middleware global para padronizar respostas de erro.

Componentes principais:

- `NotFoundException`
- `ConflictException`
- `GlobalExceptionHandler`
- `ProblemDetails`

Mapeamento esperado:

| Exceção | Status HTTP | Uso |
|---|---|---|
| `ArgumentException` | 400 | Dados inválidos ou regra de validação violada. |
| `UnauthorizedAccessException` | 401 | Acesso não autorizado. |
| `NotFoundException` ou `KeyNotFoundException` | 404 | Recurso inexistente. |
| `ConflictException` | 409 | Conflito de estado, duplicidade ou regra de unicidade. |
| Exceções não tratadas | 500 | Erro interno com mensagem genérica ao usuário. |

Controllers não devem duplicar lógica de tratamento de exceções. A responsabilidade de traduzir exceções para respostas HTTP é do middleware global.

---

## 10. Testes

O projeto utiliza testes automatizados para backend, integração, inferência e frontend.

### Backend

- Framework: `xUnit`.
- Escopo: domínio, services, controllers, mappers, middleware, infraestrutura e regras de negócio.
- Fakes são usados para isolar dependências em testes unitários.

Comando:

```bash
dotnet test src/backend/BackEnd.Aventura.Tests/BackEnd.Aventura.Tests.csproj
```

### Integração

- Framework: `xUnit`.
- Recursos: `WebApplicationFactory`, `Testcontainers`, `Respawn`, `WireMock.Net` e `FluentAssertions`.
- Escopo: API, banco, constraints, triggers, LGPD, autenticação e integrações controladas.

Comando:

```bash
dotnet test src/backend/BackEnd.Aventura.IntegrationTests/BackEnd.Aventura.IntegrationTests.csproj
```

### Inferência

- Framework: `xUnit`.
- Escopo: carregamento de artefatos, serviço híbrido de recomendação, batch e repositórios de dados.

Comando:

```bash
dotnet test src/inference-service/InferenceService.Tests/InferenceService.Tests.csproj
```

### Frontend Mobile

- Framework: `Jest`.
- Escopo: services, viewmodels, onboarding, wishlist, autenticação, busca, notificações e integração ViaCEP mockada.

Comando:

```bash
cd src/frontend/mobile-app-aventura
npm test
```

---

## 11. Padrões de Projeto Aplicados

Esta seção separa padrões de projeto GoF de padrões arquiteturais para evitar classificação incorreta.

### Padrão criacional aplicado

| Tipo | Padrão | Implementação | Justificativa |
|---|---|---|---|
| Criacional | Factory | `src/backend/infrastructure/DbConnectionFactory.cs` | Centraliza a criação de `NpgsqlConnection`, encapsula a construção da string de conexão e permite fallback entre `appsettings` e variáveis de ambiente. |

### Padrões estruturais ou arquiteturais usados

| Classificação | Nome | Implementação | Observação |
|---|---|---|---|
| Estrutural/arquitetural | Mapper | `src/backend/mappings/DtoMapper.cs` e `src/backend/infrastructure/persistence/EspetaculoMapper.cs` | Converte entidades e registros de persistência para contratos adequados. |
| Arquitetural | Repository | `src/backend/repository/*Repository.cs` | Organiza acesso a dados e SQL, mas não é padrão comportamental GoF. |
| Arquitetural | Service Layer | `src/backend/services/*Service.cs` | Organiza regras de aplicação, mas não é padrão comportamental GoF. |
| Infraestrutura ASP.NET | Middleware | `GlobalExceptionHandler` | Centraliza tratamento de exceções, mas não é padrão comportamental GoF. |
| Arquitetural frontend | MVVM inspirado | `screens/`, `viewmodels/`, `repositories/`, `services/` | Separa interface, estado de tela e acesso a dados. |

### Padrão comportamental aplicado

| Tipo | Padrão | Implementação | Justificativa |
|---|---|---|---|
| Comportamental | Strategy | `IRecommendationStrategy`, `StoredRecommendationStrategy` e `FavoriteGenresRecommendationStrategy` | Permite alternar a origem das recomendações sem alterar o contrato público de `RecomendacaoService`. Primeiro são buscadas recomendações persistidas; se não houver resultado, aplica-se fallback por gêneros favoritos. |

O uso de Strategy separa cada algoritmo de recomendação em uma classe própria. Assim, novas estratégias, como cold-start, recomendação por conteúdo ou recomendação colaborativa, podem ser adicionadas sem concentrar todas as decisões dentro de `RecomendacaoService`.

---

## 12. Definition of Ready e Definition of Done

### Definition of Ready

Uma tarefa está pronta para desenvolvimento quando:

- O objetivo da tarefa está claro.
- A tarefa possui escopo pequeno o suficiente para ser concluído dentro da sprint.
- Critérios de aceite foram definidos.
- Dependências técnicas ou de dados foram identificadas.
- Impactos em backend, frontend, banco, ML ou documentação foram mapeados.
- A branch adequada pode ser criada a partir da branch base correta.
- O responsável entende quais arquivos, módulos ou documentos serão alterados.

### Definition of Done

Uma tarefa é considerada concluída quando:

- A implementação atende aos critérios de aceite.
- O código segue os padrões de nomenclatura, camadas e responsabilidade deste guia.
- Não há lógica de negócio indevida em controllers ou telas.
- Erros são tratados pelo mecanismo padronizado do projeto.
- DTOs e mappers foram atualizados quando houver alteração de contrato.
- Testes relevantes foram criados ou atualizados.
- Testes aplicáveis foram executados localmente ou a limitação foi registrada.
- Documentação relacionada foi atualizada quando a alteração impacta uso, arquitetura, banco, ML ou fluxo do produto.
- O Pull Request descreve objetivamente a mudança e referencia issue quando aplicável.
- A branch foi revisada antes do merge.

---

## Referências Internas

- [Projeto](./Projeto.md)
- [Gestão e Configuração](./Gestao_Projeto_Configuracao.md)
- [Modelo de Recomendação](./Modelo_Recomendacao.md)
- [Banco de Dados](../src/database/README.md)
