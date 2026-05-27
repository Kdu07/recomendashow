using System;
using System.Collections.Generic;
using System.Linq;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IEspetaculoRepository _espetaculoRepository;

        public WishlistService(IWishlistRepository wishlistRepository, IEspetaculoRepository espetaculoRepository)
        {
            _wishlistRepository = wishlistRepository ?? throw new ArgumentNullException(nameof(wishlistRepository));
            _espetaculoRepository = espetaculoRepository ?? throw new ArgumentNullException(nameof(espetaculoRepository));
        }

        public IEnumerable<Espetaculo> GetWishlistEspetaculos(Guid usuarioId)
        {
            if (usuarioId == Guid.Empty)
            {
                throw new ArgumentException("Id de usuário inválido.", nameof(usuarioId));
            }

            var ids = _wishlistRepository.GetItemIds(usuarioId);
            return ids.Select(id => _espetaculoRepository.GetById(id)).Where(e => e is not null)!;
        }

        public bool AddEspetaculoToWishlist(Guid usuarioId, Guid espetaculoId)
        {
            if (usuarioId == Guid.Empty)
            {
                throw new ArgumentException("Id de usuário inválido.", nameof(usuarioId));
            }

            if (espetaculoId == Guid.Empty)
            {
                throw new ArgumentException("Id de espetáculo inválido.", nameof(espetaculoId));
            }

            var espetaculo = _espetaculoRepository.GetById(espetaculoId);
            if (espetaculo is null)
            {
                throw new InvalidOperationException("Espetáculo não encontrado.");
            }

            return _wishlistRepository.AddItem(usuarioId, espetaculoId);
        }

        public bool RemoveEspetaculoFromWishlist(Guid usuarioId, Guid espetaculoId)
        {
            if (usuarioId == Guid.Empty)
            {
                throw new ArgumentException("Id de usuário inválido.", nameof(usuarioId));
            }

            if (espetaculoId == Guid.Empty)
            {
                throw new ArgumentException("Id de espetáculo inválido.", nameof(espetaculoId));
            }

            return _wishlistRepository.RemoveItem(usuarioId, espetaculoId);
        }

        public bool IsEspetaculoInWishlist(Guid usuarioId, Guid espetaculoId)
        {
            if (usuarioId == Guid.Empty)
            {
                throw new ArgumentException("Id de usuário inválido.", nameof(usuarioId));
            }

            if (espetaculoId == Guid.Empty)
            {
                throw new ArgumentException("Id de espetáculo inválido.", nameof(espetaculoId));
            }

            return _wishlistRepository.ContainsItem(usuarioId, espetaculoId);
        }
    }
}
