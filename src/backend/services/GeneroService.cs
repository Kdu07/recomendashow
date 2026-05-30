using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Services
{
    public class GeneroService : IGeneroService
    {
        private readonly IGeneroRepository _repository;

        public GeneroService(IGeneroRepository repository)
        {
            _repository = repository;
        }

        public IReadOnlyList<string> GetAll() => _repository.GetAllNomes().ToList();
    }
}
