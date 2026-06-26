using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Aventura.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Usuários")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        /// <summary>Retorna os dados de um usuário.</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UsuarioResponse> GetById(Guid id) =>
            Ok(_usuarioService.GetById(id));

        /// <summary>Atualiza os dados de um usuário.</summary>
        /// <remarks>Apenas os campos enviados são alterados. O e-mail deve ser único.</remarks>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UsuarioResponse> Update(Guid id, [FromBody] UsuarioUpdateRequest request) =>
            Ok(_usuarioService.Update(id, request));

        /// <summary>Solicita exclusão da conta (soft delete LGPD).</summary>
        /// <remarks>Marca o usuário como excluído. O job de anonimização remove os dados pessoais em até 15 dias.</remarks>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Delete(Guid id)
        {
            _usuarioService.SolicitarExclusao(id);
            return NoContent();
        }
    }
}
