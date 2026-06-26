using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Mappings;
using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Aventura.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Preferências")]
    public class PreferenciasController : ControllerBase
    {
        private readonly IPreferenciasService _service;

        public PreferenciasController(IPreferenciasService service)
        {
            _service = service;
        }

        /// <summary>Retorna as preferências de um usuário (cidade, gêneros, faixa de preço).</summary>
        [HttpGet("{usuarioId:guid}")]
        [ProducesResponseType(typeof(PreferenciasDto), StatusCodes.Status200OK)]
        public ActionResult<PreferenciasDto> GetPreferencias(Guid usuarioId) =>
            Ok(_service.GetByUsuario(usuarioId).ToDto());

        /// <summary>Atualiza as preferências de um usuário.</summary>
        /// <remarks>Usado no onboarding e na tela de edição de interesses.</remarks>
        [HttpPut("{usuarioId:guid}")]
        [ProducesResponseType(typeof(PreferenciasDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<PreferenciasDto> UpdatePreferencias(Guid usuarioId, [FromBody] PreferenciasUpdateDto request) =>
            Ok(_service.Update(usuarioId, request).ToDto());
    }
}
