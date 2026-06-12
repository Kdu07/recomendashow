# Plano de Testes — Requisitos Funcionais e Não Funcionais

> **Branch:** `feature/testes-RF-RNF` · **Data:** 12/06/2026
> Este documento consolida o diagnóstico do que já existe de testes no projeto, o que falta, e o plano de execução e documentação para fechar a cobertura dos 10 RFs e 8 RNFs definidos no [Projeto.md](./Projeto.md) (seções 9 e 10). O resultado da execução deve ser registrado na **seção 20 do Projeto.md**, que hoje está parcialmente preenchida com placeholders.

---

## 1. Diagnóstico — o que já existe

### 1.1 Planejamento (seção 17 do Projeto.md) — ✅ completo

| Frente | Casos planejados | Status do planejamento |
|---|---|---|
| Funcionais | TF-01 a TF-10 (1 por RF, com cenários positivos e negativos) | Completo |
| Não funcionais | RNF01 a RNF08, com cenários positivos e negativos | Completo |
| Usabilidade | TU-01 a TU-08 + questionário SUS | Completo |
| APIs externas | ViaCEP (3 casos: válido, inválido, resiliência) | Completo |
| Integração | TI-01 a TI-05 (Frontend→Backend, Backend→BD, Backend→Recomendação, Backend→ViaCEP, Backend→Ticketera) | Completo |

### 1.2 Execução (seção 20 do Projeto.md) — ⚠️ parcial

| Subseção | Conteúdo | Status |
|---|---|---|
| 20.3 Testes de funcionalidade | TF-01 a TF-10 | ❌ **Template vazio** (placeholders `[informar...]`) |
| 20.4 Testes de integração | TI-01 a TI-05 | ❌ **Template vazio** |
| 20.5 Regras de negócio | Suíte unitária do backend | ✅ Executado em 11/06/2026 — 123/123 aprovados, evidências `.trx` |
| 20.6 Testes de RNFs | RNF01 a RNF08 | ❌ **Template vazio** |
| 20.7 Usabilidade | TU-01 a TU-08 + SUS | ✅ Executado — 5 participantes, 100% conclusão, SUS 88,5 |
| 20.8 APIs externas | ViaCEP | ❌ **Template vazio** |
| 20.9 Análise consolidada | Indicadores gerais | ❌ **Template vazio** |

### 1.3 Suítes automatizadas existentes no código

| Suíte | Localização | Escopo | Observações |
|---|---|---|---|
| Backend unit (xUnit) | `src/backend/BackEnd.Aventura.Tests/` | 123 testes: controllers, services, domain, DTOs, DI, middleware, exceções, regras de negócio | Verde em 11/06/2026 |
| Backend integração (xUnit + Testcontainers) | `src/backend/BackEnd.Aventura.IntegrationTests/` | Schema, constraints, triggers (consentimento, preferências), timestamptz, LGPD — contra PostgreSQL 16 real em container | **Requer Docker**; execução não documentada na seção 20 |
| Inference service (xUnit) | `src/inference-service/InferenceService.Tests/` | Modelo híbrido (loader, preprocessor, batch job, recomendação) | Execução não documentada |
| Frontend (Jest) | `src/frontend/mobile-app-aventura/tests/` | 8 suítes: viewmodels (auth, onboarding, wishlist, search, notifications, edit interests) e services | **Sem script `test` no `package.json`** — não executável via `npm test` |
| Conexão banco | `tests/TesteConexaoBanco.cs` | Smoke test de conexão | Utilitário avulso |

---

## 2. Gaps — o que falta

| # | Gap | Impacto | RF/RNF afetado |
|---|---|---|---|
| G1 | Execução manual de TF-01 a TF-10 não realizada/documentada (20.3 vazio) | A entrega exige testes funcionais executados com cenários positivos e negativos | RF01–RF10 |
| G2 | Nenhum RNF executado/documentado (20.6 vazio) | Critérios de aceitação (p95, hash, LGPD, uptime, carga, geoloc, cobertura) sem medição | RNF01–RNF08 |
| G3 | Não existem scripts de teste de carga (k6/Locust) no repositório | RNF06 não verificável | RNF06 |
| G4 | Cobertura de código nunca foi medida (sem coverlet/reporte) | RNF08 exige cobertura ≥ 70% nos módulos críticos | RNF08 |
| G5 | TI-01, TI-03, TI-04 e TI-05 planejam ferramentas (WireMock.Net, VHS, WebApplicationFactory) que **não existem no código** — só TI-02 (Backend→BD) está parcialmente implementado via `IntegrationTests` | Plano de integração descolado da implementação real | RNF08, RF03, RF09, RF10 |
| G6 | Frontend sem script `npm test` configurado | Suíte Jest existente não roda de forma reproduzível | RF01, RF05, RF06, RF08 |
| G7 | Testes de APIs externas (ViaCEP) não executados nem automatizados (20.8 vazio) | Integração de geolocalização sem validação | RNF07, RF02 |
| G8 | Análise consolidada (20.9) vazia | Sem visão geral de qualidade/prontidão | Todos |
| G9 | Sem monitoramento de uptime configurado e **sem endpoint `/health` no backend** (pré-condição do caso 17.4) | RNF05 (disponibilidade ≥ 99%) não mensurável | RNF05 |
| G10 | Inconsistência interna do Projeto.md no RNF06: o critério de aceitação (seção 10) pede 1.000 usuários, mas os cenários planejados em 17.4 especificam 100 (positivo), 150 (stress) e spike 0→100 em 30s | Escopo de carga ambíguo; 1.000 VUs contra a VM única da OCI é arriscado | RNF06 |

