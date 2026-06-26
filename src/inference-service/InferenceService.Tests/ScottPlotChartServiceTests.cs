using AventuraML.Models;
using FluentAssertions;
using InferenceService.Services;
using Xunit;

namespace InferenceService.Tests;

/// <summary>
/// Testes do ScottPlotChartService: garante que cada gerador produz um PNG
/// válido (assinatura de 8 bytes do formato) e não-vazio. Cobre os três
/// gráficos que substituem as visualizações matplotlib dos notebooks.
/// </summary>
public class ScottPlotChartServiceTests
{
    // Assinatura mágica do formato PNG: 0x89 'P' 'N' 'G' 0x0D 0x0A 0x1A 0x0A.
    private static readonly byte[] PngSignature =
        [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    private readonly ScottPlotChartService _service = new();

    private static void ShouldBeValidPng(byte[] image)
    {
        image.Should().NotBeNullOrEmpty();
        image.Length.Should().BeGreaterThan(PngSignature.Length);
        image.Take(PngSignature.Length).Should().Equal(PngSignature);
    }

    [Fact]
    public void GenerateErrorHistogram_ComErros_RetornaPngValido()
    {
        double[] errors = [-2.0, -1.5, -1.0, -0.5, 0.0, 0.0, 0.5, 1.0, 1.0, 1.5, 2.0, 2.5];

        var image = _service.GenerateErrorHistogram(errors, "Distribuição de Erros");

        ShouldBeValidPng(image);
    }

    [Fact]
    public void GenerateRealVsPredictedScatter_ComPontos_RetornaPngValido()
    {
        double[] real = [1.0, 2.0, 3.0, 4.0, 5.0];
        double[] predicted = [1.2, 1.9, 3.3, 3.8, 4.7];

        var image = _service.GenerateRealVsPredictedScatter(real, predicted, "Real × Predito");

        ShouldBeValidPng(image);
    }

    [Fact]
    public void GenerateRankingComparisonChart_ComMetricas_RetornaPngValido()
    {
        var svd = new RankingMetrics(HitRateAtK: 0.60, PrecisionAtK: 0.40, RecallAtK: 0.30, CategoryPrecisionAtK: 0.50);
        var hybrid = new RankingMetrics(HitRateAtK: 0.75, PrecisionAtK: 0.55, RecallAtK: 0.45, CategoryPrecisionAtK: 0.65);

        var image = _service.GenerateRankingComparisonChart(svd, hybrid, topK: 10);

        ShouldBeValidPng(image);
    }

    [Fact]
    public void GenerateRankingComparisonChart_MetricasIguais_RetornaPngValido()
    {
        var metrics = new RankingMetrics(0.5, 0.5, 0.5, 0.5);

        var image = _service.GenerateRankingComparisonChart(metrics, metrics, topK: 5);

        ShouldBeValidPng(image);
    }
}
