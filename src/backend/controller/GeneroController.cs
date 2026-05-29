using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Aventura.Controller
{
    [Route("api/[controller]")]
    public class GeneroController : ControllerBase
    {
        private readonly IGeneroService _service;

        public GeneroController(IGeneroService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
        public ActionResult<IReadOnlyList<string>> GetAll() =>
            Ok(_service.GetAll());
    }
}