**Restrições de ambiente conhecidas (infra OCI):** o backend roda na VM `163.176.204.208:5253` (HTTP, porta alta); as portas 80/443 estão bloqueadas pela Security List e o time não tem acesso ao console OCI. Consequências para o plano: (i) RNF02 (HTTPS/TLS) não é verificável no ambiente publicado — validar a parte de hashing no banco e **documentar a limitação de TLS como risco de infra**; (ii) testes de uptime e carga devem apontar para a porta 5253.

---

## 3. Estratégia e matriz de rastreabilidade

Cada RF será verificado por **duas camadas**: a suíte automatizada que já cobre a regra de negócio (rastreada abaixo) e a execução manual do caso TF correspondente no app (Expo + backend), com evidências (prints/vídeos). Cada RNF será verificado pelo procedimento planejado na seção 17.4, adaptado às restrições de ambiente.

| Requisito | Caso planejado | Verificação automatizada (existente ou a criar) | Verificação manual |
|---|---|---|---|
| RF01 Onboarding | TF-01 | `Useonboardingviewmodel.test.js`, `Onboardingservice.test.js`, `PreferenciasServiceTests` | Fluxo no app (positivo + pular/parcial) |
| RF02 Geolocalização | TF-02 | `RecomendacaoServiceTests` (filtro cidade) | Carrossel "Perto de você" SP→RJ + empty state |
| RF03 Autenticação | TF-03 | `AuthControllerTests`, `AuthServiceTests`, `Useauthviewmodel.test.js` | Login válido/inválido, sessão persistente, logout |
| RF04 Catálogo | TF-04 | `EspetaculoControllerTests`, `GeneroServiceTests` | Filtro por categoria + categoria rara |
| RF05 Busca | TF-05 | `Usesearchviewmodel.test.js`, `EspetaculoServiceTests` | Busca título/teatro/termo inexistente |
| RF06 Wishlist | TF-06 | `WishlistControllerTests`, `WishlistServiceTests`, `Usewishlistviewmodel.test.js` | Adicionar/remover + empty state + notificações |
| RF07 Teatros | TF-07 | `TeatroControllerTests`, `TeatroServiceTests` | Seção "Conheça os Teatros" + link externo |
| RF08 Perfil | TF-08 | `PreferenciasControllerTests`, `UsuarioServiceTests`, `Useeditinterestsviewmodel.test.js` | Editar interesses/cidade, limpar filtros, auto-save |
| RF09 Compra | TF-09 | `EspetaculoTests` (URL externa obrigatória) | Botão sticky + redirecionamento 2 ticketeiras |
| RF10 Recomendação | TF-10 | `RecomendacaoControllerTests`, `InferenceService.Tests` | Carrossel "Pra você" + "Semelhantes" + empty state |
| RNF01 Tempo de resposta | 17.4 | Script de medição (k6 smoke ou curl loop, 20+ amostras) | DevTools/Lighthouse na home + negativos 17.4: Slow 3G (throttling) e fallback com backend 503 |
| RNF02 Credenciais | 17.4 | `AuthServiceTests` (hash) + consulta direta ao banco (BCrypt + salt) | Inspeção de tráfego + **brute force** (10 logins inválidos, cenário negativo 1 de 17.4); **TLS/HSTS: limitação documentada** |
| RNF03 LGPD | 17.4 | `LgpdDatabaseTests`, `TriggerConsentimentoTests`, `AnonimizacaoJob` (integração) | Fluxo de exclusão/edição no app + verificação no banco |
| RNF04 Responsividade | 17.4 | — | Já coberto em 20.7 (SUS 88,5); complementar com inspeção 480–1920 px |
| RNF05 Disponibilidade | 17.4 | Monitor de uptime (UptimeRobot ou script agendado) contra `:5253/health` (**endpoint criado na Fase 0**) | Relatório da janela de medição (24h fecha o caso 17.4; 7 dias como esforço extra rumo ao requisito de 30 dias) |
| RNF06 Carga | 17.4 | **Criar** `tests/load/k6-pico.js` com os 3 cenários de 17.4: 100 VUs/15 min (positivo), 150 VUs/10 min (stress), spike 0→100 em 30s; rampa a 1.000 VUs como etapa adicional condicionada à autorização do time | — |
| RNF07 Geolocalização | 17.4 | Casos ViaCEP mockados/reais | 10 perfis (5 SP, 5 RJ) na home |
| RNF08 Modularidade | 17.4 | `dotnet test` + **coverlet** (meta ≥ 70% módulos críticos); deploy isolado do inference-service; threshold de cobertura no CI (cenário negativo 2 de 17.4): **registrar como limitação/backlog** — não há gate de cobertura no pipeline | — |

