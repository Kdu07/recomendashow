using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Exceptions;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Services
{
    public class TeatroService : ITeatroService
    {
        private readonly ITeatroRepository _repository;

        public TeatroService(ITeatroRepository repository)
        {
            _repository = repository;
        }

        public IReadOnlyList<Teatro> GetAll() => _repository.GetAll();

        public Teatro GetById(Guid id)
        {
            var teatro = _repository.GetById(id);
            if (teatro is null)
            {
                throw new NotFoundException($"Teatro com ID {id} não encontrado.");
            }

            return teatro;
        }

        public IReadOnlyList<Espetaculo> GetEspetaculos(Guid teatroId) =>
            GetById(teatroId).Espetaculos.ToList();
    }
}
