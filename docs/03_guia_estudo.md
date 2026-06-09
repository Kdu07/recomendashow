# Guia de Estudo - Sprint 3

Este guia foi elaborado a partir de `AUTOESTUDOS.md`, `ARTEFATOS.md`, `docs/01_mapa_sprint.md` e `docs/02_artefatos_vs_codigo.md`. O foco e preparar a defesa tecnica e possiveis perguntas de prova sobre os conceitos teoricos aplicados no projeto.

## 1. PostgreSQL e SQL

### Definicao

PostgreSQL e um sistema gerenciador de banco de dados relacional. SQL e a linguagem usada para definir estruturas, manipular dados e consultar informacoes em bancos relacionais.

### Como funciona

O banco organiza dados em tabelas, colunas e linhas. Comandos DDL criam estruturas, como `CREATE TABLE`; comandos DML manipulam dados, como `INSERT`, `UPDATE` e `DELETE`; comandos de consulta, como `SELECT`, recuperam dados.

### Onde aparece no projeto

Na implementacao do banco e no backend, que acessa o PostgreSQL por SQL direto usando `Npgsql`.

### Arquivos envolvidos

- `src/database/01_create_database.sql`
- `src/database/02_initial_data.sql`
- `src/database/README.md`
- `src/backend/repository/*Repository.cs`
- `src/backend/infrastructure/DbConnectionFactory.cs`
- `DbRunner/Program.cs`

### Justificativa da escolha

PostgreSQL atende bem a modelagem relacional do projeto, com usuarios, preferencias, espetaculos, teatros, generos, interacoes e recomendacoes. Tambem permite constraints, enums, indices e chaves estrangeiras.

### Alternativas possiveis

- MySQL ou MariaDB.
- SQL Server.
- SQLite para ambiente local simples.
- MongoDB ou outro NoSQL, caso o modelo fosse menos relacional.
- ORM, como Entity Framework, mas o artefato proibia ORMs.

### Vantagens

- Forte integridade relacional.
- SQL padronizado e expressivo.
- Bom suporte a constraints, indices e tipos especificos.
- Adequado para dados estruturados.
- Permite acesso direto sem ORM.

### Limitacoes

- Requer cuidado com modelagem e migracoes.
- SQL manual pode gerar duplicacao e maior esforco de manutencao.
- Mudancas estruturais precisam ser coordenadas com backend e dados existentes.

### Perguntas de prova provaveis

**Pergunta:** Por que o projeto usa PostgreSQL?

**Resposta esperada:** Porque o dominio possui entidades relacionais e exige integridade entre usuarios, espetaculos, preferencias, generos, interacoes e recomendacoes. PostgreSQL oferece constraints, chaves estrangeiras, indices e bom suporte a SQL.

**Pergunta:** Qual a diferenca entre DDL e DML no projeto?

**Resposta esperada:** DDL aparece no `01_create_database.sql`, criando tabelas, enums, constraints e indices. DML aparece no `02_initial_data.sql`, inserindo dados iniciais.

## 2. Modelo Fisico de Banco de Dados

### Definicao

Modelo fisico e a transformacao do modelo logico em estruturas reais de banco: tabelas, tipos, colunas, chaves primarias, chaves estrangeiras, indices e restricoes.

### Como funciona

As entidades do dominio sao representadas por tabelas. Relacionamentos 1:1, 1:N e N:N sao implementados com chaves estrangeiras e tabelas associativas. Regras de negocio podem ser aplicadas por constraints.

### Onde aparece no projeto

Na Sprint 3, o modelo fisico foi materializado em scripts SQL dentro de `src/database`.

### Arquivos envolvidos

- `src/database/01_create_database.sql`
- `src/database/02_initial_data.sql`
- `src/database/README.md`
- `assets/diagrama-logico.png`
- `assets/diagrama_logico.png`

### Justificativa da escolha

O artefato exigia transformar o modelo logico em um esquema funcional, sem ORM. O modelo fisico explicita como os dados sao armazenados e como as regras sao garantidas.

### Alternativas possiveis

- Usar migracoes de ORM.
- Usar ferramenta de schema migration, como Flyway ou Liquibase.
- Criar schema manual diretamente no banco, sem scripts versionados.

### Vantagens

- Reprodutibilidade.
- Controle claro sobre tabelas e relacionamentos.
- Facil avaliacao academica.
- Evita dependencia de ORM.

### Limitacoes

- Exige manutencao manual dos scripts.
- Mudancas no modelo podem exigir scripts adicionais de migracao.
- Risco de divergencia entre documentacao, banco e codigo se nao houver governanca.

### Perguntas de prova provaveis

**Pergunta:** O que significa implementar o modelo fisico?

**Resposta esperada:** Significa criar no banco as estruturas reais que representam o modelo logico: tabelas, colunas, chaves, relacionamentos, constraints, indices e dados iniciais.

**Pergunta:** Como o projeto representa relacionamentos N:N?

**Resposta esperada:** Por tabelas associativas, como `usuario_genero_favorito`, `espetaculo_genero` e `teatro_espetaculo`.

## 3. Integridade Referencial e Constraints

### Definicao

Integridade referencial garante que relacionamentos entre tabelas permaneçam consistentes. Constraints sao regras declaradas no banco para impedir dados invalidos.

### Como funciona

Chaves estrangeiras impedem referencias a registros inexistentes. `CHECK` valida condicoes de negocio. `UNIQUE` impede duplicidade. `ON DELETE CASCADE` remove dependencias automaticamente; `ON DELETE RESTRICT` bloqueia exclusoes perigosas.

### Onde aparece no projeto

No script de criacao do banco e nas regras duplicadas em dominio/backend.

### Arquivos envolvidos

- `src/database/01_create_database.sql`
- `src/backend/domain/Usuario.cs`
- `src/backend/domain/Espetaculo.cs`
- `src/backend/domain/Recomendacao.cs`
- `src/backend/domain/EntityGuards.cs`
- `src/backend/BackEnd.Aventura.Tests/*Tests.cs`

### Justificativa da escolha

Regras criticas precisam ser protegidas mesmo quando o acesso ocorre fora da API. Por isso, o banco valida email unico, idade minima, datas validas, preco nao negativo e score entre 0 e 1.

