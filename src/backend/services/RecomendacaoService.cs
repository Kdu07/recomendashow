using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Dto;

namespace BackEnd.Aventura.Services
{
    public class RecomendacaoService : IRecomendacaoService
    {
        private readonly IReadOnlyList<IRecommendationStrategy> _strategies;

        public RecomendacaoService(IEnumerable<IRecommendationStrategy> strategies)
        {
            _strategies = strategies?.ToList() ?? throw new ArgumentNullException(nameof(strategies));
            if (_strategies.Count == 0)
            {
                throw new InvalidOperationException("Ao menos uma estrategia de recomendacao deve ser registrada.");
            }
        }

        public IReadOnlyList<RecomendacaoItemDto> GetRecomendacoes(Guid usuarioId, int limit)
        {
            EntityGuards.NotEmpty(usuarioId, nameof(usuarioId));
            if (limit <= 0 || limit > 50)
            {
                throw new ArgumentException("Limit deve estar entre 1 e 50.");
            }

            foreach (var strategy in _strategies)
            {
                var recommendations = strategy.GetRecommendations(usuarioId, limit);
                if (recommendations.Count > 0)
                {
                    return recommendations;
                }
            }

            return Array.Empty<RecomendacaoItemDto>();
        }
    }
}
