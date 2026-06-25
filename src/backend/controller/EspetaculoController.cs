using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Mappings;
using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Aventura.Controller
{
    [Route("api/[controller]")]
    public class EspetaculoController : ControllerBase
    {
        private readonly IEspetaculoService _service;
        private readonly IEspetaculoImagemService _imagemService;

        public EspetaculoController(IEspetaculoService service, IEspetaculoImagemService imagemService)
        {
            _service = service;
            _imagemService = imagemService;
        }
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<EspetaculoDto>), StatusCodes.Status200OK)]
        public ActionResult<IReadOnlyList<EspetaculoDto>> GetAll() =>
            Ok(_service.GetAll().Select(e => e.ToDto()).ToList());

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(EspetaculoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<EspetaculoDto> GetById(Guid id) =>
            Ok(_service.GetById(id).ToDto());

        [HttpGet("search")]
        [ProducesResponseType(typeof(IReadOnlyList<EspetaculoDto>), StatusCodes.Status200OK)]
        public ActionResult<IReadOnlyList<EspetaculoDto>> Search(
            [FromQuery] string? genero = null,
            [FromQuery] string? nome = null) =>
            Ok(_service.Search(genero, nome).Select(e => e.ToDto()).ToList());

        [HttpGet("{id:guid}/imagem")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetImagem(Guid id)
        {
            var imagem = _imagemService.GetImagem(id);
            return File(imagem.Conteudo, imagem.ContentType, imagem.NomeArquivo);
        }
    }
}