### Alternativas possiveis

- Validar somente na aplicacao.
- Validar somente no frontend.
- Usar triggers para regras mais complexas.
- Usar stored procedures.

### Vantagens

- Protege a consistencia na camada mais proxima dos dados.
- Evita registros orfaos.
- Reduz risco de dados invalidos.
- Facilita confiabilidade das recomendacoes.

### Limitacoes

- Mensagens de erro do banco podem ser menos amigaveis.
- Regras duplicadas entre banco e backend exigem sincronizacao.
- Constraints simples nao cobrem fluxos de negocio complexos.

### Perguntas de prova provaveis

**Pergunta:** Por que usar constraints se o backend ja valida dados?

**Resposta esperada:** Porque o banco e a ultima barreira de consistencia. Mesmo que outro cliente acesse o banco ou haja bug no backend, constraints impedem dados invalidos.

**Pergunta:** Qual a diferenca entre `CASCADE` e `RESTRICT`?

**Resposta esperada:** `CASCADE` remove registros dependentes automaticamente; `RESTRICT` impede a exclusao quando ha dependencias.

## 4. Transacoes e ACID

### Definicao

Transacao e uma unidade logica de trabalho no banco. ACID representa atomicidade, consistencia, isolamento e durabilidade.

### Como funciona

Uma transacao agrupa operacoes para que sejam confirmadas juntas ou revertidas em caso de erro. Atomicidade garante tudo ou nada; consistencia preserva regras; isolamento evita interferencia entre transacoes; durabilidade garante persistencia apos commit.

### Onde aparece no projeto

O conceito aparece no autoestudo e se relaciona aos scripts SQL e aos repositories. No codigo atual, as operacoes de repository usam comandos individuais; nao foi encontrada orquestracao explicita de transacoes multi-comando.

### Arquivos envolvidos

- `src/database/01_create_database.sql`
- `src/database/02_initial_data.sql`
- `src/backend/repository/*Repository.cs`
- `DbRunner/Program.cs`

### Justificativa da escolha

Mesmo quando nao declaradas explicitamente, transacoes sao fundamentais para proteger insercoes, atualizacoes e relacoes entre dados.

### Alternativas possiveis

- Transacoes explicitas com `BEGIN`, `COMMIT` e `ROLLBACK`.
- Unit of Work.
- Stored procedures transacionais.
- Mensageria/eventual consistency para arquiteturas distribuidas.

### Vantagens

- Evita estado parcial.
- Mantem consistencia dos dados.
- Importante para operacoes compostas, como cadastro com preferencias.

### Limitacoes

- Transacoes longas podem causar locks.
- Em sistemas distribuidos, transacoes ficam mais complexas.
- O projeto atual nao evidencia transacoes explicitas em repositories.

### Perguntas de prova provaveis

**Pergunta:** O que e atomicidade?

**Resposta esperada:** E a propriedade que garante que uma transacao seja executada por completo ou nao tenha efeito nenhum.

**Pergunta:** Onde transacoes seriam uteis neste projeto?

**Resposta esperada:** No cadastro de usuario com criacao de preferencias, em atualizacoes de wishlist e em qualquer operacao que altere multiplas tabelas relacionadas.

## 5. Triggers

### Definicao

Trigger e uma rotina executada automaticamente pelo banco quando ocorre um evento, como `INSERT`, `UPDATE` ou `DELETE`.

### Como funciona

O banco monitora eventos em uma tabela e executa uma funcao associada. Pode ser usada para auditoria, atualizacao automatica de timestamps, validacoes complexas ou propagacao de efeitos.

### Onde aparece no projeto

Triggers aparecem nos autoestudos, mas nao foram encontradas implementadas no script `01_create_database.sql`.

### Arquivos envolvidos

- `AUTOESTUDOS.md`
- `docs/01_mapa_sprint.md`
- `src/database/01_create_database.sql`, como ponto onde poderiam aparecer

### Justificativa da escolha

O time optou por constraints e regras no backend em vez de triggers. Para as regras atuais, constraints simples resolvem boa parte do problema.

### Alternativas possiveis

- Constraints `CHECK`, `UNIQUE` e FKs.
- Regras no backend.
- Stored procedures.
- Jobs ou workers assíncronos.

### Vantagens

- Automatizam regras diretamente no banco.
- Evitam dependencia do cliente que fez a alteracao.
- Uteis para auditoria e campos derivados.

### Limitacoes

- Podem esconder comportamento.
- Dificultam debug.
- Podem causar efeitos colaterais inesperados.
- Nao estao implementadas no projeto atual.

### Perguntas de prova provaveis

**Pergunta:** O projeto usa triggers?

**Resposta esperada:** Nao foram encontradas triggers implementadas. O projeto usa principalmente constraints, chaves estrangeiras e validacoes no backend.

**Pergunta:** Dê um exemplo de trigger util para este projeto.

**Resposta esperada:** Uma trigger para atualizar automaticamente `data_atualizacao` em `preferencias` sempre que o registro fosse alterado.

## 6. Docker e Configuracao Reproduzivel

### Definicao

Docker permite empacotar servicos e dependencias em containers. Configuracao reproduzivel significa que diferentes membros da equipe conseguem subir o mesmo ambiente com o mesmo comportamento.

### Como funciona

O `docker-compose` define servicos, portas, variaveis e volumes. Ao executar o compose, o ambiente sobe com parametros padronizados.

### Onde aparece no projeto

No banco de dados local.

### Arquivos envolvidos

- `docker-compose-db.yml`
- `src/database/Dockerfile`
- `.env.example`
- `README.md`
- `src/database/README.md`

### Justificativa da escolha

O artefato exigia banco dockerizado e funcionando. Docker reduz divergencias entre maquinas.

### Alternativas possiveis

- PostgreSQL instalado diretamente na maquina.
- Banco em nuvem apenas.
- Scripts de provisionamento em VM.
- Dev containers.

### Vantagens

- Ambiente padronizado.
- Facil execucao local.
- Volume preserva dados.
- Facilita onboarding da equipe.

### Limitacoes

- Exige Docker instalado.
- Variaveis incorretas no `.env` impedem subir o servico.
- Nao substitui configuracao de producao.

### Perguntas de prova provaveis

