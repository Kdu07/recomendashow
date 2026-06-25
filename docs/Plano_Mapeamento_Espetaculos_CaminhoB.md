# Plano — Caminho B: Mapeamento de Espetáculos (coluna de mapeamento + código)

> Plano temporário de branch. Remover este arquivo (e referências) antes do merge na `develop`.
> Data: 2026-06-25 · Branch alvo: a definir (sugestão: `feat/espetaculo-dataset-mapping`)

## 1. Problema

O batch de recomendação (`RecommendationBatchJob`) gera, para cada `userId` do dataset,
uma lista de `RecommendationResult` cujo `Espetaculo` é o **nome cru do show do dataset**
(`r.Show`, em CAIXA ALTA, ex.: `A NOVIÇA REBELDE`). A persistência casa por **igualdade
exata de string**:

```sql
-- RecommendationWriteRepository.cs:70-74
INSERT INTO recomendacoes(...)
SELECT @UsuarioId, e.id, ...
FROM espetaculos e
WHERE e.nome = @EspetaculoNome
```

Como `espetaculos.nome` guarda o nome "bonito" (`A Noviça Rebelde`), nenhuma recomendação
casa → `recomendacoes` fica em 0. (O mapeamento de usuários via `ml_id` já foi resolvido;
ver [[project_d4_user_mapping]] / [[project_recs_persist_blocked]].)

**Objetivo do Caminho B:** persistir recomendações **mantendo os nomes "bonitos"** em
`espetaculos.nome`, introduzindo uma coluna de mapeamento + ajuste no repositório de escrita.
Difere do Caminho A (renomear espetáculos para o nome cru do dataset, sem tocar em código).

## 2. Decisão de design

Coluna de mapeamento em `espetaculos`, análoga ao `ml_id` dos usuários:

- **`espetaculos.item_dataset TEXT`** — guarda o nome do show do dataset (exatamente como o
  modelo emite) correspondente àquele espetáculo. `UNIQUE`, nullable (espetáculo sem
  correspondência fica `NULL` e simplesmente não recebe recs).
- A escrita passa a resolver o espetáculo por `item_dataset`, com **normalização defensiva**
  (TRIM + UPPER + unaccent) dos dois lados, para absorver diferenças de espaço/acento/caixa
  entre o valor emitido pelo modelo e o valor gravado.

Cardinalidade: 1:1 (cada espetáculo ↔ 1 show). Cobre apenas os espetáculos com `item_dataset`
preenchido. Para cobertura ampla, ver a variante N:1 na seção 7.

## 3. Fases

### Fase 0 — Confirmar o valor exato emitido pelo modelo (pré-requisito)
- Inspecionar em `src/ml` (AventuraML.Recommender) como `r.Show` é construído e qual o
  formato exato (há aspas? espaço à direita? o `item_id` cru do dataset tem `" ... "`).
- Critério de saída: ter 2-3 exemplos do valor literal de `r.Show` para validar o casamento.
- Risco coberto: se o modelo emitir o `item_id` com lixo de parsing (espaço/aspas), a
  normalização da Fase 2 precisa contemplá-lo.

### Fase 1 — Banco (migration idempotente)
Novo script `src/database/03_espetaculo_dataset_mapping.sql` (ou append em `02_sprint4_improvements.sql`):

1. `ALTER TABLE espetaculos ADD COLUMN IF NOT EXISTS item_dataset TEXT;`
   `CREATE UNIQUE INDEX IF NOT EXISTS idx_espetaculos_item_dataset ON espetaculos(item_dataset);`
2. (Opcional) `CREATE EXTENSION IF NOT EXISTS unaccent;` se a normalização for feita em SQL.
3. Popular `item_dataset` dos espetáculos com correspondência:
   - `A Noviça Rebelde → A NOVIÇA REBELDE` (exato)
   - `Elis → ELIS, A MUSICAL`; `Hairspray → MUSICAL HAIRSPRAY`;
     `Les Misérables → LES MISERABLES SCHOOL EDITION`;
     `Vozes Negras → VOZES NEGRAS – A FORÇA DO CANTO FEMININO`;
     `Romeu e Julieta → CIA DE BALLET DO RIO DE JANEIRO - ROMEU E JULIETA` (parciais — revisar)
4. (Decisão de catálogo — ver seção 4) inserir N shows populares do dataset como novos
   espetáculos, com `nome` bonito + `item_dataset` = nome cru + `data_inicio`/`data_fim`
   placeholder (NOT NULL) + `tipo_evento`/`valor_ingresso` do dataset.

