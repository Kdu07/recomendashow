# Mapa da Sprint 3

Este documento consolida o inventario inicial dos artefatos e autoestudos indicados em `ARTEFATOS.md` e `AUTOESTUDOS.md`.

## 1. Artefatos encontrados

| ID | Artefato | Objetivo |
| --- | --- | --- |
| A1 | Implementacao do Banco de Dados | Implementar o modelo fisico do banco de dados a partir do modelo logico, entregando scripts SQL funcionais, dicionario de dados, dados iniciais, informacoes de implantacao e regras de negocio no banco. |
| A2 | Desenvolvimento do Projeto | Evidenciar o progresso incremental da solucao de software, demonstrando implementacao de backend e frontend conforme o planejamento da Sprint 3. |
| A3 | Planejamento de Testes | Estruturar o planejamento completo de testes do projeto, abrangendo testes funcionais, nao funcionais, usabilidade, APIs externas e integracao. |

## 2. Detalhamento dos artefatos

### A1. Implementacao do Banco de Dados

Objetivo: materializar a modelagem de dados em um esquema funcional, com scripts SQL, dicionario de dados, dados iniciais e configuracoes de implantacao.

Entregas previstas:

- Script SQL de criacao do banco de dados: `01_create_database.sql`;
- Dicionario de dados;
- Script de insercao de dados iniciais: `02_initial_data.sql`;
- Registro de informacoes de implantacao;
- Regras de negocio implementadas diretamente no banco de dados;
- README.md em `src/database`.

Restricoes e expectativas:

- Nao utilizar ORMs;
- Banco de dados funcional no servidor Inteli;
- Ambiente dockerizado;
- Scripts SQL funcionais e otimizados;
- Configuracao reproduzivel;
- Regras de negocio documentadas.

### A2. Desenvolvimento do Projeto

Objetivo: demonstrar a evolucao incremental da solucao por meio de backend, frontend, arquitetura, padroes, features, testes e documentacao.

Entregas previstas:

- Arquitetura de codificacao e estrutura de diretorios;
- Documentacao da arquitetura;
- Diagrama da estrutura;
- Padroes de codificacao e padroes de projeto;
- Pelo menos um padrao criacional;
- Pelo menos um padrao comportamental;
- Features implementadas;
- Progresso documentado;
- Contratos de APIs documentados;
- Estrategia de testes;
- Testes unitarios implementados;
- Cobertura documentada;
- Frontend integrado;
- API de autenticacao externa;
- Pelo menos uma API adicional;
- APIs presentes na modelagem tecnica;
- Swagger funcional;
- Video demonstrativo;
- Evidencias de TDD;
- `docs/GuiaPadroesDesenvolvimento.md`;
- `docs/Projeto.md`.

Expectativas:

- Codigo de qualidade;
- Boas praticas;
- Arquitetura definida;
- TDD aplicado;
- Evidencias de execucao.

### A3. Planejamento de Testes

Objetivo: organizar a estrategia de validacao da solucao considerando diferentes tipos de testes e criterios de execucao.

Entregas previstas:

- Planejamento de testes funcionais;
- Planejamento de testes de requisitos nao funcionais;
- Planejamento de testes de usabilidade com pelo menos 5 usuarios externos;
- Uso de SUS ou metodo equivalente nos testes de usabilidade;
- Planejamento de testes de APIs externas;
- Planejamento de testes de integracao;
- Uso obrigatorio do modulo VHS nos testes de integracao;
- Registro em `docs/Projeto.md`.

Cada planejamento deve conter:

- Proposito;
- Instrucoes de execucao;
- Cenarios positivos e negativos;
- Pre-condicoes;
- Pos-condicoes;
- Abrangencia;
- Ferramentas utilizadas, quando aplicavel;
- Metodo de validacao, quando aplicavel.

Expectativas:

- Cobertura de diferentes tipos de testes;
- Coerencia com RFs e RNFs;
- Clareza e completude.

