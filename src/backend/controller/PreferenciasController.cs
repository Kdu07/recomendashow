using System;
using System.Collections.Generic;
using System.Linq;
using BackEnd.Aventura.Repository;
using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Aventura.Controller
{
    /// <summary>
    /// Gerencia as preferências do usuário para recomendações e filtros.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PreferenciasController : ControllerBase
    {
        private readonly IPreferenciasService _preferenciasService;

        public PreferenciasController(IPreferenciasService preferenciasService)
        {
            _preferenciasService = preferenciasService;
        }

        /// <summary>
        /// Retorna as preferências do usuário.
        /// </summary>
        /// <param name="usuarioId">ID do usuário (GUID)</param>
        [HttpGet("{usuarioId}")]
        [ProducesResponseType(typeof(Preferencias), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Preferencias> GetPreferencias(Guid usuarioId)
        {
            try
            {
                var preferencias = _preferenciasService.GetPreferencias(usuarioId);
                return Ok(preferencias);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza as preferências do usuário.
        /// </summary>
        /// <param name="usuarioId">ID do usuário (GUID)</param>
        /// <param name="request">Dados de atualização das preferências</param>
        [HttpPut("{usuarioId}")]
        [ProducesResponseType(typeof(Preferencias), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Preferencias> UpdatePreferencias(Guid usuarioId, [FromBody] PreferenciasUpdateRequest request)
        {
            try
            {
                if (request is null)
                {
                    return BadRequest(new { message = "Corpo da requisição obrigatório." });
                }

                var preferencias = _preferenciasService.GetPreferencias(usuarioId);

                if (request.Generos is not null)
                {
                    preferencias.SetGeneros(request.Generos);
                }

                if (request.Tags is not null)
                {
                    preferencias.SetTags(request.Tags);
                }

                preferencias.AtualizarFaixaPreco(request.FaixaPrecoMin, request.FaixaPrecoMax);
                preferencias.SetReceberNotificacoes(request.ReceberNotificacoes);

                var updated = _preferenciasService.SavePreferencias(preferencias);
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
    }

    public class PreferenciasUpdateRequest
    {
        public IEnumerable<string>? Generos { get; set; }
        public IEnumerable<string>? Tags { get; set; }
        public decimal? FaixaPrecoMin { get; set; }
        public decimal? FaixaPrecoMax { get; set; }
        public bool ReceberNotificacoes { get; set; } = true;
    }
}
