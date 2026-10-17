# Inteli - Instituto de Tecnologia e Liderança

<div align="center">
  <img src="assets/logo.png" alt="Logo do Inteli" width="320">
</div>

---

# RecomendaShow

## Grupo 2 - Aventureiros

Recomendação inteligente de espetáculos para o Portal Aventura.

**Repositório:** `2026-1b/t17/g02`  
[https://git.inteli.edu.br/graduacao/2026-1b/t17/g02](https://git.inteli.edu.br/graduacao/2026-1b/t17/g02)

---

## Equipe

- [Alexsander da Silva Barbosa](https://www.linkedin.com/in/alexsander-dsbarbosa/)
- [Carlos Eduardo Serrano Quaglia](https://www.linkedin.com/in/carlos-quaglia-309088357/)
- [Catarina Sayuri Arashiro Braga Felipe](https://www.linkedin.com/in/catarina-sayuri/)
- [Débora Pereira Nogueira](https://www.linkedin.com/in/debora-pereira-nogueira/)
- [Isaac Souza Santos](https://www.linkedin.com/in/isaac-souza-santos/)
- [Nicholas Anantha Krishnan Millani](https://www.linkedin.com/in/nicholas-millani-83a950357/)
- [Paulo Henrique Bueno Fernandes](https://www.linkedin.com/in/paulo-henrique0601/)

## Corpo Docente

### Orientadora
- [Vanessa Nunes](https://www.linkedin.com/in/vanunes/)

### Instrutores
- [Afonso Brandão](https://www.linkedin.com/in/afonsolelis/)
- [Hermano Peixoto](https://www.linkedin.com/in/hermano-peixoto-1091796/)
- [Francisco Escobar](https://www.linkedin.com/in/francisco-escobar/)
- [Rafael Jacomossi](https://www.linkedin.com/in/rafael-jacomossi-6135b0a1/)
- [Luciano Galdino](https://www.linkedin.com/in/luciano-galdino-26191b36/)
- [Filipe Gonçalves](https://www.linkedin.com/in/filipe-gon%C3%A7alves-08a55015b/)

---

## Sobre o Projeto

O RecomendaShow é uma solução de recomendação para a **Aventura Teatros**, criada para sugerir espetáculos, conteúdos e experiências culturais alinhadas ao perfil e ao comportamento dos usuários do Portal Aventura.

A plataforma recomenda espetáculos personalizados, organiza vitrines dinâmicas de conteúdo, considera preferências e localização do usuário e redireciona a compra de ingressos para parceiros de bilheteria. O produto não processa pagamentos, não substitui o CMS da Aventura e não atua como rede social.

---

## Documentação

| Documento | Descrição |
|---|---|
| [Índice da Documentação](docs/Index.md) | Navegação central dos documentos do projeto. |
| [Projeto](docs/Projeto.md) | Contexto, problema, requisitos, arquitetura, diagramas, testes e evolução da solução. |
| [Guia de Padrões de Desenvolvimento](docs/GuiaPadroesDesenvolvimento.md) | Padrões de pastas, branches, commits, nomenclatura, camadas, erros, testes, padrões de projeto, DoR e DoD. |
| [Gestão do Projeto](docs/GestaoProjeto.md) | Sprints, retrospectivas, ações de melhoria e planejamento. |
| [Gestão e Configuração](docs/Gestao_Projeto_Configuracao.md) | Versionamento, branches, commits, Pull Requests e configuração. |
| [Modelo de Recomendação](docs/Modelo_Recomendacao.md) | Pipeline de dados, treinamento, inferência, métricas e artefatos do modelo. |
| [Projeto Visual](docs/projeto_visual.md) | Diretrizes visuais, telas, fluxos e decisões de interface. |
| [Banco de Dados](src/database/README.md) | Schema, infraestrutura, Docker, dicionário de dados e evoluções do banco. |

---

## Arquitetura

A solução segue uma abordagem orientada a serviços, separando interface mobile, API, banco de dados, treinamento de modelo, inferência e documentação.

| Módulo | Responsabilidade |
|---|---|
| `src/frontend/mobile-app-aventura` | Aplicativo mobile em React Native/Expo. |
| `src/backend` | API principal em C#/.NET, com controllers, services, repositories e regras de negócio. |
| `src/database` | Scripts SQL, schema PostgreSQL e configuração do banco. |
| `src/mongodb` | Configuração do MongoDB local usado no ambiente Docker. |
| `src/ml` | Preprocessamento, treinamento, avaliação e geração dos artefatos do modelo. |
| `src/inference-service` | Host de inferência, métricas, diagnósticos e batch de recomendações. |
| `data` | Dados brutos e artefatos treinados do modelo. |
| `docs` | Documentação acadêmica, técnica, visual e de gestão. |

### Tecnologias

| Camada | Tecnologias |
|---|---|
| Mobile | React Native, Expo, JavaScript, Jest |
| Backend | C#, .NET 9, ASP.NET Core, xUnit |
| Dados | PostgreSQL, MongoDB, Docker Compose |
| Machine Learning | ML.NET, pipeline híbrido de recomendação |
| Infraestrutura | Oracle Cloud Infrastructure para banco remoto e Docker para ambiente local |
| Colaboração | Git, GitLab, VS Code |

---

## Banco de Dados

O banco de dados foi modelado em PostgreSQL com identificadores UUID e entidades voltadas ao registro de usuários, preferências, espetáculos, teatros, gêneros, interações, recomendações, wishlist e curtidas.

A infraestrutura atual contempla execução local via Docker Compose, com PostgreSQL e MongoDB, e banco remoto hospedado em uma máquina virtual Linux na Oracle Cloud Infrastructure. Os scripts SQL ficam em `src/database` e são responsáveis pela criação do schema, carga inicial, melhorias incrementais, constraints, índices, triggers e procedures.

Principais relacionamentos implementados:

- Usuário e preferências (1:1)
- Usuário e interações (1:N)
- Usuário e gêneros favoritos (N:N)
- Espetáculos e gêneros (N:N)
- Teatro e espetáculos (N:N)
- Usuário e recomendações (1:N)

A documentação completa do banco, incluindo infraestrutura, execução local, dicionário de dados e evolução dos scripts, está em [src/database/README.md](src/database/README.md).

---

## Estrutura do Repositório

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

---

## Configuração

### Pré-requisitos

- .NET SDK 9, conforme `global.json`
- Node.js e npm
- Docker e Docker Compose
- VS Code ou editor equivalente

### Variáveis de Ambiente

Crie um arquivo `.env` na raiz do projeto a partir de `.env.example` e preencha as credenciais do ambiente desejado. Não versionar valores reais de usuário, senha ou conexão.

Para o banco local via Docker e para a execução da API, o `.env.example` já traz os nomes esperados pelo projeto. Ajuste os valores conforme o seu ambiente local:

```env
POSTGRES_USER=<usuario_local>
POSTGRES_PASSWORD=<senha_local>
POSTGRES_DB=<nome_do_banco>
POSTGRES_HOST=localhost
POSTGRES_PORT=5432
MONGO_USER=<usuario_local>
MONGO_PASSWORD=<senha_local>
MONGO_PORT=27017
JWT_KEY=<chave_local_com_pelo_menos_32_caracteres>
```

O `.env` contém credenciais e não deve ser versionado. Use apenas valores locais de desenvolvimento e nunca credenciais de produção no repositório.

---

## Execução Local

### Banco de Dados

Sobe PostgreSQL e MongoDB localmente:

```bash
docker compose --env-file .env -f docker-compose-db.yml up --build
```

Para aplicar os scripts em um banco remoto, use o `DbRunner` conforme as instruções da documentação completa do banco.

### Backend

```bash
dotnet restore src/backend/BackEnd.Aventura.csproj
dotnet run --project src/backend/BackEnd.Aventura.csproj --launch-profile http
```

A API sobe em `http://localhost:5253` no perfil `http`.

### Frontend Mobile

```bash
cd src/frontend/mobile-app-aventura
npm ci
npm start
```

Também estão disponíveis os scripts:

```bash
npm run android
npm run ios
npm run web
```

### Treinamento do Modelo

O preprocessamento usa `data/raw/database_sympla.csv` e gera o dataset processado. O treinamento gera `data/models/model.zip` e artefatos auxiliares de cold-start, categorias e afinidade.

```bash
cd src/ml
dotnet run preprocess
dotnet run train
```

### Serviço de Inferência

```powershell
$env:ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=aventura;Username=postgres;Password=postgres"
dotnet restore src/inference-service/InferenceService.csproj
dotnet run --project src/inference-service/InferenceService.csproj
```

O serviço carrega os artefatos do modelo, executa o batch de recomendações e expõe endpoints de métricas e diagnóstico.

---

## Testes

```bash
dotnet test src/backend/BackEnd.Aventura.Tests/BackEnd.Aventura.Tests.csproj
dotnet test src/backend/BackEnd.Aventura.IntegrationTests/BackEnd.Aventura.IntegrationTests.csproj
dotnet test src/inference-service/InferenceService.Tests/InferenceService.Tests.csproj
```

Testes do aplicativo mobile:

```bash
cd src/frontend/mobile-app-aventura
npm test
```

Os testes de integração podem depender de Docker ativo para criação dos containers de banco.

---

## Histórico de Versões

| Versão | Data | Descrição |
|---|---|---|
| 0.1.0 | Abril/2026 | Visão inicial do produto, requisitos e dataset da Sprint 1. |
| 0.2.0 | Maio/2026 | Modelagem de banco, protótipos visuais e organização dos artefatos. |
| 0.3.0 | Maio/2026 | Estruturação do frontend mobile, backend, banco e serviços de recomendação. |
| 0.4.0 | Junho/2026 | Integração entre backend, banco, ML, inferência, testes automatizados e refinamento documental. |
| 0.5.0 | Junho/2026 | Integração com MongoDB (imagens via GridFS), cobertura de testes ≥ 90% em todos os módulos, análise ética IA4People, cadeia de valor, deploy em OCI e entrega final. |

---

## Licença

<img style="height:22px!important;margin-left:3px;vertical-align:text-bottom;" src="https://mirrors.creativecommons.org/presskit/icons/cc.svg?ref=chooser-v1"><img style="height:22px!important;margin-left:3px;vertical-align:text-bottom;" src="https://mirrors.creativecommons.org/presskit/icons/by.svg?ref=chooser-v1"><p xmlns:cc="http://creativecommons.org/ns#" xmlns:dct="http://purl.org/dc/terms/"><a property="dct:title" rel="cc:attributionURL" href="https://git.inteli.edu.br/graduacao/2026-1b/t17/g02">Recomendação de Espetáculos no Portal Aventura</a> by <a href="https://www.inteli.edu.br">Inteli</a>, <a href="https://www.linkedin.com/in/alexsander-dsbarbosa/">Alexsander da Silva Barbosa</a>, <a href="https://www.linkedin.com/in/carlos-quaglia-309088357/">Carlos Eduardo Serrano Quaglia</a>, <a href="https://www.linkedin.com/in/catarina-sayuri/">Catarina Sayuri Arashiro Braga Felipe</a>, <a href="https://www.linkedin.com/in/debora-pereira-nogueira/">Débora Pereira Nogueira</a>, <a href="https://www.linkedin.com/in/isaac-souza-santos/">Isaac Souza Santos</a>, <a href="https://www.linkedin.com/in/nicholas-millani-83a950357/">Nicholas Anantha Krishnan Millani</a>, <a href="https://www.linkedin.com/in/paulo-henrique0601/">Paulo Henrique Bueno Fernandes</a> is licensed under <a href="http://creativecommons.org/licenses/by/4.0/?ref=chooser-v1" target="_blank" rel="license noopener noreferrer" style="display:inline-block;">Attribution 4.0 International</a>.</p>

## Sobre esta cópia

Esta é uma cópia do repositório original do grupo, que fica no GitLab do Inteli. O GitHub não aceita arquivos acima de 100 MB, então três arquivos foram retirados de todo o histórico desta cópia:

- `src/src/models/matriz_item_usuario.joblib` (247 MB)
- `src/src/models/matriz_usuario_item.joblib` (247 MB)
- `assets/Screen_Recording_20260625_194027_Expo Go.mp4` (136 MB)

As duas matrizes vinham do recomendador KNN item a item (issue 64 do GitLab) e não são usadas pelo código da branch `main`. Por causa dessa limpeza, os identificadores dos commits daqui são diferentes dos do repositório original.
