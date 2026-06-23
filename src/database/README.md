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
6. [Subindo o banco localmente com Docker](#subindo-o-banco-localmente-com-docker)
   - 6.1 [Pré-requisito](#pré-requisito)
   - 6.2 [Variáveis de ambiente](#variáveis-de-ambiente)
   - 6.3 [Subir o banco](#subir-o-banco)
   - 6.4 [Verificar que o banco subiu](#verificar-que-o-banco-subiu)
   - 6.5 [Derrubar o banco](#derrubar-o-banco)
7. [Dicionário de Dados](#dicionário-de-dados)
8. [Explain Analyze](#explain-analyze)
9. [Teste de Carga (pgbench)](#teste-de-carga-pgbench)
10. [Evoluções - Sprint 4](#evoluções--sprint-4)

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
3. `03_initial_data.sql` — dados de seed

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
| data_cadastro | TIMESTAMPTZ | DEFAULT CURRENT_TIMESTAMP | — | Data e hora do registro do usuário |
| data_nascimento | DATE | — | — | Data de nascimento do usuário, usada para cálculo e validação de idade |
| cep | VARCHAR(9) | — | — | CEP do endereço (integração ViaCEP) |
| logradouro | VARCHAR(255) | — | — | Rua/avenida do endereço |
| numero | VARCHAR(10) | — | — | Número do imóvel |
| complemento | VARCHAR(255) | — | — | Complemento do endereço |
| bairro | VARCHAR(100) | — | — | Bairro do endereço |
| cidade | VARCHAR(100) | — | — | Cidade do endereço |
| consentimento_aceito | BOOLEAN | NOT NULL, DEFAULT FALSE | — | Indica se o usuário aceitou os termos de consentimento (LGPD) |
| data_consentimento | TIMESTAMPTZ | — | — | Data e hora em que o consentimento foi registrado |
| deleted_at | TIMESTAMPTZ | — | — | Marca lógica de exclusão do usuário (soft delete, LGPD) |
| anonimizado | BOOLEAN | NOT NULL, DEFAULT FALSE | — | Indica se os dados pessoais do usuário foram anonimizados (LGPD) |
| anonimizado_em | TIMESTAMPTZ | — | — | Data e hora em que a anonimização foi executada pela `sp_anonimizar_usuario` (LGPD) |
| ml_id | SERIAL | UNIQUE (idx_usuarios_ml_id) | — | Identificador inteiro sequencial para mapeamento UUID ↔ índice do modelo de recomendação ML |
| data_atualizacao | TIMESTAMPTZ | DEFAULT CURRENT_TIMESTAMP | — | Data e hora da última modificação do perfil; mantida automaticamente pelo trigger `trg_usuarios_update` |

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
| data_inicio | TIMESTAMPTZ | NOT NULL | — | Data e hora de início do espetáculo |
| data_fim | TIMESTAMPTZ | NOT NULL, CHECK(data_fim > data_inicio) | — | Data e hora de encerramento; deve ser posterior a `data_inicio` |
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
| data_registro | TIMESTAMPTZ | DEFAULT CURRENT_TIMESTAMP | — | Data e hora em que a interação foi registrada |
| data_expiracao | TIMESTAMPTZ | CHECK(data_expiracao > data_registro) | — | Data a partir da qual a interação deixa de ser considerada pelo motor de recomendação |

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
| data_atualizacao | TIMESTAMPTZ | DEFAULT CURRENT_TIMESTAMP | — | Data e hora da última atualização; mantida automaticamente pelo trigger `trg_preferencias_update` |

---

### 7.7 recomendacoes

| Coluna | Tipo | Restrições | Relações | Descrição |
|---|---|---|---|---|
| id | UUID | PK, DEFAULT gen_random_uuid() | — | Identificador único da recomendação |
| usuario_id | UUID | NOT NULL, FK | → usuarios(id) ON DELETE CASCADE | Usuário que recebeu a recomendação |
| espetaculo_id | UUID | NOT NULL, FK | → espetaculos(id) ON DELETE CASCADE | Espetáculo recomendado |
| score | DECIMAL(4,2) | CHECK(score >= 0 AND score <= 1) | — | Relevância da recomendação; escala de 0 a 1 |
| motivo | TEXT | — | — | Explicação textual gerada pelo motor de recomendação |
| data_geracao | TIMESTAMPTZ | DEFAULT CURRENT_TIMESTAMP | — | Data e hora em que a recomendação foi gerada |
| visualizada_em | TIMESTAMPTZ | — | — | Data e hora em que o usuário visualizou a recomendação; preenchido pelo backend; `NULL` indica não visualizada |

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
| adicionado_em | TIMESTAMPTZ | DEFAULT CURRENT_TIMESTAMP | — | Data e hora em que o item foi adicionado à wishlist ou curtido pelo usuário |

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
| fn_atualizar_data_usuario() | TRIGGER | usuarios | Atualiza automaticamente `data_atualizacao` para o timestamp corrente em todo UPDATE na tabela `usuarios` |
| fn_bloquear_update_anonimizado() | TRIGGER | usuarios | Impede qualquer UPDATE em registros já anonimizados, levantando exceção em conformidade com a LGPD |
| fn_sincronizar_data_consentimento() | TRIGGER | usuarios | Preenche automaticamente `data_consentimento` quando o consentimento passa de `FALSE` para `TRUE` e a data ainda não foi informada |

#### Triggers

| Nome | Evento | Tabela | Função associada | Finalidade |
|---|---|---|---|---|
| trg_preferencias_update | BEFORE UPDATE | preferencias | fn_atualizar_data_preferencias() | Mantém `data_atualizacao` sempre sincronizado com o momento real da alteração |
| trg_usuarios_update | BEFORE UPDATE | usuarios | fn_atualizar_data_usuario() | Mantém `data_atualizacao` sempre sincronizado com o momento real da alteração do perfil |
| trg_bloquear_update_anonimizado | BEFORE UPDATE | usuarios | fn_bloquear_update_anonimizado() | Protege dados anonimizados de qualquer modificação posterior (LGPD) |
| trg_sincronizar_data_consentimento | BEFORE UPDATE | usuarios | fn_sincronizar_data_consentimento() | Sincroniza automaticamente a data de consentimento quando o aceite LGPD é registrado |

#### Procedures

| Nome | Parâmetros | Tabela afetada | Finalidade |
|---|---|---|---|
| sp_anonimizar_usuario | p_id UUID | usuarios | Substitui dados pessoais (nome, e-mail, senha, endereço) por valores neutros, marca `anonimizado = TRUE` e registra `anonimizado_em = CURRENT_TIMESTAMP` no mesmo `UPDATE`; executa somente em usuários com `deleted_at IS NOT NULL` ainda não anonimizados (LGPD) |

---

## 8. Explain Analyze

Os testes de plano de execução foram realizados no banco real do projeto, utilizando dados existentes no ambiente PostgreSQL. O objetivo foi avaliar o comportamento do otimizador diante das consultas mais relevantes para o uso da aplicação.

### Volume encontrado

| Tabela | Registros |
|---|---:|
| usuarios | 17 |
| teatros | 7 |
| espetaculos | 7 |
| interacoes | 2 |
| recomendacoes | 0 |
| wishlist_curtidas | 6 |

### Índices relevantes existentes

- `idx_wishlist_usuario`
- `idx_teatros_cidade`
- `idx_espetaculos_datas`
- `idx_interacoes_usuario_expiracao`
- `idx_recomendacoes_usuario_score`

### Consulta 1 - Wishlist por usuário

```sql
SELECT *
FROM wishlist_curtidas
WHERE usuario_id = '606f6cae-1d85-46b0-aa6f-dc5fabad9037';
```

Resultado registrado do `EXPLAIN (ANALYZE, BUFFERS)`:

```text
Seq Scan
Rows Removed by Filter: 4
Buffers: shared hit=1
Execution Time: 0.021 ms
```

Apesar da existência do índice `idx_wishlist_usuario`, o PostgreSQL optou por `Seq Scan` devido ao pequeno volume da tabela, composta por 6 registros.

### Consulta 2 - Espetáculos por cidade

```sql
SELECT e.*
FROM teatros t
JOIN teatro_espetaculo te ON te.teatro_id = t.id
JOIN espetaculos e ON e.id = te.espetaculo_id
WHERE t.cidade = 'São Paulo';
```

Resultado registrado do `EXPLAIN (ANALYZE, BUFFERS)`:

```text
Nested Loop
Seq Scan em teatros
Seq Scan em espetaculos
Index Only Scan usando teatro_espetaculo_pkey
Execution Time: 0.115 ms
```

O PostgreSQL utilizou o índice da tabela associativa, por meio de `Index Only Scan` sobre `teatro_espetaculo_pkey`. Entretanto, optou por `Seq Scan` nas tabelas `teatros` e `espetaculos` devido ao baixo número de registros.

### Consulta 3 - Espetáculos por intervalo de datas

```sql
SELECT *
FROM espetaculos
WHERE data_inicio >= CURRENT_DATE
  AND data_fim <= CURRENT_DATE + INTERVAL '90 days';
```

Resultado registrado do `EXPLAIN (ANALYZE, BUFFERS)`:

```text
Seq Scan
Rows Removed by Filter: 5
Buffers: shared hit=1
Execution Time: 0.020 ms
```

Apesar da existência do índice `idx_espetaculos_datas`, o PostgreSQL escolheu `Seq Scan` devido ao pequeno tamanho da tabela `espetaculos`.

### Consulta 4 - Interações por usuário

```sql
EXPLAIN (ANALYZE, BUFFERS)
SELECT *
FROM interacoes
WHERE usuario_id = '13b71d3d-f518-4a8c-9e14-cee1bdfc173d'
ORDER BY data_expiracao DESC;
```

Resultado normal:

```text
Sort  (cost=1.03..1.04 rows=1 width=80) (actual time=0.019..0.020 rows=1 loops=1)
  Sort Key: data_expiracao DESC
  Sort Method: quicksort  Memory: 25kB
  Buffers: shared hit=4
  ->  Seq Scan on interacoes  (cost=0.00..1.02 rows=1 width=80) (actual time=0.007..0.007 rows=1 loops=1)
        Filter: (usuario_id = '13b71d3d-f518-4a8c-9e14-cee1bdfc173d'::uuid)
        Rows Removed by Filter: 1
        Buffers: shared hit=1
Planning:
  Buffers: shared hit=4
Planning Time: 0.086 ms
Execution Time: 0.032 ms
```

Com o comportamento padrão, o PostgreSQL escolheu `Seq Scan` porque a tabela `interacoes` possui apenas 2 registros.

Consulta diagnóstica com `enable_seqscan` desativado:

```sql
SET enable_seqscan = off;

EXPLAIN (ANALYZE, BUFFERS)
SELECT *
FROM interacoes
WHERE usuario_id = '13b71d3d-f518-4a8c-9e14-cee1bdfc173d'
ORDER BY data_expiracao DESC;

SET enable_seqscan = on;
```

Resultado:

```text
Index Scan Backward using idx_interacoes_usuario_expiracao on interacoes  (cost=0.13..8.14 rows=1 width=80) (actual time=1.291..1.293 rows=1 loops=1)
  Index Cond: (usuario_id = '13b71d3d-f518-4a8c-9e14-cee1bdfc173d'::uuid)
  Buffers: shared hit=2 read=1
Planning Time: 0.076 ms
Execution Time: 1.309 ms
```

Ao desativar temporariamente `enable_seqscan`, o PostgreSQL utilizou o índice `idx_interacoes_usuario_expiracao`, comprovando que o índice existe, está válido e pode ser utilizado em cenários com maior volume. Esse ajuste foi usado apenas como diagnóstico e não deve ser aplicado como configuração de produção.

### Conclusão

Os índices avaliados estão corretamente criados e disponíveis no banco de dados. Contudo, nos testes realizados, o otimizador do PostgreSQL escolheu leituras sequenciais em parte das consultas porque as tabelas possuem poucos registros. Em ambientes com maior volume de dados, espera-se maior utilização dos índices, especialmente nas consultas filtradas por usuário, cidade, intervalo de datas e interações por usuário.

## 9. Teste de Carga (pgbench)

### Metodologia

Os testes de carga foram realizados utilizando a ferramenta `pgbench` sobre o banco PostgreSQL real do projeto. Foi utilizado um workload customizado contendo exclusivamente consultas de leitura (`SELECT`), evitando qualquer alteração nos dados da aplicação durante a execução dos testes.

As consultas simuladas representam operações reais do sistema, incluindo:

- Consulta de wishlist/curtidas por usuário;
- Busca de espetáculos por cidade;
- Busca de espetáculos por intervalo de datas;
- Consulta de recomendações por usuário.

Todos os testes foram executados durante 60 segundos e utilizaram apenas operações de leitura.

### Configuração do Ambiente

- PostgreSQL 16.14
- pgbench 16.14
- Banco de dados: `aventura_db`
- Tipo de workload: somente leitura (`read-only`)
- Duração dos testes: 60 segundos

Exemplo de execução:

```bash
pgbench -n -h <host> -p 5432 -U postgres -c <clientes> -j <threads> -T 60 -f pgbench_readonly_workload.sql aventura_db
```

### Workload utilizado

O workload utilizado no `pgbench` foi definido no arquivo `pgbench_readonly_workload.sql` e contém apenas comandos `SELECT`, sem transações explícitas e sem comandos de escrita.

```sql
-- 1. Wishlist/curtidas por usuario.
SELECT *
FROM wishlist_curtidas
WHERE usuario_id = '606f6cae-1d85-46b0-aa6f-dc5fabad9037';

-- 2. Espetaculos por cidade.
SELECT e.*
FROM teatros t
JOIN teatro_espetaculo te ON te.teatro_id = t.id
JOIN espetaculos e ON e.id = te.espetaculo_id
WHERE t.cidade = 'São Paulo';

-- 3. Espetaculos por intervalo de datas.
SELECT *
FROM espetaculos
WHERE data_inicio >= CURRENT_DATE
  AND data_fim <= CURRENT_DATE + INTERVAL '90 days';

-- 4. Recomendacoes por usuario.
SELECT *
FROM recomendacoes
WHERE usuario_id = '606f6cae-1d85-46b0-aa6f-dc5fabad9037'
ORDER BY score DESC;
```

### Resultados

| Clientes | Threads | TPS | Latência Média | Falhas |
|---:|---:|---:|---:|---:|
| 5 | 2 | 50,36 | 99,28 ms | 0 |
| 20 | 4 | 96,26 | 207,77 ms | 0 |
| 50 | 4 | 345,99 | 144,52 ms | 0 |
| 75 | 8 | 389,95 | 192,33 ms | 0 |

### Análise dos Resultados

Os resultados demonstram que o banco de dados permaneceu estável durante todos os cenários executados, sem ocorrência de falhas de transação.

Observou-se aumento progressivo da taxa de transações por segundo (TPS) conforme o número de clientes simultâneos foi ampliado. O throughput evoluiu de aproximadamente 50 TPS no cenário de 5 clientes para cerca de 390 TPS no cenário de 75 clientes simultâneos.

Apesar do aumento da concorrência, a latência média permaneceu em níveis aceitáveis para o tipo de workload executado. Como o banco possui baixo volume de dados e as consultas utilizadas são predominantemente simples, grande parte dos acessos permaneceu em memória, contribuindo para a estabilidade observada.

Observou-se uma anomalia entre os cenários de 20 e 50 clientes: com 20 clientes, a latência média foi de 207,77 ms, enquanto com 50 clientes a latência média foi de 144,52 ms. Essa redução pode estar relacionada ao aquecimento de cache do PostgreSQL e do sistema operacional, pois os testes foram executados sequencialmente e o volume de dados é pequeno.

### Limitação Observada

Foi realizado um teste adicional com 100 clientes simultâneos.

Durante a execução, o PostgreSQL retornou o erro:

```text
FATAL: sorry, too many clients already
```

A configuração do banco indicou:

```sql
SHOW max_connections;
```

Resultado:

```text
max_connections = 100
```

Esse comportamento indica que o ambiente não possuía conexões disponíveis suficientes para atender 100 novos clientes simultâneos, pois parte das conexões já estava sendo utilizada por outras sessões ativas. Portanto, o limite prático observado para o ambiente de testes ficou abaixo de 100 conexões concorrentes.

Uma possível evolução para mitigar esse limite em cenários com maior concorrência seria utilizar connection pooling, por exemplo com PgBouncer, reduzindo a necessidade de abrir uma conexão direta ao PostgreSQL para cada cliente simultâneo.

### Conclusão

Os testes de carga realizados com `pgbench` forneceram evidências quantitativas do comportamento do banco de dados sob diferentes níveis de concorrência. Os resultados demonstraram estabilidade operacional, ausência de falhas e crescimento do throughput conforme o aumento do número de clientes simultâneos. Além disso, foi possível identificar uma limitação real da infraestrutura relacionada ao limite de conexões configurado no PostgreSQL, contribuindo para a compreensão da capacidade operacional do ambiente.

---

## 10. Evoluções - Sprint 4

### 10.1 Contexto

O schema definido na Sprint 3 (`01_create_database.sql`) foi auditado na Sprint 4 e identificou-se a necessidade de novos campos, correções de qualidade e objetos de banco para suportar integração com ViaCEP, conformidade com a LGPD e o motor de recomendação por ML. Todas as evoluções foram concentradas em `02_sprint4_improvements.sql` — um script idempotente e re-executável —, mantendo o script original sem modificações estruturais adicionais.

---

### 10.2 Novas colunas em tabelas existentes

#### usuarios

| Coluna | Tipo | Por que foi necessária |
|---|---|---|
| data_nascimento | DATE | Registrar a data de nascimento para cálculo e validação de idade mínima (13 anos) |
| cep | VARCHAR(9) | Suportar integração com a API ViaCEP para preenchimento automático de endereço |
| logradouro | VARCHAR(255) | Complementar os dados de endereço retornados pelo ViaCEP |
| numero | VARCHAR(10) | Campo de endereço não fornecido pelo ViaCEP; preenchido manualmente pelo usuário |
| complemento | VARCHAR(255) | Campo de endereço opcional, não fornecido pelo ViaCEP |
| bairro | VARCHAR(100) | Complementar os dados de endereço retornados pelo ViaCEP |
| cidade | VARCHAR(100) | Cidade do usuário, usada na lógica de recomendação "Perto de você" |
| consentimento_aceito | BOOLEAN NOT NULL DEFAULT FALSE | Registrar se o usuário aceitou os termos de uso e privacidade (LGPD art. 7, I) |
| data_consentimento | TIMESTAMPTZ | Registrar o momento exato em que o consentimento foi concedido (LGPD art. 8, §5) |
| deleted_at | TIMESTAMPTZ | Implementar soft-delete: marca o usuário como excluído sem remover o registro, permitindo posterior anonimização (LGPD art. 18, IV) |
| anonimizado | BOOLEAN NOT NULL DEFAULT FALSE | Sinalizar que os dados pessoais já foram substituídos por valores neutros pela `sp_anonimizar_usuario` |
| anonimizado_em | TIMESTAMPTZ | Registrar o momento exato em que `sp_anonimizar_usuario` executou a substituição dos dados pessoais; gravado pela procedure no mesmo `UPDATE` (LGPD) |
| ml_id | SERIAL UNIQUE | Prover um identificador inteiro sequencial para mapear o UUID do usuário ao índice do modelo de fatoração de matriz (SVD), que requer inteiros contíguos |
| data_atualizacao | TIMESTAMPTZ | Registrar o momento da última modificação do perfil; mantido automaticamente pelo trigger `trg_usuarios_update` a cada `UPDATE` na tabela |

#### recomendacoes

| Coluna | Tipo | Por que foi necessária |
|---|---|---|
| visualizada_em | TIMESTAMPTZ | Registrar quando o usuário visualizou a recomendação, permitindo ao motor desconsiderar recomendações já exibidas e medir o tempo entre geração e exibição; preenchido pelo backend |

#### wishlist_curtidas

| Coluna | Tipo | Por que foi necessária |
|---|---|---|
| adicionado_em | TIMESTAMPTZ | Registrar quando o usuário adicionou o item à wishlist ou curtiu, habilitando ordenação por data de adição e análise de comportamento temporal; preenchido automaticamente pelo banco via `DEFAULT CURRENT_TIMESTAMP` |

#### interacoes

A coluna `data_expiracao` já existia em `01_create_database.sql`, mas sem nenhuma restrição de integridade. A Sprint 4 adicionou a constraint `chk_data_expiracao CHECK(data_expiracao > data_registro)`, formalizando no banco a regra de negócio de que a data de expiração deve ser sempre posterior ao momento do registro da interação.

---

### 10.3 Mudança de tipo de dado

Como parte da Sprint 4, colunas de data/hora que usavam `TIMESTAMP` (sem fuso) foram corrigidas para `TIMESTAMPTZ` no script base (`01_create_database.sql`). O banco roda na Oracle Cloud Infrastructure (UTC); colunas `TIMESTAMP` armazenam instantes sem informação de fuso, gerando ambiguidade na interpretação de horários entre clientes em timezones distintos. `TIMESTAMPTZ` armazena o instante em UTC e permite conversão correta no momento da leitura.

| Coluna | Tabela | Situação após Sprint 4 |
|---|---|---|
| data_cadastro | usuarios | TIMESTAMPTZ (corrigido em `01_create_database.sql`) |
| data_registro | interacoes | TIMESTAMPTZ (corrigido em `01_create_database.sql`) |
| data_expiracao | interacoes | TIMESTAMPTZ (corrigido em `01_create_database.sql`) |
| data_atualizacao | preferencias | TIMESTAMPTZ (corrigido em `01_create_database.sql`) |
| data_geracao | recomendacoes | TIMESTAMPTZ (corrigido em `01_create_database.sql`) |
| data_consentimento | usuarios | TIMESTAMPTZ (novo — adicionado em `02_sprint4_improvements.sql`) |
| deleted_at | usuarios | TIMESTAMPTZ (novo — adicionado em `02_sprint4_improvements.sql`) |
| data_inicio | espetaculos | TIMESTAMPTZ (corrigido em `01_create_database.sql` e migrado em `02_sprint4_improvements.sql`) |
| data_fim | espetaculos | TIMESTAMPTZ (corrigido em `01_create_database.sql` e migrado em `02_sprint4_improvements.sql`) |
| anonimizado_em | usuarios | TIMESTAMPTZ (novo — adicionado em `02_sprint4_improvements.sql`) |
| visualizada_em | recomendacoes | TIMESTAMPTZ (novo — adicionado em `02_sprint4_improvements.sql`) |
| data_atualizacao | usuarios | TIMESTAMPTZ (novo — adicionado em `02_sprint4_improvements.sql`) |

---

### 10.4 Nova constraint

A Sprint 4 adicionou três constraints ausentes no script original:

| Constraint | Tabela | Definição | Motivação |
|---|---|---|---|
| `uq_recomendacao_usuario_espetaculo` | recomendacoes | `UNIQUE(usuario_id, espetaculo_id)` | Impede que o motor de recomendação insira registros duplicados para o mesmo par usuário/espetáculo, o que inflaria artificialmente o ranking; a constraint habilita o padrão `INSERT ... ON CONFLICT DO UPDATE` para atualização atômica de score |
| `chk_pontuacao` | interacoes | `CHECK(pontuacao >= 0 AND pontuacao <= 10)` | A coluna `pontuacao` existia sem limite superior definido no banco; a constraint formaliza a escala 0–10 das regras de negócio, impedindo valores inválidos de qualquer origem |
| `chk_data_expiracao` | interacoes | `CHECK(data_expiracao > data_registro)` | Garante que a data de expiração seja sempre posterior ao momento do registro, evitando interações que expirem instantaneamente ou no passado |

---

### 10.5 Índices substituídos

Dois índices simples do Sprint 3 foram removidos e substituídos por índices compostos que cobrem os padrões de busca reais usados pelos módulos de recomendação.

| Índice removido | Índice criado | Ganho |
|---|---|---|
| `idx_interacoes_usuario` — `ON interacoes(usuario_id)` | `idx_interacoes_usuario_expiracao` — `ON interacoes(usuario_id, data_expiracao)` | O motor de recomendação filtra sempre por `usuario_id` **e** `data_expiracao`; o índice composto elimina a varredura extra na coluna de expiração e satisfaz ambos os predicados em um único Index Scan |
| `idx_recomendacao_usuario` — `ON recomendacoes(usuario_id)` | `idx_recomendacoes_usuario_score` — `ON recomendacoes(usuario_id, score DESC)` | Recuperar as top-N recomendações de um usuário ordenadas por relevância passa a ser um Index Scan sem Sort Step adicional, pois o índice já entrega as linhas na ordem decrescente de `score` |

Novos índices sem predecessor no Sprint 3:

| Índice | Tabela | Colunas | Finalidade |
|---|---|---|---|
| `idx_teatros_cidade` | teatros | (cidade) | Filtragem do carrossel "Perto de você" pela cidade do teatro |
| `idx_espetaculos_datas` | espetaculos | (data_inicio, data_fim) | Filtragem de espetáculos em cartaz por intervalo de datas |
| `idx_wishlist_usuario` | wishlist_curtidas | (usuario_id) | Carregamento da aba de wishlist/curtidas por usuário |
| `idx_usuarios_ml_id` | usuarios | (ml_id) UNIQUE | Lookup bidirecional UUID ↔ índice inteiro para o modelo ML; unicidade garante a bijeção necessária |

---

### 10.6 Objetos de banco — motivação

#### fn_atualizar_data_preferencias / trg_preferencias_update

- **Problema que resolve:** a coluna `preferencias.data_atualizacao` precisaria ser atualizada manualmente por cada camada da aplicação que executasse um `UPDATE` nessa tabela, tornando a manutenção do timestamp sujeita a esquecimento ou inconsistência.
- **Por que implementado no banco e não na aplicação:** o trigger `BEFORE UPDATE` garante que qualquer cliente — API, script administrativo ou acesso direto via psql — atualize `data_atualizacao` automaticamente, sem depender de lógica de aplicação.

#### fn_atualizar_data_usuario / trg_usuarios_update

- **Problema que resolve:** toda modificação no perfil do usuário — nome, endereço, e-mail — não deixava rastro de quando ocorreu. Sem esse timestamp, não é possível saber a data da última alteração nem ordenar históricos de mudança.
- **Por que implementado no banco e não na aplicação:** o trigger `BEFORE UPDATE` garante que `data_atualizacao` seja sempre reescrito com o momento correto, independentemente da camada que disparou o `UPDATE` — API, script administrativo ou acesso direto via psql. A aplicação não precisa se preocupar em enviar esse campo.

#### fn_bloquear_update_anonimizado / trg_bloquear_update_anonimizado

- **Problema que resolve:** após a anonimização de um usuário, qualquer `UPDATE` posterior poderia inadvertidamente reintroduzir dados pessoais ou reverter o estado `anonimizado = TRUE`.
- **Por que implementado no banco e não na aplicação:** uma restrição implementada apenas na camada de aplicação pode ser contornada por acesso direto ao banco, scripts de migração ou ferramentas de administração. O trigger ao nível do banco bloqueia qualquer `UPDATE` independentemente da origem.
- **Princípio LGPD:** art. 18, IV — direito à eliminação. Uma vez que o titular exerceu o direito de exclusão e os dados foram anonimizados, eles não podem ser recuperados ou reconstituídos. O trigger torna essa garantia estrutural, não apenas procedimental.

#### sp_anonimizar_usuario

- **Problema que resolve:** ao receber uma solicitação de exclusão (LGPD art. 18, IV), os dados pessoais precisam ser substituídos por valores neutros de forma atômica, preservando o UUID e o histórico de interações para análise estatística agregada (art. 7, IV — tratamento para pesquisa).
- **Por que implementado no banco e não na aplicação:** a substituição de múltiplos campos sensíveis (nome, email, senha_hash, seis colunas de endereço) ocorre em uma única transação, eliminando janelas de inconsistência que existiriam se a aplicação fizesse os `UPDATE`s em passos separados. No mesmo `UPDATE`, a procedure registra `anonimizado_em = CURRENT_TIMESTAMP`, garantindo que o momento exato da anonimização seja gravado atomicamente junto à operação.
- **Princípio LGPD:** art. 5, XI (tratamento) e art. 18, IV (direito à eliminação/anonimização). A procedure exige `deleted_at IS NOT NULL AND anonimizado = FALSE` como guarda dupla, prevenindo dupla anonimização e garantindo que o soft-delete preceda a anonimização.

#### fn_sincronizar_data_consentimento / trg_sincronizar_data_consentimento

- **Problema que resolve:** `data_consentimento` poderia ficar `NULL` mesmo com `consentimento_aceito = TRUE` caso a aplicação omitisse o campo no `UPDATE` — quebra silenciosa de rastreabilidade LGPD sem nenhum erro visível.
- **Por que implementado no banco e não na aplicação:** o banco atua como rede de segurança: se a aplicação enviar o aceite sem o timestamp, o trigger preenche `data_consentimento = CURRENT_TIMESTAMP` automaticamente antes de persistir. Se a aplicação enviar um valor explícito, o trigger respeita e não sobrescreve.
- **Princípio LGPD:** art. 8, §5 — o consentimento deve ser registrado de forma que permita demonstrar que foi obtido. Garantir que `data_consentimento` nunca fique nulo quando `consentimento_aceito = TRUE` assegura essa demonstrabilidade independentemente da camada que executou o `UPDATE`.

---

### 10.7 Estado Sprint 3 → Sprint 4

| Tabela | Sprint 3 (`01_create_database.sql`) | Sprint 4 (adições via `02_sprint4_improvements.sql`) |
|---|---|---|
| usuarios | id, nome, email, senha_hash, idade, data_cadastro | + cep, logradouro, numero, complemento, bairro, cidade; + consentimento_aceito, data_consentimento, deleted_at, anonimizado, anonimizado_em; + ml_id; + data_atualizacao; + triggers `trg_bloquear_update_anonimizado`, `trg_usuarios_update`, `trg_sincronizar_data_consentimento` |
| teatros | id, nome, endereco, cidade, horario_funcionamento | + índice `idx_teatros_cidade` |
| espetaculos | id, nome, tipo_evento, descricao, url_bilheteria, data_inicio, data_fim, valor_ingresso | + TIMESTAMPTZ em data_inicio e data_fim; + índice `idx_espetaculos_datas` |
| interacoes | id, usuario_id, espetaculo_id, tipo, pontuacao, data_registro, data_expiracao; índice `idx_interacoes_usuario` | + constraints `chk_pontuacao`, `chk_data_expiracao`; `idx_interacoes_usuario` → `idx_interacoes_usuario_expiracao` |
| preferencias | id, usuario_id, cidade, tags, faixa_preco_min, faixa_preco_max, receber_notificacoes, data_atualizacao | + trigger `trg_preferencias_update` |
| wishlist_curtidas | usuario_id, espetaculo_id, tipo | + `adicionado_em`; + índice `idx_wishlist_usuario` |
| recomendacoes | id, usuario_id, espetaculo_id, score, motivo, data_geracao; índice `idx_recomendacao_usuario` | + `UNIQUE(usuario_id, espetaculo_id)`; + visualizada_em; `idx_recomendacao_usuario` → `idx_recomendacoes_usuario_score` |
| usuario_genero_favorito | usuario_id, genero_id | sem alterações |
| espetaculo_genero | espetaculo_id, genero_id | sem alterações |
| teatro_espetaculo | teatro_id, espetaculo_id | sem alterações |

---

### 10.9 Rastreabilidade das transações

#### Mecanismos existentes no schema

| Mecanismo | Onde | O que registra |
|---|---|---|
| `DEFAULT CURRENT_TIMESTAMP` | `usuarios.data_cadastro` | Momento da criação da conta; gravado no INSERT, nunca atualizado |
| `DEFAULT CURRENT_TIMESTAMP` | `interacoes.data_registro` | Momento do registro da interação; gravado no INSERT, nunca atualizado |
| `DEFAULT CURRENT_TIMESTAMP` | `preferencias.data_atualizacao` | Valor inicial na criação do registro de preferências |
| `trg_preferencias_update` → `fn_atualizar_data_preferencias()` | `preferencias.data_atualizacao` | Reescrito com `CURRENT_TIMESTAMP` a cada `UPDATE` na tabela `preferencias`, independentemente da origem da alteração |
| `DEFAULT CURRENT_TIMESTAMP` | `recomendacoes.data_geracao` | Momento em que o motor de recomendação gerou a linha; gravado no INSERT, nunca atualizado |
| Aplicação (explícito) | `interacoes.data_expiracao` | Janela temporal definida pela camada de aplicação; determina até quando a interação é considerada pelo motor de recomendação |
| Aplicação (explícito) + `trg_sincronizar_data_consentimento` | `usuarios.data_consentimento` | Gravado pela aplicação no momento do aceite; quando a aplicação omite o valor, o trigger preenche automaticamente com `CURRENT_TIMESTAMP` ao detectar que `consentimento_aceito` mudou de FALSE para TRUE |
| Aplicação (explícito) | `usuarios.deleted_at` | Momento do soft-delete; gravado pela aplicação; pré-requisito obrigatório para execução de `sp_anonimizar_usuario` |
| `sp_anonimizar_usuario` | `usuarios.anonimizado_em` | Gravado pela procedure no mesmo `UPDATE` que substitui os dados pessoais; confirma o momento exato da anonimização (LGPD) |
| Aplicação (explícito) | `recomendacoes.visualizada_em` | Gravado pelo backend quando a recomendação é exibida ao usuário; `NULL` enquanto não visualizada |
| `trg_usuarios_update` → `fn_atualizar_data_usuario()` | `usuarios.data_atualizacao` | Reescrito com `CURRENT_TIMESTAMP` a cada `UPDATE` no perfil do usuário, independentemente da origem da alteração |
| `DEFAULT CURRENT_TIMESTAMP` | `wishlist_curtidas.adicionado_em` | Gravado automaticamente no INSERT; registra quando o usuário adicionou o item à wishlist ou curtiu |

#### Consideração futura

- **`lgpd_audit` (tabela de auditoria)** — não implementada. Seria útil caso o sistema ganhe um painel de administração com operadores humanos disparando a anonimização manualmente. Enquanto `sp_anonimizar_usuario` for chamada exclusivamente pelo backend de forma automatizada, o log da aplicação já cobre a rastreabilidade de quem solicitou e quando.
