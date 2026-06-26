namespace BackEnd.Aventura.Repository
{
    public interface IWishlistRepository
    {
        bool AddItem(Guid usuarioId, Guid espetaculoId);
        bool RemoveItem(Guid usuarioId, Guid espetaculoId);
        bool ContainsItem(Guid usuarioId, Guid espetaculoId);
        IReadOnlyList<Guid> GetItemIds(Guid usuarioId);
        IReadOnlyList<string> GetGenerosDaWishlist(Guid usuarioId);
    }
}
