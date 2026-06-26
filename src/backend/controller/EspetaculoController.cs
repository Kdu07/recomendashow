using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Mappings;
using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Aventura.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Espetáculos")]
    public class EspetaculoController : ControllerBase
    {
        private readonly IEspetaculoService _service;
        private readonly IEspetaculoImagemService _imagemService;

        public EspetaculoController(IEspetaculoService service, IEspetaculoImagemService imagemService)
        {
            _service = service;
            _imagemService = imagemService;
        }

        /// <summary>Retorna todos os espetáculos do catálogo.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<EspetaculoDto>), StatusCodes.Status200OK)]
        public ActionResult<IReadOnlyList<EspetaculoDto>> GetAll() =>
            Ok(_service.GetAll().Select(e => e.ToDto()).ToList());

        /// <summary>Retorna um espetáculo pelo ID.</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(EspetaculoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<EspetaculoDto> GetById(Guid id) =>
            Ok(_service.GetById(id).ToDto());

        /// <summary>Busca espetáculos por gênero e/ou nome.</summary>
        /// <param name="genero">Filtra pelo gênero (ex.: "Teatro", "Stand-up").</param>
        /// <param name="nome">Filtra pelo nome parcial do espetáculo.</param>
        [HttpGet("search")]
        [ProducesResponseType(typeof(IReadOnlyList<EspetaculoDto>), StatusCodes.Status200OK)]
        public ActionResult<IReadOnlyList<EspetaculoDto>> Search(
            [FromQuery] string? genero = null,
            [FromQuery] string? nome = null) =>
            Ok(_service.Search(genero, nome).Select(e => e.ToDto()).ToList());

        /// <summary>Retorna a imagem do espetáculo via MongoDB/GridFS. Endpoint público (sem JWT).</summary>
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