**Pergunta:** Por que dockerizar o banco?

**Resposta esperada:** Para garantir ambiente reproduzivel, reduzir diferencas entre maquinas e facilitar execucao local do PostgreSQL.

**Pergunta:** Qual o papel do volume no compose?

**Resposta esperada:** Persistir os dados do PostgreSQL mesmo se o container for recriado.

## 7. API REST e Swagger

### Definicao

API REST e uma interface HTTP organizada por recursos e metodos como GET, POST, PUT e DELETE. Swagger documenta e permite testar endpoints de forma interativa.

### Como funciona

Controllers recebem requisicoes, validam parametros e chamam services. Swagger le metadados da aplicacao e gera uma UI de documentacao.

### Onde aparece no projeto

No backend principal e no servico de recomendacao.

### Arquivos envolvidos

- `src/backend/Program.cs`
- `src/backend/controller/*Controller.cs`
- `src/backend/dto/*.cs`
- `src/recommendation-service/Program.cs`
- `src/recommendation-service/Controllers/*Controller.cs`
- `docs/Projeto.md`

### Justificativa da escolha

REST e simples de consumir por frontend mobile e servicos externos. Swagger atende a exigencia de contratos de API documentados.

### Alternativas possiveis

- GraphQL.
- gRPC.
- Minimal APIs.
- Documentacao manual sem Swagger.

### Vantagens

- Padrao conhecido.
- Facil teste por Swagger/Postman.
- Boa integracao com frontend.
- Contratos mais visiveis.

### Limitacoes

- REST pode gerar excesso de endpoints.
- Swagger documenta o contrato exposto, mas nao garante qualidade da implementacao.
- Versionamento precisa ser planejado.

### Perguntas de prova provaveis

**Pergunta:** Para que serve o Swagger no projeto?

**Resposta esperada:** Para documentar e testar os endpoints da API em ambiente de desenvolvimento, facilitando validacao dos contratos.

**Pergunta:** Qual o fluxo de uma requisicao REST no backend?

**Resposta esperada:** Controller recebe a requisicao, chama um service, o service aplica regras e usa repositories quando precisa acessar o banco, depois retorna DTOs.

## 8. React Native e Desenvolvimento Mobile

### Definicao

React Native e um framework para construir aplicativos mobile usando JavaScript/React, renderizando componentes nativos.

### Como funciona

A interface e composta por componentes. O estado e controlado por hooks. A navegacao organiza telas em stacks e tabs. Expo simplifica execucao e configuracao do app.

### Onde aparece no projeto

No app mobile do Portal Aventura.

### Arquivos envolvidos

- `src/frontend/mobile-app-aventura/App.js`
- `src/frontend/mobile-app-aventura/RootNavigator.js`
- `src/frontend/mobile-app-aventura/screens/*`
- `src/frontend/mobile-app-aventura/components/*`
- `src/frontend/mobile-app-aventura/package.json`
- `src/frontend/mobile-app-aventura/assets/*`

### Justificativa da escolha

Permite desenvolver uma experiencia mobile com componentes reutilizaveis e evolucao rapida de telas, adequada para validar jornadas como onboarding, busca, wishlist e detalhes de espetaculo.

### Alternativas possiveis

- Flutter.
- Desenvolvimento nativo Android/iOS.
- PWA responsiva.
- Ionic.

### Vantagens

- Reuso de codigo entre plataformas.
- Ecossistema React.
- Rapidez para prototipar e testar.
- Boa organizacao por componentes.

### Limitacoes

- Dependencia de bibliotecas nativas.
- Performance pode exigir cuidado em telas complexas.
- Integracao real com backend ainda nao esta completa no projeto.

### Perguntas de prova provaveis

**Pergunta:** Qual o papel do React Native no projeto?

**Resposta esperada:** Implementar o frontend mobile do Portal Aventura, com telas de autenticacao, onboarding, home, busca, wishlist, perfil e detalhes de espetaculo.

**Pergunta:** O frontend ja consome a API real?

**Resposta esperada:** Majoritariamente nao. O relatorio indica uso de mocks locais e repositories preparados para futura troca por chamadas HTTP.

## 9. Componentizacao

### Definicao

Componentizacao e a divisao da interface em partes reutilizaveis e independentes.

### Como funciona

Componentes recebem propriedades, renderizam uma parte da UI e podem ser combinados para formar telas completas.

### Onde aparece no projeto

Na pasta `components` do app mobile.

### Arquivos envolvidos

- `src/frontend/mobile-app-aventura/components/AppButton.js`
- `src/frontend/mobile-app-aventura/components/PosterCard.js`
- `src/frontend/mobile-app-aventura/components/BottomTabBar.js`
- `src/frontend/mobile-app-aventura/components/SearchBar.js`
- `src/frontend/mobile-app-aventura/components/HomeHeroBanner.js`
- `src/frontend/mobile-app-aventura/screens/*`

### Justificativa da escolha

A aplicacao possui varias telas com padroes visuais repetidos. Componentes reduzem duplicacao e mantem consistencia visual.

### Alternativas possiveis

- Criar tudo diretamente dentro das telas.
- Usar biblioteca pronta de UI.
- Criar design system mais formal.

### Vantagens

- Reuso.
- Manutencao mais simples.
- Consistencia visual.
- Testabilidade maior.

### Limitacoes

- Abstracao excessiva pode dificultar leitura.
- Componentes genericos demais ficam complicados.
- Requer convencoes claras de props e estilos.

### Perguntas de prova provaveis

**Pergunta:** Por que separar componentes das telas?

**Resposta esperada:** Para reutilizar UI, reduzir duplicacao, manter consistencia e deixar as telas mais focadas no fluxo.

## 10. MVVM

### Definicao

MVVM significa Model-View-ViewModel. O padrao separa dados/modelos, apresentacao visual e logica de estado/comportamento da tela.

### Como funciona

Views exibem a interface. ViewModels controlam estado, validacoes e eventos. Models representam dados. Repositories e services fornecem ou persistem informacoes.

### Onde aparece no projeto

No frontend mobile.

### Arquivos envolvidos

