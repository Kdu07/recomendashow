using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Tests.Fakes
{
    /// <summary>
    /// In-memory fake implementation of ITeatroRepository for testing purposes.
    /// </summary>
    public class FakeTeatroRepository : ITeatroRepository
    {
        private readonly Dictionary<Guid, Teatro> _teatros = new();

        public IReadOnlyList<Teatro> GetAll()
        {
            return _teatros.Values.ToList();
        }

        public Teatro? GetById(Guid id)
        {
            _teatros.TryGetValue(id, out var teatro);
            return teatro;
        }

        public void AddTeatro(Teatro teatro)
        {
            if (teatro is null)
                throw new ArgumentNullException(nameof(teatro));

            _teatros[teatro.Id] = teatro;
        }

        public void Clear() => _teatros.Clear();
    }
}
