using RecommendationService.Models;

namespace RecommendationService.Data;

public interface IRecommendationWriteRepository
{
    
    Task SaveAsync(
        int userId,
        IReadOnlyList<RecommendationResult> recommendations,
        string model,
        CancellationToken cancellationToken = default);
}
