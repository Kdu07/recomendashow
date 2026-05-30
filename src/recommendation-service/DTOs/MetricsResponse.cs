using RecommendationService.Models;

namespace RecommendationService.DTOs;

public sealed class MetricsResponse
{
    public EvaluationMetrics Svd { get; init; } = new();
}
