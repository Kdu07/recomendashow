using BackEnd.Aventura.Domain;

namespace BackEnd.Aventura.Repository
{
    public interface IEspetaculoRepository
    {
        IReadOnlyList<Espetaculo> GetAll();
        Espetaculo? GetById(Guid id);
        IReadOnlyList<Espetaculo> GetByIds(IReadOnlyList<Guid> ids);
        IReadOnlyList<Espetaculo> GetByGenero(string genero);
        IReadOnlyList<Espetaculo> GetByGenerosFavoritos(Guid usuarioId, int limit);
        IReadOnlyList<Espetaculo> GetByGenerosDaWishlist(Guid usuarioId, int limit);
        bool Exists(Guid id);
    }
}
