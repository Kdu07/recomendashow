using BackEnd.Aventura.Dto;

namespace BackEnd.Aventura.Services
{
    public interface IRecommendationStrategy
    {
        string Name { get; }

        IReadOnlyList<RecomendacaoItemDto> GetRecommendations(Guid usuarioId, int limit);
    }
}
