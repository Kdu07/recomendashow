## Sumário

1. [Banco de Dados](#banco-de-dados)
   - 1.1 [Principais Entidades](#principais-entidades)
   - 1.2 [Relacionamentos Implementados](#relacionamentos-implementados)
2. [Regras de Negócio Implementadas](#regras-de-negócio-implementadas)
3. [Configuração e Execução](#configuração-e-execução)
   - 3.1 [Pré-requisitos](#pré-requisitos)
   - 3.2 [Variáveis de Ambiente](#variáveis-de-ambiente)
   - 3.3 [Instalar Dependências do Executor](#instalar-dependências-do-executor)
   - 3.4 [Executar Scripts SQL](#executar-scripts-sql)
4. [Modelo de Dados](#modelo-de-dados)
5. [Infraestrutura de Banco de Dados](#infraestrutura-de-banco-de-dados)
   - 5.1 [Migração da Infraestrutura](#migração-da-infraestrutura)
   - 5.2 [Especificações da Máquina Virtual](#especificações-da-máquina-virtual)
   - 5.3 [Benefícios da Arquitetura Utilizada](#benefícios-da-arquitetura-utilizada)

---

## 1. Banco de Dados

O banco de dados foi modelado em PostgreSQL utilizando identificadores UUID, hospedado inicialmente na plataforma Supabase e posteriormente migrado para uma máquina virtual Linux na Oracle Cloud Infrastructure.

A modelagem contempla entidades relacionadas ao comportamento do usuário, espetáculos, preferências e mecanismos de recomendação.

### 1.1 Principais Entidades

- Usuários
- Preferências
- Espetáculos
- Teatros
- Gêneros
- Interações
- Recomendações
- Wishlist e Curtidas

### 1.2 Relacionamentos Implementados

- Usuário ↔ Preferências (1:1)
- Usuário ↔ Interações (1:N)
- Usuário ↔ Gêneros Favoritos (N:N)
- Espetáculos ↔ Gêneros (N:N)
- Teatro ↔ Espetáculos (N:N)
- Usuário ↔ Recomendações (1:N)

---

## 2. Regras de Negócio Implementadas

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

## 3. Configuração e Execução

### 3.1 Pré-requisitos

- .NET SDK instalado
- Acesso à máquina virtual Oracle Cloud (Linux/Ubuntu 22.04)
- Visual Studio Code

### 3.2 Variáveis de Ambiente

Criar um arquivo `.env` na raiz do projeto com os dados de conexão da VM Oracle:

```env
DB_HOST=ip_publico_da_vm_oracle
DB_PORT=5432
DB_NAME=postgres
DB_USER=seu_usuario
DB_PASSWORD=sua_senha
```

> **Observação:** A porta padrão do PostgreSQL hospedado na VM Oracle é `5432`. Certifique-se de que essa porta está liberada nas regras de firewall e na lista de segurança da Oracle Cloud.

### 3.3 Instalar Dependências do Executor

Entrar na pasta:

```bash
cd DbRunner
```

Instalar pacotes:

```bash
dotnet add package Npgsql
dotnet add package DotNetEnv
```

### 3.4 Executar Scripts SQL

```bash
dotnet run
```

O processo realiza:

1. Leitura do arquivo `.env`
2. Conexão com o PostgreSQL na VM Oracle Cloud
3. Execução do script estrutural
4. Execução dos dados iniciais
5. Validação da conexão

---

## 4. Modelo de Dados

A modelagem foi construída com foco em personalização de recomendações, rastreamento de comportamento e organização das informações culturais disponibilizadas pelo Portal Aventura.

O modelo utiliza:

- UUID como chave primária
- Tabelas associativas para relacionamentos N:N
- Constraints de integridade
- Índices para otimização
- Enums para categorização de interações

Diagramas e detalhes adicionais encontram-se na pasta:

```text
assets/
```

---

## 5. Infraestrutura de Banco de Dados

### 5.1 Migração da Infraestrutura

Como a infraestrutura gratuita da AWS apresentava interrupções frequentes e reinicializações periódicas, o deploy do banco de dados foi migrado para uma máquina virtual Linux hospedada na Oracle Cloud Infrastructure.

A escolha da Oracle foi motivada principalmente pela estabilidade e pelos recursos oferecidos no plano Free Tier. Diferente do ambiente utilizado anteriormente, as máquinas virtuais gratuitas da Oracle disponibilizam maior capacidade de processamento, memória RAM e armazenamento, permitindo uma execução mais estável e adequada para serviços persistentes como bancos de dados.

A migração trouxe as seguintes vantagens:

- Eliminação da necessidade de alterar constantemente as strings de conexão
- Maior disponibilidade da aplicação
- Possibilidade de hospedar banco de dados e serviços auxiliares na mesma instância
- Maior controle sobre configurações de rede, firewall e armazenamento
- Melhor desempenho para operações de leitura e escrita no banco

A Oracle disponibiliza recursos "Always Free", permitindo manter a máquina virtual ativa continuamente sem custos adicionais, desde que respeitados os limites do plano gratuito.

### 5.2 Especificações da Máquina Virtual

A infraestrutura foi hospedada em uma instância Linux da Oracle Cloud utilizando a distribuição Ubuntu Server Minimal.

#### 5.2.1 Sistema Operacional

| Configuração | Valor |
|---|---|
| Sistema Operacional | Canonical Ubuntu |
| Versão | Ubuntu 22.04 Minimal |
| Imagem Utilizada | Canonical-Ubuntu-22.04-Minimal-2026.04.30 |

#### 5.2.2 Configuração da Instância

| Configuração | Valor |
|---|---|
| Forma da Instância | VM.Standard.E2.1.Micro |
| Quantidade de OCPUs | 1 |
| Memória RAM | 1 GB |
| Largura de Banda de Rede | 0.48 Gbps |
| Disco Local | Armazenamento em Blocos |

### 5.3 Benefícios da Arquitetura Utilizada

**Persistência de Serviço**
A VM da Oracle permanece ativa continuamente, evitando quedas periódicas do banco de dados e interrupções durante o desenvolvimento.

**Controle de Infraestrutura**
A solução permite controle total sobre configuração de firewall, abertura de portas, gerenciamento de usuários, armazenamento persistente, regras de acesso à rede e instalação de dependências e serviços auxiliares.

**Facilidade de Manutenção**
Com o banco hospedado em uma máquina persistente, o time não precisa reconfigurar constantemente conexões ou endpoints da aplicação, reduzindo falhas operacionais.

**Escalabilidade**
A infraestrutura possibilita futuras expansões, como hospedagem de APIs auxiliares, execução de containers Docker, utilização de serviços de monitoramento, backups automatizados e replicação do banco de dados.

---

## 6. Subindo o banco localmente com Docker

### 6.1 Pré-requisito

- [Docker](https://docs.docker.com/get-docker/) instalado (inclui Docker Compose)

### 6.2 Variáveis de ambiente

Crie um arquivo `.env` na raiz do projeto com as seguintes variáveis:

```env
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres
POSTGRES_DB=aventura
POSTGRES_PORT=5432
MONGO_USER=mongo
MONGO_PASSWORD=mongo
MONGO_PORT=27017
```

### 6.3 Subir o banco

```bash
docker compose -f docker-compose-db.yml up -d
```

Isso inicia dois containers: `postgres_db` (PostgreSQL 16) e `aventura_mongodb` (MongoDB). Os scripts SQL são executados automaticamente na primeira inicialização, na ordem:

1. `01_create_database.sql` — estrutura do banco
2. `02_sprint4_improvements.sql` — melhorias da Sprint 4
3. `02_initial_data.sql` — dados de seed

### 6.4 Verificar que o banco subiu

```bash
docker ps
```

O container `postgres_db` deve aparecer com status `Up`. Para conectar:

```bash
psql -h localhost -p 5432 -U postgres -d aventura
```

Ou via qualquer cliente (DBeaver, TablePlus etc.) com `host=localhost`, `port=5432`, `database=aventura`.

### 6.5 Derrubar o banco

```bash
docker compose -f docker-compose-db.yml down
```

Para remover também os volumes (apaga todos os dados):

```bash
docker compose -f docker-compose-db.yml down -v
```

---

## 7. Dicionário de Dados

Esta seção descreve todas as 11 tabelas do schema PostgreSQL do projeto Aventura Teatros, com base nos arquivos `01_create_database.sql` e `02_sprint4_improvements.sql`.

**Tipos personalizados (ENUMs):**
- `tipo_interacao`: `'Clique'`, `'Acesso'`, `'TempoDeTela'`
- `tipo_lista`: `'curtida'`, `'wishlist'`

---

### 7.1 usuarios

| Coluna | Tipo | Restrições | Relações | Descrição |
|---|---|---|---|---|
| id | UUID | PK, DEFAULT gen_random_uuid() | — | Identificador único do usuário |
| nome | VARCHAR(100) | NOT NULL | — | Nome completo do usuário |
| email | VARCHAR(255) | NOT NULL, UNIQUE | — | Endereço de e-mail, usado como login |
| senha_hash | TEXT | NOT NULL | — | Hash BCrypt da senha do usuário |
| idade | INT | CHECK(idade >= 13) | — | Idade do usuário; mínimo 13 anos |
| data_cadastro | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | — | Data e hora do registro do usuário |
| cep | VARCHAR(9) | — | — | CEP do endereço (integração ViaCEP) |
| logradouro | VARCHAR(255) | — | — | Rua/avenida do endereço |
| numero | VARCHAR(10) | — | — | Número do imóvel |
| complemento | VARCHAR(255) | — | — | Complemento do endereço |
| bairro | VARCHAR(100) | — | — | Bairro do endereço |
| cidade | VARCHAR(100) | — | — | Cidade do endereço |
| consentimento_aceito | BOOLEAN | NOT NULL, DEFAULT FALSE | — | Indica se o usuário aceitou os termos de consentimento (LGPD) |
| data_consentimento | TIMESTAMP | — | — | Data e hora em que o consentimento foi registrado |
| deleted_at | TIMESTAMP | — | — | Marca lógica de exclusão do usuário (soft delete, LGPD) |
| anonimizado | BOOLEAN | NOT NULL, DEFAULT FALSE | — | Indica se os dados pessoais do usuário foram anonimizados (LGPD) |
| ml_id | SERIAL | UNIQUE (idx_usuarios_ml_id) | — | Identificador inteiro sequencial para mapeamento UUID ↔ índice do modelo de recomendação ML |

---

### 7.2 teatros

| Coluna | Tipo | Restrições | Relações | Descrição |
|---|---|---|---|---|
| id | UUID | PK, DEFAULT gen_random_uuid() | — | Identificador único do teatro |
| nome | VARCHAR(150) | NOT NULL, UNIQUE | — | Nome do teatro |
| endereco | VARCHAR(255) | — | — | Endereço completo do teatro |
| cidade | VARCHAR(100) | — | — | Cidade onde o teatro está localizado |
| horario_funcionamento | VARCHAR(100) | — | — | Descrição dos horários de funcionamento |

---

### 7.3 espetaculos

| Coluna | Tipo | Restrições | Relações | Descrição |
|---|---|---|---|---|
| id | UUID | PK, DEFAULT gen_random_uuid() | — | Identificador único do espetáculo |
| nome | VARCHAR(150) | NOT NULL, UNIQUE | — | Nome do espetáculo |
| tipo_evento | VARCHAR(80) | — | — | Categoria do evento (ex.: peça, musical, dança) |
| descricao | TEXT | — | — | Descrição detalhada do espetáculo |
| url_bilheteria | TEXT | — | — | URL para compra de ingressos |
| data_inicio | TIMESTAMP | NOT NULL | — | Data e hora de início do espetáculo |
| data_fim | TIMESTAMP | NOT NULL, CHECK(data_fim > data_inicio) | — | Data e hora de encerramento; deve ser posterior a `data_inicio` |
| valor_ingresso | DECIMAL(10,2) | CHECK(valor_ingresso >= 0) | — | Preço do ingresso; não pode ser negativo |

---

### 7.4 generos

| Coluna | Tipo | Restrições | Relações | Descrição |
|---|---|---|---|---|
| id | UUID | PK, DEFAULT gen_random_uuid() | — | Identificador único do gênero |
| nome | VARCHAR(50) | NOT NULL, UNIQUE | — | Nome do gênero artístico (ex.: drama, comédia) |

---

### 7.5 interacoes

| Coluna | Tipo | Restrições | Relações | Descrição |
|---|---|---|---|---|
| id | UUID | PK, DEFAULT gen_random_uuid() | — | Identificador único da interação |
| usuario_id | UUID | NOT NULL, FK | → usuarios(id) ON DELETE CASCADE | Usuário que realizou a interação |
| espetaculo_id | UUID | NOT NULL, FK | → espetaculos(id) ON DELETE CASCADE | Espetáculo com o qual o usuário interagiu |
| tipo | tipo_interacao | NOT NULL | — | Tipo da interação: `Clique`, `Acesso` ou `TempoDeTela` |
| pontuacao | DECIMAL(4,2) | CHECK(pontuacao >= 0 AND pontuacao <= 10) | — | Pontuação atribuída à interação; escala de 0 a 10 |
| data_registro | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | — | Data e hora em que a interação foi registrada |
| data_expiracao | TIMESTAMP | CHECK(data_expiracao > data_registro) | — | Data a partir da qual a interação deixa de ser considerada pelo motor de recomendação |

---

### 7.6 preferencias

| Coluna | Tipo | Restrições | Relações | Descrição |
|---|---|---|---|---|
| id | UUID | PK, DEFAULT gen_random_uuid() | — | Identificador único do registro de preferências |
| usuario_id | UUID | UNIQUE, FK | → usuarios(id) ON DELETE CASCADE | Usuário ao qual as preferências pertencem (relação 1:1) |
| cidade | VARCHAR(100) | — | — | Cidade preferida para busca de espetáculos |
| tags | TEXT[] | — | — | Array de tags de interesse do usuário |
| faixa_preco_min | DECIMAL(10,2) | — | — | Valor mínimo da faixa de preço desejada |
| faixa_preco_max | DECIMAL(10,2) | — | — | Valor máximo da faixa de preço desejada |
| receber_notificacoes | BOOLEAN | DEFAULT TRUE | — | Indica se o usuário deseja receber notificações |
| data_atualizacao | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | — | Data e hora da última atualização; mantida automaticamente pelo trigger `trg_preferencias_update` |

---

### 7.7 recomendacoes

| Coluna | Tipo | Restrições | Relações | Descrição |
|---|---|---|---|---|
| id | UUID | PK, DEFAULT gen_random_uuid() | — | Identificador único da recomendação |
| usuario_id | UUID | NOT NULL, FK | → usuarios(id) ON DELETE CASCADE | Usuário que recebeu a recomendação |
| espetaculo_id | UUID | NOT NULL, FK | → espetaculos(id) ON DELETE CASCADE | Espetáculo recomendado |
| score | DECIMAL(4,2) | CHECK(score >= 0 AND score <= 1) | — | Relevância da recomendação; escala de 0 a 1 |
| motivo | TEXT | — | — | Explicação textual gerada pelo motor de recomendação |
| data_geracao | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | — | Data e hora em que a recomendação foi gerada |

> **Constraint Sprint 4:** `UNIQUE(usuario_id, espetaculo_id)` — impede recomendações duplicadas para o mesmo par usuário/espetáculo.

---

### 7.8 usuario_genero_favorito

Tabela associativa N:N entre usuários e gêneros favoritos.

| Coluna | Tipo | Restrições | Relações | Descrição |
|---|---|---|---|---|
| usuario_id | UUID | PK (composta), FK | → usuarios(id) ON DELETE CASCADE | Usuário que marcou o gênero como favorito |
| genero_id | UUID | PK (composta), FK | → generos(id) ON DELETE CASCADE | Gênero marcado como favorito pelo usuário |

---

### 7.9 espetaculo_genero

Tabela associativa N:N entre espetáculos e gêneros.

| Coluna | Tipo | Restrições | Relações | Descrição |
|---|---|---|---|---|
| espetaculo_id | UUID | PK (composta), FK | → espetaculos(id) ON DELETE CASCADE | Espetáculo ao qual o gênero está vinculado |
| genero_id | UUID | PK (composta), FK | → generos(id) ON DELETE CASCADE | Gênero vinculado ao espetáculo |

---

### 7.10 teatro_espetaculo

Tabela associativa N:N entre teatros e espetáculos.

| Coluna | Tipo | Restrições | Relações | Descrição |
|---|---|---|---|---|
| teatro_id | UUID | PK (composta), FK | → teatros(id) ON DELETE RESTRICT | Teatro onde o espetáculo é exibido; exclusão bloqueada se houver espetáculos vinculados |
| espetaculo_id | UUID | PK (composta), FK | → espetaculos(id) ON DELETE CASCADE | Espetáculo exibido no teatro |

---

### 7.11 wishlist_curtidas

Tabela que unifica wishlist e curtidas do usuário em espetáculos.

| Coluna | Tipo | Restrições | Relações | Descrição |
|---|---|---|---|---|
| usuario_id | UUID | PK (composta), FK | → usuarios(id) ON DELETE CASCADE | Usuário que realizou a ação |
| espetaculo_id | UUID | PK (composta), FK | → espetaculos(id) ON DELETE CASCADE | Espetáculo alvo da ação |
| tipo | tipo_lista | PK (composta) | — | Tipo da ação: `curtida` ou `wishlist` |

---

### 7.12 Objetos de Banco — Sprint 4

Objetos criados em `02_sprint4_improvements.sql`.

#### Índices

| Nome | Tipo | Tabela | Colunas | Finalidade |
|---|---|---|---|---|
| idx_interacoes_usuario_expiracao | B-tree | interacoes | (usuario_id, data_expiracao) | Filtragem de interações por usuário e janela temporal para o motor de recomendação; substitui `idx_interacoes_usuario` |
| idx_teatros_cidade | B-tree | teatros | (cidade) | Suporte ao carrossel "Perto de você", que filtra espetáculos pela cidade do teatro |
| idx_espetaculos_datas | B-tree | espetaculos | (data_inicio, data_fim) | Filtragem de espetáculos em cartaz por intervalo de datas |
| idx_recomendacoes_usuario_score | B-tree | recomendacoes | (usuario_id, score DESC) | Recuperação das top recomendações de um usuário ordenadas por relevância; substitui `idx_recomendacao_usuario` |
| idx_wishlist_usuario | B-tree | wishlist_curtidas | (usuario_id) | Carregamento da wishlist/curtidas na abertura da aba pelo usuário |
| idx_usuarios_ml_id | B-tree (UNIQUE) | usuarios | (ml_id) | Garante unicidade do identificador inteiro para mapeamento UUID ↔ índice do modelo ML |

#### Funções

| Nome | Retorno | Tabela afetada | Finalidade |
|---|---|---|---|
| fn_atualizar_data_preferencias() | TRIGGER | preferencias | Atualiza automaticamente `data_atualizacao` para o timestamp corrente em todo UPDATE na tabela `preferencias` |
| fn_bloquear_update_anonimizado() | TRIGGER | usuarios | Impede qualquer UPDATE em registros já anonimizados, levantando exceção em conformidade com a LGPD |

#### Triggers

| Nome | Evento | Tabela | Função associada | Finalidade |
|---|---|---|---|---|
| trg_preferencias_update | BEFORE UPDATE | preferencias | fn_atualizar_data_preferencias() | Mantém `data_atualizacao` sempre sincronizado com o momento real da alteração |
| trg_bloquear_update_anonimizado | BEFORE UPDATE | usuarios | fn_bloquear_update_anonimizado() | Protege dados anonimizados de qualquer modificação posterior (LGPD) |

#### Procedures

| Nome | Parâmetros | Tabela afetada | Finalidade |
|---|---|---|---|
| sp_anonimizar_usuario | p_id UUID | usuarios | Substitui dados pessoais (nome, e-mail, senha, endereço) por valores neutros e marca `anonimizado = TRUE`; executa somente em usuários com `deleted_at IS NOT NULL` ainda não anonimizados (LGPD) |