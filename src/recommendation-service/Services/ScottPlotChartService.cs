using RecommendationService.Models;
using RecommendationService.Services.Interfaces;
using ScottPlot;

namespace RecommendationService.Services;

/// <summary>
/// Gera gráficos PNG via ScottPlot.
/// Substitui as chamadas matplotlib/seaborn dos notebooks Jupyter.
/// </summary>
public sealed class ScottPlotChartService : IChartService
{
    private const int Width = 800;
    private const int Height = 500;

    /// <summary>
    /// Histograma dos erros de predição (real − predito).
    /// Substitui o ax.hist() das células de visualização do notebook 04.
    /// </summary>
    public byte[] GenerateErrorHistogram(IReadOnlyList<double> errors, string title)
    {
        var plt = new Plot();
        plt.Title(title);
        plt.XLabel("Erro (real − predito)");
        plt.YLabel("Frequência");

        double[] data = [.. errors];
        var hist = ScottPlot.Statistics.Histogram.WithBinCount(30, data);
        plt.Add.Histogram(hist);

        return plt.GetImageBytes(Width, Height, ImageFormat.Png);
    }

    /// <summary>
    /// Scatter plot de rankings reais × preditos.
    /// Substitui o ax.scatter() do notebook 04.
    /// </summary>
    public byte[] GenerateRealVsPredictedScatter(
        IReadOnlyList<double> real,
        IReadOnlyList<double> predicted,
        string title)
    {
        var plt = new Plot();
        plt.Title(title);
        plt.XLabel("Ranking Real");
        plt.YLabel("Ranking Predito");

        double[] x = [.. real];
        double[] y = [.. predicted];
        var scatter = plt.Add.Scatter(x, y);
        scatter.MarkerSize = 3;
        scatter.LineWidth = 0;

        // Linha de referência y=x (predição perfeita)
        double min = Math.Min(x.Min(), y.Min());
        double max = Math.Max(x.Max(), y.Max());
        plt.Add.Line(min, min, max, max);

        return plt.GetImageBytes(Width, Height, ImageFormat.Png);
    }

    /// <summary>
    /// Gráfico de barras comparando RMSE, MAE e F1@K de SVD × KNN.
    /// Substitui o ax.bar() do notebook 04.
    /// </summary>
    public byte[] GenerateMetricsComparisonChart(EvaluationMetrics svd, EvaluationMetrics knn)
    {
        var plt = new Plot();
        plt.Title("Comparação de Métricas: SVD × KNN");
        plt.YLabel("Valor");

        string[] metricNames = ["RMSE", "MAE", "F1@K"];
        double[] svdValues = [svd.Rmse, svd.Mae, svd.F1AtK];
        double[] knnValues = [knn.Rmse, knn.Mae, knn.F1AtK];

        for (int i = 0; i < metricNames.Length; i++)
        {
            double x = i * 3;
            var barSvd = plt.Add.Bar(x, svdValues[i]);
            if (i == 0) barSvd.LegendText = "SVD";

            var barKnn = plt.Add.Bar(x + 1, knnValues[i]);
            if (i == 0) barKnn.LegendText = "KNN";
        }

        plt.Legend.IsVisible = true;
        return plt.GetImageBytes(Width, Height, ImageFormat.Png);
    }
}
