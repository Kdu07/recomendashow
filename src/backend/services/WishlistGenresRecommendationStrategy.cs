using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Mappings;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Services
{
    public sealed class WishlistGenresRecommendationStrategy : IRecommendationStrategy
    {
        private readonly IEspetaculoRepository _espetaculoRepository;

        public WishlistGenresRecommendationStrategy(IEspetaculoRepository espetaculoRepository)
        {
            _espetaculoRepository = espetaculoRepository;
        }

        public string Name => "wishlist-genres";

        public IReadOnlyList<RecomendacaoItemDto> GetRecommendations(Guid usuarioId, int limit)
        {
            return _espetaculoRepository
                .GetByGenerosDaWishlist(usuarioId, limit)
                .Select((espetaculo, index) => new RecomendacaoItemDto
                {
                    Espetaculo = espetaculo.ToDto(),
                    Score = Math.Max(0.5m, 1.0m - index * 0.05m),
                    Motivo = "Baseado nos itens da sua wishlist"
                })
                .ToList();
        }
    }
}