- `src/frontend/mobile-app-aventura/screens/*`
- `src/frontend/mobile-app-aventura/viewmodels/*`
- `src/frontend/mobile-app-aventura/models/*`
- `src/frontend/mobile-app-aventura/repositories/*`
- `src/frontend/mobile-app-aventura/services/*`
- `src/frontend/mobile-app-aventura/tests/*`

### Justificativa da escolha

O autoestudo de React Native incluia MVVM. O projeto usa esse padrao para reduzir acoplamento entre interface, estado e fonte de dados.

### Alternativas possiveis

- MVC.
- Redux centralizado.
- Zustand, MobX ou Context API.
- Hooks diretamente nas telas sem camada ViewModel.

### Vantagens

- Telas mais declarativas.
- Facilita testes de viewmodels.
- Permite trocar mocks por API real nos repositories.
- Separa responsabilidades.

### Limitacoes

- Mais arquivos e camadas.
- Pode parecer excessivo em telas simples.
- Sem disciplina, viewmodels podem ficar grandes.

### Perguntas de prova provaveis

**Pergunta:** Como o MVVM aparece no app?

**Resposta esperada:** As telas em `screens` representam a View; hooks como `useAuthViewModel` e `useHomeViewModel` representam ViewModels; `models`, `repositories` e `services` organizam dados e fontes.

**Pergunta:** Por que o MVVM ajuda na integracao futura com backend?

**Resposta esperada:** Porque a fonte dos dados fica encapsulada em repositories/services. Assim, trocar mock por HTTP tende a preservar telas e viewmodels.

## 11. Redux e Gerenciamento de Estado

### Definicao

Redux e uma biblioteca de gerenciamento de estado global baseada em store unica, actions e reducers.

### Como funciona

Eventos disparam actions; reducers calculam o novo estado; componentes leem dados da store e reagem a mudancas.

### Onde aparece no projeto

Redux aparece nos autoestudos de React Native/MVVM, mas nao foi encontrado implementado no projeto.

### Arquivos envolvidos

- `AUTOESTUDOS.md`
- `docs/01_mapa_sprint.md`
- O projeto usa hooks e estado local em `src/frontend/mobile-app-aventura/viewmodels/*`, nao Redux.

### Justificativa da escolha

O time optou por hooks, viewmodels e repositories. Para o escopo atual, estado local e AsyncStorage atendem os fluxos implementados.

### Alternativas possiveis

- Redux Toolkit.
- Context API.
- Zustand.
- MobX.
- React Query para estado de servidor.

### Vantagens

- Bom para estado global complexo.
- Fluxo previsivel.
- Ferramentas maduras de debug.

### Limitacoes

- Mais boilerplate.
- Pode ser excessivo para MVP com mocks locais.
- Nao esta aplicado no codigo atual.

### Perguntas de prova provaveis

**Pergunta:** O projeto usa Redux?

**Resposta esperada:** Nao. Redux aparece como conceito estudado, mas a implementacao atual usa hooks/viewmodels, repositories e AsyncStorage.

**Pergunta:** Quando Redux seria util neste projeto?

**Resposta esperada:** Quando houver estado global complexo, como sessao autenticada real, cache de catalogo, preferencias compartilhadas e sincronizacao ampla entre telas.

## 12. Programacao Orientada a Objetos

### Definicao

OOP organiza software em objetos que combinam dados e comportamento. Seus pilares incluem encapsulamento, heranca, polimorfismo e abstracao.

### Como funciona

Classes definem atributos e metodos. Objetos sao instancias. Interfaces permitem programar contra contratos. Encapsulamento protege invariantes internas.

### Onde aparece no projeto

Principalmente no backend C#.

### Arquivos envolvidos

- `src/backend/domain/*.cs`
- `src/backend/services/*.cs`
- `src/backend/services/I*Service.cs`
- `src/backend/repository/*.cs`
- `src/backend/repository/I*Repository.cs`
- `src/backend/exceptions/*.cs`
- `src/backend/BackEnd.Aventura.Tests/*Tests.cs`

### Justificativa da escolha

C# favorece OOP. O dominio do projeto possui entidades claras, como `Usuario`, `Espetaculo`, `Teatro`, `Preferencias` e `Recomendacao`.

### Alternativas possiveis

- Programacao funcional.
- Scripts procedurais.
- Arquitetura anemica com DTOs apenas.
- Active Record.

### Vantagens

- Modela entidades do dominio.
- Encapsula regras.
- Facilita testes unitarios.
- Interfaces permitem fakes e inversao de dependencia.

### Limitacoes

- Pode gerar excesso de classes.
- Heranca mal usada aumenta acoplamento.
- Regras duplicadas entre dominio e banco precisam ser mantidas.

### Perguntas de prova provaveis

**Pergunta:** Onde ha encapsulamento no projeto?

**Resposta esperada:** Em entidades como `Usuario`, que possui setters privados e metodos como `Create` e `AtualizarPerfil` para preservar regras como idade minima.

**Pergunta:** Como o projeto usa polimorfismo?

**Resposta esperada:** Por interfaces como `IUsuarioRepository`, `IAuthService` e `IRecommendationService`, que permitem trocar implementacoes reais por fakes em testes.

## 13. TDD e Testes Unitarios

### Definicao

TDD e uma metodologia em que testes orientam a implementacao. Teste unitario valida uma unidade isolada de codigo, como uma classe, service ou funcao.

### Como funciona

No ciclo classico red-green-refactor, primeiro escreve-se um teste que falha, depois o codigo minimo para passar, e por fim refatora-se mantendo os testes verdes.

### Onde aparece no projeto

Nas suites do backend e frontend, e na documentacao de `docs/Projeto.md`.

### Arquivos envolvidos

- `src/backend/BackEnd.Aventura.Tests/*.cs`
- `src/backend/BackEnd.Aventura.Tests/Fakes/*.cs`
- `src/frontend/mobile-app-aventura/tests/*.js`
- `src/frontend/mobile-app-aventura/package.json`
- `docs/Projeto.md`

### Justificativa da escolha

O artefato exigia testes unitarios e evidencias de TDD. O projeto valida regras de dominio, services, controllers, viewmodels e services locais.

### Alternativas possiveis

- Testes manuais apenas.
- Testes de integracao como foco principal.
- Testes end-to-end.
- BDD automatizado com Cucumber.

### Vantagens

