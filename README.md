# G2 - Recomendação de Espetáculos no Portal Aventura

**Repositório:** `2026-1a/t17/g02`  
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
| [Principais Entendimentos do Negócio](docs/Projeto.md) | Esquema e detalhes que regem o entendimento do projeto |
| [Gerenciamento de Projeto](docs/Gestao_Projeto_Configuracao.md) | Metodologia e gestão do projeto |

---

---

## Arquitetura da Solução

A solução foi desenvolvida utilizando uma arquitetura modular baseada em separação de responsabilidades entre frontend, backend, banco de dados e camada de recomendação.

### Tecnologias utilizadas

| Camada | Tecnologia |
|----------|-------------|
| Frontend | HTML, CSS, JavaScript |
| Backend | C# (.NET) |
| Banco de Dados | PostgreSQL |
| Infraestrutura | Supabase |
| Controle de versão | Git/GitLab |
| Ambiente de desenvolvimento | VS Code |

### Estrutura do Repositório

```text
g02-2/
│ .env
│ .gitignore
│ README.md
│
├── assets/
├── docs/
├── src/
│   ├── backend/
│   ├── frontend/
│   ├── models/
│   ├── dataset/
│   └── database/
│       ├──01_create_database.sql
│       └──02_initial_data.sql
│
├── DbRunner/
```

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
- Conta no Supabase
- Visual Studio Code

### Variáveis de ambiente

Criar um arquivo `.env` na raiz do projeto:

```env
DB_HOST=seu_host
DB_PORT=6543
DB_NAME=postgres
DB_USER=seu_usuario
DB_PASSWORD=sua_senha
```

---

### Instalar dependências do executor

Entrar na pasta:

```bash
cd DbRunner
```

Instalar pacotes:

```bash
dotnet add package Npgsql
dotnet add package DotNetEnv
```

---

### Executar scripts SQL

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
