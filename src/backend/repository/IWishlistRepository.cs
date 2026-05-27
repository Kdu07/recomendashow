using System;
using System.Collections.Generic;

namespace BackEnd.Aventura.Repository
{
    public interface IWishlistRepository
    {
        Wishlist GetOrCreateWishlist(Guid usuarioId);
        Wishlist? GetWishlist(Guid usuarioId);
        bool AddItem(Guid usuarioId, Guid espetaculoId);
        bool RemoveItem(Guid usuarioId, Guid espetaculoId);
        bool ContainsItem(Guid usuarioId, Guid espetaculoId);
        IEnumerable<Guid> GetItemIds(Guid usuarioId);
    }
}
