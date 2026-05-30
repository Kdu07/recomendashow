using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Aventura.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class RecommendationController : ControllerBase
{
    private readonly IRecommendationService _service;

    public RecommendationController(IRecommendationService service) =>
        _service = service;

    /// <summary>Retorna as top-N recomendações para um usuário.</summary>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="n">Quantidade de recomendações (padrão: 10).</param>
    [HttpGet("{userId:int}")]
    public ActionResult<List<RecommendationResult>> Get(int userId, [FromQuery] int n = 10)
    {
        if (n is <= 0 or > 100)
            return BadRequest("n deve estar entre 1 e 100.");

        var results = _service.Recommend(userId, n);
        return Ok(results);
    }
}