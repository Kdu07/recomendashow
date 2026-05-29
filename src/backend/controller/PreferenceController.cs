using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Aventura.Controllers;

public record SetPreferencesRequest(List<string> Tipos);

[ApiController]
[Route("api/[controller]")]
public sealed class PreferenceController : ControllerBase
{
    private readonly IPreferenceService _preferenceService;

    public PreferenceController(IPreferenceService preferenceService) =>
        _preferenceService = preferenceService;

    /// <summary>Salva as preferências de tipo_evento de um usuário (usadas no cold-start).</summary>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="request">Lista de tipos de evento preferidos.</param>
    [HttpPost("{userId:int}")]
    public IActionResult Set(int userId, [FromBody] SetPreferencesRequest request)
    {
        if (request.Tipos is not { Count: > 0 })
            return BadRequest("Informe ao menos um tipo de evento.");

        _preferenceService.SetPreferences(userId, request.Tipos);
        return NoContent();
    }

    /// <summary>Retorna as preferências salvas de um usuário.</summary>
    [HttpGet("{userId:int}")]
    public ActionResult<IReadOnlyList<string>> Get(int userId) =>
        Ok(_preferenceService.GetPreferences(userId));
}
