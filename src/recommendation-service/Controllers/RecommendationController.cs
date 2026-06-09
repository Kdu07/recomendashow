using Microsoft.AspNetCore.Mvc;
using RecommendationService.Data;
using RecommendationService.DTOs;
using RecommendationService.Services.Interfaces;

namespace RecommendationService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class RecommendationController : ControllerBase
{
    private readonly IRecommendationService _service;
    private readonly IRecommendationWriteRepository _recommendationWriteRepository;
    private readonly ILogger<RecommendationController> _logger;

    public RecommendationController(
        IRecommendationService service,
        IRecommendationWriteRepository recommendationWriteRepository,
        ILogger<RecommendationController> logger)
    {
        _service = service;
        _recommendationWriteRepository = recommendationWriteRepository;
        _logger = logger;
    }

    
    [HttpGet("{userId:int}")]
    [ProducesResponseType<RecommendationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRecommendations(int userId, [FromQuery] int n = 10)
    {
        if (n <= 0) return BadRequest(new { error = "n deve ser maior que zero." });

        var recs = await _service.RecommendAsync(userId, n);
        bool isColdStart = recs.Any() && recs[0].Score is null;

        try
        {
            await _recommendationWriteRepository.SaveAsync(userId, recs, _service.ModelType);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Falha controlada ao persistir recomendações para userId={UserId}. A resposta calculada será preservada.",
                userId);
        }

        return Ok(new RecommendationResponse
        {
            UserId = userId,
            Model = _service.ModelType,
            ColdStart = isColdStart,
            Recommendations = recs,
        });
    }

    
    [HttpGet("cold-start")]
    [ProducesResponseType<RecommendationResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetColdStart([FromQuery] int n = 10)
    {
        if (n <= 0) return BadRequest(new { error = "n deve ser maior que zero." });

        var recs = await _service.RecommendAsync(userId: -1, n);

        return Ok(new RecommendationResponse
        {
            UserId = -1,
            Model = "popularity",
            ColdStart = true,
            Recommendations = recs,
        });
    }
}
