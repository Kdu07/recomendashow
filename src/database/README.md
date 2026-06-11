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
6. [Explain Analyze](#explain-analyze)

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

## 6. Explain Analyze

Os testes de plano de execução foram realizados no banco real do projeto, utilizando dados existentes no ambiente PostgreSQL. O objetivo foi avaliar o comportamento do otimizador diante das consultas mais relevantes para o uso da aplicação.

## Volume encontrado

| Tabela | Registros |
|---|---:|
| usuarios | 17 |
| teatros | 7 |
| espetaculos | 7 |
| interacoes | 2 |
| recomendacoes | 1 |
| wishlist_curtidas | 6 |

## Índices relevantes existentes

- `idx_wishlist_usuario`
- `idx_teatros_cidade`
- `idx_espetaculos_datas`
- `idx_interacoes_usuario_expiracao`
- `idx_recomendacoes_usuario_score`

## Consulta 1 - Wishlist por usuário

```sql
SELECT *
FROM wishlist_curtidas
WHERE usuario_id = '606f6cae-1d85-46b0-aa6f-dc5fabad9037';
```

Resultado do `EXPLAIN ANALYZE`:

- `Seq Scan`
- `Rows Removed by Filter: 4`
- `Buffers: shared hit=1`
- `Execution Time: 0.021 ms`

Apesar da existência do índice `idx_wishlist_usuario`, o PostgreSQL optou por `Seq Scan` devido ao pequeno volume da tabela, composta por 6 registros.

## Consulta 2 - Espetáculos por cidade

```sql
SELECT e.*
FROM teatros t
JOIN teatro_espetaculo te ON te.teatro_id = t.id
JOIN espetaculos e ON e.id = te.espetaculo_id
WHERE t.cidade = 'São Paulo';
```

Resultado do `EXPLAIN ANALYZE`:

- `Nested Loop`
- `Seq Scan` em `teatros`
- `Seq Scan` em `espetaculos`
- `Index Only Scan` usando `teatro_espetaculo_pkey`
- `Execution Time: 0.115 ms`

O PostgreSQL utilizou o índice da tabela associativa, por meio de `Index Only Scan` sobre `teatro_espetaculo_pkey`. Entretanto, optou por `Seq Scan` nas tabelas `teatros` e `espetaculos` devido ao baixo número de registros.

## Consulta 3 - Espetáculos por intervalo de datas

```sql
SELECT *
FROM espetaculos
WHERE data_inicio >= CURRENT_DATE
  AND data_fim <= CURRENT_DATE + INTERVAL '90 days';
```

Resultado do `EXPLAIN ANALYZE`:

- `Seq Scan`
- `Rows Removed by Filter: 5`
- `Buffers: shared hit=1`
- `Execution Time: 0.020 ms`

Apesar da existência do índice `idx_espetaculos_datas`, o PostgreSQL escolheu `Seq Scan` devido ao pequeno tamanho da tabela `espetaculos`.

## Conclusão

Os índices avaliados estão corretamente criados e disponíveis no banco de dados. Contudo, nos testes realizados, o otimizador do PostgreSQL escolheu leituras sequenciais em parte das consultas porque as tabelas possuem poucos registros. Em ambientes com maior volume de dados, espera-se maior utilização dos índices, especialmente nas consultas filtradas por usuário, cidade e intervalo de datas.
