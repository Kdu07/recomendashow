# Plano — Resolução das pendências importantes de testes

> **Branch:** `feature/testes-RF-RNF` · **Data:** 12/06/2026
> Consolida as pendências deixadas pela execução do [Plano_Testes_RF_RNF.md](./Plano_Testes_RF_RNF.md) (registradas na seção 20 do [Projeto.md](./Projeto.md)), ordenadas por prioridade. Complementa — sem duplicar — o [Plano_CI_Minimo.md](./Plano_CI_Minimo.md).

## Pendências e prioridades

| # | Pendência | Por que importa | Prioridade |
|---|---|---|---|
| P1 | 16 testes Jest falhando (4 suítes: `__DEV__ is not defined` + asserções do search) | Suíte vermelha invalida o "todas as suítes verdes" do critério de saída; bloqueia o gate de CI | **Alta** |
| P2 | Cobertura .NET nunca medida (coverlet ausente dos `.csproj` de teste) | RNF08 exige ≥ 70% nos módulos críticos — hoje não há número | **Alta** |
| P3 | k6 pleno não rodado (só smoke): cenários 100 VUs / 150 stress / spike | RNF06 sem medição real; seção 20.6 está com limitação | **Alta** |
| P4 | `/health` não publicado na VM + sem janela de uptime | RNF05 sem medição; monitor depende do deploy (ou da alternativa zero-deploy) | Média |
| P5 | CI mínimo com gate de cobertura | Cenário negativo 2 do RNF08; plano pronto em [Plano_CI_Minimo.md](./Plano_CI_Minimo.md) | Média |
| P6 | Fechamento da branch: commit do `.gitignore` + planos, linha no [Index.md](./Index.md), MR | Sem isso o trabalho não chega em `develop` | **Alta** (rápida) |

**Fora de escopo (decisão registrada em 12/06/2026):** rate limiting no login (RNF02 — lacuna documentada no 20.6/20.9), evidências manuais dos TFs, push/MR feitos pelo agente.

---

## Fase 1 — Suítes Jest verdes + cobertura .NET (P1, P2) — ½ dia

Idêntica à Fase 1 do [Plano_CI_Minimo.md](./Plano_CI_Minimo.md); executá-la aqui fecha as duas.

1. **`__DEV__ is not defined`** (derruba `Onboardingservice.test.js` e mais 2 suítes): adicionar ao bloco `jest` do `package.json`:
   ```json
   "globals": { "__DEV__": true }
   ```
2. **`Usesearchviewmodel.test.js` (16 falhas de asserção)**: depurar se é teste desatualizado ou bug real no `useSearchViewModel` (filtro por gênero+cidade retornando vazio). Se for bug real → issue referenciando TF-05/RF05 e corrigir.
3. **coverlet**: `dotnet add package coverlet.msbuild` em `BackEnd.Aventura.Tests` e `InferenceService.Tests`.
4. Medir e registrar:
   ```powershell
   dotnet test src/backend/BackEnd.Aventura.Tests /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
   dotnet test src/inference-service/InferenceService.Tests /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
   cd src/frontend/mobile-app-aventura; npm test -- --coverage
   ```
5. Atualizar **20.5** (nova contagem verde) e **20.6/RNF08** (percentual real vs. meta 70%) no Projeto.md; salvar relatórios em `assets/testes/dotnet-coverage/`.

**Saída:** 8/8 suítes Jest verdes; número de cobertura .NET registrado.

## Fase 2 — Carga plena local (P3) — ½ dia

Pré-requisito: stack local de pé (sem depender da rede para pull de imagens):

```powershell
docker compose -f docker-compose-db.yml up -d db          # só o postgres (mongo não é necessário)
$env:POSTGRES_HOST = "localhost"
$env:ASPNETCORE_URLS = "http://localhost:5253"
dotnet run --project src/backend/BackEnd.Aventura.csproj
```

1. Rodar os 3 cenários de 17.4 contra `http://localhost:5253` com `tests/load/k6-pico.js`:
   - `SCENARIO=pico` (0→100 VUs, 15 min — metas: média ≤ 3s, p95 ≤ 5s, erro < 1%)
   - `SCENARIO=stress` (150 VUs, 10 min — degradação elegante)
   - `SCENARIO=spike` (0→100 em 30s — recuperação ≤ 60s)
2. Salvar saídas em `assets/testes/k6-pico-{pico,stress,spike}.txt` (`Tee-Object`).
3. Atualizar **20.6/RNF06**: métricas locais + ressalva de que números absolutos não representam a VM de produção; rampa 1.000 VUs contra a VM permanece condicionada à autorização do time.