- Detecta regressao cedo.
- Melhora design por exigir baixo acoplamento.
- Ajuda a documentar comportamento.
- Fakes e mocks isolam a unidade testada.

### Limitacoes

- Nao garante que camadas integradas funcionem juntas.
- Pode passar mesmo com banco/API real quebrados.
- Exige manutencao quando regras mudam.

### Perguntas de prova provaveis

**Pergunta:** Qual a diferenca entre teste unitario e teste de integracao?

**Resposta esperada:** Teste unitario valida uma unidade isolada com mocks/fakes. Teste de integracao valida a comunicacao real entre componentes, como backend e banco.

**Pergunta:** Como o projeto isola services nos testes?

**Resposta esperada:** Usando fakes de repositories em `BackEnd.Aventura.Tests/Fakes`.

## 14. BDD e Cenarios de Aceitacao

### Definicao

BDD e uma abordagem orientada ao comportamento esperado do sistema, geralmente expressa por cenarios compreensiveis por pessoas tecnicas e nao tecnicas.

### Como funciona

Define-se comportamento por cenarios, normalmente com pre-condicoes, acao e resultado esperado. O formato Given-When-Then e comum, mas nao obrigatorio.

### Onde aparece no projeto

No planejamento de testes em `docs/Projeto.md`, com cenarios positivos, negativos, pre-condicoes, pos-condicoes e criterios de sucesso.

### Arquivos envolvidos

- `docs/Projeto.md`
- `ARTEFATOS.md`
- `docs/01_mapa_sprint.md`
- `docs/02_artefatos_vs_codigo.md`

### Justificativa da escolha

O artefato exigia planejamento claro de testes. BDD ajuda a conectar requisitos, comportamento esperado e validacao.

### Alternativas possiveis

- Casos de teste tradicionais.
- Checklists manuais.
- Testes exploratorios.
- Cucumber/Gherkin automatizado.

### Vantagens

- Linguagem mais proxima do usuario.
- Facilita alinhamento com requisitos.
- Ajuda a pensar em cenarios positivos e negativos.

### Limitacoes

- No projeto, BDD aparece mais como planejamento conceitual do que como automacao formal.
- Pode virar documentacao desatualizada se nao for mantido junto do codigo.

### Perguntas de prova provaveis

**Pergunta:** O projeto usa BDD automatizado?

**Resposta esperada:** Nao foi encontrado Cucumber ou Gherkin automatizado. O projeto usa uma abordagem conceitual de BDD no planejamento de cenarios.

**Pergunta:** Por que cenarios negativos sao importantes?

**Resposta esperada:** Porque validam como o sistema se comporta diante de erros, entradas invalidas e indisponibilidades, evitando falhas inesperadas.

## 15. Repository Pattern

### Definicao

Repository Pattern isola o acesso a dados atras de uma interface ou classe especializada.

### Como funciona

Services chamam repositories em vez de executar SQL diretamente. O repository conhece a fonte de dados e retorna entidades ou modelos.

### Onde aparece no projeto

No backend C# e no frontend JavaScript.

### Arquivos envolvidos

- `src/backend/repository/*Repository.cs`
- `src/backend/repository/I*Repository.cs`
- `src/frontend/mobile-app-aventura/repositories/*Repository.js`
- `src/backend/BackEnd.Aventura.Tests/Fakes/*Repository.cs`

### Justificativa da escolha

Como o projeto nao usa ORM, repositories organizam SQL e reduzem acoplamento entre regra de negocio e persistencia.

### Alternativas possiveis

- Acesso direto ao banco dentro dos services.
- ORM com DbContext.
- Active Record.
- DAO simples sem interfaces.

### Vantagens

- Isola persistencia.
- Facilita testes com fakes.
- Centraliza queries.
- Ajuda a trocar fonte de dados.

### Limitacoes

- Pode criar muitas classes.
- Queries complexas ainda ficam manuais.
- Repositories muito grandes viram ponto de acoplamento.

### Perguntas de prova provaveis

**Pergunta:** Por que usar repositories no backend?

**Resposta esperada:** Para isolar o acesso ao PostgreSQL, manter SQL fora dos services e facilitar testes substituindo repositories reais por fakes.

## 16. Dependency Injection

### Definicao

Dependency Injection e uma tecnica em que dependencias sao fornecidas externamente a uma classe, em vez de criadas dentro dela.

### Como funciona

Interfaces e implementacoes sao registradas em um container. Quando uma classe e instanciada, o container injeta as dependencias necessarias.

### Onde aparece no projeto

No backend ASP.NET Core.

### Arquivos envolvidos

- `src/backend/Program.cs`
- `src/backend/extensions/DependencyInjection.cs`
- `src/backend/services/*Service.cs`
- `src/backend/repository/*Repository.cs`
- `src/backend/BackEnd.Aventura.Tests/DependencyInjectionTests.cs`

### Justificativa da escolha

Reduz acoplamento, facilita testes e organiza o ciclo de vida de services, repositories e singletons.

### Alternativas possiveis

- Instanciar dependencias manualmente com `new`.
- Service locator.
- Factories manuais.
- Container externo.

### Vantagens

- Baixo acoplamento.
- Testes mais simples.
- Configuracao centralizada.
- Suporte nativo no ASP.NET Core.

### Limitacoes

- Erros de registro aparecem em runtime.
- Excesso de interfaces pode aumentar complexidade.
- Ciclos de vida incorretos causam bugs sutis.

### Perguntas de prova provaveis

**Pergunta:** Como DI ajuda nos testes?

**Resposta esperada:** Permite substituir implementacoes reais por fakes ou mocks, testando services sem banco real.

**Pergunta:** Onde os services sao registrados?

**Resposta esperada:** Em `src/backend/extensions/DependencyInjection.cs` e tambem em `Program.cs` para alguns singletons.

## 17. Factory Pattern

### Definicao

Factory Pattern centraliza a criacao de objetos, escondendo detalhes de construcao.

### Como funciona

Uma classe fabrica instancias prontas para uso. Quem consome nao precisa conhecer todos os parametros ou regras de criacao.

### Onde aparece no projeto

Na criacao de conexoes com o banco.

### Arquivos envolvidos

