using System;
using System.Collections.Generic;
using BackEnd.Aventura.Repository;
using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Aventura.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeatroController : ControllerBase
    {
        private readonly ITeatroService _teatroService;

        public TeatroController(ITeatroService teatroService)
        {
            _teatroService = teatroService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Teatro>> GetAll()
        {
            return Ok(_teatroService.GetAllTeatros());
        }

        [HttpGet("{id}")]
        public ActionResult<Teatro> GetById(Guid id)
        {
            var teatro = _teatroService.GetTeatroById(id);
            if (teatro is null)
            {
                return NotFound();
            }

            return Ok(teatro);
        }

        [HttpGet("{id}/espetaculos")]
        public ActionResult<IEnumerable<Espetaculo>> GetEspetaculos(Guid id)
        {
            try
            {
                return Ok(_teatroService.GetEspetaculos(id));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
