using System;
using System.Collections.Generic;
using System.Linq;

namespace BackEnd.Aventura.Repository
{
    public class InMemoryTeatroRepository : ITeatroRepository
    {
        private readonly List<Teatro> _teatros = new();

        public InMemoryTeatroRepository()
        {
            var teatro1 = new Teatro(
                Guid.NewGuid(),
                "Teatro Central",
                "Avenida das Artes, 123",
                "São Paulo",
                "Terça a Domingo, 14h às 22h");

            teatro1.AdicionarEspetaculo(new Espetaculo(
                Guid.NewGuid(),
                "Musical dos Sonhos",
                "Musical",
                "Peça",
                "Um musical emocionante sobre a busca pelos próprios sonhos.",
                "https://bilheteria.exemplo.com/musical-dos-sonhos",
                DateTime.UtcNow.AddDays(-2),
                DateTime.UtcNow.AddDays(30),
                120m));

            teatro1.AdicionarEspetaculo(new Espetaculo(
                Guid.NewGuid(),
                "Noite de Comédia",
                "Comédia",
                "Stand-up",
                "Uma noite divertida com os melhores comediantes da cidade.",
                "https://bilheteria.exemplo.com/noite-de-comedia",
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow.AddDays(15),
                80m));

            var teatro2 = new Teatro(
                Guid.NewGuid(),
                "Teatro Estrela",
                "Rua do Espetáculo, 45",
                "Rio de Janeiro",
                "Quarta a Domingo, 15h às 23h");

            teatro2.AdicionarEspetaculo(new Espetaculo(
                Guid.NewGuid(),
                "Drama da Cidade",
                "Drama",
                "Peça",
                "Um drama contemporâneo que aborda a vida urbana.",
                "https://bilheteria.exemplo.com/drama-da-cidade",
                DateTime.UtcNow.AddDays(-5),
                DateTime.UtcNow.AddDays(20),
                95m));

            _teatros.Add(teatro1);
            _teatros.Add(teatro2);
        }

        public IEnumerable<Teatro> GetAll() => _teatros;

        public Teatro? GetById(Guid id) => _teatros.FirstOrDefault(t => t.Id == id);
    }
}
