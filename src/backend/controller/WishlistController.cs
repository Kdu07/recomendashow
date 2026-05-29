using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Mappings;
using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Aventura.Controller
{
    [Route("api/[controller]")]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _service;

        public WishlistController(IWishlistService service)
        {
            _service = service;
        }

        [HttpGet("{usuarioId:guid}")]
        [ProducesResponseType(typeof(IReadOnlyList<EspetaculoDto>), StatusCodes.Status200OK)]
        public ActionResult<IReadOnlyList<EspetaculoDto>> GetByUsuario(Guid usuarioId) =>
            Ok(_service.GetWishlist(usuarioId).Select(e => e.ToDto()).ToList());

        [HttpGet("{usuarioId:guid}/contains/{espetaculoId:guid}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public ActionResult<bool> Contains(Guid usuarioId, Guid espetaculoId) =>
            Ok(_service.Contains(usuarioId, espetaculoId));

        [HttpPost("{usuarioId:guid}/{espetaculoId:guid}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult Add(Guid usuarioId, Guid espetaculoId)
        {
            _service.Add(usuarioId, espetaculoId);
            return CreatedAtAction(nameof(GetByUsuario), new { usuarioId }, new { message = "Espetáculo adicionado à wishlist." });
        }

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
