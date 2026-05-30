using RecommendationService.Data;
using RecommendationService.DTOs;
using RecommendationService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace RecommendationService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class DiagnosticsController : ControllerBase
{
    private readonly IModelLoaderService _loader;
    private readonly IDatasetRepository _dataset;

    public DiagnosticsController(IModelLoaderService loader, IDatasetRepository dataset)
    {
        _loader = loader;
        _dataset = dataset;
    }

    /// <summary>Health check — retorna 200 OK quando o modelo está pronto.</summary>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public IActionResult Health()
    {
        if (!_loader.IsLoaded)
            return StatusCode(StatusCodes.Status503ServiceUnavailable,
                new { status = "loading", message = "Modelo ainda sendo carregado." });

        return Ok(new { status = "ok", message = "Modelo carregado e pronto." });
    }

    /// <summary>Retorna hiperparâmetros do modelo SVD e informações do dataset.</summary>
    [HttpGet("model-info")]
    [ProducesResponseType<ModelInfoResponse>(StatusCodes.Status200OK)]
    public IActionResult ModelInfo()
    {
        var svd = _loader.SvdWeights;

        return Ok(new ModelInfoResponse
        {
            Svd = new SvdInfoDto
            {
                NUsers = svd.NUsers,
                NItems = svd.NItems,
                NFactors = svd.Hyperparameters.NFactors,
                LrAll = svd.Hyperparameters.LrAll,
                RegAll = svd.Hyperparameters.RegAll,
                NEpochs = svd.Hyperparameters.NEpochs,
                GlobalMean = svd.GlobalMean,
                RatingScale = svd.RatingScale,
            },
            Dataset = new DatasetInfoDto
            {
                TotalInteractions = _dataset.AllInteractions.Count,
                TotalItems = _dataset.AllItems.Count,
                ColdStartItemsCount = _dataset.ColdStartItems.Count,
            },
        });
    }
}
