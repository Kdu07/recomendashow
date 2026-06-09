// controller/RecomendacaoController.cs
using BackEnd.Aventura.Services;
using BackEnd.Aventura.Dto;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Aventura.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecomendacaoController : ControllerBase
    {
        private readonly IRecomendacaoService _service;

        public RecomendacaoController(IRecomendacaoService service)
        {
            _service = service;
        }

        [HttpGet("{usuarioId:guid}")]
        [ProducesResponseType(typeof(IReadOnlyList<RecomendacaoItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IReadOnlyList<RecomendacaoItemDto>> Get(Guid usuarioId, [FromQuery] int limit = 10)
        {
            var result = _service.GetRecomendacoes(usuarioId, limit);
            return Ok(result);
        }
    }
}