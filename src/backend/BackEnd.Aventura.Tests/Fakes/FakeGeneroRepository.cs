using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Tests.Fakes
{
    /// <summary>
    /// In-memory fake implementation of IGeneroRepository for testing purposes.
    /// </summary>
    public class FakeGeneroRepository : IGeneroRepository
    {
        private readonly List<string> _generos = new();

        public IEnumerable<string> GetAllNomes()
        {
            return _generos.AsReadOnly();
        }

        public void AddGenero(string genero)
        {
            if (!string.IsNullOrWhiteSpace(genero) && !_generos.Contains(genero))
                _generos.Add(genero);
        }

        public void Clear() => _generos.Clear();
    }
}
