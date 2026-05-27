using System;
using System.Collections.Generic;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Services
{
    public interface IWishlistService
    {
        IEnumerable<Espetaculo> GetWishlistEspetaculos(Guid usuarioId);
        bool AddEspetaculoToWishlist(Guid usuarioId, Guid espetaculoId);
        bool RemoveEspetaculoFromWishlist(Guid usuarioId, Guid espetaculoId);
        bool IsEspetaculoInWishlist(Guid usuarioId, Guid espetaculoId);
    }
}
