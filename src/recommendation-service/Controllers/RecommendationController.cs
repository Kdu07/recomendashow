using RecommendationService.DTOs;
using RecommendationService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace RecommendationService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class RecommendationController : ControllerBase
{
    private readonly IRecommendationService _service;

    public RecommendationController(IRecommendationService service)
    {
        _service = service;
    }

    /// <summary>Retorna top-N recomendações para um usuário (SVD).</summary>
    [HttpGet("{userId:int}")]
    [ProducesResponseType<RecommendationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRecommendations(int userId, [FromQuery] int n = 10)
    {
        if (n <= 0) return BadRequest(new { error = "n deve ser maior que zero." });

        var recs = await _service.RecommendAsync(userId, n);
        bool isColdStart = recs.Any() && recs[0].Score is null;

        return Ok(new RecommendationResponse
        {
            UserId = userId,
            Model = _service.ModelType,
            ColdStart = isColdStart,
            Recommendations = recs,
        });
    }

    /// <summary>Retorna os N itens mais populares (fallback cold-start).</summary>
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
