using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Mappings;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Services
{
    public sealed class StoredRecommendationStrategy : IRecommendationStrategy
    {
        private readonly IRecomendacaoRepository _recomendacaoRepository;

        public StoredRecommendationStrategy(IRecomendacaoRepository recomendacaoRepository)
        {
            _recomendacaoRepository = recomendacaoRepository;
        }

        public string Name => "stored";

        public IReadOnlyList<RecomendacaoItemDto> GetRecommendations(Guid usuarioId, int limit)
        {
            return _recomendacaoRepository
                .GetByUsuario(usuarioId, limit)
                .Select(recomendacao => recomendacao.ToDto())
                .ToList();
        }
    }
}
