using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Tests.Fakes
{
    /// <summary>
    /// In-memory fake implementation of IPreferenciasRepository for testing purposes.
    /// </summary>
    public class FakePreferenciasRepository : IPreferenciasRepository
    {
        private readonly Dictionary<Guid, Preferencias> _preferencias = new();

        public Preferencias GetPreferencias(Guid usuarioId)
        {
            if (_preferencias.TryGetValue(usuarioId, out var prefs))
                return prefs;

            // Create default if not exists
            var novaPrefs = new Preferencias(usuarioId);
            _preferencias[usuarioId] = novaPrefs;
            return novaPrefs;
        }

        public bool Save(Preferencias preferencias)
        {
            if (preferencias is null)
                throw new ArgumentNullException(nameof(preferencias));

            _preferencias[preferencias.UsuarioId] = preferencias;
            return true;
        }

        public void Clear() => _preferencias.Clear();
    }
}
