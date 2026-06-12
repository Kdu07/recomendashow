using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Tests.Fakes
{
    /// <summary>
    /// In-memory fake implementation of IUsuarioRepository for testing purposes.
    /// Stores usuarios in a dictionary instead of a database.
    /// </summary>
    public class FakeUsuarioRepository : IUsuarioRepository
    {
        private readonly Dictionary<Guid, Usuario> _usuarios = new();
        private readonly Dictionary<string, Usuario> _usuariosByEmail = new();

        public Usuario? GetById(Guid id)
        {
            _usuarios.TryGetValue(id, out var usuario);
            return usuario;
        }

        public Usuario? GetByEmail(string email)
        {
            var normalizedEmail = email?.ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(normalizedEmail))
                return null;

            _usuariosByEmail.TryGetValue(normalizedEmail, out var usuario);
            return usuario;
        }

        public bool EmailExists(string email)
        {
            var normalizedEmail = email?.ToLowerInvariant();
            return !string.IsNullOrWhiteSpace(normalizedEmail) && _usuariosByEmail.ContainsKey(normalizedEmail);
        }

        public Usuario Create(Usuario usuario)
        {
            if (usuario is null)
                throw new ArgumentNullException(nameof(usuario));

            _usuarios[usuario.Id] = usuario;
            _usuariosByEmail[usuario.Email.ToLowerInvariant()] = usuario;
            return usuario;
        }

        public bool Update(Usuario usuario)
        {
            if (usuario is null)
                throw new ArgumentNullException(nameof(usuario));

            if (!_usuarios.ContainsKey(usuario.Id))
                return false;

            var emailAntigo = _usuariosByEmail
                .FirstOrDefault(entry => entry.Value.Id == usuario.Id)
                .Key;

            if (!string.IsNullOrWhiteSpace(emailAntigo) &&
                !string.Equals(emailAntigo, usuario.Email, StringComparison.OrdinalIgnoreCase))
            {
                _usuariosByEmail.Remove(emailAntigo);
            }

            _usuarios[usuario.Id] = usuario;
            _usuariosByEmail[usuario.Email.ToLowerInvariant()] = usuario;
            return true;
        }

        public bool SolicitarExclusao(Guid id)
        {
            if (!_usuarios.TryGetValue(id, out var usuario))
                return false;

            usuario.SolicitarExclusao();
            return true;
        }

        public Task<int?> ObterMlIdAsync(Guid usuarioId) => Task.FromResult<int?>(null);
    }
}
