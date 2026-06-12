# Plano de Deploy — Sistema de Recomendação (Docker + Oracle Cloud)

> **Status:** executado em 11/06/2026 — artefatos criados e verificação local (§5.1) concluída; deploy na VM aguarda IP + `.env.deploy` + allowlist (ver `docs/Deploy_Recomendacao.md`).
> **Data:** 11/06/2026 (rev. 3) · **Branch de referência:** `fix/reorganizacao-modelo`
>
> **rev. 3 (11/06/2026):** verificação corrigida para `data_geracao` (a coluna `criado_em` não existe) e tornada ciente da D4; `.dockerignore` adicionado (§2.2); `HEALTHCHECK` com `--retries`/`--timeout` e `start_period` dimensionado por medição; D4 com **solução definida** (`ml_user_mapping`) **agendada para a próxima sprint** (fora deste deploy); notas sobre OOM do primeiro batch e sobre auth nos endpoints; `scp -C`.
> **rev. 2 (10/06/2026):** endpoint de healthcheck corrigido; build movido para a máquina local.

## 1. Contexto

O motor de recomendação híbrido (Funk SVD via ML.NET + afinidade de categoria) roda no host `src/inference-service` (.NET 9), que carrega os artefatos de `data/models/`, executa um batch a cada 24h e persiste na tabela `recomendacoes` do PostgreSQL. Hoje **nada disso está deployado**: só o Postgres roda em Docker numa VM Oracle Cloud (OCI, `VM.Standard.E2.1.Micro`, IP `137.131.172.248`). Não há Dockerfile para o serviço, nem CI/CD, nem script de deploy.

### Decisões tomadas

| Decisão | Escolha |
|---|---|
| Escopo | Apenas o sistema de recomendação (`src/inference-service`). Backend e frontend ficam de fora; o Postgres continua na VM atual. |
| Destino | Segunda VM OCI **x64** (acesso SSH com `chave-back.key`). IP ainda não conhecido → parametrizado em arquivo de config local (gitignored). |
| Deploy | Script manual (sem CI/CD por enquanto) + `docker compose` na VM. A imagem é **buildada localmente** (Windows, x64) e enviada via `docker save`/`scp`/`docker load` — a VM de 1 GB não compila nada. |
| Exposição | Nginx com HTTPS (certificado **self-signed**, sem domínio definido) na frente de `/api/metrics` e `/api/diagnostics`. O serviço não fica exposto diretamente. |
| Mapeamento D4 | Resolvido por uma **tabela nova `ml_user_mapping`** (ver §4). **Implementação na próxima sprint — não entra neste deploy.** |

### Fatos do código que moldam o plano

- `InferenceService.csproj` referencia `..\ml\AventuraML.csproj` → o build context do Docker precisa ser a **raiz do repo**.
- Connection string lida via `configuration.GetConnectionString("DefaultConnection")` (`src/inference-service/Data/RecommendationWriteRepository.cs:25`) → sobrescrevível pela env var padrão `ConnectionStrings__DefaultConnection`, sem mudança de código.
- Caminhos dos artefatos são relativos ao content root (`data/models/...`, `data/dataset/dataset.csv`) → montar `./data` em `/app/data`.
- `data/models/*` (15 MB) está no git, mas **`dataset.csv` (~7 MB) é gitignored** → o script de deploy envia o dataset via scp.
- ML.NET MatrixFactorization precisa de `libgomp1` no Linux; ScottPlot 5 (gráficos) precisa de `libfontconfig1` → instalar ambos na imagem runtime.
- O endpoint de health é `GET /api/diagnostics/health` (`src/inference-service/Controllers/DiagnosticsController.cs:23`) — **não existe GET em `/api/diagnostics`** (retornaria 404). Ele responde **503 enquanto os artefatos carregam** (o Kestrel sobe antes do `HybridModelLoader`, então o health é servido durante o carregamento) → o `HEALTHCHECK` precisa de `start_period`.
- `RecommendationBatchJob` roda o **primeiro batch imediatamente após o load** (não em T+24h) → o pico de memória/risco de OOM é no startup (ver §4).
- `chave-back.key` e `chave.key` **já estão no `.gitignore`** (edição ainda não commitada) e a chave nunca foi rastreada pelo git → faltam apenas as demais entradas de deploy (§2.9) e commitar.
- `cold_start_by_category.json` é gerado no treino mas **não é consumido pelo host** — vai junto no `data/models/*` sem custo, nenhum tratamento especial.
- `Program.cs:49` chama `UseHttpsRedirection()`; atrás do nginx (HTTP interno) vira no-op com warning no log ("Failed to determine the https port") — inofensivo, documentar no runbook para ninguém investigar à toa.
- Swagger só roda em `Development`; em produção, `ASPNETCORE_ENVIRONMENT=Production`.

