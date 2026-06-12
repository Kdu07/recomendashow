# Plano — CI mínimo viável com gate de cobertura (RNF08)

> **Branch alvo:** `feature/testes-RF-RNF` · **Data:** 12/06/2026
> Objetivo: fechar o cenário negativo 2 do RNF08 (seção 17.4 do [Projeto.md](./Projeto.md)) — pipeline que **falha** quando a cobertura dos módulos críticos cai abaixo de 70%. Hoje não existe `.gitlab-ci.yml` no repositório.

## Escopo (mínimo viável)

Um único `.gitlab-ci.yml` na raiz, com 2 jobs em 1 stage (`test`). Sem deploy, sem lint, sem build de imagem — só testes + gate de cobertura.

| Job | Imagem | O que roda | Gate |
|---|---|---|---|
| `backend-tests` | `mcr.microsoft.com/dotnet/sdk:9.0` | `BackEnd.Aventura.Tests` + `InferenceService.Tests` | Cobertura de linha ≥ 70% (coverlet `Threshold=70`) |
| `frontend-tests` | `node:20` | Jest das 8 suítes | `coverageThreshold` global ≥ 70% (statements/lines) |

**Fora do escopo:** `IntegrationTests` (Testcontainers exige Docker-in-Docker — fica para evolução), testes de carga, deploy.

## Pré-requisitos (descobertos na execução da seção 20)

1. **`coverlet` não está instalado nos projetos de teste** — a execução de 12/06 acusou `Unable to find a datacollector with friendly name 'XPlat Code Coverage'`. Adicionar a cada `.csproj` de teste:
   - `coverlet.collector` (para `--collect`) **ou** `coverlet.msbuild` (necessário para `/p:Threshold`, que é o que dá o gate). → usar **`coverlet.msbuild`**.
2. **16 testes Jest falham hoje** (4 suítes: `Usesearchviewmodel`, `Onboardingservice` com `__DEV__ is not defined`, etc.). Um gate agora deixaria o pipeline permanentemente vermelho. O plano trata isso na Fase 1.
3. Cobertura Jest atual ≈ 76,7% statements (acima do gate); cobertura .NET ainda não medida com coverlet — se vier < 70%, registrar e ajustar o gate inicial para o valor real - 5 p.p., subindo até 70% no backlog.

## Fases

### Fase 1 — Destravar as suítes (pré-gate)
1. Corrigir as 4 suítes Jest que falham:
   - `__DEV__ is not defined`: adicionar `globals: { "__DEV__": true }` no bloco `jest` do `package.json` (ou trocar `testEnvironment` para o preset `react-native`).
   - Reavaliar as asserções quebradas de `Usesearchviewmodel` (16 falhas) — corrigir teste ou código.
2. Adicionar `coverlet.msbuild` aos `.csproj` de `BackEnd.Aventura.Tests` e `InferenceService.Tests`.
3. Validar localmente:
   ```powershell
   dotnet test src/backend/BackEnd.Aventura.Tests /p:CollectCoverage=true /p:Threshold=70 /p:ThresholdType=line
   cd src/frontend/mobile-app-aventura; npm test -- --coverage
   ```

### Fase 2 — `.gitlab-ci.yml`
Criar na raiz:

```yaml
stages: [test]

backend-tests:
  stage: test
  image: mcr.microsoft.com/dotnet/sdk:9.0
  script:
    - dotnet test src/backend/BackEnd.Aventura.Tests /p:CollectCoverage=true /p:Threshold=70 /p:ThresholdType=line /p:CoverletOutputFormat=cobertura
    - dotnet test src/inference-service/InferenceService.Tests /p:CollectCoverage=true /p:Threshold=70 /p:ThresholdType=line /p:CoverletOutputFormat=cobertura
  coverage: '/Total[^|]*\|[^|]*\s+([\d\.]+)%/'
  artifacts:
    when: always
    paths: ["**/coverage.cobertura.xml"]

frontend-tests:
  stage: test
  image: node:20
  script:
    - cd src/frontend/mobile-app-aventura
    - npm ci
    - npm test -- --coverage
  # gate via "coverageThreshold" no bloco jest do package.json:
  # "coverageThreshold": { "global": { "lines": 70, "statements": 70 } }
```

### Fase 3 — Validação e documentação
1. Push da branch e conferir o pipeline no GitLab (verde com as suítes corrigidas; vermelho ao simular queda de cobertura — evidência do cenário negativo 2 do RNF08).
2. Atualizar a seção **20.6 (RNF08)** do Projeto.md: trocar "limitação/backlog" pelo link do pipeline e print do gate falhando/passando.
3. Atualizar [Index.md](./Index.md) com este documento.

## Critérios de saída
- [ ] `.gitlab-ci.yml` na raiz, pipeline verde na branch.
- [ ] Gate de 70% ativo nos 2 jobs (e evidência de que falha quando violado).
- [ ] 8/8 suítes Jest passando localmente e no CI.
- [ ] 20.6 (RNF08) atualizado sem a limitação de "sem gate no CI".

## Riscos
| Risco | Mitigação |
|---|---|
| Cobertura .NET real < 70% travar o time | Medir antes (Fase 1.3); se < 70%, gate inicial no valor real − 5 p.p. e subir gradualmente |
| Runner do GitLab da disciplina sem imagem .NET 9 ou sem acesso à internet | Verificar runners disponíveis antes; fallback: job único em imagem `node:20` + script de instalação do SDK |
| Correção das 16 falhas Jest revelar bug real no `useSearchViewModel` | Tratar como issue separada referenciando TF-05/RF05 |
