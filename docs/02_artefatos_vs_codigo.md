# Artefatos vs Codigo - Sprint 3

Este documento cruza os artefatos descritos em `ARTEFATOS.md`, os autoestudos de `AUTOESTUDOS.md` e o mapa inicial de `docs/01_mapa_sprint.md` com a implementacao encontrada no repositorio.

## Visao geral do repositorio analisado

O repositorio esta organizado em camadas e servicos:

| Area | Caminhos principais | Papel tecnico |
| --- | --- | --- |
| Banco de dados | `src/database`, `docker-compose-db.yml`, `DbRunner`, `tests` | Scripts PostgreSQL, carga inicial, execucao dos scripts e teste simples de conexao. |
| Backend principal | `src/backend` | API REST em ASP.NET Core para autenticacao, usuarios, preferencias, espetaculos, teatros, generos, wishlist e recomendacoes. |
| Frontend mobile | `src/frontend/mobile-app-aventura` | App React Native/Expo com navegacao, telas, componentes, viewmodels, repositories, services e testes Jest. |
| Servico de recomendacao | `src/recommendation-service` | API propria de recomendacao baseada em pesos SVD exportados e fallback de cold start. |
| Treinamento e ML | `src/training-service`, `src/ml`, `data` | Processamento de dados, treinamento SVD/ML.NET, benchmark, avaliacao e exportacao de modelos. |
| Testes | `src/backend/BackEnd.Aventura.Tests`, `src/frontend/mobile-app-aventura/tests`, `tests` | Testes unitarios de backend, testes de viewmodels/services do frontend e teste de conexao com banco. |
| Documentacao | `docs`, `README.md`, `assets` | Documento principal do projeto, planejamento de testes, arquitetura, visual, gestao e diagramas/imagens. |

## A1. Implementacao do Banco de Dados

### Onde foi implementado

O artefato foi implementado principalmente em `src/database`, com suporte de execucao em `DbRunner` e configuracao local em `docker-compose-db.yml`.

### Arquivos relevantes

| Arquivo | Funcao |
| --- | --- |
| `src/database/01_create_database.sql` | Cria extensao `pgcrypto`, enums, tabelas, chaves, constraints, indices e relacionamentos. |
| `src/database/02_initial_data.sql` | Insere generos, teatros, usuarios, espetaculos, preferencias, interacoes, wishlist e recomendacoes iniciais. |
| `src/database/README.md` | Documenta entidades, relacionamentos, regras de negocio, execucao e infraestrutura de banco. |
| `src/database/Dockerfile` | Imagem do banco usada pelo `docker-compose`. |
| `docker-compose-db.yml` | Sobe PostgreSQL com variaveis de ambiente e volume persistente. |
| `DbRunner/Program.cs` | Executor .NET que carrega `.env`, conecta ao PostgreSQL e roda scripts SQL. |
| `tests/TesteConexaoBanco.cs` | Teste simples de abertura de conexao via `Npgsql`. |
| `README.md` | Reforca instrucoes de banco local, Supabase/Oracle e execucao do DbRunner. |

### Fluxo tecnico

1. O ambiente recebe credenciais pelo `.env`.
2. O banco pode ser iniciado localmente por `docker-compose-db.yml`, usando `POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_DB` e `POSTGRES_PORT`.
3. O `DbRunner` localiza o `.env`, monta a conexao com PostgreSQL via `Npgsql` e executa os scripts.
4. `01_create_database.sql` prepara o esquema fisico com UUIDs, enums, tabelas normalizadas, relacionamentos 1:1, 1:N e N:N, constraints e indices.
5. `02_initial_data.sql` popula dados minimos com `ON CONFLICT DO NOTHING`, tornando a carga inicial idempotente.
6. O backend acessa essas tabelas por repositories com SQL direto, sem ORM.

### Padroes utilizados