## 2. Arquivos a criar/alterar

### 2.1 `src/inference-service/Dockerfile` (novo)

Multi-stage, build context = raiz do repo. O build roda **dentro do container SDK Linux** (Docker Desktop em modo Linux), gerando binários linux-x64 — "local/Windows" se refere apenas a onde o `docker` CLI roda, não ao alvo dos binários. O `.dockerignore` (§2.2) é o que mantém o contexto enxuto.

- **build:** `mcr.microsoft.com/dotnet/sdk:9.0` → copia `src/ml/` e `src/inference-service/`, `dotnet publish -c Release -o /out src/inference-service/InferenceService.csproj`.
- **runtime:** `mcr.microsoft.com/dotnet/aspnet:9.0` → `apt-get install -y libgomp1 libfontconfig1 curl`; usuário não-root; `WORKDIR /app`; `ASPNETCORE_URLS=http://+:8080`; healthcheck:

  ```dockerfile
  HEALTHCHECK --start-period=180s --interval=30s --timeout=5s --retries=5 \
    CMD curl -f http://localhost:8080/api/diagnostics/health || exit 1
  ```

  O endpoint responde **503 enquanto o `HybridModelLoader` carrega** e **200** quando pronto. O `start_period` é janela de carência (falhas nela não marcam `unhealthy`). **Atenção:** o compose puro **não reinicia** container `unhealthy`; se o load estourar `start_period` + `retries`×`interval`, o nginx (`depends_on: service_healthy`) nunca sobe. Por isso a verificação local (§5.1) **mede o tempo de load** e o `start_period` deve ser ~1,5–2× esse valor (180s é o palpite inicial).

### 2.2 `.dockerignore` (novo, raiz do repo)

O build context é a raiz do repo, mas o Dockerfile só dá `COPY` em `src/`. Sem `.dockerignore`, todo `docker build` empacota o repo inteiro (`node_modules/`, `**/bin`, `**/obj`, `.git`, `data/` ~22 MB, `assets/`) e envia ao daemon — build lento e risco de levar `bin/obj` de Windows para a imagem. O `data/` **não** vai na imagem: entra por bind mount em runtime (`./data:/app/data:ro`), então é excluído do contexto sem prejuízo.

```gitignore
**/bin/
**/obj/
node_modules/
.git/
.vs/
.vscode/
data/
deploy/
docs/
assets/
tests/
*.tar
*.key
.env*
README.md
```

### 2.3 `docker-compose.deploy.yml` (novo, raiz do repo)

Dois serviços em rede interna:

- **inference:** `image: aventura-recsys-inference:latest` **e** `build: { context: ., dockerfile: src/inference-service/Dockerfile }` — o `build` serve para a verificação local (`docker compose build`); na VM a imagem chega via `docker load` e o `up -d` (sem `--build`) usa a tag já carregada. `env_file: .env.deploy`; volume `./data:/app/data:ro`; `restart: unless-stopped`; `mem_limit: 600m` (VM de 1 GB; o primeiro batch roda logo no startup e é o pico de memória — ver §4); `expose: 8080` (sem `ports` — só o nginx alcança).
- **nginx:** `nginx:alpine`; `ports: 80:80, 443:443`; volumes `./deploy/nginx.conf:/etc/nginx/conf.d/default.conf:ro` e `./deploy/certs:/etc/nginx/certs:ro`; `depends_on: { inference: { condition: service_healthy } }` (só sobe com o healthcheck verde); `restart: unless-stopped`.

### 2.4 `deploy/nginx.conf` (novo)

- `server` 80 → redirect 301 para HTTPS.
- `server` 443 ssl com cert/key em `/etc/nginx/certs/`. Roteamento:
  - `location = /api/diagnostics/health` → `proxy_pass` **sem auth** (match exato; libera o smoke test e o monitoramento).
  - `location /api/` → `proxy_pass http://inference:8080;` com headers `X-Forwarded-*`, **protegido** por allowlist de IP (`allow <ip_do_time>; deny all;`). `/api/metrics` e `/api/diagnostics/*` não têm auth própria e o `/api/metrics` faz avaliação cara (≈200 usuários × 446 itens; cacheado por parâmetro, mas variar os params força recomputo → vetor barato de DoS numa VM de 1 GB), então não devem ficar abertos.
  - Demais paths → 404.
  - Se os IPs do time forem instáveis, trocar a allowlist por `auth_basic` + `.htpasswd` montado em `deploy/` (o `/health` continua aberto pelo `location =`).

### 2.5 `.env.deploy.example` (novo, raiz)

