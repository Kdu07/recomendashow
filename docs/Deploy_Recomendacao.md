# Runbook — Deploy do Sistema de Recomendação

> Companion de `Plano_Deploy_Recomendacao.md` (plano aprovado, rev. 3).
> Escopo: apenas o `src/inference-service` numa VM OCI x64, em Docker, atrás de nginx
> (HTTPS self-signed). O Postgres continua na VM atual (`137.131.172.248`).

## 1. Pré-requisitos manuais (uma vez)

1. **VM:** obter o IP público da nova VM OCI x64 e copiar
   `deploy/deploy.config.example` → `deploy/deploy.config` (gitignored), preenchendo `VM_HOST`.
2. **OCI Console:** liberar ingress **TCP 80 e 443 na Security List** da VM de inferência
   (único passo que o script não automatiza). O firewall *local* (iptables) é aberto pelo
   `deploy/bootstrap-vm.sh`.
3. **Chave SSH:** `chave-back.key` na raiz do repo (gitignored — nunca commitar).
4. **Banco — parâmetros confirmados (11/06/2026, via `.env` real do backend):**
   host `137.131.172.248`, porta `5432`, banco **`aventura_db`**, usuário `postgres`,
   servidor **sem SSL** (`DB_SSL=false`; o `6543 + sslmode=require` do `.env.example` do
   backend é resquício). Sanity check a partir da nova VM:

   ```bash
   psql "host=137.131.172.248 port=5432 dbname=aventura_db user=postgres sslmode=prefer" -c "\conninfo"
   ```

5. **Env do serviço:** copiar `.env.deploy.example` → `.env.deploy` (gitignored) e preencher
   connection string confirmada + senha real.
6. **nginx:** decisão do time (11/06/2026) — endpoints **sem autenticação** por enquanto
   (risco do `/api/metrics` aceito; ver Troubleshooting). Nada a configurar.
7. **Dataset:** `data/dataset/dataset.csv` existe localmente (é gitignored; se faltar:
   `cd src/ml && dotnet run preprocess`).
8. **Docker Desktop** em modo Linux na máquina de quem faz o deploy.

## 2. Como rodar o deploy

```powershell
powershell -File deploy\deploy.ps1
```

O script: builda a imagem localmente (linux-x64) → `docker save` → `scp -C` (imagem,
tarball do `git archive HEAD`, `dataset.csv`, `data/models/*`, `.env.deploy`) → na VM roda
`bootstrap-vm.sh` (Docker, swap 2 GB, iptables), `docker load`, gera cert self-signed se
ausente e `docker compose up -d` (**sem** `--build` — a VM de 1 GB nunca compila) → smoke
test com retry (~2 min) e mostra os últimos logs.

**Importante:** só vai para a VM o que está **commitado** (`git archive HEAD`) + os
arquivos enviados explicitamente (dataset, modelos, `.env.deploy`). Mudou compose/nginx/
script? Commit antes de deployar.

## 3. Operação

```bash
# Logs (na VM)
cd /home/ubuntu/aventura-recsys
sudo docker compose -f docker-compose.deploy.yml logs -f inference

# Status + healthcheck
sudo docker compose -f docker-compose.deploy.yml ps

# Forçar uma rodada do batch (ele roda no startup e a cada 24h)
sudo docker compose -f docker-compose.deploy.yml restart inference
```

- Health: `curl -k https://163.176.216.177/api/diagnostics/health`
- Métricas: `curl -k "https://163.176.216.177/api/metrics?k=10&rankingUsers=200"`

## 4. Troubleshooting

### Health retorna 503 logo após subir
**Normal durante o carregamento.** O Kestrel sobe antes do `HybridModelLoader`; o endpoint
responde 503 até os artefatos carregarem e 200 quando pronto. O `HEALTHCHECK` tem
`start_period=180s` de carência. Se o 503 persistir por vários minutos, ver logs — o loader
é fail-fast: artefato ausente derruba a aplicação (confira o bind mount `./data:/app/data`).