| Padrao | Evidencia |
| --- | --- |
| SQL idempotente | Uso de `CREATE ... IF NOT EXISTS`, blocos `DO $$` e `ON CONFLICT DO NOTHING`. |
| Integridade referencial | FKs com `ON DELETE CASCADE` e `ON DELETE RESTRICT`. |
| Modelo relacional normalizado | Tabelas associativas como `usuario_genero_favorito`, `espetaculo_genero` e `teatro_espetaculo`. |
| Repository no backend | Repositories acessam as tabelas por SQL parametrizado, isolando persistencia da regra de aplicacao. |
| Factory de conexao | `DbConnectionFactory` centraliza a criacao de conexoes `NpgsqlConnection`. |

### Decisoes tecnicas tomadas

- Uso de PostgreSQL como banco relacional principal.
- Uso de UUID como chave primaria.
- Uso de `pgcrypto` para `gen_random_uuid()`.
- Ausencia de ORM, conforme restricao do artefato.
- Regras de negocio criticas tambem no banco: email unico, idade minima, nomes unicos, score entre 0 e 1, ingresso nao negativo e datas validas.
- Separacao entre script estrutural e script de dados iniciais.
- Ambiente dockerizado para reproducibilidade local.
- Documentacao registra hospedagem em Oracle Cloud/Supabase em momentos diferentes; o codigo suporta variaveis `POSTGRES_*` e `DB_*`.

### Relacao com os autoestudos

| Autoestudo | Relacao com a implementacao |
| --- | --- |
| E1 Banco de Dados PostgreSQL | Relacao direta: DDL, DML, constraints, indices, enums, transacoes implicitas por script e integridade relacional. |
| E3 TDD, OOP e BDD | Relacao parcial: teste de conexao e regras duplicadas em dominio/backend permitem validacao automatizada, embora o banco em si nao tenha suite extensa de testes de integracao no repositorio atual. |
| E4 Engenharia de Software | Relacao direta: separacao de responsabilidades, documentacao, configuracao reproduzivel e rastreabilidade entre modelo logico e fisico. |

## A2. Desenvolvimento do Projeto

### Onde foi implementado

O desenvolvimento aparece distribuido em:

- `src/backend`: API principal em ASP.NET Core.
- `src/frontend/mobile-app-aventura`: aplicacao React Native/Expo.
- `src/recommendation-service`: API especifica do motor de recomendacao.
- `src/training-service` e `src/ml`: treinamento, avaliacao e geracao de artefatos de modelo.
- `data/models` e `data/raw`: modelos e dados utilizados pelo motor.
- `docs/Projeto.md`, `docs/projeto_visual.md` e `README.md`: documentacao da arquitetura, progresso e evidencias.

### Arquivos relevantes

| Area | Arquivos |
| --- | --- |
| Entrada backend | `src/backend/Program.cs`, `src/backend/appsettings.json`, `src/backend/extensions/DependencyInjection.cs` |
| Controllers | `src/backend/controller/*Controller.cs`, `src/backend/controller/AuthController.cs`, `src/backend/controller/RecommendationController.cs` |
| Services | `src/backend/services/*Service.cs`, `src/backend/services/I*Service.cs` |
| Repositories | `src/backend/repository/*Repository.cs`, `src/backend/repository/I*Repository.cs` |
| Dominio | `src/backend/domain/*.cs`, `src/backend/domain/EntityGuards.cs` |
| Infraestrutura | `src/backend/infrastructure/DbConnectionFactory.cs`, `src/backend/infrastructure/middleware/GlobalExceptionHandler.cs`, `src/backend/infrastructure/persistence/*` |
| DTOs e mapeamento | `src/backend/dto/*.cs`, `src/backend/mappings/DtoMapper.cs` |
| Frontend | `src/frontend/mobile-app-aventura/App.js`, `RootNavigator.js`, `screens`, `components`, `viewmodels`, `repositories`, `services`, `models`, `constants` |
| Recomendacao | `src/recommendation-service/Program.cs`, `Controllers`, `Services`, `Data`, `Models`, `DTOs` |
| Treinamento/ML | `src/training-service/Program.cs`, `Training/SvdTrainer.cs`, `Data/DataProcessor.cs`, `Export/WeightsExporter.cs`, `src/ml/*.cs` |
| Testes | `src/backend/BackEnd.Aventura.Tests/*.cs`, `src/frontend/mobile-app-aventura/tests/*.js` |
| Documentacao | `docs/Projeto.md`, `docs/projeto_visual.md`, `docs/Modelo_Recomendacao.md`, `README.md` |

