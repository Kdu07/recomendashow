using System;
using System.Collections.Generic;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Services
{
    public class TeatroService : ITeatroService
    {
        private readonly ITeatroRepository _repository;

        public TeatroService(ITeatroRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public IEnumerable<Teatro> GetAllTeatros() => _repository.GetAll();

        public Teatro? GetTeatroById(Guid id) => _repository.GetById(id);

        public IReadOnlyCollection<Espetaculo> GetEspetaculos(Guid teatroId)
        {
            var teatro = _repository.GetById(teatroId);
            if (teatro == null)
            {
                throw new KeyNotFoundException("Teatro não encontrado.");
            }

            return teatro.BuscarEspetaculos();
        }
    }
}
