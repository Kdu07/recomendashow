using BackEnd.Aventura.Domain;

namespace BackEnd.Aventura.Services
{
    public interface ITeatroService
    {
        IReadOnlyList<Teatro> GetAll();
        Teatro GetById(Guid id);
        IReadOnlyList<Espetaculo> GetEspetaculos(Guid teatroId);
    }
}