## 3. Autoestudos encontrados

| ID | Autoestudo | Fontes indicadas | Conceitos teoricos presentes |
| --- | --- | --- | --- |
| E1 | Banco de Dados PostgreSQL | Documentacao PostgreSQL 15, tutorial de transacoes Neon, material sobre triggers Neon | SQL, comandos SQL, PostgreSQL, modelo fisico de banco de dados, transacoes, propriedades ACID, consistencia de dados, triggers, automacao de regras no banco, integridade e regras de negocio no banco de dados. |
| E2 | React Native | DevMedia primeiro app React Native, video introdutorio, artigo sobre MVVM com Redux em React Native | Desenvolvimento mobile, componentes React Native, estrutura de aplicacao, interface nativa, estado da aplicacao, MVVM, Redux, separacao de responsabilidades, integracao frontend. |
| E3 | TDD, OOP e BDD | Notion sobre TDD/OOP/BDD, video sobre TDD, artigo DevMedia sobre fundamentos de TDD | Test-Driven Development, ciclo red-green-refactor, testes unitarios, programacao orientada a objetos, classes, objetos, encapsulamento, heranca, polimorfismo, Behavior-Driven Development, especificacao por comportamento, cenarios e criterios de aceitacao. |
| E4 | Engenharia de Software | Aulas de Engenharia de Software, licoes 4, 5, 6 e 7 | Arquitetura de software, padroes de projeto, padroes de codificacao, documentacao tecnica, planejamento e desenvolvimento incremental, qualidade de software, requisitos funcionais e nao funcionais, testes, validacao e boas praticas de engenharia. |
| E5 | Videos Gerais | Quatro videos gerais indicados no arquivo de autoestudos | Conteudos complementares relacionados a desenvolvimento do projeto, apoio conceitual e pratico para banco de dados, frontend, engenharia de software, testes ou integracao, conforme aplicacao no contexto da sprint. |

## 4. Matriz relacionando artefatos e autoestudos

Legenda:

- Forte: relacao direta com entregas e criterios do artefato.
- Media: relacao de apoio ou fundamentacao complementar.
- Baixa: relacao indireta ou contextual.

| Artefato / Autoestudo | E1 Banco de Dados PostgreSQL | E2 React Native | E3 TDD, OOP e BDD | E4 Engenharia de Software | E5 Videos Gerais |
| --- | --- | --- | --- | --- | --- |
| A1 Implementacao do Banco de Dados | Forte: fundamenta SQL, transacoes, triggers, regras de negocio e implantacao do banco. | Baixa: pode apoiar entendimento da integracao futura com a aplicacao, mas nao e central ao artefato. | Media: apoia organizacao de regras, validacoes e possiveis testes relacionados ao banco. | Media: apoia documentacao, boas praticas, estrutura e coerencia da entrega tecnica. | Baixa: apoio complementar conforme o conteudo dos videos seja usado para banco ou infraestrutura. |
| A2 Desenvolvimento do Projeto | Media: apoia persistencia, APIs que dependem do banco e regras de negocio. | Forte: fundamenta o frontend integrado e o desenvolvimento mobile. | Forte: fundamenta TDD, testes unitarios, OOP, BDD, padroes e evidencias de desenvolvimento. | Forte: fundamenta arquitetura, padroes, documentacao, qualidade e desenvolvimento incremental. | Media: apoio complementar para praticas de desenvolvimento, integracao e demonstracao. |
| A3 Planejamento de Testes | Media: apoia cenarios de teste ligados a dados, transacoes, integridade e regras no banco. | Media: apoia testes de interface, fluxos mobile, usabilidade e integracao frontend. | Forte: fundamenta testes funcionais, unitarios, BDD, cenarios positivos e negativos. | Forte: fundamenta planejamento de testes, RNFs, validacao, qualidade e alinhamento com requisitos. | Media: apoio complementar para definicao de cenarios, ferramentas e execucao. |

