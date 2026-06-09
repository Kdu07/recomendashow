# Simulado - Sprint 3

Banco de questoes baseado em `docs/01_mapa_sprint.md`, `docs/02_artefatos_vs_codigo.md` e `docs/03_guia_estudo.md`.

## 1. Questoes Objetivas

### Q1. Qual arquivo implementa a criacao do modelo fisico do banco de dados?

A. `src/backend/Program.cs`  
B. `src/database/01_create_database.sql`  
C. `src/frontend/mobile-app-aventura/App.js`  
D. `docs/01_mapa_sprint.md`

**Resposta correta:** B.

**Justificativa:** O arquivo `01_create_database.sql` cria extensao, enums, tabelas, constraints, indices e relacionamentos.

**Relacao com o projeto:** Corresponde ao artefato A1, Implementacao do Banco de Dados.

### Q2. Qual restricao da Sprint 3 impactou diretamente a forma de acesso ao banco?

A. Era obrigatorio usar GraphQL.  
B. Nao eram permitidos ORMs.  
C. O frontend deveria ser web puro.  
D. Era obrigatorio usar MongoDB.

**Resposta correta:** B.

**Justificativa:** `ARTEFATOS.md` informa que ORMs nao sao permitidos; por isso o backend usa repositories com SQL direto via `Npgsql`.

**Relacao com o projeto:** Explica a presenca de `src/backend/repository/*Repository.cs` e `DbConnectionFactory`.

### Q3. Qual padrao criacional foi identificado no backend?

A. Factory.  
B. Observer.  
C. Singleton visual.  
D. Decorator.

**Resposta correta:** A.

**Justificativa:** `DbConnectionFactory` centraliza a criacao de conexoes `NpgsqlConnection`.

**Relacao com o projeto:** Aparece em `src/backend/infrastructure/DbConnectionFactory.cs` e e usado pelos repositories.

### Q4. Qual padrao isola o acesso ao banco de dados no backend?

A. MVVM.  
B. Repository.  
C. BDD.  
D. Redux.

**Resposta correta:** B.

**Justificativa:** Repositories encapsulam SQL e acesso a persistencia, evitando que services executem queries diretamente.

**Relacao com o projeto:** Aparece em `src/backend/repository/*Repository.cs`.

### Q5. No frontend, qual estrutura corresponde ao ViewModel no padrao MVVM?

A. `assets`.  
B. `viewmodels`.  
C. `constants`.  
D. `appsettings.json`.

**Resposta correta:** B.

**Justificativa:** Hooks como `useAuthViewModel` e `useHomeViewModel` concentram estado, validacao e handlers.

**Relacao com o projeto:** Aparece em `src/frontend/mobile-app-aventura/viewmodels`.

### Q6. Qual tecnologia foi usada para persistir localmente a wishlist no frontend?

A. PostgreSQL direto no app.  
B. AsyncStorage.  
C. Redis.  
D. SQLite via ORM.

**Resposta correta:** B.

**Justificativa:** O service `wishlistStorageService.js` usa `@react-native-async-storage/async-storage`.

**Relacao com o projeto:** Aparece em `src/frontend/mobile-app-aventura/services/wishlistStorageService.js`.

### Q7. Qual ferramenta documenta interativamente os endpoints da API?

A. Swagger.  
B. Docker Compose.  
C. BCrypt.  
D. Respawn.

**Resposta correta:** A.

**Justificativa:** O backend e o recommendation-service registram `AddSwaggerGen()` e habilitam Swagger UI em desenvolvimento.

**Relacao com o projeto:** Aparece em `src/backend/Program.cs` e `src/recommendation-service/Program.cs`.

### Q8. Qual tecnologia e usada para hash de senha no backend?

A. JWT puro.  
B. BCrypt.  
C. MD5.  
D. Base64.

**Resposta correta:** B.

**Justificativa:** `AuthService` usa `BCrypt.Net.BCrypt.HashPassword` e `BCrypt.Verify`.

**Relacao com o projeto:** Aparece em `src/backend/services/AuthService.cs`.

