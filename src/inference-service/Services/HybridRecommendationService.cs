using InferenceService.Models;
using InferenceService.Services.Interfaces;

namespace InferenceService.Services;

/// <summary>
/// Adaptador fino sobre AventuraML.Recommender — a fórmula híbrida
/// (score = α·SVDnorm + (1−α)·afinidade de categoria) vive apenas em src/ml.
/// Cold-start (usuário sem histórico) cai no fallback por popularidade dentro
/// do próprio Recommender, com Score == null.
/// </summary>
public sealed class HybridRecommendationService : IRecommendationService
{
    public string ModelType => "hibrido";

    private readonly IHybridModelLoader _loader;

    public HybridRecommendationService(IHybridModelLoader loader)
    {
        _loader = loader;
    }

    public Task<IReadOnlyList<RecommendationResult>> RecommendAsync(int userId, int n = 10)
    {
        IReadOnlyList<RecommendationResult> result = _loader.Recommender
            .Recommend(userId, n)
            .Select(r => new RecommendationResult(r.Show, r.Score))
            .ToList();

        return Task.FromResult(result);
    }
}
