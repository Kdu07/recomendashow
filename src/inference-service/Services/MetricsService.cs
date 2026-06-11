using AventuraML;
using AventuraML.Models;
using InferenceService.Data;
using InferenceService.Services.Interfaces;

namespace InferenceService.Services;

/// <summary>
/// Métricas de avaliação do modelo híbrido sobre o model.zip carregado.
/// O cálculo vem da camada src/ml (Evaluator.EvaluateWithPredictor) — fonte única:
///   - RMSE/MAE usam o score SVD bruto (escala de rating 1–5) contra o ranking real;
///   - Precision@K, Recall@K, Hit Rate@K e Category Precision@K comparam o ranking
///     SVD puro × híbrido (α·SVDnorm + (1−α)·afinidade).
/// Split 80/20 por usuário com seed fixa; a avaliação de ranking é amostrada
/// (rankingUsers) porque custa ~1 predict por item candidato por usuário.
/// Resultados são cacheados por parâmetro — o modelo é estático no processo.
/// </summary>
public sealed class MetricsService
{
    private readonly IHybridModelLoader _loader;
    private readonly IDatasetRepository _dataset;
    private readonly object _cacheLock = new();
    private readonly Dictionary<(int TopK, int? RankingUsers), EvaluationReport> _cache = [];

    public MetricsService(IHybridModelLoader loader, IDatasetRepository dataset)
    {
        _loader = loader;
        _dataset = dataset;
    }

    public EvaluationReport Evaluate(int topK = 10, int? rankingUsers = 200)
    {
        var key = (topK, rankingUsers);
        lock (_cacheLock)
        {
            if (_cache.TryGetValue(key, out var cached))
                return cached;
        }

        var recommender = _loader.Recommender;
        var rows = MapRows(_dataset.AllInteractions);
        var (trainRows, testByUser) = Evaluator.SplitByUser(rows);

        var report = Evaluator.EvaluateWithPredictor(
            recommender.PredictRating,
            trainRows,
            testByUser,
            recommender.ItemCategories,
            recommender.CategoryAffinity,
            topK,
            recommender.Alpha,
            maxRankingUsers: rankingUsers);

        lock (_cacheLock)
        {
            _cache[key] = report;
        }

        return report;
    }

    public IReadOnlyList<double> GetPredictionErrors(int topK = 10, int? rankingUsers = 200) =>
        Evaluate(topK, rankingUsers).RatingPairs
            .Select(p => (double)(p.Real - p.Predicted))
            .ToList();

    public (IReadOnlyList<double> Real, IReadOnlyList<double> Predicted) GetRealAndPredicted(
        int topK = 10, int? rankingUsers = 200)
    {
        var pairs = Evaluate(topK, rankingUsers).RatingPairs;
        return (
            pairs.Select(p => (double)p.Real).ToList(),
            pairs.Select(p => (double)p.Predicted).ToList());
    }

    // ---------------------------------------------------------------------------

    private static List<CsvRatingRow> MapRows(IReadOnlyList<InteractionRecord> interactions) =>
        interactions
            .Select(r => new CsvRatingRow
            {
                UserId = r.UserId,
                ItemId = r.ItemId,
                Ranking = r.Ranking,
                ValorMedioIngresso = (float)r.ValorMedioIngresso,
                TipoEvento = r.TipoEvento,
            })
            .ToList();
}