### Q9. Qual item foi planejado, mas nao localizado como implementacao no codigo atual?

A. SQL de criacao do banco.  
B. Backend com controllers.  
C. Modulo VHS.  
D. Testes unitarios de backend.

**Resposta correta:** C.

**Justificativa:** `docs/02_artefatos_vs_codigo.md` registra que VHS foi planejado, mas nao encontrado no codigo.

**Relacao com o projeto:** Importante para diferenciar planejamento de implementacao real.

### Q10. Qual tecnica principal aparece no servico de recomendacao?

A. Funk SVD.  
B. Bubble Sort.  
C. Criptografia simetrica.  
D. Renderizacao server-side.

**Resposta correta:** A.

**Justificativa:** `SvdTrainer` e `SvdRecommendationService` trabalham com pesos SVD e fatores latentes.

**Relacao com o projeto:** Aparece em `src/training-service/Training/SvdTrainer.cs` e `src/recommendation-service/Services/SvdRecommendationService.cs`.

## 2. Questoes Discursivas

### Q11. Explique como o artefato de banco de dados foi implementado no repositorio.

**Resposta correta:** O artefato foi implementado em `src/database`, principalmente por `01_create_database.sql`, `02_initial_data.sql` e `README.md`. O projeto tambem possui `docker-compose-db.yml` para subir PostgreSQL localmente e `DbRunner/Program.cs` para carregar `.env`, conectar ao banco e executar os scripts.

**Justificativa:** Esses arquivos cobrem criacao do esquema fisico, carga inicial, documentacao e configuracao reproduzivel.

**Relacao com o projeto:** Relaciona diretamente A1 aos arquivos reais do repositorio.

### Q12. Explique o fluxo tecnico de uma requisicao REST no backend principal.

**Resposta correta:** A requisicao chega a um controller em `src/backend/controller`. O controller chama um service em `src/backend/services`, que aplica regras de negocio e usa repositories quando precisa de dados. Os repositories acessam o PostgreSQL via `Npgsql` e `DbConnectionFactory`. A resposta e convertida para DTO por mappers e retornada ao cliente.

**Justificativa:** Esse fluxo demonstra separacao de responsabilidades em controller, service, repository, dominio e DTO.

**Relacao com o projeto:** Resume a arquitetura implementada no backend.

### Q13. Explique por que o frontend foi classificado como majoritariamente mockado.

**Resposta correta:** Porque os repositories e services do frontend retornam dados locais ou simulados, como `authService.js` com tokens mockados e `wishlistStorageService.js` com AsyncStorage. Nao foi encontrada integracao REST completa com o backend.

**Justificativa:** A propria documentacao indica que a troca por chamadas HTTP reais ficaria para etapas posteriores.

**Relacao com o projeto:** Conecta a implementacao em `src/frontend/mobile-app-aventura` as lacunas registradas em `docs/02_artefatos_vs_codigo.md`.

### Q14. Explique a diferenca entre testes unitarios e testes de integracao no contexto do projeto.

**Resposta correta:** Testes unitarios validam unidades isoladas, como services, controllers, entidades e viewmodels, usando fakes ou mocks. Testes de integracao validariam a comunicacao real entre componentes, como backend, banco, APIs externas e servico de recomendacao. No projeto, testes unitarios existem; testes de integracao com Testcontainers, WireMock e VHS aparecem mais como planejamento.

**Justificativa:** A diferenca esta no grau de isolamento e no numero de componentes reais envolvidos.

**Relacao com o projeto:** Aplica-se a `BackEnd.Aventura.Tests`, testes Jest e planejamento em `docs/Projeto.md`.

### Q15. Explique como o projeto lida com cold start no sistema de recomendacao.

**Resposta correta:** Quando o usuario nao possui historico suficiente, o sistema retorna itens populares ou itens baseados em preferencias/categorias, com score nulo ou fallback. Isso evita que a recomendacao fique vazia para usuarios novos.

**Justificativa:** Sistemas de recomendacao precisam tratar usuarios sem interacoes anteriores.

**Relacao com o projeto:** Aparece em `SvdRecommendationService` e no `RecommendationService` do backend.