- `src/backend/infrastructure/DbConnectionFactory.cs`
- `src/backend/infrastructure/IDbConnectionFactory.cs`
- `src/backend/repository/*Repository.cs`
- `src/backend/BackEnd.Aventura.Tests/DbConnectionFactoryTests.cs`

### Justificativa da escolha

Centralizar connection string e criacao de `NpgsqlConnection`, evitando repeticao nos repositories.

### Alternativas possiveis

- Criar `NpgsqlConnection` direto em cada repository.
- Usar ORM/DbContext.
- Usar pool/factory externa.

### Vantagens

- Evita duplicacao.
- Facilita teste da configuracao.
- Mantem repositories simples.

### Limitacoes

- Ainda exige abrir e fechar conexoes corretamente.
- Nao gerencia transacoes globais por si so.
- Se mal configurada, afeta todos os repositories.

### Perguntas de prova provaveis

**Pergunta:** Qual padrao criacional aparece no projeto?

**Resposta esperada:** Factory, por meio de `DbConnectionFactory`, que cria conexoes com PostgreSQL para os repositories.

## 18. DTO e Mapeamento

### Definicao

DTO, Data Transfer Object, e um objeto usado para transportar dados entre camadas ou pela API, sem expor diretamente o modelo de dominio.

### Como funciona

Entidades de dominio sao convertidas para DTOs antes de serem retornadas por controllers. Requests tambem usam DTOs para entrada.

### Onde aparece no projeto

No backend principal.

### Arquivos envolvidos

- `src/backend/dto/*.cs`
- `src/backend/mappings/DtoMapper.cs`
- `src/backend/controller/*Controller.cs`
- `src/backend/BackEnd.Aventura.Tests/DtoMapperTests.cs`

### Justificativa da escolha

Evita expor detalhes internos do dominio e permite controlar o contrato da API.

### Alternativas possiveis

- Retornar entidades diretamente.
- Usar AutoMapper.
- Usar records especificos por endpoint.

### Vantagens

- Contratos claros.
- Menor acoplamento entre dominio e API.
- Permite formatar respostas.
- Facilita testes de mapeamento.

### Limitacoes

- Exige codigo de conversao.
- Pode gerar duplicacao de propriedades.
- Mapeadores manuais precisam ser atualizados quando o modelo muda.

### Perguntas de prova provaveis

**Pergunta:** Por que nao retornar entidades diretamente?

**Resposta esperada:** Para evitar acoplamento com o dominio, proteger campos internos e manter contratos de API estaveis.

## 19. Middleware e Tratamento Global de Erros

### Definicao

Middleware e um componente do pipeline HTTP que processa requisicoes e respostas. Tratamento global de erros padroniza respostas quando excecoes ocorrem.

### Como funciona

Quando uma excecao e lancada, o handler intercepta, define status HTTP e retorna um formato padronizado, como `ProblemDetails`.

### Onde aparece no projeto

No backend principal e no servico de recomendacao.

### Arquivos envolvidos

- `src/backend/infrastructure/middleware/GlobalExceptionHandler.cs`
- `src/backend/Program.cs`
- `src/recommendation-service/Infrastructure/Middleware/ExceptionHandlingMiddleware.cs`
- `src/backend/BackEnd.Aventura.Tests/GlobalExceptionHandlerTests.cs`

### Justificativa da escolha

Evita repeticao de `try/catch` em controllers e torna erros previsiveis para clientes da API.

### Alternativas possiveis

- `try/catch` manual em cada controller.
- Filtros de excecao.
- Retornos padronizados sem excecoes.

### Vantagens

- Padroniza erros.
- Reduz duplicacao.
- Melhora experiencia do consumidor da API.
- Facilita testes de status HTTP.

### Limitacoes

- Pode esconder detalhes uteis se logs forem insuficientes.
- Mapeamento de excecoes precisa ser bem definido.
- Erros de validacao podem exigir tratamento especifico.

### Perguntas de prova provaveis

**Pergunta:** Por que usar tratamento global de excecoes?

**Resposta esperada:** Para centralizar a conversao de excecoes em respostas HTTP padronizadas e evitar repeticao em controllers.

## 20. Arquitetura em Camadas e SOA

### Definicao

Arquitetura em camadas separa responsabilidades em apresentacao, aplicacao, dominio, persistencia e infraestrutura. SOA, Service-Oriented Architecture, organiza a solucao em servicos com responsabilidades especificas.

### Como funciona

Cada camada ou servico possui um papel. O frontend interage com APIs; o backend orquestra regras; o banco persiste dados; servicos de recomendacao e treinamento tratam ML.

### Onde aparece no projeto

Na estrutura geral do repositorio.

### Arquivos envolvidos

- `src/backend`
- `src/frontend/mobile-app-aventura`
- `src/recommendation-service`
- `src/training-service`
- `src/ml`
- `src/database`
- `README.md`
- `docs/Projeto.md`

### Justificativa da escolha

O produto envolve app mobile, API, banco, recomendacao e treinamento. Separar responsabilidades facilita evolucao, manutencao e testes.

### Alternativas possiveis

- Monolito unico.
- Microservicos completos.
- Arquitetura serverless.
- Aplicacao fullstack simples.

### Vantagens

- Separacao de responsabilidades.
- Evolucao independente de partes do sistema.
- Maior clareza de arquitetura.
- Permite trocar motor de recomendacao sem alterar todo o app.

### Limitacoes

- Mais projetos para configurar.
- Integracao entre servicos exige contratos claros.
- Deploy e observabilidade ficam mais complexos.

### Perguntas de prova provaveis

**Pergunta:** Por que o projeto pode ser visto como SOA?

**Resposta esperada:** Porque possui modulos/servicos separados para frontend, backend, banco, recomendacao, treinamento e ML, cada um com responsabilidade especifica.

## 21. Requisitos Funcionais, RNFs e Qualidade

### Definicao

Requisitos funcionais descrevem o que o sistema deve fazer. Requisitos nao funcionais descrevem atributos de qualidade, como desempenho, seguranca, usabilidade, disponibilidade e escalabilidade.

### Como funciona

RFs viram features e endpoints. RNFs viram criterios de qualidade e cenarios de teste. A validacao considera tanto comportamento quanto atributos do sistema.

### Onde aparece no projeto

