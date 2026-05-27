using System;
using System.Collections.Generic;
using System.Linq;

namespace BackEnd.Aventura.Repository
{
    public class InMemoryEspetaculoRepository : IEspetaculoRepository
    {
        private readonly List<Espetaculo> _espetaculos = new();

        public InMemoryEspetaculoRepository()
        {
            _espetaculos.Add(new Espetaculo(
                Guid.NewGuid(),
                "Musical dos Sonhos",
                "Musical",
                "Peça",
                "Um musical emocionante sobre a busca pelos próprios sonhos.",
                "https://bilheteria.exemplo.com/musical-dos-sonhos",
                DateTime.UtcNow.AddDays(-2),
                DateTime.UtcNow.AddDays(30),
                120m));

            _espetaculos.Add(new Espetaculo(
                Guid.NewGuid(),
                "Noite de Comédia",
                "Comédia",
                "Stand-up",
                "Uma noite divertida com os melhores comediantes da cidade.",
                "https://bilheteria.exemplo.com/noite-de-comedia",
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow.AddDays(15),
                80m));

            _espetaculos.Add(new Espetaculo(
                Guid.NewGuid(),
                "Drama da Cidade",
                "Drama",
                "Peça",
                "Um drama contemporâneo que aborda a vida urbana.",
                "https://bilheteria.exemplo.com/drama-da-cidade",
                DateTime.UtcNow.AddDays(-5),
                DateTime.UtcNow.AddDays(20),
                95m));
        }

        public IEnumerable<Espetaculo> GetAll() => _espetaculos;

        public Espetaculo? GetById(Guid id) => _espetaculos.FirstOrDefault(e => e.Id == id);

        public IEnumerable<Espetaculo> GetByGenero(string genero)
        {
            if (string.IsNullOrWhiteSpace(genero)) return Enumerable.Empty<Espetaculo>();
            return _espetaculos.Where(e => string.Equals(e.Genero, genero, StringComparison.OrdinalIgnoreCase));
        }
    }
}