```env
# Confirme nome/porta/SSL com o Postgres real antes de preencher (ver §3):
# o appsettings.json usa Database=aventura; o .env.example do backend sugere porta 6543 + SSL require.
ConnectionStrings__DefaultConnection=Host=137.131.172.248;Port=5432;Database=<CONFIRMAR>;Username=postgres;Password=<senha>;SSL Mode=Prefer
ASPNETCORE_ENVIRONMENT=Production
```

O time copia para `.env.deploy` (gitignored), **confirma nome do banco/porta/SSL** (§3) e preenche a senha real.

### 2.6 `deploy/deploy.config.example` (novo)

```env
VM_HOST=<ip_da_vm>
VM_USER=ubuntu
SSH_KEY=chave-back.key
REMOTE_DIR=/home/ubuntu/aventura-recsys
```

Copiado para `deploy/deploy.config` (gitignored) quando o IP for conhecido.

### 2.7 `deploy/bootstrap-vm.sh` (novo)

Roda uma vez na VM (idempotente):

- Instala Docker Engine + plugin compose (repositório oficial Docker) se ausentes.
- Cria swapfile de **2 GB** se não existir (VM de 1 GB de RAM; margem para o pico de memória do primeiro batch — ver §4).
- Abre 80/443 no firewall local (iptables — padrão das imagens Ubuntu da OCI, que **não** usam ufw e têm regras restritivas em `/etc/iptables/rules.v4`).

### 2.8 `deploy/deploy.ps1` (novo — execução a partir de Windows)

1. Lê `deploy/deploy.config`; valida que `.env.deploy` e `chave-back.key` existem.
2. **Build local:** `docker build -f src/inference-service/Dockerfile -t aventura-recsys-inference:latest .` → `docker save -o inference-image.tar aventura-recsys-inference:latest` (arquivo temporário; **não** usar pipe `docker save | ssh` — o pipeline do PowerShell 5.1 corrompe dados binários).
3. `git archive HEAD` → tarball do repo (garante que só vai o que está commitado — compose, nginx.conf, scripts).
4. `scp -C` (compressão SSH no fio; com `-i chave-back.key`): `inference-image.tar`, tarball do repo, `data/dataset/dataset.csv`, `data/models/*` (cobre artefatos regenerados não commitados), `.env.deploy`. O `-C` reduz o tempo de transferência do tar (que é **descompactado** — uma imagem aspnet:9.0 + app costuma dar ~400–600 MB, não ~250) sem ferramenta extra nem pipe binário.
5. `ssh`: extrai no `REMOTE_DIR`, roda `bootstrap-vm.sh`, `docker load -i inference-image.tar` (e remove o tar), gera cert self-signed em `deploy/certs/` se ausente (`openssl req -x509 -nodes -days 365 -newkey rsa:2048`), `docker compose -f docker-compose.deploy.yml up -d` (**sem `--build`** — a VM nunca compila).
6. Smoke test: `curl -k https://$VM_HOST/api/diagnostics/health` (com retry por ~2 min — o healthcheck só fica verde após o carregamento dos artefatos) e exibe `docker compose logs --tail 30 inference`.

### 2.9 `.gitignore` (editar)

`chave.key` e `chave-back.key` já constam (edição local não commitada). Adicionar: `*.key` (cobre futuras chaves), `.env.deploy`, `deploy/deploy.config`, `deploy/certs/`, `inference-image.tar` — e **commitar junto com o restante do plano**. (O `.dockerignore` de §2.2 **é** versionado — não entra aqui.)

### 2.10 `docs/Deploy_Recomendacao.md` (novo — runbook)

Pré-requisitos manuais na OCI, como rodar o deploy, como ver logs, como forçar uma rodada do batch (restart do container), troubleshooting (conexão ao Postgres; healthcheck 503 durante carregamento; container `unhealthy` que não reinicia; **OOM/crash-loop do primeiro batch**; **`recomendacoes` vazia = pendência D4**, não bug; credencial/IP para `/api/metrics` e `/api/diagnostics`; warning inofensivo do `UseHttpsRedirection` no log) e como evoluir para Let's Encrypt quando houver domínio.

## 3. Pré-requisitos manuais (fora do script)

- Obter o IP público da nova VM e preencher `deploy/deploy.config`.
- Na OCI Console: liberar ingress **80/443 na Security List** da VM de inferência (único passo que não dá para automatizar por script).
- **Confirmar os parâmetros reais do banco** (nome, porta, SSL) no Postgres de `137.131.172.248` antes de preencher `.env.deploy` — há divergência no repo (`appsettings.json` usa `aventura`; o `.env.example` do backend sugere `6543` + `sslmode=require`). Teste a partir da nova VM:

  ```bash
  psql "host=137.131.172.248 port=5432 dbname=<CONFIRMAR> user=<CONFIRMAR> sslmode=prefer" -c "\conninfo"
  # ou listar os DBs reais na VM do banco:
  docker exec postgres_db psql -U <user> -l
  ```

