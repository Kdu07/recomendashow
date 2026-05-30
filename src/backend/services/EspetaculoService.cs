using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Exceptions;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Services
{
    public class EspetaculoService : IEspetaculoService
    {
        private readonly IEspetaculoRepository _repository;

        public EspetaculoService(IEspetaculoRepository repository)
        {
            _repository = repository;
        }

        public IReadOnlyList<Espetaculo> GetAll() => _repository.GetAll();

        public Espetaculo GetById(Guid id)
        {
            var espetaculo = _repository.GetById(id);
            if (espetaculo is null)
            {
                throw new NotFoundException($"Espetáculo com ID {id} não encontrado.");
            }

            return espetaculo;
        }

        public IReadOnlyList<Espetaculo> Search(string? genero, string? nome)
        {
            IReadOnlyList<Espetaculo> resultado = !string.IsNullOrWhiteSpace(genero)
                ? _repository.GetByGenero(genero)
                : _repository.GetAll();

            if (string.IsNullOrWhiteSpace(nome))
            {
                return resultado;
            }

            return resultado
                .Where(e => e.Nome.Contains(nome, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}