---

## 4. Plano de execução (fases)

### Fase 0 — Preparação do ambiente (½ dia)
1. Adicionar script `"test": "jest"` ao `package.json` do frontend e confirmar que as 8 suítes passam (fecha G6).
2. Subir ambiente local: `docker-compose-db.yml` (PostgreSQL) + backend (`dotnet run`) + app (`npx expo start`); aplicar seeds com espetáculos em SP e RJ em múltiplos gêneros.
3. Criar pasta de evidências: `assets/testes/` (prints, vídeos, relatórios) e `tests/load/` (scripts k6).
4. Criar contas de teste: usuário completo (onboarding feito, wishlist ≥ 2 itens), usuário sem onboarding, usuário sem cidade.
5. **Adicionar `MapHealthChecks("/health")` no `Program.cs` do backend** (`AddHealthChecks()` + endpoint; nativo do ASP.NET Core, sem pacote extra) e fazer deploy na VM da OCI, habilitando o monitor de uptime do RNF05 em `163.176.204.208:5253/health` (fecha a parte de endpoint do G9).

### Fase 1 — Suítes automatizadas + cobertura (½ dia)
1. `dotnet test` nas 3 suítes (.NET) com `--collect:"XPlat Code Coverage"` (coverlet) e logger `trx`; salvar `.trx` e relatório de cobertura em `TestResults/`.
2. `npm test` no frontend com `--coverage`.
3. Preencher **20.4** (integração: TI-02 via Testcontainers — escopo real executado, registrando que TI-01/03/04/05 foram verificados em nível de serviço com fakes/mocks da suíte unitária, e o que fica como evolução) e atualizar **20.5** se a contagem mudar.
4. Registrar percentual de cobertura para o **RNF08** (fecha G4, parte de G5).

### Fase 2 — Execução manual TF-01 a TF-10 (1–1,5 dia)
1. Executar cada TF conforme instruções da seção 17.3, **incluindo todos os cenários negativos**, coletando 1 evidência por cenário (print/vídeo) em `assets/testes/`.
2. Registrar resultado por caso em tabela: ID, cenários executados, resultado (aprovado/aprovado com ressalva/reprovado), evidência, desvios encontrados.
3. Preencher **20.3** com a tabela consolidada, análise e links das evidências (fecha G1).
4. Bugs encontrados viram issues no GitLab com referência ao TF.