## 3. Questoes Sobre Decisoes Tecnicas

### Q16. Por que o time escolheu SQL direto em vez de ORM?

**Resposta correta:** Porque o artefato proibia ORMs e exigia scripts SQL funcionais. A escolha por SQL direto com `Npgsql` atende a restricao e da controle sobre queries, constraints e estrutura fisica.

**Justificativa:** A decisao e guiada por requisito academico e por controle tecnico do banco.

**Relacao com o projeto:** Explica `src/database/*.sql`, `DbConnectionFactory` e repositories.

### Q17. Por que usar UUID como chave primaria?

**Resposta correta:** UUID reduz risco de colisao, facilita identificadores unicos distribuidos e evita expor sequencias incrementais. No projeto, `gen_random_uuid()` gera ids para entidades principais.

**Justificativa:** O dominio possui varias entidades e relacionamentos; UUID e uma escolha robusta para ids globais.

**Relacao com o projeto:** Aparece em `src/database/01_create_database.sql`.

### Q18. Por que BCrypt e mais adequado que MD5 para senhas?

**Resposta correta:** BCrypt e projetado para hash de senhas, possui custo computacional ajustavel e dificulta ataques de força bruta. MD5 e rapido e inseguro para senhas.

**Justificativa:** Senhas exigem hash forte e lento, nao codificacao simples ou hash fraco.

**Relacao com o projeto:** `AuthService` usa BCrypt para cadastro e login.

### Q19. Por que usar Swagger no backend?

**Resposta correta:** Para documentar e testar endpoints em ambiente de desenvolvimento, dando visibilidade aos contratos da API.

**Justificativa:** O artefato exigia contratos de API e Swagger funcional.

**Relacao com o projeto:** Configurado em `Program.cs` do backend e do recommendation-service.

### Q20. Por que o frontend foi estruturado com MVVM?

**Resposta correta:** Para separar tela, estado, comportamento e fonte de dados. Isso permite testar viewmodels e futuramente trocar mocks por chamadas HTTP sem reescrever as telas.

**Justificativa:** MVVM reduz acoplamento e melhora manutenibilidade.

**Relacao com o projeto:** Aparece em `screens`, `viewmodels`, `models`, `repositories` e `services`.

### Q21. Por que usar fakes nos testes de backend?

**Resposta correta:** Para testar services isoladamente, sem depender do banco real. Fakes tornam testes mais rapidos, determinísticos e focados na regra de negocio.

**Justificativa:** Testes unitarios devem evitar dependencias externas.

**Relacao com o projeto:** Fakes aparecem em `src/backend/BackEnd.Aventura.Tests/Fakes`.

## 4. Questoes Sobre Arquitetura

### Q22. Descreva a arquitetura geral do repositorio.

**Resposta correta:** O repositorio tem frontend mobile em React Native/Expo, backend principal em ASP.NET Core, banco PostgreSQL em scripts SQL, recommendation-service para recomendacoes, training-service e `src/ml` para treinamento/avaliacao de modelos, alem de documentacao e testes.

**Justificativa:** Essa separacao evidencia uma arquitetura em camadas e orientada a servicos.

**Relacao com o projeto:** Resume `src/backend`, `src/frontend`, `src/database`, `src/recommendation-service`, `src/training-service`, `src/ml`, `docs` e `tests`.

### Q23. Por que a solucao pode ser interpretada como SOA?

**Resposta correta:** Porque possui modulos/servicos com responsabilidades especificas: app mobile, API principal, banco, servico de recomendacao e servico de treinamento.

**Justificativa:** SOA organiza capacidades do sistema em servicos separados e integraveis.

**Relacao com o projeto:** Essa estrutura aparece no README e em `docs/02_artefatos_vs_codigo.md`.

### Q24. Qual a vantagem de separar backend principal e recommendation-service?

**Resposta correta:** Permite evoluir o motor de recomendacao de forma independente, carregar modelos/pesos como artefatos pesados e manter a API principal focada em regras de aplicacao.

**Justificativa:** Separacao reduz acoplamento e favorece especializacao de servicos.

