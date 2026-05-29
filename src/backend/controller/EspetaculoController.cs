using System;
using System.Collections.Generic;
using System.Linq;
using BackEnd.Aventura.Repository;
using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Aventura.Controller
{
    /// <summary>
    /// Gerencia consulta e descoberta de espetáculos cadastrados.
    /// Suporta listagem completa, busca por ID, filtragem por gênero e busca por nome.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EspetaculoController : ControllerBase
    {
        private readonly IEspetaculoService _service;

        public EspetaculoController(IEspetaculoService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retorna a lista completa de espetáculos cadastrados (RF04 - Catálogo Categorizado).
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Espetaculo>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Espetaculo>> GetAll()
        {
            try
            {
                return Ok(_service.GetAllEspetaculos());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Erro ao listar espetáculos: {ex.Message}" });
            }
        }

        /// <summary>
        /// Retorna um espetáculo específico pelo seu ID.
        /// </summary>
        /// <param name="id">ID único do espetáculo (GUID)</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Espetaculo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Espetaculo> GetById(Guid id)
        {
            try
            {
                var esp = _service.GetEspetaculoById(id);
                if (esp is null)
                    return NotFound(new { message = $"Espetáculo com ID {id} não encontrado." });
                return Ok(esp);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Erro ao buscar espetáculo: {ex.Message}" });
            }
        }

        /// <summary>
        /// Busca espetáculos por gênero ou por termo no nome.
        /// Se ambos os parâmetros forem vazios, retorna todos os espetáculos (RF05 - Busca Específica).
        /// </summary>
        /// <param name="genero">Gênero artístico para filtrar (ex: 'Musical', 'Comédia')</param>
        /// <param name="nome">Termo para buscar no nome do espetáculo (ex: 'Mamma')</param>
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<Espetaculo>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Espetaculo>> Search([FromQuery] string? genero = null, [FromQuery] string? nome = null)
        {
            try
            {
                IEnumerable<Espetaculo> resultado = _service.GetAllEspetaculos();

                // Filtrar por gênero se fornecido
                if (!string.IsNullOrWhiteSpace(genero))
                {
                    resultado = _service.GetEspetaculosByGenero(genero);
                }

                // Filtrar por nome se fornecido
                if (!string.IsNullOrWhiteSpace(nome))
                {
                    resultado = resultado.Where(e => e.Nome.Contains(nome, StringComparison.OrdinalIgnoreCase));
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Erro na busca: {ex.Message}" });
            }
        }
    }
}
