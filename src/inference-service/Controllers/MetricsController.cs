using InferenceService.DTOs;
using InferenceService.Services;
using InferenceService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InferenceService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class MetricsController : ControllerBase
{
    private readonly MetricsService _metrics;
    private readonly IChartService _charts;

    public MetricsController(MetricsService metrics, IChartService charts)
    {
        _metrics = metrics;
        _charts = charts;
    }

    /// <summary>
    /// Métricas do modelo híbrido: RMSE/MAE (score SVD bruto do model.zip) e
    /// ranking top-K (SVD puro × híbrido). A avaliação de ranking é amostrada em
    /// <paramref name="rankingUsers"/> usuários de teste (custo alto por usuário);
    /// o resultado é cacheado por parâmetro.
    /// </summary>
    [HttpGet]
    [ProducesResponseType<MetricsResponse>(StatusCodes.Status200OK)]
    public IActionResult GetMetrics([FromQuery] int k = 10, [FromQuery] int rankingUsers = 200)
    {
        if (k <= 0) return BadRequest(new { error = "k deve ser maior que zero." });
        if (rankingUsers <= 0) return BadRequest(new { error = "rankingUsers deve ser maior que zero." });

        var report = _metrics.Evaluate(k, rankingUsers);

        return Ok(new MetricsResponse
        {
            Alpha = report.Alpha,
            TopK = report.TopK,
            UsersEvaluated = report.UsersEvaluated,
            Rating = report.Rating,
            RankingSvd = report.SvdRanking,
            RankingHybrid = report.HybridRanking,
        });
    }

    /// <summary>Retorna PNG com histograma dos erros de predição (SVD bruto do modelo híbrido).</summary>
    [HttpGet("charts/error-histogram")]
    [Produces("image/png")]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    public IActionResult GetErrorHistogram()
    {
        var errors = _metrics.GetPredictionErrors();
        var png = _charts.GenerateErrorHistogram(errors, "Erros de Predição — Modelo Híbrido (SVD bruto)");
        return File(png, "image/png");
    }

    /// <summary>Retorna PNG de scatter plot ranking real × predito (SVD bruto do modelo híbrido).</summary>
    [HttpGet("charts/real-vs-predicted")]
    [Produces("image/png")]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    public IActionResult GetRealVsPredicted()
    {
        var (real, predicted) = _metrics.GetRealAndPredicted();
        var png = _charts.GenerateRealVsPredictedScatter(real, predicted, "Real vs Predito — Modelo Híbrido (SVD bruto)");
        return File(png, "image/png");
    }

    /// <summary>Retorna PNG comparando as métricas de ranking do SVD puro × híbrido.</summary>
    [HttpGet("charts/svd-vs-hybrid")]
    [Produces("image/png")]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    public IActionResult GetSvdVsHybrid([FromQuery] int k = 10, [FromQuery] int rankingUsers = 200)
    {
        if (k <= 0) return BadRequest(new { error = "k deve ser maior que zero." });
        if (rankingUsers <= 0) return BadRequest(new { error = "rankingUsers deve ser maior que zero." });

        var report = _metrics.Evaluate(k, rankingUsers);
        var png = _charts.GenerateRankingComparisonChart(report.SvdRanking, report.HybridRanking, report.TopK);
        return File(png, "image/png");
    }
}