**Relacao com o projeto:** `src/backend` e `src/recommendation-service` existem como projetos separados.

### Q25. Qual e o papel da camada de dominio no backend?

**Resposta correta:** Representar entidades e regras centrais do negocio, como validacao de idade em `Usuario`, datas em `Espetaculo` e score em `Recomendacao`.

**Justificativa:** A camada de dominio protege invariantes e evita espalhar regras em controllers.

**Relacao com o projeto:** Aparece em `src/backend/domain/*.cs` e nos testes de dominio.

### Q26. Como a injecao de dependencias contribui para a arquitetura?

**Resposta correta:** Ela registra interfaces e implementacoes no container, permitindo que controllers e services dependam de abstracoes. Isso reduz acoplamento e facilita testes.

**Justificativa:** DI e um mecanismo central para arquitetura em camadas.

**Relacao com o projeto:** Aparece em `src/backend/extensions/DependencyInjection.cs`.

## 5. Questoes Sobre Modelagem

### Q27. Como o projeto representa relacionamentos muitos-para-muitos?

**Resposta correta:** Por tabelas associativas, como `usuario_genero_favorito`, `espetaculo_genero` e `teatro_espetaculo`.

**Justificativa:** Relacionamentos N:N em bancos relacionais sao representados por tabelas intermediarias com chaves estrangeiras.

**Relacao com o projeto:** Aparece em `src/database/01_create_database.sql`.

### Q28. Cite tres regras de negocio implementadas diretamente no banco.

**Resposta correta:** Email de usuario unico, idade minima de 13 anos, valor de ingresso nao negativo, data final posterior a data inicial, score de recomendacao entre 0 e 1, nomes de generos/espetaculos/teatros unicos.

**Justificativa:** Essas regras aparecem como `UNIQUE` ou `CHECK`.

**Relacao com o projeto:** Relaciona artefato A1 a constraints SQL.

### Q29. Qual a funcao de `ON DELETE CASCADE` no projeto?

**Resposta correta:** Remover automaticamente registros dependentes quando o registro principal e excluido, como dependencias de usuario em preferencias, interacoes, wishlist e recomendacoes.

**Justificativa:** Evita registros orfaos e mantem integridade referencial.

**Relacao com o projeto:** Aparece nas FKs em `01_create_database.sql`.

### Q30. Qual a funcao de `ON DELETE RESTRICT` em `teatro_espetaculo`?

**Resposta correta:** Impedir a exclusao de teatros vinculados a espetaculos, protegendo relacionamentos importantes.

**Justificativa:** `RESTRICT` bloqueia exclusoes que quebrariam associacoes relevantes.

**Relacao com o projeto:** Aparece no relacionamento entre teatros e espetaculos.

### Q31. Por que o modelo usa tabela de `interacoes`?

**Resposta correta:** Para registrar comportamento do usuario, como clique, acesso e tempo de tela, que pode alimentar recomendacoes e analises.

**Justificativa:** Sistemas de recomendacao dependem de historico de interacao usuario-item.

**Relacao com o projeto:** A tabela `interacoes` esta em `01_create_database.sql` e e populada em `02_initial_data.sql`.

### Q32. O projeto implementa triggers no banco?

**Resposta correta:** Nao foram encontradas triggers implementadas.

**Justificativa:** Embora triggers aparecam nos autoestudos, o script usa principalmente constraints, FKs, indices e enums.

**Relacao com o projeto:** Diferencia conceito estudado de implementacao real.

## 6. Questoes Sobre Implementacao

### Q33. Como o `DbRunner` participa da implementacao do banco?

**Resposta correta:** Ele carrega variaveis do `.env`, conecta ao PostgreSQL com `Npgsql` e executa os scripts SQL de estrutura e dados iniciais.

**Justificativa:** Automatiza a execucao dos scripts e facilita reproducibilidade.

**Relacao com o projeto:** Aparece em `DbRunner/Program.cs`.

### Q34. O que acontece no cadastro de usuario no backend?

