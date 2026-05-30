using RecommendationService.Models;

namespace RecommendationService.DTOs;

public sealed class RecommendationResponse
{
    public int UserId { get; init; }
    public string Model { get; init; } = string.Empty;
    public bool ColdStart { get; init; }
    public IReadOnlyList<RecommendationResult> Recommendations { get; init; } = [];
}
