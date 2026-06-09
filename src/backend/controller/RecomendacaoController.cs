using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Aventura.Controller
{
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
        public ActionResult<IReadOnlyList<RecomendacaoItemDto>> GetRecomendacoes(Guid usuarioId, [FromQuery] int limit = 10) =>
            Ok(_service.GetRecomendacoes(usuarioId, limit));
    }
}