**Resposta correta:** `AuthController` recebe a requisicao, `AuthService` valida dados, verifica email duplicado, gera hash BCrypt da senha, cria `Usuario`, persiste no repository e garante preferencias associadas.

**Justificativa:** Esse fluxo protege regras de autenticacao e persistencia.

**Relacao com o projeto:** Aparece em `AuthController.cs`, `AuthService.cs`, `Usuario.cs` e `UsuarioRepository.cs`.

### Q35. Como a wishlist funciona no frontend?

**Resposta correta:** O viewmodel chama repository, que chama service de armazenamento local. O service usa AsyncStorage para salvar, remover, alternar e consultar eventos na wishlist.

**Justificativa:** E uma implementacao local/mockada, adequada para validar fluxo de UI antes da integracao REST.

**Relacao com o projeto:** Aparece em `useWishlistViewModel.js`, `wishlistRepository.js` e `wishlistStorageService.js`.

### Q36. Como as recomendacoes SVD sao calculadas no recommendation-service?

**Resposta correta:** O servico carrega pesos SVD, calcula score com media global, vies do usuario, vies do item e produto escalar dos fatores latentes, remove itens consumidos e retorna top-N.

**Justificativa:** Essa e a logica de inferencia do Funk SVD.

**Relacao com o projeto:** Aparece em `SvdRecommendationService.cs`.

### Q37. Como o projeto padroniza erros HTTP no backend?

**Resposta correta:** Por tratamento global de excecoes usando `GlobalExceptionHandler`, que mapeia excecoes para status HTTP e `ProblemDetails`.

**Justificativa:** Centraliza respostas de erro e evita `try/catch` repetido em controllers.

**Relacao com o projeto:** Aparece em `src/backend/infrastructure/middleware/GlobalExceptionHandler.cs`.

### Q38. Como os DTOs sao usados?

**Resposta correta:** DTOs transportam dados entre API e cliente. Entidades de dominio sao convertidas em DTOs por `DtoMapper` antes de serem retornadas.

**Justificativa:** DTOs protegem o dominio e estabilizam o contrato da API.

**Relacao com o projeto:** Aparece em `src/backend/dto` e `src/backend/mappings/DtoMapper.cs`.

### Q39. Como os testes unitarios do frontend validam viewmodels?

**Resposta correta:** Usam Jest e `@testing-library/react`, mockando repositories e verificando estado, validacoes, handlers e chamadas de navegacao.

**Justificativa:** Isso isola a logica do ViewModel sem depender de UI completa ou backend real.

**Relacao com o projeto:** Aparece em `src/frontend/mobile-app-aventura/tests/*.js`.

### Q40. Qual arquivo esperado no artefato de desenvolvimento nao foi encontrado?

**Resposta correta:** `docs/GuiaPadroesDesenvolvimento.md`.

**Justificativa:** O arquivo e citado em `ARTEFATOS.md`, mas `docs/02_artefatos_vs_codigo.md` registrou que nao foi localizado.

**Relacao com o projeto:** Indica uma divergencia entre artefato esperado e repositorio analisado.

## 7. Questoes Mistas de Revisao

### Q41. Se fosse necessario implementar transacao explicita no cadastro, onde ela deveria aparecer?

**Resposta correta:** No fluxo de persistencia envolvendo `AuthService`, `UsuarioRepository` e `PreferenciasRepository`, ou em uma camada de Unit of Work/repository que controle uma mesma conexao e transacao.

**Justificativa:** O cadastro pode envolver criacao de usuario e preferencias; se uma etapa falhar, o ideal e reverter tudo.

**Relacao com o projeto:** Aplica conceito de transacoes ao fluxo real de autenticacao.

### Q42. Se o frontend passar a consumir o backend real, qual camada deve mudar primeiro?

**Resposta correta:** Principalmente repositories e services do frontend, substituindo mocks e AsyncStorage parcial por chamadas HTTP. As telas e muitos viewmodels podem ser preservados.

**Justificativa:** MVVM isola fonte de dados da interface.

**Relacao com o projeto:** Reflete a decisao de usar `repositories` e `services` no app.

### Q43. Qual risco existe em documentar Testcontainers, WireMock e VHS sem implementa-los?

