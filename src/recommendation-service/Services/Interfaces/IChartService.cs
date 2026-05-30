using RecommendationService.Models;

namespace RecommendationService.Services.Interfaces;

public interface IChartService
{
    /// <summary>Histograma dos erros de predição (real − predito).</summary>
    byte[] GenerateErrorHistogram(IReadOnlyList<double> errors, string title);

    /// <summary>Scatter plot de rankings reais vs. preditos.</summary>
    byte[] GenerateRealVsPredictedScatter(
        IReadOnlyList<double> real,
        IReadOnlyList<double> predicted,
        string title);

    /// <summary>Gráfico de barras comparando métricas SVD × KNN.</summary>
    byte[] GenerateMetricsComparisonChart(EvaluationMetrics svd, EvaluationMetrics knn);
}
