using AventuraML.Models;

namespace InferenceService.DTOs;

/// <summary>
/// Métricas do modelo híbrido (model.zip): RMSE/MAE sobre o score SVD bruto e
/// métricas de ranking comparando SVD puro × híbrido.
/// </summary>
public sealed class MetricsResponse
{
    public string ModelType { get; init; } = "hibrido";
    public float Alpha { get; init; }
    public int TopK { get; init; }
    public int UsersEvaluated { get; init; }
    public required RatingAccuracy Rating { get; init; }
    public required RankingMetrics RankingSvd { get; init; }
    public required RankingMetrics RankingHybrid { get; init; }
}