Observacao: `docs/GuiaPadroesDesenvolvimento.md` e citado em `ARTEFATOS.md`, mas nao foi encontrado no repositorio.

### Fluxo tecnico

#### Backend principal

1. `Program.cs` carrega variaveis de ambiente procurando `.env` em caminhos candidatos.
2. Registra controllers, Swagger, CORS, tratamento global de excecoes e injecao de dependencias.
3. `DependencyInjection.cs` registra `DbConnectionFactory`, repositories e services.
4. Controllers expostos em `/api/[controller]` recebem requisicoes REST.
5. Services concentram regras de aplicacao, validacoes e orquestracao.
6. Repositories executam SQL parametrizado via `Npgsql`, sem ORM.
7. Domain model aplica invariantes com metodos de criacao/alteracao e `EntityGuards`.
8. `DtoMapper` converte entidades de dominio para DTOs de resposta.
9. `GlobalExceptionHandler` padroniza respostas de erro em `ProblemDetails`.

#### Autenticacao

1. `AuthController` expoe `POST /api/auth/register` e `POST /api/auth/login`.
2. `AuthService.Register` valida nome, email, senha e idade.
3. Senha e armazenada como hash usando `BCrypt.Net.BCrypt.HashPassword`.
4. Usuario e criado por `Usuario.Create`, persistido por `UsuarioRepository` e associado a preferencias.
5. `AuthService.Login` busca usuario por email e valida senha com `BCrypt.Verify`.

Nao foi encontrada integracao real com provedor externo de autenticacao no backend. No frontend existe fluxo de Google, mas implementado como mock em `services/authService.js`.

#### Frontend mobile

1. `App.js` monta `NavigationContainer`.
2. `RootNavigator.js` define stack navigation e bottom tabs.
3. Telas em `screens` consomem hooks em `viewmodels`.
4. Viewmodels encapsulam estado, validacao e handlers de interacao.
5. Repositories abstraem a fonte dos dados.
6. Services usam mocks ou armazenamento local, como `AsyncStorage` para wishlist.
7. Componentes reutilizaveis (`AppButton`, `PosterCard`, `BottomTabBar`, `SearchBar`, etc.) compoem a UI.

O frontend opera majoritariamente com dados locais/mockados, o que tambem esta descrito em `docs/Projeto.md`.

#### Recomendacao e ML

1. `src/training-service` processa `data/raw/database_sympla.csv`, gera dataset e exporta `svd_weights.json`.
2. `SvdTrainer` implementa Funk SVD com mapeamento de usuarios/itens, split treino/teste, SGD, biases e fatores latentes.
3. `src/recommendation-service` carrega pesos SVD e dataset como singletons.
4. `SvdRecommendationService` calcula `global_mean + bu + bi + pu dot qi`, remove itens ja consumidos e aplica fallback cold start.
5. `src/ml` contem pipeline alternativo com ML.NET para treino, recomendacao, benchmark e avaliacao.
6. O backend principal tambem possui `RecommendationService` que carrega modelo ML.NET e combina score SVD normalizado com afinidade de categoria.

#### APIs e Swagger

O Swagger esta configurado no backend principal e no servico de recomendacao por `AddEndpointsApiExplorer()` e `AddSwaggerGen()`, habilitado em ambiente de desenvolvimento.

### Padroes utilizados

