using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Mappings;
using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Aventura.Controller
{
    [Route("api/[controller]")]
    public class TeatroController : ControllerBase
    {
        private readonly ITeatroService _service;

        public TeatroController(ITeatroService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<TeatroDto>), StatusCodes.Status200OK)]
        public ActionResult<IReadOnlyList<TeatroDto>> GetAll() =>
            Ok(_service.GetAll().Select(t => t.ToDto()).ToList());

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(TeatroDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TeatroDto> GetById(Guid id) =>
            Ok(_service.GetById(id).ToDto());

        [HttpGet("{id:guid}/espetaculos")]
        [ProducesResponseType(typeof(IReadOnlyList<EspetaculoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IReadOnlyList<EspetaculoDto>> GetEspetaculos(Guid id) =>
            Ok(_service.GetEspetaculos(id).Select(e => e.ToDto()).ToList());
    }
}
