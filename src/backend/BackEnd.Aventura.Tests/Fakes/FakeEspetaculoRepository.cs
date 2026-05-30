using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Tests.Fakes
{
    /// <summary>
    /// In-memory fake implementation of IEspetaculoRepository for testing purposes.
    /// </summary>
    public class FakeEspetaculoRepository : IEspetaculoRepository
    {
        private readonly Dictionary<Guid, Espetaculo> _espetaculos = new();
        private readonly Dictionary<Guid, List<Espetaculo>> _espetaculosPorUsuario = new();

        public IReadOnlyList<Espetaculo> GetAll()
        {
            return _espetaculos.Values.ToList();
        }

        public Espetaculo? GetById(Guid id)
        {
            _espetaculos.TryGetValue(id, out var espetaculo);
            return espetaculo;
        }

        public IReadOnlyList<Espetaculo> GetByIds(IReadOnlyList<Guid> ids)
        {
            return ids
                .Where(id => _espetaculos.ContainsKey(id))
                .Select(id => _espetaculos[id])
                .ToList();
        }

        public IReadOnlyList<Espetaculo> GetByGenero(string genero)
        {
            return _espetaculos.Values
                .Where(e => e.PertenceAoGenero(genero))
                .ToList();
        }

        public IReadOnlyList<Espetaculo> GetByGenerosFavoritos(Guid usuarioId, int limit)
        {
            if (!_espetaculosPorUsuario.TryGetValue(usuarioId, out var espetaculos))
                return new List<Espetaculo>();

            return espetaculos.Take(limit).ToList();
        }

        public bool Exists(Guid id)
        {
            return _espetaculos.ContainsKey(id);
        }

        public void AddEspetaculo(Espetaculo espetaculo)
        {
            if (espetaculo is null)
                throw new ArgumentNullException(nameof(espetaculo));

            _espetaculos[espetaculo.Id] = espetaculo;
        }

        public void AddEspetaculoParaUsuario(Guid usuarioId, Espetaculo espetaculo)
        {
            if (espetaculo is null)
                throw new ArgumentNullException(nameof(espetaculo));

            if (!_espetaculosPorUsuario.ContainsKey(usuarioId))
                _espetaculosPorUsuario[usuarioId] = new List<Espetaculo>();

            _espetaculosPorUsuario[usuarioId].Add(espetaculo);
        }

        public void Clear()
        {
            _espetaculos.Clear();
            _espetaculosPorUsuario.Clear();
        }
    }
}
