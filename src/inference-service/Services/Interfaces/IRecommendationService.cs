using InferenceService.Models;

namespace InferenceService.Services.Interfaces;

public interface IRecommendationService
{
    /// <summary>
    /// Retorna top-n recomendações para um usuário.
    /// score == null indica cold-start (fallback por popularidade).
    /// Espelha a assinatura de recomendar(user_id, n) em inference.py.
    /// </summary>
    Task<IReadOnlyList<RecommendationResult>> RecommendAsync(int userId, int n = 10);

    /// <summary>"svd" ou "knn"</summary>
    string ModelType { get; }
}
