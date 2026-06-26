# Manual de Implantação — RecomendaShow

Este documento descreve como implantar o sistema RecomendaShow em um servidor Linux com Docker. É voltado para a equipe técnica da Aventura Teatros ou para qualquer pessoa responsável por colocar a solução em produção.

Para o procedimento técnico detalhado do motor de recomendação (inclui diagrama, troubleshooting e operação diária), consulte [`Deploy_Recomendacao.md`](./Deploy_Recomendacao.md).

## Sumário

1. [Visão geral do sistema](#1-visão-geral-do-sistema)
2. [Pré-requisitos](#2-pré-requisitos)
3. [Infraestrutura necessária](#3-infraestrutura-necessária)
4. [Configuração inicial (uma vez)](#4-configuração-inicial-uma-vez)
5. [Subindo o banco de dados](#5-subindo-o-banco-de-dados)
6. [Subindo o backend](#6-subindo-o-backend)
7. [Subindo o motor de recomendação](#7-subindo-o-motor-de-recomendação)
8. [Verificando se tudo está funcionando](#8-verificando-se-tudo-está-funcionando)
9. [Atualizando o sistema](#9-atualizando-o-sistema)
10. [Suporte e contato](#10-suporte-e-contato)

---

## 1. Visão geral do sistema

O RecomendaShow é composto por quatro componentes que precisam estar em execução para o app funcionar:

| Componente | O que faz | Tecnologia |
|---|---|---|
| **Banco de dados** | Armazena usuários, espetáculos, wishlist e recomendações | PostgreSQL 16 + MongoDB |
| **Backend** | API REST consumida pelo aplicativo mobile | ASP.NET Core .NET 9 |
| **Motor de recomendação** | Calcula recomendações personalizadas a cada 24h | ASP.NET Core .NET 9 + ML.NET |
| **Aplicativo mobile** | Interface do usuário (iOS/Android) | React Native (Expo) |

O aplicativo mobile é distribuído via Expo ou stores (App Store/Google Play) e não precisa de deploy de servidor — os outros três componentes, sim.

---

## 2. Pré-requisitos

Na máquina que executará o deploy (pode ser um notebook Windows ou um servidor Linux):

- **Docker Desktop** (Windows/Mac) ou **Docker Engine + Docker Compose** (Linux)
- **.NET SDK 9.0** — para o passo de build (não necessário na VM de produção se usar imagem pré-compilada)
- **OpenSSH** — para transferir arquivos para a VM remota (já incluso no Windows 10/11)
- Acesso à VM de produção por SSH

---

## 3. Infraestrutura necessária

O sistema foi validado na seguinte configuração mínima:

| Recurso | Mínimo recomendado |
|---|---|
| CPU | 1 vCPU (x64) |
| RAM | 2 GB (1 GB funciona com swap de 2 GB configurado) |
| Disco | 20 GB |
| Sistema operacional | Ubuntu 22.04 LTS |
| Portas abertas | TCP 80, 443 (nginx), TCP 5432 (PostgreSQL, apenas internamente) |

A Aventura Teatros pode usar qualquer provedor de nuvem: Oracle Cloud (OCI), AWS, Azure, Google Cloud ou um servidor próprio. O sistema está testado na OCI.

---

## 4. Configuração inicial (uma vez)

### 4.1 Clone o repositório

```bash
git clone <url-do-repositorio-gitlab>
cd aventura-recsys
```

### 4.2 Crie os arquivos de configuração

Copie os templates e preencha com os valores reais:

```bash
# Configurações de deploy para a VM
cp deploy/deploy.config.example deploy/deploy.config

# Variáveis de ambiente do serviço de recomendação
cp .env.deploy.example .env.deploy
```

**`deploy/deploy.config`** — preencha:

```
VM_HOST=<IP-da-VM>         # ex: 163.176.216.177
VM_USER=ubuntu             # usuário SSH da VM
SSH_KEY=chave-back.key     # caminho para a chave SSH (relativo à raiz do repo)
REMOTE_DIR=/home/ubuntu/aventura-recsys
```

**`.env.deploy`** — preencha:

```
ConnectionStrings__DefaultConnection=Host=<IP-do-banco>;Port=5432;Database=aventura_db;Username=postgres;Password=<senha-real>;SSL Mode=Prefer
ASPNETCORE_ENVIRONMENT=Production
```

**`src/backend/.env`** — preencha (para o backend):

```
ConnectionStrings__DefaultConnection=Host=<IP-do-banco>;Port=5432;Database=aventura_db;Username=postgres;Password=<senha-real>;SSL Mode=Prefer
JWT_KEY=<chave-secreta-jwt-minimo-32-caracteres>
ASPNETCORE_ENVIRONMENT=Production
MONGO_CONNECTION_STRING=mongodb://<usuario>:<senha>@<host>:27017
```

### 4.3 Adicione a chave SSH

Coloque o arquivo `chave-back.key` (fornecido pela equipe) na raiz do repositório. **Nunca commite este arquivo** — ele está no `.gitignore`.

---

## 5. Subindo o banco de dados

O banco deve estar em execução antes do backend e do motor de recomendação.

### Em ambiente local (desenvolvimento)

```bash
docker compose -f docker-compose-db.yml up -d
```

Isso sobe PostgreSQL (porta 5432) e MongoDB (porta 27017) e aplica automaticamente todos os scripts SQL de criação e seed.

### Em produção (VM dedicada)

Se o banco já está em execução na VM da Aventura Teatros (`137.131.172.248`), não é necessário fazer nada — o backend e o motor já apontam para esse endereço via variável de ambiente.

Para recriar o banco do zero (atenção: apaga todos os dados):

```bash
docker compose -f docker-compose-db.yml down -v   # apaga volumes
docker compose -f docker-compose-db.yml up -d      # recria e aplica scripts
```

---

## 6. Subindo o backend

### Em ambiente local

```bash
cd src/backend
cp .env.example .env    # edite com suas credenciais
dotnet run
```

A API estará disponível em `http://localhost:5253`. O Swagger (documentação interativa) fica em `http://localhost:5253/swagger`.

### Em produção (Docker)

```bash
# Na raiz do repositório:
docker compose -f docker-compose-back.yml up -d
```

O backend escuta na porta 5253 internamente. Configure um proxy reverso (nginx) para expor na porta 80/443 se necessário.

---

## 7. Subindo o motor de recomendação

O motor de recomendação precisa de artefatos de modelo treinados antes de subir. Se for o primeiro deploy:

### 7.1 Gere os artefatos de modelo (uma vez, ou após novo dataset)

```bash
cd src/ml
dotnet run preprocess   # processa o dataset bruto → data/dataset/dataset.csv
dotnet run train        # treina o modelo → data/models/ (model.zip + 4 JSONs)
```

### 7.2 Execute o deploy na VM

```powershell
powershell -File deploy\deploy.ps1
```

O script faz automaticamente:
1. Build da imagem Docker do inference-service (linux/amd64)
2. Transfere a imagem, os artefatos de modelo e o dataset para a VM por SSH/SCP
3. Sobe os containers (inference-service + nginx) na VM
4. Aguarda o health check e exibe os logs

**Tempo estimado:** 5–15 minutos dependendo da velocidade da internet (a imagem tem ~300 MB).

### 7.3 Liberação de portas na OCI (se usar Oracle Cloud)

Se a VM está na Oracle Cloud, é preciso liberar as portas 80 e 443 na Security List:

1. Acesse o console da OCI → Networking → Virtual Cloud Networks
2. Selecione a VCN da VM de inferência → Security Lists → Default Security List
3. Adicione regras de ingress:
   - TCP, Source: `0.0.0.0/0`, Destination Port: `80`
   - TCP, Source: `0.0.0.0/0`, Destination Port: `443`

---

## 8. Verificando se tudo está funcionando

### Backend

```bash
curl http://<IP-do-backend>:5253/api/diagnostics/health
# Esperado: HTTP 200
```

### Motor de recomendação (via nginx)

```bash
curl -k https://<IP-da-VM>/api/diagnostics/health
# Esperado: HTTP 200 com JSON contendo status do modelo
```

> O `-k` ignora o aviso de certificado self-signed. Em produção com domínio real, use Let's Encrypt e remova o `-k`.

### Banco de dados

```bash
psql "host=<IP-do-banco> port=5432 dbname=aventura_db user=postgres" -c "SELECT count(*) FROM usuarios;"
# Esperado: número de usuários cadastrados (mínimo 3 do seed)
```

---

## 9. Atualizando o sistema

### Atualizar o motor de recomendação (novo modelo)

```powershell
# Retreine o modelo localmente:
cd src/ml && dotnet run train

# Faça o deploy novamente:
powershell -File deploy\deploy.ps1
```

### Atualizar o backend

```bash
# Reconstrua e reinicie o container:
docker compose -f docker-compose-back.yml up -d --build
```

### Atualizar o banco de dados (nova migration)

Adicione o novo script SQL em `src/database/` com numeração sequencial e execute:

```bash
psql "host=<IP-do-banco> port=5432 dbname=aventura_db user=postgres" -f src/database/<novo-script>.sql
```

---

## 10. Suporte e contato

Para dúvidas técnicas sobre a solução, entre em contato com a equipe de desenvolvimento:

- Repositório GitLab: `<url-do-repositorio>`
- Documentação técnica completa: [`docs/Projeto.md`](./Projeto.md)
- Documentação de deploy detalhada: [`docs/Deploy_Recomendacao.md`](./Deploy_Recomendacao.md)
- Padrões de desenvolvimento: [`docs/GuiaPadroesDesenvolvimento.md`](./GuiaPadroesDesenvolvimento.md)