| Padrao | Onde aparece | Explicacao |
| --- | --- | --- |
| MVC/API Controller | `controller/*Controller.cs` | Controllers recebem HTTP e delegam regras aos services. |
| Service Layer | `services/*Service.cs` | Regras de aplicacao ficam separadas de controllers e persistencia. |
| Repository | `repository/*Repository.cs` e repositories JS | Isola acesso a dados, tanto SQL no backend quanto mocks/storage no frontend. |
| Dependency Injection | `DependencyInjection.cs`, `Program.cs` | Interfaces sao registradas e resolvidas pelo container ASP.NET. |
| Factory | `DbConnectionFactory` | Centraliza a criacao de conexoes com PostgreSQL. |
| DTO / Mapper | `dto`, `DtoMapper.cs` | Separa modelo de dominio do contrato HTTP. |
| Guards / Invariantes de dominio | `EntityGuards.cs`, entidades em `domain` | Valida entrada e protege regras como idade minima, GUID vazio e strings invalidas. |
| Middleware / Exception Handler | `GlobalExceptionHandler`, `ExceptionHandlingMiddleware` | Padroniza tratamento de erros. |
| MVVM | `screens`, `viewmodels`, `models` no frontend | Telas ficam mais declarativas; viewmodels concentram estado e comportamento. |
| Componentizacao | `components` no frontend | UI reutilizavel e separada por responsabilidade. |
| Singleton para artefatos pesados | `RecommendationService`, `ModelLoaderService`, SVD service | Modelos e pesos sao carregados uma vez e reutilizados. |
| Strategy/porta por interface | `IRecommendationService` | Permite trocar implementacao de recomendacao por SVD, ML.NET ou outro motor. |

### Decisoes tecnicas tomadas

- Backend principal em C#/.NET com ASP.NET Core.
- Acesso direto a PostgreSQL com `Npgsql`, sem ORM.
- Uso de SQL parametrizado nos repositories.
- Uso de BCrypt para hash de senha.
- Erros HTTP padronizados por exception handler.
- Swagger habilitado para documentacao interativa em desenvolvimento.
- Separacao em servicos: backend principal, recommendation-service, training-service e modulo `src/ml`.
- React Native/Expo para app mobile.
- MVVM no frontend para separar view, estado, modelos e fonte de dados.
- Mocks locais no frontend para validar experiencia antes da integracao REST completa.
- AsyncStorage para persistencia local da wishlist no app.
- Motor de recomendacao baseado em SVD/Funk SVD, com cold start por popularidade/categoria.
- Reuso de artefatos de modelo em `data/models`.
- Uso de testes unitarios com fakes no backend e mocks no frontend.

### Relacao com os autoestudos

| Autoestudo | Relacao com a implementacao |
| --- | --- |
| E1 Banco de Dados PostgreSQL | Backend e banco usam SQL direto, constraints e queries parametrizadas; repositories refletem o modelo fisico. |
| E2 React Native | App implementado com React Native/Expo, componentes, navegacao, hooks e MVVM. |
| E3 TDD, OOP e BDD | Testes unitarios cobrem services, controllers, dominio, mappers e viewmodels; OOP aparece em entidades, services, interfaces e repositories. |
| E4 Engenharia de Software | Arquitetura em camadas/SOA, separacao por servicos, documentacao, padroes, testes, requisitos e Swagger. |
| E5 Videos Gerais | Relacao complementar com implementacao de frontend, backend, testes, recomendacao e integracao conforme os temas gerais da sprint. |

### Lacunas ou divergencias encontradas

| Item esperado/documentado | Estado observado |
| --- | --- |
| `docs/GuiaPadroesDesenvolvimento.md` | Nao encontrado. Parte dos padroes esta documentada em `docs/Projeto.md`. |
| API de autenticacao externa obrigatoria | Frontend possui botao/fluxo Google mockado; backend possui autenticacao propria por email/senha. Nao foi encontrada integracao real com provedor externo. |
| Pelo menos uma API adicional externa | ViaCEP e ticketera aparecem em `docs/Projeto.md`, mas nao foram encontrados endpoints/services implementados para chamadas HTTP reais. |
| VHS obrigatorio para testes de integracao | Planejado em `docs/Projeto.md`, mas nao encontrado modulo/codigo `VHS`, `VhsTestClient` ou testes correspondentes no repositorio. |
| Testcontainers/Respawn/WireMock citados | Citados em documentacao, mas nao encontrados como implementacao de testes no codigo atual. |
| Integracao REST completa do frontend com backend | Frontend esta majoritariamente mockado/local; documentacao indica que a troca dos repositories por HTTP ficaria para proximas etapas. |

