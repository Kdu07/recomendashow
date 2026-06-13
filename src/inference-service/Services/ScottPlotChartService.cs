using AventuraML.Models;
using InferenceService.Services.Interfaces;
using ScottPlot;

namespace InferenceService.Services;

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
    /// Gráfico de barras comparando as métricas de ranking do SVD puro × híbrido.
    /// </summary>
    public byte[] GenerateRankingComparisonChart(RankingMetrics svd, RankingMetrics hybrid, int topK)
    {
        var plt = new Plot();
        plt.Title($"Ranking top-{topK}: SVD puro × Híbrido");
        plt.YLabel("Valor");

        string[] metricNames = [$"HitRate@{topK}", $"Precision@{topK}", $"Recall@{topK}", $"CatPrecision@{topK}"];
        double[] svdValues = [svd.HitRateAtK, svd.PrecisionAtK, svd.RecallAtK, svd.CategoryPrecisionAtK];
        double[] hybridValues = [hybrid.HitRateAtK, hybrid.PrecisionAtK, hybrid.RecallAtK, hybrid.CategoryPrecisionAtK];

        var ticks = new List<Tick>();
        for (int i = 0; i < metricNames.Length; i++)
        {
            double x = i * 3;
            var barSvd = plt.Add.Bar(x, svdValues[i]);
            if (i == 0) barSvd.LegendText = "SVD puro";

            var barHybrid = plt.Add.Bar(x + 1, hybridValues[i]);
            if (i == 0) barHybrid.LegendText = "Híbrido";

            ticks.Add(new Tick(x + 0.5, metricNames[i]));
        }

        plt.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual([.. ticks]);
        plt.Legend.IsVisible = true;
        return plt.GetImageBytes(Width, Height, ImageFormat.Png);
    }
}