**Saída:** 3 cenários executados com evidência; 20.6 sem a limitação "não rodado".

## Fase 3 — RNF05: `/health` na VM + janela de uptime (P4) — ½ dia + 24h de espera

⚠️ **Reinicia o backend de produção — alinhar com o time antes.**

1. Descobrir o layout da VM: `ssh -i chave-back.key ubuntu@163.176.204.208 "find ~ -name docker-compose-back.yml; docker ps"`.
2. Publicar o backend com o `/health` (commitado em `1ae26d0`): `git pull` + rebuild na VM (`docker compose -f src/backend/docker-compose-back.yml up -d --build` a partir da raiz do repo na VM, ou o mecanismo que estiver em uso).
3. Validar: `curl http://163.176.204.208:5253/health` → `Healthy`.
4. Monitor 24h (escolher 1):
   - **UptimeRobot** (grátis, web): monitor HTTP em `:5253/health`, intervalo 5 min.
   - **Zero-infra:** loop PowerShell/cron com GET a cada 1 min anexando em `assets/testes/rnf05-uptime-24h.json` (já existe sondagem inicial em `rnf05-sondagem-vm.json`).
   - *(Fallback se o deploy não for autorizado: sondar `api/Espetaculo` tratando 401 como "no ar".)*
5. Após 24h: calcular % de disponibilidade e atualizar **20.6/RNF05**, extrapolando e registrando a limitação da janela vs. requisito de 30 dias.

**Saída:** endpoint público respondendo + relatório de 24h no 20.6.

## Fase 4 — CI mínimo (P5) — ½ dia

Executar as Fases 2–3 do [Plano_CI_Minimo.md](./Plano_CI_Minimo.md) (o pré-requisito — Fase 1 de lá — já terá sido feito na Fase 1 daqui): criar `.gitlab-ci.yml` com `backend-tests` (gate coverlet 70%) e `frontend-tests` (gate `coverageThreshold` 70%), validar pipeline no GitLab e atualizar **20.6/RNF08**.

## Fase 5 — Fechamento (P6) — 1h

1. Adicionar [Plano_CI_Minimo.md](./Plano_CI_Minimo.md) e este documento ao [Index.md](./Index.md).
2. Atualizar **20.9** removendo as pendências resolvidas das limitações.
3. Commits temáticos (gitignore+planos / jest+coverlet / k6 / CI) e `git push -u origin feature/testes-RF-RNF` (push feito pelo time).
4. Abrir MR `feature/testes-RF-RNF → develop` no GitLab.

---

## Ordem de execução recomendada

```
Fase 1 (Jest + coverlet)  ──►  Fase 4 (CI)        ─┐
Fase 2 (k6 local)                                   ├──►  Fase 5 (fechamento + MR)
Fase 3 (deploy /health)   ──►  [24h de medição]   ─┘
```

A Fase 3 deve começar cedo (a janela de 24h corre sozinha). Fases 1 e 2 são independentes entre si. O MR **não precisa esperar a Fase 3/4**: se o prazo apertar, fechar o MR após Fases 1–2 e tratar RNF05/CI em MR seguinte.

## Critérios de saída

- [ ] 8/8 suítes Jest verdes (`npm test` limpo).
- [ ] Cobertura .NET medida e registrada no 20.6 (RNF08), relatórios versionados.
- [ ] k6 pico/stress/spike executados com evidências em `assets/testes/`.
- [ ] `/health` respondendo na VM + relatório de uptime 24h **ou** limitação justificada (deploy não autorizado).
- [ ] `.gitlab-ci.yml` com gate de 70% (ou explicitamente movido para MR seguinte).
- [ ] Index.md atualizado; MR aberto para `develop`.

## Riscos

| Risco | Mitigação |
|---|---|
| Falhas do `Usesearchviewmodel` serem bug real no código | Tratar como issue TF-05/RF05; corrigir código, não afrouxar o teste |
| Cobertura .NET < 70% | Gate inicial em (valor real − 5 p.p.) com plano de subida; registrar no 20.6 |
| Deploy na VM sem janela acordada com o time | Fase 3 só inicia com OK explícito; fallback zero-deploy descrito |
| Rede instável para pulls Docker/NuGet | Stack local usa só imagens já presentes (`g02-db`); backend via `dotnet run` |
| Prazo da sprint | Fases 1–2 + 5 são o núcleo; 3–4 podem ir em MR seguinte |