Na documentacao do projeto e no planejamento de testes.

### Arquivos envolvidos

- `docs/Projeto.md`
- `ARTEFATOS.md`
- `docs/01_mapa_sprint.md`
- `docs/02_artefatos_vs_codigo.md`

### Justificativa da escolha

O artefato de planejamento de testes exige coerencia com RFs e RNFs. O produto precisa ser funcional, seguro, usavel e sustentavel.

### Alternativas possiveis

- Backlog sem classificacao formal.
- Criterios de aceite por historia.
- Modelos de qualidade como ISO 25010.

### Vantagens

- Alinha tecnologia e negocio.
- Facilita planejamento de testes.
- Ajuda a priorizar qualidade.

### Limitacoes

- RNFs podem ser dificeis de medir.
- Documentacao pode ficar defasada.
- Nem todos os RNFs possuem implementacao automatizada no repositorio atual.

### Perguntas de prova provaveis

**Pergunta:** Dê um exemplo de RF e RNF no projeto.

**Resposta esperada:** RF: usuario adicionar espetaculo a wishlist. RNF: interface mobile deve ser usavel e nao apresentar elementos cortados, com teste SUS planejado.

## 22. Testes de Usabilidade e SUS

### Definicao

Teste de usabilidade avalia se usuarios conseguem usar o produto de forma eficaz e satisfatoria. SUS, System Usability Scale, e um questionario padronizado para medir percepcao de usabilidade.

### Como funciona

Usuarios executam tarefas, como onboarding, busca e wishlist. Observa-se sucesso, erros, tempo e duvidas. Depois, aplica-se o questionario SUS.

### Onde aparece no projeto

No planejamento em `docs/Projeto.md`.

### Arquivos envolvidos

- `docs/Projeto.md`
- `docs/projeto_visual.md`
- `src/frontend/mobile-app-aventura/screens/*`
- `src/frontend/mobile-app-aventura/components/*`

### Justificativa da escolha

O produto e mobile e depende da experiencia do usuario para descoberta e conversao. O artefato tambem exige pelo menos 5 usuarios externos e SUS ou metodo equivalente.

### Alternativas possiveis

- Entrevistas qualitativas.
- Heuristic evaluation.
- Teste A/B.
- Analytics de uso.

### Vantagens

- Avalia experiencia real.
- Identifica problemas que testes automatizados nao pegam.
- SUS gera metrica comparavel.

### Limitacoes

- Depende de usuarios reais.
- Amostras pequenas podem ter vies.
- No repositorio, foi encontrado planejamento, mas nao relatorio preenchido com resultados reais.

### Perguntas de prova provaveis

**Pergunta:** Por que teste unitario nao substitui teste de usabilidade?

**Resposta esperada:** Porque teste unitario valida codigo isolado. Usabilidade avalia compreensao, fluidez e satisfacao de usuarios reais.

**Pergunta:** Qual meta SUS foi planejada?

**Resposta esperada:** O planejamento cita SUS maior ou igual a 70.

## 23. Testes de Integracao, APIs Externas e VHS

### Definicao

Teste de integracao valida a comunicacao entre componentes. APIs externas sao servicos fora do sistema, como ViaCEP ou ticketera. VHS foi planejado como modulo de cache/isolamento para chamadas HTTP externas em testes.

### Como funciona

O backend chamaria servicos externos por meio de uma camada intermediaria. Em testes, ferramentas como WireMock poderiam simular respostas, e VHS poderia armazenar respostas para tornar os testes determinísticos.

### Onde aparece no projeto

O conceito esta documentado em `docs/Projeto.md`, mas a implementacao nao foi localizada no codigo atual.

### Arquivos envolvidos

- `docs/Projeto.md`
- `docs/02_artefatos_vs_codigo.md`
- Possiveis pontos futuros: services HTTP no backend e testes de integracao

### Justificativa da escolha

Integracoes externas sao instaveis por natureza. Planejar isolamento evita que testes dependam de internet, disponibilidade de terceiros ou credenciais.

### Alternativas possiveis

- Mock direto de `HttpClient`.
- WireMock sem VHS.
- Testes contra APIs reais.
- Contratos pact/contract testing.

### Vantagens

- Testes mais reprodutiveis.
- Evita chamada real a terceiros.
- Permite simular indisponibilidade.
- Ajuda a validar fallback.

### Limitacoes

- Nao foi encontrado implementado no repositorio atual.
- Pode aumentar complexidade de testes.
- Cache mal configurado pode mascarar erros reais.

### Perguntas de prova provaveis

**Pergunta:** O que foi planejado para ViaCEP e ticketera?

**Resposta esperada:** Testes de APIs externas e integracao usando simulacao/cache via VHS, mas a implementacao correspondente nao foi localizada no codigo atual.

**Pergunta:** Por que nao testar sempre contra API real?

**Resposta esperada:** Porque APIs reais podem estar indisponiveis, lentas, mudar comportamento ou exigir rede/credenciais, tornando testes instaveis.

## 24. Sistema de Recomendacao e SVD

### Definicao

Sistema de recomendacao sugere itens para usuarios com base em preferencias, historico ou similaridade. SVD, Singular Value Decomposition, e uma tecnica de fatoracao que representa usuarios e itens em fatores latentes.

### Como funciona

O modelo aprende vetores para usuarios e itens. A previsao de interesse combina media global, vies do usuario, vies do item e produto escalar entre fatores latentes. O projeto tambem usa fallback cold start quando nao ha historico.

### Onde aparece no projeto

Nos servicos de treinamento, recomendacao e ML.

### Arquivos envolvidos

- `src/training-service/Training/SvdTrainer.cs`
- `src/training-service/Program.cs`
- `src/recommendation-service/Services/SvdRecommendationService.cs`
- `src/recommendation-service/Program.cs`
- `src/ml/Trainer.cs`
- `src/ml/Recommender.cs`
- `src/ml/Program.cs`
- `data/models/*`
- `data/raw/database_sympla.csv`

### Justificativa da escolha

O objetivo do produto e recomendar espetaculos. SVD e adequado para recomendacao baseada em interacoes usuario-item e permite ranquear itens ainda nao consumidos.

### Alternativas possiveis

