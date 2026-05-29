using RecommendationService.Data;
using RecommendationService.Models;
using RecommendationService.Services.Interfaces;

namespace RecommendationService.Services;

/// <summary>
/// Recomendação via Funk SVD.
/// Porta direta de inference.py:
///   - ComputeAllScores → linhas 78–83 (global_mean + bu[uid] + bi + qi @ pu[uid], clipped)
///   - RecommendAsync   → função recomendar() com tratamento de cold-start
/// Fórmula: r̂_ui = μ + b_u + b_i + p_u · q_i^T
/// </summary>
public sealed class SvdRecommendationService : IRecommendationService
{
    public string ModelType => "svd";

    private readonly SvdWeights _w;
    private readonly IDatasetRepository _dataset;
    private readonly ILogger<SvdRecommendationService> _logger;

    // Cache de lookup: inner index (int) → item name
    private readonly string[] _innerToItem;

    public SvdRecommendationService(
        IModelLoaderService loader,
        IDatasetRepository dataset,
        ILogger<SvdRecommendationService> logger)
    {
        _w = loader.SvdWeights;
        _dataset = dataset;
        _logger = logger;

        // Constrói array indexado para O(1) lookup em ComputeAllScores
        _innerToItem = new string[_w.NItems];
        foreach (var (innerStr, name) in _w.InnerToItemId)
            _innerToItem[int.Parse(innerStr)] = name;
    }

    public Task<IReadOnlyList<RecommendationResult>> RecommendAsync(int userId, int n = 10)
    {
        if (n <= 0) throw new ArgumentOutOfRangeException(nameof(n), "n deve ser maior que zero.");

        var consumed = _dataset.GetConsumedItems(userId);

        // Cold-start: usuário desconhecido ou sem histórico (inference.py linhas 64–74)
        if (consumed.Count == 0 || !TryGetInnerUserId(userId, out int innerUid))
        {
            _logger.LogDebug("Cold-start para userId={UserId}.", userId);
            IReadOnlyList<RecommendationResult> coldResult = _dataset.ColdStartItems
                .Where(item => !consumed.Contains(item))
                .Take(n)
                .Select(item => new RecommendationResult(item, Score: null))
                .ToList();
            return Task.FromResult(coldResult);
        }

        double[] allScores = ComputeAllScores(innerUid);
        double minRating = _w.RatingScale[0];
        double maxRating = _w.RatingScale[1];

        IReadOnlyList<RecommendationResult> result = Enumerable.Range(0, _w.NItems)
            .Where(i => !consumed.Contains(_innerToItem[i]))
            .Select(i => new RecommendationResult(_innerToItem[i], Math.Round(allScores[i], 4)))
            .OrderByDescending(r => r.Score)
            .Take(n)
            .ToList();

        return Task.FromResult(result);
    }

    /// <summary>
    /// Calcula scores para todos os itens de um usuário em um único passo vetorizado.
    /// Espelha inference.py linhas 78–83:
    ///   all_scores = (global_mean + bu[inner_uid] + bi + qi @ pu[inner_uid]).clip(low, high)
    /// </summary>
    internal double[] ComputeAllScores(int innerUserId)
    {
        double globalMean = _w.GlobalMean;
        double userBias = _w.Bu[innerUserId];
        double[] userFactors = _w.Pu[innerUserId];
        int nFactors = userFactors.Length;
        double low = _w.RatingScale[0];
        double high = _w.RatingScale[1];

        double[] scores = new double[_w.NItems];

        for (int i = 0; i < _w.NItems; i++)
        {
            double[] itemFactors = _w.Qi[i];
            double dot = 0.0;
            for (int f = 0; f < nFactors; f++)
                dot += itemFactors[f] * userFactors[f];

            scores[i] = Math.Clamp(globalMean + userBias + _w.Bi[i] + dot, low, high);
        }

        return scores;
    }

    internal bool TryGetInnerUserId(int userId, out int innerUserId)
    {
        return _w.UserIdToInner.TryGetValue(userId.ToString(), out innerUserId);
    }
}
