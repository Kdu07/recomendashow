using InferenceService.Data;
using InferenceService.DTOs;
using InferenceService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InferenceService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class DiagnosticsController : ControllerBase
{
    private readonly IHybridModelLoader _loader;
    private readonly IDatasetRepository _dataset;

    public DiagnosticsController(IHybridModelLoader loader, IDatasetRepository dataset)
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

    /// <summary>Retorna metadados do modelo híbrido e informações do dataset.</summary>
    [HttpGet("model-info")]
    [ProducesResponseType<ModelInfoResponse>(StatusCodes.Status200OK)]
    public IActionResult ModelInfo()
    {
        var recommender = _loader.Recommender;

        return Ok(new ModelInfoResponse
        {
            Model = new HybridModelInfoDto
            {
                Alpha = recommender.Alpha,
                KnownUsers = recommender.KnownUserCount,
                CatalogItems = recommender.CatalogSize,
                LatentFactors = recommender.LatentFactors,
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
