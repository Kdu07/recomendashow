using BackEnd.Aventura.Services;
using BackEnd.Aventura.Dto;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Aventura.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Recomendações")]
    public class RecomendacaoController : ControllerBase
    {
        private readonly IRecomendacaoService _service;

        public RecomendacaoController(IRecomendacaoService service)
        {
            _service = service;
        }

        /// <summary>Retorna as recomendações personalizadas do usuário.</summary>
        /// <remarks>
        /// Geradas pelo motor Funk SVD com cold-start por gênero favorito e fallback por wishlist.
        /// As recomendações são pré-computadas e atualizadas a cada 24 h.
        /// </remarks>
        /// <param name="usuarioId">ID do usuário.</param>
        /// <param name="limit">Número máximo de recomendações (padrão: 10).</param>
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