- O Postgres da VM atual já aceita conexões externas (porta 5432 aberta, backend conecta hoje) — nenhuma mudança esperada; o runbook inclui um teste de conexão a partir da nova VM.

## 4. Riscos e ressalvas

- **Pendência D4 (GATE-B) — solução definida, fora do escopo deste deploy:** o mapeamento `user_id` do dataset ↔ `usuarios.id` (UUID) será resolvido por uma **tabela nova `ml_user_mapping`** (`dataset_user_id INT PK` ↔ `usuario_id UUID FK`, com `origem` para rastreabilidade), mais um ajuste **retrocompatível** no `RecommendationWriteRepository.ResolveUsuarioIdAsync` (consultar a tabela antes do fallback de coluna atual). Escolheu-se a tabela dedicada (em vez de uma coluna em `usuarios`) para não poluir a tabela de usuários reais, não fabricar linhas-fantasma e facilitar LGPD. **A implementação fica para a próxima sprint — não entra neste plano.** Enquanto isso, como hoje **não existe coluna/tabela de mapeamento**, o `SaveAsync` resolve `usuario_id` como null e **sai sem inserir** (loga "Não foi possível persistir…"): este deploy sobe o serviço e roda o batch, mas `recomendacoes` tende a ficar **vazia** até a D4 ser implementada. O deploy não resolve nem piora isso.
- **Primeiro batch no startup (pico de memória):** o `RecommendationBatchJob` roda o primeiro batch **logo após o load**, não em T+24h. O pico de memória (e o risco de OOM sob `mem_limit: 600m`) acontece no startup, concorrendo com o nginx subindo. Se o batch der OOM, o container morre → `restart: unless-stopped` recarrega → batch → OOM = **crash loop** (assinatura: container reiniciando em ciclos, log cortando no meio do batch; checar `docker compose logs inference --tail 50` e `dmesg | grep -i oom`). Mitigação: swap de 2 GB (§2.7). Lado bom: OOM aparece na hora, não em 24h.
- **VM de 1 GB:** o build foi movido para a máquina local justamente para a VM nunca compilar (um `dotnet publish` com restore de ML.NET numa E2.1.Micro é cenário clássico de OOM). Em **runtime**, `mem_limit` + swap mitigam; se o batch ainda sofrer OOM, o próximo passo é trocar para shape A1 Flex (exigiria validar ML.NET em arm64, pois o trainer de Matrix Factorization usa binário nativo — a imagem também passaria a ser buildada para arm64).
- **Transferência da imagem (~400–600 MB de tar):** a cada deploy o `inference-image.tar` sobe inteiro via `scp -C`. Aceitável para deploy manual esporádico; se virar rotina, o upgrade natural é um registry (GitHub Container Registry) — fica documentado no runbook.
- **Cert self-signed:** clientes precisam de `-k`/exceção no navegador; caminho de upgrade para sslip.io + Let's Encrypt fica documentado.

## 5. Verificação

1. **Local (antes do deploy):** `docker compose -f docker-compose.deploy.yml up --build` apontando para o Postgres da OCI via `.env.deploy`; conferir no log o carregamento dos artefatos (`HybridModelLoader` é fail-fast) e o início do batch; **anotar o tempo entre "Carregando modelo…" e "Modelo híbrido carregado"** para dimensionar o `start_period` (§2.1); aguardar o container ficar `healthy` (`docker compose ps`); `curl -k https://localhost/api/diagnostics/health` e `/api/metrics` via nginx (com credencial/IP conforme §2.4).
2. **Na VM (quando o IP existir):** rodar `deploy/deploy.ps1`; smoke test automático do script; `docker compose ps` mostrando healthcheck `healthy`; reboot da VM para validar `restart: unless-stopped`.
3. **Persistência (ciente da D4):** `recomendacoes` só recebe linhas **depois** que a D4 estiver implementada (§4). Para distinguir "vazio esperado" de "falha real":
   - Tabela/coluna de mapeamento existe?

     ```sql
     SELECT to_regclass('public.ml_user_mapping') AS tabela_mapeamento;   -- null até a próxima sprint
     SELECT column_name FROM information_schema.columns
     WHERE table_name = 'usuarios'
       AND column_name IN ('ml_id','user_id','dataset_user_id','external_user_id','legacy_user_id')
       AND data_type IN ('integer','bigint');                              -- vazio = D4 não resolvida
     ```

   - Log do batch: `docker compose -f docker-compose.deploy.yml logs inference | Select-String "Não foi possível persistir"` → se aparece, o `count` 0 é a D4, **não** erro de deploy.
   - Contagem (esperada **0** até a D4; > 0 só depois):

     ```sql
     SELECT count(*), max(data_geracao) FROM recomendacoes;
     ```
