using System;
using System.Collections.Generic;
using BackEnd.Aventura.Repository;
using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Aventura.Controller
{
    /// <summary>
    /// Gerencia informações de teatros e seus espetáculos associados.
    /// Suporta listagem, busca por ID e consulta de espetáculos por teatro (RF07 - Informações de Teatros).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TeatroController : ControllerBase
    {
        private readonly ITeatroService _teatroService;

        public TeatroController(ITeatroService teatroService)
        {
            _teatroService = teatroService;
        }

        /// <summary>
        /// Retorna a lista de todos os teatros cadastrados (RF07).
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Teatro>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Teatro>> GetAll()
        {
            try
            {
                return Ok(_teatroService.GetAllTeatros());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Erro ao listar teatros: {ex.Message}" });
            }
        }

        /// <summary>
        /// Retorna um teatro específico com seus metadados (endereço, horário) (RF07).
        /// </summary>
        /// <param name="id">ID único do teatro (GUID)</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Teatro), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Teatro> GetById(Guid id)
        {
            try
            {
                var teatro = _teatroService.GetTeatroById(id);
                if (teatro is null)
                {
                    return NotFound(new { message = $"Teatro com ID {id} não encontrado." });
                }

                return Ok(teatro);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Erro ao buscar teatro: {ex.Message}" });
            }
        }

        /// <summary>
        /// Retorna os espetáculos em cartaz em um teatro específico (RF07).
        /// </summary>
        /// <param name="id">ID do teatro</param>
        [HttpGet("{id}/espetaculos")]
        [ProducesResponseType(typeof(IEnumerable<Espetaculo>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<Espetaculo>> GetEspetaculos(Guid id)
        {
            try
            {
                return Ok(_teatroService.GetEspetaculos(id));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Teatro com ID {id} não encontrado." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Erro ao buscar espetáculos: {ex.Message}" });
            }
        }
    }
}
