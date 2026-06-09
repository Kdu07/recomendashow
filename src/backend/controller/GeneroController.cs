using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BackEnd.Aventura.Controller
{
    [Route("api/[controller]")]
    public class GeneroController : ControllerBase
    {
        private readonly IGeneroService _service;

        public GeneroController(IGeneroService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
        public ActionResult<IReadOnlyList<string>> GetAll() =>
            Ok(_service.GetAll());

        [HttpGet("validated")]
        [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<IReadOnlyList<string>>> GetAllValidated()
        {
            try
            {
                return Ok(await _service.GetAllValidatedAsync());
            }
            catch (Exception ex) when (
                ex is HttpRequestException ||
                ex is TaskCanceledException ||
                ex is JsonException ||
                ex is InvalidOperationException)
            {
                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    new { message = "Serviço externo de catálogo de gêneros indisponível." });
            }
        }
    }
}