### Container `unhealthy` e nginx que não sobe
O compose puro **não reinicia** container `unhealthy`, e o nginx tem
`depends_on: service_healthy` — se o carregamento estourar `start_period` +
`retries`×`interval`, o nginx nunca sobe. **Medição local (11/06/2026, Docker Desktop):**
load completo em ~7 s (dataset 3 s + modelo 4 s), health 200 em < 5 s após o bind, ~190 MiB
no início do batch. O `start_period=180s` do Dockerfile é margem deliberada para a CPU
fraca da E2.1.Micro; se na VM o load passar de ~90 s, aumentar o `start_period` para
~2× o valor observado.

### Crash loop / OOM no primeiro batch
O `RecommendationBatchJob` roda o primeiro batch **imediatamente após o load** (não em
T+24h) — o pico de memória é no startup, sob `mem_limit: 600m`. Assinatura: container
reiniciando em ciclos, log cortando no meio do batch.

```bash
sudo docker compose -f docker-compose.deploy.yml logs --tail 50 inference
dmesg | grep -i oom
```

Mitigação já aplicada: swap de 2 GB (bootstrap). Se persistir: subir `mem_limit`
(a VM tem 1 GB — margem curta) ou migrar para shape A1 Flex (exige validar ML.NET em
arm64: o trainer de Matrix Factorization usa binário nativo; a imagem passaria a ser
buildada para arm64).

### Tabela `recomendacoes` vazia
**Pendência D4, não bug de deploy.** Hoje não existe coluna/tabela de mapeamento
`user_id` do dataset ↔ `usuarios.id` (UUID); o writer resolve `usuario_id` como null e sai
sem inserir, logando `"Não foi possível persistir…"`. A solução (`ml_user_mapping` +
ajuste no `ResolveUsuarioIdAsync`) está agendada para a próxima sprint. Diagnóstico:

```sql
SELECT to_regclass('public.ml_user_mapping') AS tabela_mapeamento;  -- null até a D4
SELECT count(*), max(data_geracao) FROM recomendacoes;              -- 0 esperado até a D4
```

```bash
sudo docker compose -f docker-compose.deploy.yml logs inference | grep "Não foi possível persistir"
# aparece -> count 0 é a D4, não erro de deploy
```

### Erro de conexão com o Postgres
Testar da VM de inferência: `psql "host=137.131.172.248 ..."` (ver §1.4). Causas comuns:
nome do banco errado (`aventura`?), porta (5432 × 6543), SSL, senha, ou egress/ingress
entre as duas VMs na OCI.

### Serviço lento / fora do ar após chamadas a `/api/metrics`
Decisão do time (11/06/2026): os endpoints estão **sem autenticação**. O `/api/metrics`
dispara avaliação cara (~200 usuários × 446 itens; o resultado é cacheado por parâmetro,
mas variar `k`/`rankingUsers` força recomputo) — numa VM de 1 GB, chamadas repetidas com
parâmetros variados podem saturar CPU/memória. Se isso virar problema, reintroduzir no
`location /api/` do `deploy/nginx.conf` uma allowlist de IP (`allow <ip>; deny all;`) ou
`auth_basic` + `.htpasswd`.

### Warning "Failed to determine the https port" no log
Inofensivo. `Program.cs` chama `UseHttpsRedirection()`, mas atrás do nginx o serviço só
fala HTTP interno — o middleware vira no-op com esse warning. Não investigar.

## 5. Evoluções documentadas (fora deste deploy)

- **HTTPS de verdade:** quando houver domínio (ou usando `sslip.io` sobre o IP), trocar o
  cert self-signed por Let's Encrypt (certbot ou proxy tipo Caddy). Clientes deixam de
  precisar de `-k`/exceção no navegador.
- **Registry:** se o deploy virar rotina, substituir `docker save`/`scp` (~400–600 MB por
  deploy) por um registry (ex.: GitHub Container Registry) e `docker pull` na VM.
- **D4 (`ml_user_mapping`):** próxima sprint — habilita a persistência real em
  `recomendacoes`.
