# Handoff — Mapeamento de Espetáculos para Persistência de Recomendações

> Documento de contexto + plano para retomar em outra sessão. Temporário de branch:
> remover (com referências) antes do merge na `develop`.
> Origem: sessão de deploy do inference-service em 2026-06-25.

---

## 1. Contexto do sistema

- **inference-service** (`src/inference-service`, C#/.NET 9): roda um batch
  (`RecommendationBatchJob`) que, para cada `userId` do dataset, pede 10 recomendações ao
  modelo híbrido (`AventuraML.Recommender` em `src/ml`) e tenta persistir na tabela
  `recomendacoes` do Postgres.
- **Deploy**: `powershell -File deploy\deploy.ps1` (rodar da raiz do repo). Builda a imagem
  Docker, envia para a VM via SSH, sobe `nginx` + `inference` com `docker-compose.deploy.yml`.
- **Infra** (ver memória `project_infra_oci`):
  - VM app/inference: `163.176.204.208` (user `ubuntu`, chave `chave-back.key` na raiz).
  - VM banco: `137.131.172.248`, Postgres, db `aventura_db`, user `postgres`.
    Senha está em `.env.deploy` (gitignored) — não reproduzir aqui.
  - Security List da OCI bloqueia 80/443 externamente; o time não tem console OCI. Serviço
    só é acessível de dentro da VM (`https://localhost/api/diagnostics/health` → 200).

## 2. O que já foi feito nesta sessão

1. **Deploy concluído** — `inference` rodando e `healthy` na VM. Modelo carregado.
2. **Mapeamento de usuários (`ml_id`) resolvido em produção**:
   - Os 3 usuários da Decisão D4 estavam com `ml_id` errado (SERIAL 1/2/3 em vez de
     63261/14888/55251). Os `UPDATE`s do `02_sprint4_improvements.sql` nunca tinham rodado
     em prod. **Corrigido.**
   - Os outros 31 usuários também foram mapeados para user_ids reais do dataset.
   - Hoje os 34 usuários do banco têm `ml_id` válido. (ver memória `project_recs_persist_blocked`)
3. **Diagnóstico do bloqueio de itens** (abaixo).

## 3. O problema central (itens / espetáculos)

`recomendacoes` continua em **0**. Causa: a persistência casa por **igualdade exata de string**:

```sql
-- src/inference-service/Data/RecommendationWriteRepository.cs:70-74
INSERT INTO recomendacoes(usuario_id, espetaculo_id, score, motivo, data_geracao)
SELECT @UsuarioId, e.id, @Score, @Motivo, CURRENT_TIMESTAMP
FROM espetaculos e
WHERE e.nome = @EspetaculoNome
```

- `@EspetaculoNome` = `r.Show`, o **nome cru do show do dataset**, em CAIXA ALTA
  (ex.: `A NOVIÇA REBELDE`).
- `espetaculos.nome` no banco está formatado "bonito" (`A Noviça Rebelde`).
- Igualdade exata é case/acento/espaço-sensível → **nenhum dos 22 espetáculos do banco casa**.

Além disso, os catálogos são quase disjuntos: o dataset tem **447 shows** (musicais/stand-up,
nomes em caixa alta), o banco tem **22 peças curadas**. Só **6** têm correspondência semântica.

## 4. Dados de referência (já levantados)

### 4.1 Espetáculos do banco COM correspondência no dataset (6)
| Banco | Show no dataset (nome cru) | Tipo |
|-------|---------------------------|------|
| A Noviça Rebelde | `A NOVIÇA REBELDE` | exato |
| Elis | `ELIS, A MUSICAL` | parcial |
| Hairspray | `MUSICAL HAIRSPRAY` | parcial |
| Les Misérables | `LES MISERABLES SCHOOL EDITION` | parcial |
| Vozes Negras | `VOZES NEGRAS – A FORÇA DO CANTO FEMININO` | parcial |
| Romeu e Julieta | `CIA DE BALLET DO RIO DE JANEIRO - ROMEU E JULIETA` | parcial (duvidoso — é ballet) |

### 4.2 Espetáculos do banco SEM correspondência (16 — candidatos a remover)
A autoestima do homem hetero · Autobiografia do vermelho · Ayrton Senna, o Musical ·
Chacrinha · Charlie e Lola - A Peça · Deus da Carnificina · Escafandristas · Espelho Mágico ·
Hamlet · Johnny Hooker · Mamma Mia! · O Fantasma da Ópera · Piaf · Victor Sarro ·
Viva – A Vida é Uma Festa In Concert · Mariana Armellini

### 4.3 Top-15 shows populares do dataset AUSENTES no banco (candidatos a adicionar)
Use o **nome cru exato** (coluna `item_dataset`) na hora de mapear.
```
 1. NOSSA HISTÓRIA COM CHICO BUARQUE                        — 3645 int — Show Musical       — R$84
 2. CLUBE DA ESQUINA - OS SONHOS NÃO ENVELHECEM             — 3466 int — Show Musical       — R$84
 3. PRIMA FACIE COM DÉBORA FALABELLA                        — 3148 int — Espetáculo Teatral — R$71
 4. MACACOS                                                 — 2517 int — Espetáculo Teatral — R$51
 5. PORTÁTIL                                                — 2176 int — Espetáculo Teatral — R$67
 6. KING KONG FRAN                                          — 2069 int — Espetáculo Teatral — R$47
 7. A COR PÚRPURA, O MUSICAL                                — 2014 int — Espetáculo Musical — R$59
 8. MUSEU NACIONAL [TODAS AS VOZES DO FOGO]                 — 1782 int — Outros             — R$49
 9. A ÚLTIMA SESSÃO DE FREUD COM ODILON WAGNER E M.AIROLDI  — 1621 int — Espetáculo Teatral — R$69
10. ANTES DO ANO QUE VEM                                    — 1539 int — Espetáculo Teatral — R$41
11. BIBI – UMA VIDA EM MUSICAL                              — 1400 int — Espetáculo Musical — R$47
12. THIAGO VENTURA - MODO EFETIVO                           — 1354 int — Show Stand-Up      — R$62
13. UM FILME ARGENTINO - COM LETÍCIA COLIN E MICHEL MELAMED — 1320 int — Espetáculo Teatral — R$61
14. A LISTA                                                 — 1284 int — Espetáculo Teatral — R$63
15. RENATO ALBANI EM NOVO SHOW                              — 1275 int — Show Stand-Up      — R$81
```
> Os top-22 shows do dataset cobrem ~47,8% das interações. Para mais cobertura, gerar a lista
> com `top 30`/`top 50` (script de contagem por `item_id`, ordenado desc).

### 4.4 Schema `espetaculos`
| coluna | tipo | obrigatório |
|--------|------|-------------|
| id | uuid (default gen_random_uuid()) | auto |
| nome | varchar | **NOT NULL** |
| tipo_evento | varchar | opcional |
| descricao | text | opcional |
| url_bilheteria | text | opcional |
| data_inicio | timestamp | **NOT NULL** |
| data_fim | timestamp | **NOT NULL** |
| valor_ingresso | numeric | opcional |

### 4.5 FKs que referenciam `espetaculos` (todas `ON DELETE CASCADE`)
`interacoes`, `recomendacoes`, `espetaculo_genero`, `teatro_espetaculo`, `wishlist_curtidas`.
Remover os 16 apaga em cascata: **2 interações, 13 wishlist**, 5 gênero, 15 teatro
(recomendacoes=0). **Fazer backup antes de qualquer DELETE.**

## 5. Tarefa que o USUÁRIO fará (manual, depois)
- Remover os 16 espetáculos sem match (seção 4.2) — com backup prévio das linhas + cascata.
- Adicionar shows populares do dataset (seção 4.3) como novos espetáculos.

## 6. Plano de código (Caminho B) — manter nomes "bonitos" + coluna de mapeamento
Detalhado em `docs/Plano_Mapeamento_Espetaculos_CaminhoB.md`. Resumo:

1. **Fase 0** — confirmar em `src/ml` (AventuraML.Recommender) o **formato exato** de `r.Show`
   (tem aspas? espaço à direita? — o `item_id` cru no CSV vem como `" ... "` às vezes).
2. **Fase 1 — banco** (migration idempotente, ex. `src/database/03_espetaculo_dataset_mapping.sql`):
   - `ALTER TABLE espetaculos ADD COLUMN IF NOT EXISTS item_dataset TEXT;`
   - `CREATE UNIQUE INDEX IF NOT EXISTS idx_espetaculos_item_dataset ON espetaculos(item_dataset);`
   - Popular `item_dataset` (seção 4.1 para os existentes; nome cru para os novos da 4.3).
3. **Fase 2 — código** (`RecommendationWriteRepository`): resolver o espetáculo por
   `item_dataset` com normalização defensiva (TRIM+UPPER+unaccent dos dois lados). Atualizar
   testes em `InferenceService.Tests/RecommendationWriteRepositoryTests.cs`.
4. **Fase 3 — deploy**: `dotnet test` → `deploy.ps1` → aplicar migration em prod (transação).
5. **Fase 4 — validar**: `SELECT COUNT(*) FROM recomendacoes` > 0; conferir queda dos warnings.

### Alternativa (Caminho A, sem código)
Renomear `espetaculos.nome` para o nome cru exato do dataset. Persiste com o código atual,
mas os nomes ficam em CAIXA ALTA no produto.

### Variante N:1 (cobertura total)
Tabela `espetaculo_item_dataset(item_dataset TEXT PK, espetaculo_id UUID)` mapeando os 447
shows → espetáculos. Cobre 100% das recs, mas a maioria dos mapeamentos é artificial.

## 7. Comandos úteis

```bash
# (rodar da raiz do repo; chave-back.key na raiz)
KEY="chave-back.key"; VM="ubuntu@163.176.204.208"

# Acesso ao banco (psql efêmero via Docker na VM; senha em .env.deploy):
ssh -i "$KEY" "$VM" 'sudo docker run --rm -e PGPASSWORD=<senha> postgres:16-alpine \
  psql -h 137.131.172.248 -U postgres -d aventura_db -c "SELECT COUNT(*) FROM recomendacoes;"'

# Status dos containers:
ssh -i "$KEY" "$VM" 'cd /home/ubuntu/aventura-recsys && sudo docker compose -f docker-compose.deploy.yml ps'

# Reprocessar o batch (após mexer no banco):
ssh -i "$KEY" "$VM" 'cd /home/ubuntu/aventura-recsys && sudo docker compose -f docker-compose.deploy.yml restart inference'

# Health interno:
ssh -i "$KEY" "$VM" 'curl -sk https://localhost/api/diagnostics/health'

# Re-deploy completo (build + envio + subida):
powershell -File deploy\deploy.ps1
```

## 8. Avisos
- Aplicar mudanças no banco em **transação**; `DELETE` de espetáculos é destrutivo (cascata).
- Acesso externo (80/443) seguirá bloqueado pela Security List da OCI — independente disto.
- O batch processa todos os user_ids do dataset (gera muito log; inofensivo). Próxima execução
  automática a cada 24h; para reprocessar na hora, reiniciar o container `inference`.
- Remover este arquivo e o plano antes do merge na `develop`.
```