## A3. Planejamento de Testes

### Onde foi implementado

O planejamento esta documentado em `docs/Projeto.md`, especialmente na secao 17. A execucao automatizada aparece parcialmente em:

- `src/backend/BackEnd.Aventura.Tests`
- `src/frontend/mobile-app-aventura/tests`
- `tests/TesteConexaoBanco.cs`

### Arquivos relevantes

| Arquivo ou pasta | Funcao |
| --- | --- |
| `docs/Projeto.md` | Planejamento de testes funcionais, RNFs, usabilidade, APIs externas e integracao. |
| `src/backend/BackEnd.Aventura.Tests/*.cs` | Testes unitarios de dominio, services, controllers, middleware, DI, mappers e factory de conexao. |
| `src/backend/BackEnd.Aventura.Tests/Fakes/*.cs` | Fakes de repositories para isolar services em testes unitarios. |
| `src/frontend/mobile-app-aventura/tests/*.js` | Testes Jest de viewmodels e services do app mobile. |
| `tests/TesteConexaoBanco.cs` | Teste manual/simples de conexao ao PostgreSQL. |
| `src/frontend/mobile-app-aventura/package.json` | Configuracao de Jest e script `npm test`. |
| `src/backend/BackEnd.Aventura.Tests/BackEnd.Aventura.Tests.csproj` | Projeto xUnit do backend. |

### Fluxo tecnico dos testes

#### Backend

1. Testes instanciam services/controllers com fakes de repositories.
2. Entidades de dominio sao testadas diretamente, verificando invariantes como idade minima, datas, duplicidade e guards.
3. Services sao testados por cenarios positivos e negativos, por exemplo cadastro duplicado, login invalido, wishlist duplicada e limite de recomendacoes.
4. Controllers sao testados para validar resposta HTTP esperada.
5. Middleware global e testado para mapear excecoes em status HTTP adequados.
6. DI e `DbConnectionFactory` possuem testes proprios.

#### Frontend

1. Testes usam Jest e `@testing-library/react`.
2. Repositories sao mockados para isolar viewmodels.
3. Hooks como `useAuthViewModel`, `useWishlistViewModel`, `useSearchViewModel`, `useOnboardingViewModel` e outros sao validados quanto a estado, validacao e handlers.
4. Services locais como onboarding e wishlist storage sao testados com mocks de armazenamento.

#### Planejamento documentado

1. `docs/Projeto.md` define testes funcionais com pre-condicoes, cenarios positivos/negativos, instrucoes e pos-condicoes.
2. RNFs sao planejados para desempenho, seguranca, disponibilidade, usabilidade, compatibilidade e escalabilidade.
3. Usabilidade preve 5 usuarios externos e questionario SUS.
4. APIs externas preve ViaCEP e ticketera.
5. Integracao preve backend, banco, frontend, recommendation-service, ViaCEP, ticketera e uso do modulo VHS.

### Padroes utilizados

| Padrao | Onde aparece | Explicacao |
| --- | --- | --- |
| Arrange-Act-Assert | Testes C# e JS | Estrutura explicita em varios testes unitarios. |
| Fakes | `BackEnd.Aventura.Tests/Fakes` | Substituem repositories reais para isolar services. |
| Mocks | Testes JS com `jest.mock` | Isolam repositories/services ao testar viewmodels. |
| Testes por camada | Domain, Service, Controller, Middleware, Infrastructure, Frontend ViewModel | Permite validar responsabilidades separadas. |
| Planejamento por cenario | `docs/Projeto.md` | Usa pre-condicao, cenario positivo, cenario negativo e pos-condicao. |
| BDD conceitual | Planejamento com user stories, criterios e cenarios | Embora nao use Cucumber/Gherkin, a documentacao segue raciocinio comportamental. |

