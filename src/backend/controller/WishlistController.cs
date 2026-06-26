using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Mappings;
using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Aventura.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Wishlist")]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _service;

        public WishlistController(IWishlistService service)
        {
            _service = service;
        }

        /// <summary>Retorna os espetáculos na wishlist do usuário.</summary>
        [HttpGet("{usuarioId:guid}")]
        [ProducesResponseType(typeof(IReadOnlyList<EspetaculoDto>), StatusCodes.Status200OK)]
        public ActionResult<IReadOnlyList<EspetaculoDto>> GetByUsuario(Guid usuarioId) =>
            Ok(_service.GetWishlist(usuarioId).Select(e => e.ToDto()).ToList());

        /// <summary>Verifica se um espetáculo está na wishlist do usuário.</summary>
        [HttpGet("{usuarioId:guid}/contains/{espetaculoId:guid}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public ActionResult<bool> Contains(Guid usuarioId, Guid espetaculoId) =>
            Ok(_service.Contains(usuarioId, espetaculoId));

        /// <summary>Adiciona um espetáculo à wishlist.</summary>
        /// <remarks>Retorna 409 se o espetáculo já estiver na wishlist.</remarks>
        [HttpPost("{usuarioId:guid}/{espetaculoId:guid}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult Add(Guid usuarioId, Guid espetaculoId)
        {
            _service.Add(usuarioId, espetaculoId);
            return CreatedAtAction(nameof(GetByUsuario), new { usuarioId }, new { message = "Espetáculo adicionado à wishlist." });
        }

        /// <summary>Remove um espetáculo da wishlist.</summary>
        [HttpDelete("{usuarioId:guid}/{espetaculoId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Remove(Guid usuarioId, Guid espetaculoId)
        {
            _service.Remove(usuarioId, espetaculoId);
            return NoContent();
        }
    }
}
