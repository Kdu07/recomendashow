using RecommendationService.DTOs;
using RecommendationService.Services;
using RecommendationService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace RecommendationService.Controllers;

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

    /// <summary>Retorna RMSE, MAE, Precision@K, Recall@K e F1@K para o modelo SVD.</summary>
    [HttpGet]
    [ProducesResponseType<MetricsResponse>(StatusCodes.Status200OK)]
    public IActionResult GetMetrics([FromQuery] int k = 1)
    {
        return Ok(new MetricsResponse { Svd = _metrics.EvaluateSvd(k) });
    }

    /// <summary>Retorna PNG com histograma dos erros de predição do SVD.</summary>
    [HttpGet("charts/error-histogram")]
    [Produces("image/png")]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    public IActionResult GetErrorHistogram()
    {
        var errors = _metrics.GetPredictionErrors();
        var png = _charts.GenerateErrorHistogram(errors, "Erros de Predição — SVD");
        return File(png, "image/png");
    }

    /// <summary>Retorna PNG de scatter plot ranking real × predito (SVD).</summary>
    [HttpGet("charts/real-vs-predicted")]
    [Produces("image/png")]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    public IActionResult GetRealVsPredicted()
    {
        var (real, predicted) = _metrics.GetRealAndPredicted();
        var png = _charts.GenerateRealVsPredictedScatter(real, predicted, "Real vs Predito — SVD");
        return File(png, "image/png");
    }
}
