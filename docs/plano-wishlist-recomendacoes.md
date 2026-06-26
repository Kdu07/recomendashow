# Plano — Caminho A: wishlist como sinal de afinidade no backend

> **Nota:** Documento de trabalho temporário desta branch (`feat/wishlist-interacoes`).
> Deve ser removido — junto com referências a ele — antes do merge na `develop`.

## Objetivo e princípios

Usar a wishlist para melhorar as recomendações **sem tocar no pipeline de ML**
(`src/ml`, `src/inference-service`) e **sem depender do mapeamento de identidade
UUID↔modelo**. Tudo roda no backend, funciona para qualquer usuário real e é reversível.

Três efeitos, entregues em fases independentes:

1. **Exclusão** — nunca recomendar o que o usuário já salvou (ganho de UX imediato, risco mínimo).
2. **Fallback por wishlist** — quando não há recs do modelo, recomendar por gêneros dos itens salvos.
3. **Re-ranking (boost)** — dar peso a recomendações cujo gênero combina com a wishlist.

## Contexto da arquitetura atual

- `RecomendacaoService` percorre as `IRecommendationStrategy` **na ordem de registro**
  e devolve a primeira lista não-vazia (chain of responsibility).
  Cadeia atual: `stored` → `favorite-genres`.
- A wishlist é salva em `wishlist_curtidas (usuario_id, espetaculo_id, tipo='wishlist')`,
  sinal **binário** (sem timestamp, sem peso).
- O modelo de ML treina exclusivamente do `dataset.csv` (compras Sympla) e **não lê**
  `wishlist_curtidas` nem `interacoes`.

## Decisões de design

- **Exclusão + boost** ficam num pós-processador dedicado
  (`WishlistRecommendationPostProcessor`), função pura aplicada ao resultado da estratégia
  vencedora. Mantém as estratégias intactas e o pós-processo testável isoladamente.
- **Fallback** é uma nova `IRecommendationStrategy` (`WishlistGenresRecommendationStrategy`),
  registrada **entre** `stored` e `favorite-genres`.
- **Boost** = multiplicador suave sobre o `Score` por gênero em comum, reordenando por `Score desc`.
- **Wishlist vazia** → no-ops.
- **Buffer de limit:** o pós-processo pode encolher a lista; o `RecomendacaoService`
  pede `limit + tamanho-da-wishlist` às estratégias e trunca para `limit` ao final.

## Fase 0 — Camada de dados *(implementada)*

- `IWishlistRepository.GetItemIds` **já existia** — reusado para exclusão.
- Novo `IEspetaculoRepository.GetByGenerosDaWishlist(usuarioId, limit)`, espelhando
  `GetByGenerosFavoritos` (reusa `EspetaculoQueries`). Recomenda espetáculos de gêneros
  presentes na wishlist do usuário, excluindo os já salvos. Base para a Fase 2.

## Fase 1 — Exclusão *(implementada)*

- `IWishlistRecommendationPostProcessor` + `WishlistRecommendationPostProcessor`: recebe a lista
  de recomendações + os IDs da wishlist + o limit; remove itens já salvos e trunca para o limit.
- `RecomendacaoService` injeta `IWishlistRepository` e o pós-processador; busca os IDs uma vez,
  usa-os como buffer e aplica a exclusão ao resultado da estratégia vencedora.
- Registro em `DependencyInjection`.

## Fase 2 — Fallback por gêneros da wishlist *(implementada)*

- `WishlistGenresRecommendationStrategy : IRecommendationStrategy`
  (`Name => "wishlist-genres"`), análoga a `FavoriteGenresRecommendationStrategy`,
  usando `GetByGenerosDaWishlist`. `Motivo = "Baseado nos itens da sua wishlist"`.
- Registrada na ordem: `stored` → `wishlist-genres` → `favorite-genres`.

## Fase 3 — Boost / re-ranking *(implementada)*

- `IWishlistRepository.GetGenerosDaWishlist(usuarioId)`: gêneros efetivos dos itens salvos
  (nome do gênero + `tipo_evento` como fallback, alinhado ao COALESCE de `EspetaculoQueries`).
- `WishlistRecommendationPostProcessor` estendido: além de excluir, aplica multiplicador de
  `Score` (x1.15) aos itens cujo `Genero` está na wishlist, ajusta o `Motivo` para
  "Combina com a sua wishlist" e reordena por `Score desc` (OrderBy estável).
- `RecomendacaoService` busca os gêneros da wishlist (só quando há itens salvos) e os repassa.

## Testes

- **Unitários** (`RecomendacaoServiceTests`, com fakes em memória): wishlist vazia (no-op),
  exclusão efetiva, re-preenchimento até o limit via buffer, precedência da cadeia
  (`stored` > `wishlist-genres` > `favorite-genres`), boost reordenando por gênero e
  match de gênero **case-insensitive**. `DependencyInjectionTests` cobre o novo registro.
- **Integração** (`WishlistRecommendationQueriesTests`, Testcontainers/Postgres): exercita o
  SQL real de `GetByGenerosDaWishlist` (mesmo gênero + exclusão dos salvos + wishlist vazia)
  e `GetGenerosDaWishlist` (gênero nomeado + fallback `tipo_evento`). Requer Docker para rodar.

## Lacunas conhecidas

- O boost compara apenas o gênero **principal** do `EspetaculoDto` (o `COALESCE` traz só um);
  espetáculos multi-gênero são considerados pelo primeiro gênero apenas.
- `Score` pós-boost pode exceder 1.0 (não há cap; só a ordem relativa importa hoje).
- Não há teste end-to-end (HTTP) cobrindo o pós-processamento via `GET /api/recomendacao`.

## Considerações

- **Sem migração de banco** — apenas leitura de tabelas existentes.
- **Performance:** +1 query de wishlist por request (pequena, coberta pela PK).
- **Não conflita com o caminho C** (treino do SVD) no futuro — camadas complementares.