**Resposta correta:** A documentacao pode sugerir uma cobertura de integracao que o codigo ainda nao possui, gerando divergencia entre planejamento e evidencia executavel.

**Justificativa:** Documentacao nao substitui testes reais.

**Relacao com o projeto:** `docs/02_artefatos_vs_codigo.md` registra essa divergencia.

### Q44. Qual seria uma boa justificativa para nao usar Redux neste momento?

**Resposta correta:** O estado atual do app e controlavel por hooks, viewmodels, repositories e AsyncStorage. Redux adicionaria complexidade antes de existir estado global real mais complexo.

**Justificativa:** A ferramenta deve acompanhar a necessidade do projeto.

**Relacao com o projeto:** Redux aparece nos autoestudos, mas nao foi implementado.

### Q45. O que garante que um espetaculo nao tenha valor de ingresso negativo?

**Resposta correta:** A constraint `CHECK(valor_ingresso >= 0)` no banco e validacoes de dominio/testes relacionados.

**Justificativa:** A regra e protegida na camada de persistencia e tambem pode ser validada no dominio.

**Relacao com o projeto:** Aparece em `src/database/01_create_database.sql` e nos testes de dominio.

### Q46. Qual a principal vantagem de usar `ON CONFLICT DO NOTHING` no script de dados iniciais?

**Resposta correta:** Tornar a carga inicial idempotente, evitando erro ao executar o script mais de uma vez com dados ja existentes.

**Justificativa:** Scripts idempotentes sao mais seguros em ambientes reproduziveis.

**Relacao com o projeto:** Aparece em `src/database/02_initial_data.sql`.

### Q47. Como o projeto demonstra TDD ou cultura de testes?

**Resposta correta:** Pela existencia de testes unitarios para dominio, services, controllers, middleware, DI, mappers e viewmodels, alem de fakes/mocks e documentacao de estrategia de testes.

**Justificativa:** Mesmo sem provar a ordem red-green-refactor, ha evidencia de cobertura automatizada em partes criticas.

**Relacao com o projeto:** Aparece em `BackEnd.Aventura.Tests`, testes Jest e `docs/Projeto.md`.

### Q48. Qual e a diferenca entre autenticacao propria e autenticacao externa no projeto?

**Resposta correta:** Autenticacao propria e o fluxo implementado no backend com email/senha e BCrypt. Autenticacao externa seria login por provedor como Google; no frontend ha fluxo Google mockado, mas nao foi encontrada integracao real.

**Justificativa:** A questao separa implementacao real de funcionalidade simulada/planejada.

**Relacao com o projeto:** Aparece em `AuthService.cs` e `src/frontend/mobile-app-aventura/services/authService.js`.

### Q49. Em que sentido o banco reforca as regras de negocio?

**Resposta correta:** Ele aplica constraints e chaves estrangeiras que impedem dados invalidos, como email duplicado, idade abaixo de 13, datas invalidas e score fora de 0 a 1.

**Justificativa:** Regras no banco protegem dados mesmo se outra camada falhar.

**Relacao com o projeto:** Aparece no artefato A1 e em `01_create_database.sql`.

### Q50. Qual resposta seria adequada se perguntarem "o projeto esta 100% integrado ponta a ponta"?

**Resposta correta:** Nao. O projeto tem banco, backend, frontend mockado, servicos de recomendacao e testes unitarios, mas a integracao REST completa do frontend, APIs externas reais, VHS e alguns testes de integracao aparecem como planejamento ou lacunas.

**Justificativa:** A resposta honesta diferencia progresso implementado de itens planejados.

**Relacao com o projeto:** Resume as conclusoes de `docs/02_artefatos_vs_codigo.md`.

## Gabarito Rapido

| Questao | Resposta |
| --- | --- |
| Q1 | B |
| Q2 | B |
| Q3 | A |
| Q4 | B |
| Q5 | B |
| Q6 | B |
| Q7 | A |
| Q8 | B |
| Q9 | C |
| Q10 | A |
| Q11-Q50 | Ver respostas corretas detalhadas em cada questao. |

