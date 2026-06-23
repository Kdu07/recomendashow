using BackEnd.Aventura.Dto;

namespace BackEnd.Aventura.Services
{
    public sealed class WishlistRecommendationPostProcessor : IWishlistRecommendationPostProcessor
    {
        // Multiplicador suave: favorece itens do mesmo gênero da wishlist sem inverter
        // completamente o ranking original da estratégia.
        private const decimal BoostFactor = 1.15m;
        private const string BoostMotivo = "Combina com a sua wishlist";

        public IReadOnlyList<RecomendacaoItemDto> Apply(
            IReadOnlyList<RecomendacaoItemDto> recomendacoes,
            ISet<Guid> wishlistEspetaculoIds,
            ISet<string> wishlistGeneros,
            int limit)
        {
            if (recomendacoes.Count == 0)
            {
                return recomendacoes;
            }

            var resultado = wishlistEspetaculoIds.Count > 0
                ? recomendacoes.Where(r => !wishlistEspetaculoIds.Contains(r.Espetaculo.Id)).ToList()
                : recomendacoes.ToList();

            if (wishlistGeneros.Count > 0)
            {
                // Comparação case-insensitive: o gênero pode vir com caixa diferente entre
                // a wishlist e a recomendação (ambos saem do banco, mas sem garantia de caixa).
                var generos = new HashSet<string>(wishlistGeneros, StringComparer.OrdinalIgnoreCase);

                foreach (var item in resultado)
                {
                    if (generos.Contains(item.Espetaculo.Genero))
                    {
                        item.Score *= BoostFactor;
                        item.Motivo = BoostMotivo;
                    }
                }

                // OrderBy do LINQ é estável: itens sem boost preservam a ordem original entre si.
                return resultado.OrderByDescending(r => r.Score).Take(limit).ToList();
            }

            return resultado.Take(limit).ToList();
        }
    }
}
