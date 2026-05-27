using System;
using System.Collections.Generic;
using BackEnd.Aventura.Repository;
using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Aventura.Controller
{
    /// <summary>
    /// Gerencia a wishlist (lista de desejos) de espetáculos do usuário.
    /// Permite adicionar, remover e verificar espetáculos salvos (RF06 - Gestão de Wishlist).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _service;

        public WishlistController(IWishlistService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retorna todos os espetáculos salvos na wishlist de um usuário (RF06).
        /// </summary>
        /// <param name="usuarioId">ID do usuário (GUID)</param>
        [HttpGet("{usuarioId}")]
        [ProducesResponseType(typeof(IEnumerable<Espetaculo>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<Espetaculo>> GetByUsuario(Guid usuarioId)
        {
            try
            {
                var itens = _service.GetWishlistEspetaculos(usuarioId);
                return Ok(itens);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Erro ao buscar wishlist: {ex.Message}" });
            }
        }

        /// <summary>
        /// Verifica se um espetáculo específico está na wishlist do usuário (RF06).
        /// </summary>
        /// <param name="usuarioId">ID do usuário (GUID)</param>
        /// <param name="espetaculoId">ID do espetáculo (GUID)</param>
        [HttpGet("{usuarioId}/contains/{espetaculoId}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> ContainsItem(Guid usuarioId, Guid espetaculoId)
        {
            try
            {
                return Ok(_service.IsEspetaculoInWishlist(usuarioId, espetaculoId));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Erro ao verificar item: {ex.Message}" });
            }
        }

        /// <summary>
        /// Adiciona um espetáculo à wishlist do usuário (RF06).
        /// Retorna 201 (Created) se bem-sucedido, 409 (Conflict) se já está na lista.
        /// </summary>
        /// <param name="usuarioId">ID do usuário (GUID)</param>
        /// <param name="espetaculoId">ID do espetáculo (GUID)</param>
        [HttpPost("{usuarioId}/{espetaculoId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult AddItem(Guid usuarioId, Guid espetaculoId)
        {
            try
            {
                var added = _service.AddEspetaculoToWishlist(usuarioId, espetaculoId);
                if (!added)
                {
                    return Conflict(new { message = "Espetáculo já está na wishlist." });
                }

                return CreatedAtAction(nameof(GetByUsuario), new { usuarioId }, new { message = "Espetáculo adicionado à wishlist com sucesso." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Erro ao adicionar item: {ex.Message}" });
            }
        }

        /// <summary>
        /// Remove um espetáculo da wishlist do usuário (RF06).
        /// Retorna 204 (NoContent) se bem-sucedido, 404 (NotFound) se não existe na lista.
        /// </summary>
        /// <param name="usuarioId">ID do usuário (GUID)</param>
        /// <param name="espetaculoId">ID do espetáculo (GUID)</param>
        [HttpDelete("{usuarioId}/{espetaculoId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult RemoveItem(Guid usuarioId, Guid espetaculoId)
        {
            try
            {
                var removed = _service.RemoveEspetaculoFromWishlist(usuarioId, espetaculoId);
                if (!removed)
                {
                    return NotFound(new { message = "Espetáculo não encontrado na wishlist." });
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Erro ao remover item: {ex.Message}" });
            }
        }
    }
}
