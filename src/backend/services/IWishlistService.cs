using BackEnd.Aventura.Domain;

namespace BackEnd.Aventura.Services
{
    public interface IWishlistService
    {
        IReadOnlyList<Espetaculo> GetWishlist(Guid usuarioId);
        void Add(Guid usuarioId, Guid espetaculoId);
        void Remove(Guid usuarioId, Guid espetaculoId);
        bool Contains(Guid usuarioId, Guid espetaculoId);
    }
}
