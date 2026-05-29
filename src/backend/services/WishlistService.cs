using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Exceptions;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IEspetaculoRepository _espetaculoRepository;

        public WishlistService(IWishlistRepository wishlistRepository, IEspetaculoRepository espetaculoRepository)
        {
            _wishlistRepository = wishlistRepository;
            _espetaculoRepository = espetaculoRepository;
        }

        public IReadOnlyList<Espetaculo> GetWishlist(Guid usuarioId)
        {
            EntityGuards.NotEmpty(usuarioId, nameof(usuarioId));
            var ids = _wishlistRepository.GetItemIds(usuarioId);
            return _espetaculoRepository.GetByIds(ids);
        }

        public void Add(Guid usuarioId, Guid espetaculoId)
        {
            EntityGuards.NotEmpty(usuarioId, nameof(usuarioId));
            EntityGuards.NotEmpty(espetaculoId, nameof(espetaculoId));

            if (!_espetaculoRepository.Exists(espetaculoId))
            {
                throw new NotFoundException($"Espetáculo com ID {espetaculoId} não encontrado.");
            }

            if (!_wishlistRepository.AddItem(usuarioId, espetaculoId))
            {
                throw new ConflictException("Espetáculo já está na wishlist.");
            }
        }

        public void Remove(Guid usuarioId, Guid espetaculoId)
        {
            EntityGuards.NotEmpty(usuarioId, nameof(usuarioId));
            EntityGuards.NotEmpty(espetaculoId, nameof(espetaculoId));

            if (!_wishlistRepository.RemoveItem(usuarioId, espetaculoId))
            {
                throw new NotFoundException("Espetáculo não encontrado na wishlist.");
            }
        }

        public bool Contains(Guid usuarioId, Guid espetaculoId)
        {
            EntityGuards.NotEmpty(usuarioId, nameof(usuarioId));
            EntityGuards.NotEmpty(espetaculoId, nameof(espetaculoId));
            return _wishlistRepository.ContainsItem(usuarioId, espetaculoId);
        }
    }
}
