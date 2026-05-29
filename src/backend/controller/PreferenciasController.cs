using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Mappings;
using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Aventura.Controller
{
    [Route("api/[controller]")]
    public class PreferenciasController : ControllerBase
    {
        private readonly IPreferenciasService _service;

        public PreferenciasController(IPreferenciasService service)
        {
            _service = service;
        }

        [HttpGet("{usuarioId:guid}")]
        [ProducesResponseType(typeof(PreferenciasDto), StatusCodes.Status200OK)]
        public ActionResult<PreferenciasDto> GetPreferencias(Guid usuarioId) =>
            Ok(_service.GetByUsuario(usuarioId).ToDto());

        [HttpPut("{usuarioId:guid}")]
        [ProducesResponseType(typeof(PreferenciasDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<PreferenciasDto> UpdatePreferencias(Guid usuarioId, [FromBody] PreferenciasUpdateDto request) =>
            Ok(_service.Update(usuarioId, request).ToDto());
    }
}
