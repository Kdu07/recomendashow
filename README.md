# G2 - Recomendação de Espetáculos no Portal Aventura

**Repositório:** `2026-1b/t17/g02`  
🔗 [https://git.inteli.edu.br/graduacao/2026-1b/t17/g02](https://git.inteli.edu.br/graduacao/2026-1b/t17/g02)

---

## Equipe

| Nome                                      | LinkedIn |
|-------------------------------------------|----------|
| Alexsander da Silva Barbosa              | [LinkedIn](https://www.linkedin.com/in/alexsander-dsbarbosa/) |
| Carlos Eduardo Serrano Quaglia           | [LinkedIn](https://www.linkedin.com/in/carlos-quaglia-309088357/) |
| Catarina Sayuri Arashiro Braga Felipe    | [LinkedIn](https://www.linkedin.com/in/catarina-sayuri/) |
| Débora Pereira Nogueira                  | [LinkedIn](https://www.linkedin.com/in/debora-pereira-nogueira/) |
| Isaac Souza Santos                       | [LinkedIn](https://www.linkedin.com/in/isaac-souza-santos/) |
| Nicholas Anantha Krishnan Millani        | [LinkedIn](https://www.linkedin.com/in/nicholas-millani-83a950357/) |
| Paulo Henrique Bueno Fernandes           | [LinkedIn](https://www.linkedin.com/in/paulo-henrique0601/) |

---

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

## Descrição do Projeto

O projeto consiste no desenvolvimento de uma **solução de recomendação inteligente de espetáculos** para o **Portal Aventura**, em parceria com a **Aventura Teatros**.

Fundada em 2008, a Aventura é referência nacional na produção de espetáculos de altíssima qualidade, responsável por grandes sucessos como *Elis, o Musical*, *Mamma Mia!*, *A Noviça Rebelde*, *O Mágico de Oz*, *Ayrton Senna - O Musical*, *Pixar in Concert* e *Vozes Negras*. Com mais de 50 produções, 5 milhões de espectadores e forte atuação na gestão de venues culturais, a empresa busca inovar a experiência do público por meio da tecnologia.

**Objetivo principal:** Criar um sistema de recomendação inteligente capaz de sugerir espetáculos, conteúdos e experiências culturais alinhadas aos interesses do usuário, aumentando o engajamento, o tempo de permanência no portal e a conversão em vendas de ingressos.

---

## Escopo do Produto

### O que a plataforma faz:
- Recomenda espetáculos personalizados com base no perfil e comportamento do usuário
- Exibe vitrines e carrosséis dinâmicos de conteúdo
- Filtra sugestões por geolocalização (São Paulo e Rio de Janeiro)
- Redireciona o usuário para a compra de ingressos via parceiros de bilheteria

### O que a plataforma **não** faz:
- Processa pagamentos ou vende ingressos diretamente
- Substitui o CMS de cadastro de eventos da Aventura
- Funciona offline ou atua como rede social entre usuários

---

## Documentação

| Documento | Descrição |
|-----------|-----------|
| [Índice da Documentação](docs/Index.md) | Navegação central para todos os documentos presentes na pasta `docs` |
| [Projeto](docs/Projeto.md) | Documento principal com contexto, requisitos, arquitetura, diagramas, planejamento de testes e evolução da solução |
| [Gestão e Configuração](docs/Gestao_Projeto_Configuracao.md) | Metodologia de versionamento, organização de branches, commits e práticas de gestão |
| [Gestão de Projeto - Sprint 2](docs/Gestão_projeto_SP2.md) | Registro de planejamento e acompanhamento da Sprint 2 |
| [Gestão de Projeto - Sprint 3](docs/Gestão_projeto_SP3.md) | Registro de planejamento e acompanhamento da Sprint 3 |
| [Projeto Visual](docs/projeto_visual.md) | Diretrizes visuais, telas, fluxos e decisões de interface |

---

---

## Arquitetura da Solução

A solução adota uma abordagem baseada em **SOA (Service-Oriented Architecture)**, organizando o projeto em módulos com responsabilidades específicas para interface mobile, backend, banco de dados, recomendação, treinamento de modelos, análise de dados e documentação. Essa separação favorece a manutenção, a evolução dos componentes e a integração entre as partes da aplicação.

A organização atual da solução contempla os seguintes módulos:

| Módulo | Responsabilidade |
|---|---|
| Frontend mobile | Interface em React Native/Expo responsável pelas telas e fluxos de navegação do usuário. |
| Backend | API principal em C#/.NET, responsável por expor endpoints e concentrar regras de integração da aplicação. |
| Banco de dados | Estrutura PostgreSQL definida por scripts SQL, com suporte a execução local via Docker e configuração hospedada via Supabase. |
| Serviço de recomendação | Serviço em C#/.NET responsável por carregar modelos/dados e fornecer recomendações e métricas. |
| Serviço de treinamento | Módulo em C#/.NET voltado ao processamento dos dados e geração de artefatos de modelo. |
| Dados e notebooks | Área de análise, experimentação e avaliação dos modelos de recomendação. |
| Documentação | Registros acadêmicos, técnicos, visuais e de gestão do projeto. |

### Fluxo lógico da solução

1. Os dados são organizados em `data/` e explorados nos notebooks.
2. O serviço de treinamento processa os dados e gera artefatos de modelo.
3. O serviço de recomendação utiliza os artefatos para calcular recomendações.
4. O backend atua como camada de API e integração.
5. O frontend mobile apresenta a experiência ao usuário.
6. O banco PostgreSQL armazena entidades, preferências, interações e recomendações.

### Tecnologias utilizadas

| Camada | Tecnologia |
|----------|-------------|
| Frontend mobile | React Native, Expo, JavaScript |
| Backend | C# / .NET |
| Serviço de recomendação | C# / .NET, modelo SVD e métricas de avaliação |
| Serviço de treinamento | C# / .NET e processamento de dados |
| Análise e experimentação | Python, Jupyter Notebooks |
| Banco de Dados | PostgreSQL |
| Infraestrutura de banco | Supabase e Docker Compose para ambiente local |
| Controle de versão | Git/GitLab |
| Ambiente de desenvolvimento | VS Code |

### Estrutura do Repositório

```text
g02-2/
│ .env.example
│ .gitignore
│ docker-compose-db.yml
│ README.md
│ requirements.txt
│
├── assets/
│   ├── design/
│   └── mock/
│
├── data/
│   ├── models/
│   └── raw/
│
├── DbRunner/
│
├── docs/
│   ├── Index.md
│   ├── Projeto.md
│   ├── Gestao_Projeto_Configuracao.md
│   ├── Gestão_projeto_SP2.md
│   ├── Gestão_projeto_SP3.md
│   └── projeto_visual.md
│
├── notebooks/
│
└── src/
    ├── backend/
    ├── database/
    ├── frontend/
    │   └── mobile-app-aventura/
    ├── recommendation-service/
    └── training-service/
```

| Caminho | Finalidade |
|---|---|
| `assets/` | Imagens, diagramas, mockups e materiais visuais do projeto. |
| `data/` | Dados brutos e modelos treinados utilizados nos experimentos e serviços. |
| `DbRunner/` | Executor .NET responsável por aplicar os scripts SQL no banco configurado. |
| `docs/` | Documentação acadêmica, técnica, visual e de gestão do projeto. |
| `notebooks/` | Notebooks de análise exploratória, experimentação e avaliação de modelos. |
| `src/backend/` | Backend principal em C#/.NET. |
| `src/database/` | Scripts SQL e Dockerfile do banco PostgreSQL. |
| `src/frontend/mobile-app-aventura/` | Aplicação mobile em React Native/Expo. |
| `src/recommendation-service/` | Serviço de recomendação e métricas do modelo. |
| `src/training-service/` | Serviço de treinamento e processamento dos dados. |

---

## Banco de Dados

O banco de dados foi modelado em PostgreSQL utilizando identificadores UUID e hospedado na plataforma Supabase.

A modelagem contempla entidades relacionadas ao comportamento do usuário, espetáculos, preferências e mecanismos de recomendação.

Principais entidades:

- Usuários
- Preferências
- Espetáculos
- Teatros
- Gêneros
- Interações
- Recomendações
- Wishlist e Curtidas

Relacionamentos implementados:

- Usuário ↔ Preferências (1:1)
- Usuário ↔ Interações (1:N)
- Usuário ↔ Gêneros Favoritos (N:N)
- Espetáculos ↔ Gêneros (N:N)
- Teatro ↔ Espetáculos (N:N)
- Usuário ↔ Recomendações (1:N)

---

## Regras de Negócio Implementadas

As seguintes regras foram consideradas na modelagem:

- Cada usuário possui email único
- Usuários devem possuir idade mínima de 13 anos
- Cada gênero possui nome único
- Cada espetáculo possui nome único
- Cada teatro possui nome único
- O valor do ingresso não pode ser negativo
- A data de encerramento do espetáculo deve ser posterior à data inicial
- Recomendações possuem score entre 0 e 1
- Exclusão de usuários remove automaticamente registros dependentes (`ON DELETE CASCADE`)
- Exclusão de teatros vinculados a espetáculos utiliza restrição (`ON DELETE RESTRICT`)

---

## Configuração e Execução

### Pré-requisitos

- .NET SDK instalado
- Node.js e npm instalados
- Expo CLI ou execução via `npx expo`
- Docker e Docker Compose, caso utilize o banco local
- Conta ou projeto configurado no Supabase, caso utilize banco hospedado
- Visual Studio Code ou editor equivalente

### Variáveis de ambiente

Criar um arquivo `.env` na raiz do projeto a partir do modelo seguro:

```bash
cp .env.example .env
```

Preencha o arquivo `.env` com as credenciais do ambiente local ou hospedado. O arquivo `.env` não deve conter valores fictícios em execução real e não deve ser versionado.

---

### Banco de dados local com Docker

```bash
docker compose -f docker-compose-db.yml up --build
```

O serviço utiliza as variáveis `POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_DB` e `POSTGRES_PORT`.

---

### Executar scripts SQL com DbRunner

Entrar na pasta:

```bash
cd DbRunner
```

Restaurar dependências:

```bash
dotnet restore
```

Executar os scripts SQL:

```bash
dotnet run
```

O processo realiza:

1. Leitura do arquivo `.env`
2. Conexão com Supabase
3. Execução do script estrutural
4. Execução dos dados iniciais
5. Validação da conexão

---

### Executar frontend mobile

```bash
cd src/frontend/mobile-app-aventura
npm install
npm start
```

Também estão disponíveis os scripts `npm run android`, `npm run ios` e `npm run web`, conforme configuração do Expo.

---

### Executar backend

```bash
cd src/backend
dotnet restore
dotnet run
```

---

### Executar serviço de recomendação

```bash
cd src/recommendation-service
dotnet restore
dotnet run
```

---

### Executar serviço de treinamento

```bash
cd src/training-service
dotnet restore
dotnet run
```

Alguns serviços dependem de arquivos de dados, modelos treinados e variáveis de ambiente configuradas corretamente.

---

## Modelo de Dados

A modelagem foi construída com foco em personalização de recomendações, rastreamento de comportamento e organização das informações culturais disponibilizadas pelo Portal Aventura.

O modelo utiliza:

- UUID como chave primária
- tabelas associativas para relacionamentos N:N
- constraints de integridade
- índices para otimização
- enums para categorização de interações

Diagramas e detalhes adicionais encontram-se na pasta:

```text
assets/
```

---

## Histórico de Versões

| Versão | Data       | Descrição |
|--------|------------|---------|
| 0.1.0  | Abril/2026 | Visão inicial do produto, definição de requisitos e entrega do artefato de Dataset (Sprint 01) |
| 0.2.0  | Maio/2026 | Evolução da documentação, modelagem de banco, protótipos visuais e organização dos artefatos do projeto |
| 0.3.0-dev | Maio/2026 | Estruturação em andamento de frontend mobile, backend, serviços de recomendação, treinamento e documentação acadêmica |

---

## Licença

<p xmlns:cc="http://creativecommons.org/ns#" xmlns:dct="http://purl.org/dc/terms/">
  Este projeto,
  <a property="dct:title" rel="cc:attributionURL" href="https://git.inteli.edu.br/graduacao/2026-1b/t17/g02">Recomendação de Espetáculos no Portal Aventura</a>,
  desenvolvido por
  <a rel="cc:attributionURL dct:creator" property="cc:attributionName" href="https://www.inteli.edu.br/">Inteli</a>,
  está licenciado sob
  <a href="https://creativecommons.org/licenses/by/4.0/" target="_blank" rel="license noopener noreferrer">Creative Commons Attribution 4.0 International</a>.
</p>

---
