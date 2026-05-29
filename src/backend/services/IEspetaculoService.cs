using BackEnd.Aventura.Domain;

namespace BackEnd.Aventura.Services
{
    public interface IEspetaculoService
    {
        IReadOnlyList<Espetaculo> GetAll();
        Espetaculo GetById(Guid id);
        IReadOnlyList<Espetaculo> Search(string? genero, string? nome);
    }
}
