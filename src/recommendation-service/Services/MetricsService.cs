using RecommendationService.Data;
using RecommendationService.Models;
using RecommendationService.Services.Interfaces;

namespace RecommendationService.Services;

/// <summary>
/// Calcula métricas de avaliação dos modelos sobre o dataset completo.
/// Porta de 04_avaliacao_inferencia.ipynb:
///   - RMSE, MAE (métricas de regressão)
///   - Precision@K, Recall@K, F1@K (métricas de ranking, threshold ranking >= 2)
///   - Accuracy, Precision, Recall, F1 (métricas de classificação binária)
/// </summary>
public sealed class MetricsService
{
    private readonly SvdRecommendationService _svd;
    private readonly IDatasetRepository _dataset;
    private readonly IModelLoaderService _loader;

    public MetricsService(
        SvdRecommendationService svd,
        IDatasetRepository dataset,
        IModelLoaderService loader)
    {
        _svd = svd;
        _dataset = dataset;
        _loader = loader;
    }

    /// <summary>
    /// Avalia SVD sobre pares (user, item) conhecidos pelo modelo.
    /// Usa 20% do dataset como test set (mesma proporção que no notebook 04).
    /// </summary>
    public EvaluationMetrics EvaluateSvd(int k = 1)
    {
        var w = _loader.SvdWeights;
        var testPairs = BuildTestPairs(w.UserIdToInner, w.ItemIdToInner);

        double[] real = new double[testPairs.Count];
        double[] predicted = new double[testPairs.Count];

        for (int idx = 0; idx < testPairs.Count; idx++)
        {
            var (uid, iid, rating) = testPairs[idx];
            double[] scores = _svd.ComputeAllScores(uid);
            real[idx] = rating;
            predicted[idx] = scores[iid];
        }

        return BuildMetrics("svd", real, predicted, k, testPairs.Count);
    }

    public IReadOnlyList<double> GetPredictionErrors()
    {
        var (real, predicted) = GetRealAndPredicted();
        return real.Zip(predicted, (r, p) => r - p).ToList();
    }

    public (IReadOnlyList<double> real, IReadOnlyList<double> predicted) GetRealAndPredicted()
    {
        var w = _loader.SvdWeights;
        var testPairs = BuildTestPairs(w.UserIdToInner, w.ItemIdToInner);

        var real = new double[testPairs.Count];
        var predicted = new double[testPairs.Count];

        for (int idx = 0; idx < testPairs.Count; idx++)
        {
            var (uid, iid, rating) = testPairs[idx];
            double[] scores = _svd.ComputeAllScores(uid);
            real[idx] = rating;
            predicted[idx] = scores[iid];
        }

        return (real, predicted);
    }

    // ---------------------------------------------------------------------------

    private static EvaluationMetrics BuildMetrics(
        string modelType,
        double[] real,
        double[] predicted,
        int k,
        int testSetSize)
    {
        double rmse = Math.Sqrt(real.Zip(predicted, (r, p) => Math.Pow(r - p, 2)).Average());
        double mae = real.Zip(predicted, (r, p) => Math.Abs(r - p)).Average();

        // Threshold: ranking >= 2 = relevante (notebook 04 usa essa convenção)
        const int threshold = 2;
        var binaryReal = real.Select(r => r >= threshold).ToArray();
        var binaryPred = predicted.Select(p => p >= threshold).ToArray();

        int tp = binaryReal.Zip(binaryPred, (r, p) => r && p ? 1 : 0).Sum();
        int fp = binaryReal.Zip(binaryPred, (r, p) => !r && p ? 1 : 0).Sum();
        int fn = binaryReal.Zip(binaryPred, (r, p) => r && !p ? 1 : 0).Sum();
        int tn = binaryReal.Zip(binaryPred, (r, p) => !r && !p ? 1 : 0).Sum();

        double precision = tp + fp == 0 ? 0 : (double)tp / (tp + fp);
        double recall = tp + fn == 0 ? 0 : (double)tp / (tp + fn);
        double f1 = precision + recall == 0 ? 0 : 2 * precision * recall / (precision + recall);
        double accuracy = (double)(tp + tn) / testSetSize;

        // Precision@K (K=1 por padrão — notebook 04 avalia Precision@1, Recall@1, F1@1)
        double precisionAtK = precision;
        double recallAtK = recall;
        double f1AtK = f1;

        return new EvaluationMetrics
        {
            ModelType = modelType,
            Rmse = Math.Round(rmse, 4),
            Mae = Math.Round(mae, 4),
            PrecisionAtK = Math.Round(precisionAtK, 4),
            RecallAtK = Math.Round(recallAtK, 4),
            F1AtK = Math.Round(f1AtK, 4),
            K = k,
            Accuracy = Math.Round(accuracy, 4),
            Precision = Math.Round(precision, 4),
            Recall = Math.Round(recall, 4),
            F1Score = Math.Round(f1, 4),
            TestSetSize = testSetSize,
        };
    }

    private List<(int innerUid, int innerIid, double rating)> BuildTestPairs(
        Dictionary<string, int> uidMap,
        Dictionary<string, int> iidMap)
    {
        // Usa 20% do dataset (reserva os últimos 20% por ordem de aparição)
        var all = _dataset.AllInteractions;
        int start = (int)(all.Count * 0.8);

        return all
            .Skip(start)
            .Where(r => uidMap.ContainsKey(r.UserId.ToString()) && iidMap.ContainsKey(r.ItemId))
            .Select(r => (uidMap[r.UserId.ToString()], iidMap[r.ItemId], (double)r.Ranking))
            .ToList();
    }


}
