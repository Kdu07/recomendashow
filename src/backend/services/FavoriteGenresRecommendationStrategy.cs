using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Mappings;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Services
{
    public sealed class FavoriteGenresRecommendationStrategy : IRecommendationStrategy
    {
        private readonly IEspetaculoRepository _espetaculoRepository;

        public FavoriteGenresRecommendationStrategy(IEspetaculoRepository espetaculoRepository)
        {
            _espetaculoRepository = espetaculoRepository;
        }

        public string Name => "favorite-genres";

        public IReadOnlyList<RecomendacaoItemDto> GetRecommendations(Guid usuarioId, int limit)
        {
            return _espetaculoRepository
                .GetByGenerosFavoritos(usuarioId, limit)
                .Select((espetaculo, index) => new RecomendacaoItemDto
                {
                    Espetaculo = espetaculo.ToDto(),
                    Score = Math.Max(0.5m, 1.0m - index * 0.05m),
                    Motivo = "Baseado nos seus g\u00eaneros favoritos"
                })
                .ToList();
        }
    }
}