### Fase 3 — RNFs mensuráveis (1–1,5 dia)
| RNF | Ação | Evidência |
|---|---|---|
| RNF01 | Positivo: 20–30 medições do tempo de carga da home/recomendações (DevTools + script com timestamps) em horários distintos; calcular média, p95, p99. **Negativos (17.4):** (i) throttling Slow 3G no DevTools, 5 repetições, verificando skeleton/spinner e mensagem após timeout; (ii) backend de recomendação derrubado/mockado com 503, verificando fallback na home sem erro 500 exposto | Planilha/JSON de amostras + prints dos cenários degradados |
| RNF02 | Positivo: consulta direta ao banco (formato do hash + salt), inspeção do payload de login. **Negativo 1 (brute force):** 10 logins consecutivos com senha incorreta via curl/Postman, verificando bloqueio/limite e registro em log — executável mesmo sem TLS. **Negativo 2 (downgrade HTTPS/HSTS):** bloqueado pela infra OCI — documentar limitação | Print do banco + tráfego + saída do script de brute force |
| RNF03 | Executar fluxo de exclusão/edição no app; consultar banco antes/depois; conferir trilha de auditoria; referenciar `LgpdDatabaseTests` | Queries + `.trx` |
| RNF04 | Referenciar 20.7 (já executado) + inspeção de 480/720/1080/1920 px nas telas principais | Prints por resolução |
| RNF05 | Configurar monitor (UptimeRobot ou cron + curl em `:5253/health`, criado na Fase 0). **Janela de 24h com checagem a cada 1 min fecha o caso de teste planejado em 17.4**; estender a 7 dias como esforço extra rumo ao requisito de 30 dias da seção 10, extrapolando e registrando a limitação | Relatório do monitor |
| RNF06 | Criar `tests/load/k6-pico.js` com os **3 cenários de 17.4** (fluxo: home + carrosséis + wishlist): (i) positivo — rampa 0→50→100 VUs em 5 min, sustentar 100 por 15 min (metas: média ≤ 3s, p95 ≤ 5s, erro < 1%); (ii) stress — 150 VUs por 10 min, observando degradação elegante; (iii) spike — 0→100 VUs em 30s, observando recuperação em ≤ 60s. **Etapa adicional condicionada à autorização do time:** rampa 100→500→1.000 VUs (critério da seção 10) com monitoramento de CPU/memória da VM. Registrar no 20.6 a **inconsistência interna do Projeto.md (G10)** entre seção 10 (1.000) e 17.4 (100/150) | Saída k6 + gráficos |
| RNF07 | 10 perfis (5 SP, 5 RJ) com CEPs reais; verificar seção regional; registrar acurácia | Tabela de acertos |
| RNF08 | Cobertura da Fase 1 + demonstrar deploy isolado do inference-service sem derrubar o backend (health checks contínuos no backend durante o deploy). **Cenário negativo 2 de 17.4 (gate de cobertura < 70% falhando o pipeline): não há threshold configurado no CI — registrar como limitação no 20.6 e mover para o backlog** | Relatório coverlet + log de deploy + log de health checks |

Preencher **20.6** com métricas vs. critérios (fecha G2, G3, G9 e documenta G10).

### Fase 4 — APIs externas (½ dia)
1. Executar os 3 casos ViaCEP da seção 17.6 (CEP válido `01310-100`, inválidos `123`/`99999-999`, throttling/timeout) via Postman/cURL contra a API real e/ou mock.
2. Verificar o tratamento do backend (fallback, mensagem amigável, log).
3. Preencher **20.8** com códigos HTTP, tempos e evidências (fecha G7).

### Fase 5 — Consolidação (½ dia)
1. Preencher **20.9**: total executado, aprovados/reprovados/bloqueados, falhas principais, riscos remanescentes (TLS bloqueado na OCI, janela curta de uptime, TI-01/03/04/05 mockados), ações corretivas.
2. Atualizar [Index.md](./Index.md) com este documento.
3. Abrir MR de `feature/testes-RF-RNF` → `develop`.

---

## 5. Critérios de saída do plano

- [ ] 20.3, 20.4, 20.6, 20.8 e 20.9 sem nenhum placeholder `[informar...]`.
- [ ] 10/10 TFs executados com cenários positivos **e** negativos, cada um com evidência.
- [ ] 8/8 RNFs com medição registrada ou limitação justificada e rastreada como risco, **incluindo os cenários negativos de 17.4 executáveis no ambiente** (Slow 3G e fallback 503 do RNF01, brute force do RNF02, stress e spike do RNF06).
- [ ] Endpoint `/health` publicado no backend e monitor de uptime ativo.
- [ ] Todas as suítes automatizadas verdes, com `.trx` e relatório de cobertura versionáveis.
- [ ] Script de carga k6 versionado em `tests/load/`.
- [ ] Frontend executável via `npm test`.
- [ ] Bugs encontrados registrados como issues com referência ao caso de teste.

## 6. Riscos e mitigações

| Risco | Mitigação |
|---|---|
| Portas 80/443 bloqueadas na OCI impedem teste de TLS e dificultam carga realista | Validar hashing localmente; rodar carga contra ambiente local/homolog; registrar como risco de infra no 20.9 |
| Testcontainers exige Docker na máquina de quem executa | Documentar pré-requisito; alternativa: apontar para o PostgreSQL do `docker-compose-db.yml` |
| Janela de 30 dias de uptime inviável no prazo da sprint | Medir janela de 7 dias e extrapolar, registrando a limitação |
| 1.000 VUs podem derrubar a VM única da OCI | Escopo base são os cenários de 17.4 (100/150/spike); a rampa 100→500→1.000 é etapa adicional que só roda com autorização do time e monitoramento de CPU/memória |
| TI-01/03/04/05 exigem infraestrutura de teste (WireMock/VHS) inexistente | Registrar o que foi efetivamente coberto (fakes unitários + TI-02 real) e mover o restante para backlog da próxima sprint |
