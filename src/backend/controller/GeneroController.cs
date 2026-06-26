using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Aventura.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Gêneros")]
    public class GeneroController : ControllerBase
    {
        private readonly IGeneroService _service;

        public GeneroController(IGeneroService service)
        {
            _service = service;
        }

        /// <summary>Retorna todos os gêneros de espetáculos disponíveis no catálogo.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
        public ActionResult<IReadOnlyList<string>> GetAll() =>
            Ok(_service.GetAll());
    }
}