- KNN item-based ou user-based.
- Regras por popularidade.
- Conteudo por genero/categoria.
- Modelos hibridos.
- Redes neurais de recomendacao.

### Vantagens

- Captura padroes latentes.
- Escala melhor que comparacoes simples em muitos casos.
- Produz score para ranqueamento.
- Permite combinar com afinidade de categoria.

### Limitacoes

- Sofre com cold start.
- Requer dados historicos de interacao.
- Pode ser menos explicavel.
- Depende de qualidade do dataset.

### Perguntas de prova provaveis

**Pergunta:** Como o projeto lida com cold start?

**Resposta esperada:** Usa fallback por itens populares ou por categorias/preferencias quando nao ha historico suficiente do usuario.

**Pergunta:** Qual formula aparece no servico SVD?

**Resposta esperada:** A previsao combina media global, vies do usuario, vies do item e produto escalar entre fatores do usuario e do item.

## 25. Mock, Fake e Isolamento de Testes

### Definicao

Mocks e fakes substituem dependencias reais em testes. Fake e uma implementacao simples funcional; mock geralmente verifica chamadas e comportamentos esperados.

### Como funciona

Em vez de acessar banco ou API real, o teste usa uma dependencia controlada. Isso permite testar a unidade sem efeitos externos.

### Onde aparece no projeto

Nos testes do backend e frontend.

### Arquivos envolvidos

- `src/backend/BackEnd.Aventura.Tests/Fakes/*.cs`
- `src/backend/BackEnd.Aventura.Tests/*Tests.cs`
- `src/frontend/mobile-app-aventura/tests/*.js`
- `src/frontend/mobile-app-aventura/services/authService.js`

### Justificativa da escolha

O projeto precisava de testes unitarios rapidos e isolados. Fakes e mocks permitem testar regras sem depender de banco real ou APIs externas.

### Alternativas possiveis

- Testes com banco real.
- Testcontainers.
- Mocks gerados por bibliotecas.
- Testes end-to-end.

### Vantagens

- Rapidez.
- Determinismo.
- Isolamento da unidade.
- Facil simular erro.

### Limitacoes

- Podem divergir do comportamento real.
- Nao validam integracao completa.
- Exigem manutencao junto aos contratos.

### Perguntas de prova provaveis

**Pergunta:** Qual a diferenca entre fake e mock?

**Resposta esperada:** Fake e uma implementacao simples que substitui a real; mock e configurado para simular respostas e verificar interacoes esperadas.

**Pergunta:** Por que os testes unitarios do backend usam fakes?

**Resposta esperada:** Para testar services sem depender do banco, mantendo testes rapidos e focados nas regras de negocio.

## 26. Padroes de Codificacao e Documentacao Tecnica

### Definicao

Padroes de codificacao definem convencoes para escrever codigo consistente. Documentacao tecnica registra arquitetura, decisoes, execucao, APIs e testes.

### Como funciona

Convencoes de nomes, organizacao de pastas, separacao de camadas e documentos ajudam a equipe a manter o projeto compreensivel.

### Onde aparece no projeto

Em `docs/Projeto.md`, `README.md`, `src/database/README.md` e na propria organizacao de pastas.

### Arquivos envolvidos

- `README.md`
- `docs/Projeto.md`
- `docs/projeto_visual.md`
- `src/database/README.md`
- `docs/02_artefatos_vs_codigo.md`

### Justificativa da escolha

O artefato exigia arquitetura, estrutura de diretorios, padroes, contratos de API, testes e evidencias.

### Alternativas possiveis

- Wiki externa.
- ADRs, Architecture Decision Records.
- Documentacao gerada automaticamente.
- Comentarios no codigo apenas.

### Vantagens

- Facilita onboarding.
- Ajuda avaliacao academica.
- Registra decisoes e limitacoes.
- Melhora rastreabilidade.

### Limitacoes

- Pode ficar desatualizada.
- `docs/GuiaPadroesDesenvolvimento.md` era esperado, mas nao foi encontrado.
- Documentacao nao substitui testes executaveis.

### Perguntas de prova provaveis

**Pergunta:** Qual a importancia da documentacao tecnica no projeto?

**Resposta esperada:** Ela registra arquitetura, requisitos, testes, decisoes e instrucoes de execucao, permitindo rastreabilidade entre artefatos e codigo.

**Pergunta:** Que documento esperado nao foi encontrado?

**Resposta esperada:** `docs/GuiaPadroesDesenvolvimento.md`.

## Revisao rapida por artefato

| Artefato | Conceitos principais para revisar |
| --- | --- |
| A1 Implementacao do Banco de Dados | PostgreSQL, SQL, modelo fisico, constraints, integridade referencial, transacoes, Docker, ausencia de ORM. |
| A2 Desenvolvimento do Projeto | React Native, MVVM, OOP, Repository, DI, Factory, DTO, API REST, Swagger, SOA, recomendacao SVD. |
| A3 Planejamento de Testes | TDD, testes unitarios, BDD conceitual, mocks/fakes, RNFs, usabilidade, SUS, integracao, APIs externas, VHS planejado. |

## Perguntas integradoras finais

**Pergunta:** Como os autoestudos de banco se conectam ao codigo?

**Resposta esperada:** Eles aparecem nos scripts PostgreSQL, nas constraints, nas tabelas relacionais, nos dados iniciais e no acesso via SQL direto nos repositories.

**Pergunta:** Como os autoestudos de React Native se conectam ao codigo?

**Resposta esperada:** Eles aparecem no app Expo/React Native, nas telas, componentes, navegacao, hooks e no uso de MVVM com viewmodels, repositories e services.

**Pergunta:** Como TDD/OOP/BDD se conectam ao projeto?

**Resposta esperada:** OOP estrutura dominio, services, repositories e interfaces; TDD aparece nos testes unitarios com xUnit/Jest; BDD aparece no planejamento por cenarios, criterios e comportamentos esperados.

**Pergunta:** Qual a principal diferenca entre o que esta implementado e o que esta planejado?

**Resposta esperada:** Banco, backend, frontend mockado, recomendacao e testes unitarios estao implementados. Integracoes externas reais, VHS, WireMock/Testcontainers e relatorio real de usabilidade aparecem mais como planejamento/documentacao do que como codigo encontrado.