### Decisoes tecnicas tomadas

- Priorizar testes unitarios no backend com xUnit.
- Usar fakes em vez de repositories reais para isolar comportamento de services.
- Testar regras de dominio diretamente.
- Testar controllers separadamente dos services.
- Testar frontend no nivel de hooks/viewmodels, preservando a arquitetura MVVM.
- Planejar testes manuais/de usabilidade com SUS para validar experiencia real.
- Documentar testes de integracao mais amplos, mesmo que parte deles ainda nao tenha implementacao no codigo.

### Relacao com os autoestudos

| Autoestudo | Relacao com a implementacao |
| --- | --- |
| E1 Banco de Dados PostgreSQL | Teste de conexao e planejamento de testes de integracao com banco; repositories usam SQL parametrizado que pode ser alvo de testes futuros. |
| E2 React Native | Testes de viewmodels e services validam fluxos do app mobile, como autenticacao, onboarding, notificacoes, busca e wishlist. |
| E3 TDD, OOP e BDD | Relacao direta: xUnit/Jest, fakes, mocks, cenarios positivos/negativos, validacao de regras de dominio e services. |
| E4 Engenharia de Software | Relacao direta: estrategia de qualidade, RNFs, testes por camada, usabilidade, integracao e rastreabilidade com requisitos. |
| E5 Videos Gerais | Apoio complementar para praticas de testes, demonstracao e validacao dos fluxos. |

### Lacunas ou divergencias encontradas

| Item planejado | Estado observado |
| --- | --- |
| Testes de integracao com Testcontainers/Respawn | Citados em `docs/Projeto.md`, mas nao localizados no repositorio. |
| WireMock.Net | Citado em planejamento, mas nao localizado no codigo/projetos de teste. |
| VHS | Planejado como obrigatorio para integracoes externas, mas nao encontrado no codigo. |
| ViaCEP e ticketera | Planejados em testes de APIs externas, mas sem implementacao tecnica localizada. |
| Relatorio real de usabilidade com 5 usuarios | Ha roteiro e questionario SUS; nao foi localizado relatorio preenchido com resultados reais. |

## Matriz final: artefato, codigo e autoestudos

| Artefato | Implementacao principal | Autoestudos mais relacionados | Estado tecnico observado |
| --- | --- | --- | --- |
| A1 Implementacao do Banco de Dados | `src/database`, `docker-compose-db.yml`, `DbRunner`, repositories SQL | E1, E4, E3 | Implementado com scripts PostgreSQL, dados iniciais, docker e acesso sem ORM. |
| A2 Desenvolvimento do Projeto | `src/backend`, `src/frontend/mobile-app-aventura`, `src/recommendation-service`, `src/training-service`, `src/ml` | E2, E3, E4, E1 | Implementado em multiplas camadas; frontend ainda majoritariamente mockado; APIs externas reais nao localizadas. |
| A3 Planejamento de Testes | `docs/Projeto.md`, `BackEnd.Aventura.Tests`, testes Jest, `tests/TesteConexaoBanco.cs` | E3, E4, E2, E1 | Planejamento amplo; testes unitarios implementados; integracao/VHS/APIs externas aparecem mais como planejamento do que como codigo. |

## Conclusao tecnica

O repositorio materializa os tres artefatos em graus diferentes. O banco de dados esta bem representado em scripts SQL, documentacao e configuracao de execucao. O desenvolvimento do projeto avancou com backend em camadas, frontend React Native em MVVM, motor de recomendacao, treinamento de modelos e testes unitarios. O planejamento de testes e abrangente em documentacao e possui boa cobertura unitaria em backend/frontend, mas parte dos itens mais avancados de integracao, APIs externas e VHS ainda nao aparece implementada no codigo atual.