### Fase 2 — Código (`src/inference-service`)
- `RecommendationWriteRepository`:
  - Trocar a resolução do espetáculo para usar `item_dataset` com normalização:
    ```sql
    WHERE upper(unaccent(btrim(e.item_dataset))) = upper(unaccent(btrim(@EspetaculoNome)))
    ```
    (ou normalizar em C# e comparar coluna pré-normalizada — decidir na implementação).
  - Manter fallback opcional por `e.nome` para não regredir dados já alinhados ao Caminho A.
  - Ajustar a mensagem de warning para distinguir "espetáculo não mapeado" de
    "usuário não mapeado".
- Testes (`InferenceService.Tests/RecommendationWriteRepositoryTests.cs`):
  - Caso: espetáculo com `item_dataset` preenchido → persiste.
  - Caso: nome do modelo com caixa/acroento diferente → ainda casa via normalização.
  - Caso: espetáculo sem `item_dataset` → não persiste, warning apropriado.

### Fase 3 — Deploy
- `dotnet test` local (backend + inference).
- Rebuild + redeploy: `powershell -File deploy\deploy.ps1` (imagem nova do inference-service).
- Aplicar a migration da Fase 1 no banco de produção (`aventura_db` em 137.131.172.248),
  em transação, com backup das linhas afetadas se houver DELETE (ver seção 4).

### Fase 4 — Validação
- `SELECT COUNT(*) FROM recomendacoes;` deve sair de 0.
- `SELECT COUNT(DISTINCT usuario_id) FROM recomendacoes;` — quantos usuários cobertos.
- Conferir nos logs do `inference` a queda dos warnings "Persistidas 0/10".
- Smoke do endpoint interno (`/api/diagnostics/health`).

## 4. Decisão de catálogo (pendente — input do usuário)
Independente de A/B. Em B os 16 sem match **não precisam** ser removidos (ficam com
`item_dataset NULL`). Opções:
- (a) Manter os 16, só não recebem recs.
- (b) Remover os 16 — apaga em cascata **13 wishlist + 2 interações** + vínculos de
  gênero/teatro (FKs são `ON DELETE CASCADE`). Fazer **backup** antes.
- Quantos shows do dataset adicionar: 15 / 30 / 50 (mais = mais cobertura). Lista dos
  top-15 já levantada.

## 5. Trade-off fidelidade × cobertura
- Match por nome (1:1) é fiel mas cobre poucos shows → poucas recs persistem.
- Os 5 "parciais" são semanticamente plausíveis, mas `Romeu e Julieta → ...BALLET...` é
  duvidoso (revisar manualmente).
- Cobertura ampla exige adicionar muitos espetáculos ou adotar a variante N:1 (seção 7),
  cujos mapeamentos extras são artificiais.

## 6. Riscos / rollback
- Migration em transação; `item_dataset` é aditivo (não destrói dados).
- DELETE dos 16 (se escolhido) é destrutivo → dump prévio das linhas afetadas.
- Deploy reversível: imagem anterior (`docker tag`/registry) ou re-deploy do commit anterior.
- O batch processa todos os user_ids do dataset (milhares) e gera muito log; sem impacto
  funcional, mas considerar limitar o batch aos usuários com `ml_id` mapeado (otimização
  fora de escopo deste plano).

## 7. Variante N:1 (cobertura total — opcional)
Em vez de coluna 1:1, tabela `espetaculo_item_dataset(item_dataset TEXT PK, espetaculo_id UUID
REFERENCES espetaculos(id))` mapeando **todos os 447 shows → algum dos espetáculos do banco**.
Cobre 100% das recomendações, mas a maioria dos mapeamentos é arbitrária. Recomendado só se
o objetivo for "toda recomendação persiste" para a demo.

## 8. Checklist de execução
- [ ] Fase 0: confirmar formato de `r.Show`
- [ ] Decisão de catálogo (seção 4) + revisão dos 5 parciais
- [ ] Migration `03_espetaculo_dataset_mapping.sql` + backup se houver DELETE
- [ ] Ajuste no `RecommendationWriteRepository` + testes
- [ ] `dotnet test` verde
- [ ] Deploy (`deploy.ps1`) + aplicar migration em prod
- [ ] Validar `recomendacoes > 0` + cobertura
- [ ] Remover este plano antes do merge na `develop`
