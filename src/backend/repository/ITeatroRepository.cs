using BackEnd.Aventura.Domain;

namespace BackEnd.Aventura.Repository
{
    public interface ITeatroRepository
    {
        IReadOnlyList<Teatro> GetAll();
        Teatro? GetById(Guid id);
    }
}
