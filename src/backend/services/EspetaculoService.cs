using System;
using System.Collections.Generic;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Services
{
    public class EspetaculoService : IEspetaculoService
    {
        private readonly IEspetaculoRepository _repository;

        public EspetaculoService(IEspetaculoRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public IEnumerable<Espetaculo> GetAllEspetaculos() => _repository.GetAll();

        public Espetaculo? GetEspetaculoById(Guid id) => _repository.GetById(id);

        public IEnumerable<Espetaculo> GetEspetaculosByGenero(string genero) => _repository.GetByGenero(genero);
    }
}
