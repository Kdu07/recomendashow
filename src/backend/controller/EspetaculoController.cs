using System;
using System.Collections.Generic;
using BackEnd.Aventura.Repository;
using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Aventura.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class EspetaculoController : ControllerBase
    {
        private readonly IEspetaculoService _service;

        public EspetaculoController(IEspetaculoService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Espetaculo>> GetAll()
        {
            return Ok(_service.GetAllEspetaculos());
        }

        [HttpGet("{id}")]
        public ActionResult<Espetaculo> GetById(Guid id)
        {
            var esp = _service.GetEspetaculoById(id);
            if (esp is null) return NotFound();
            return Ok(esp);
        }

        [HttpGet("search")]
        public ActionResult<IEnumerable<Espetaculo>> Search([FromQuery] string? genero)
        {
            if (string.IsNullOrWhiteSpace(genero)) return Ok(_service.GetAllEspetaculos());
            return Ok(_service.GetEspetaculosByGenero(genero));
        }
    }
}
