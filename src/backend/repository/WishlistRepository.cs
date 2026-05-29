using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Infrastructure;
using Npgsql;

namespace BackEnd.Aventura.Repository
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public WishlistRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public bool AddItem(Guid usuarioId, Guid espetaculoId)
        {
            EntityGuards.NotEmpty(usuarioId, nameof(usuarioId));
            EntityGuards.NotEmpty(espetaculoId, nameof(espetaculoId));

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            const string sql = @"
INSERT INTO wishlist_curtidas(usuario_id, espetaculo_id, tipo)
VALUES (@UsuarioId, @EspetaculoId, 'wishlist')
ON CONFLICT (usuario_id, espetaculo_id, tipo) DO NOTHING";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UsuarioId", usuarioId);
            command.Parameters.AddWithValue("@EspetaculoId", espetaculoId);

            return command.ExecuteNonQuery() > 0;
        }

        public bool RemoveItem(Guid usuarioId, Guid espetaculoId)
        {
            EntityGuards.NotEmpty(usuarioId, nameof(usuarioId));
            EntityGuards.NotEmpty(espetaculoId, nameof(espetaculoId));

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            const string sql = @"
DELETE FROM wishlist_curtidas
WHERE usuario_id = @UsuarioId
  AND espetaculo_id = @EspetaculoId
  AND tipo = 'wishlist'";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UsuarioId", usuarioId);
            command.Parameters.AddWithValue("@EspetaculoId", espetaculoId);

            return command.ExecuteNonQuery() > 0;
        }

        public bool ContainsItem(Guid usuarioId, Guid espetaculoId)
        {
            EntityGuards.NotEmpty(usuarioId, nameof(usuarioId));
            EntityGuards.NotEmpty(espetaculoId, nameof(espetaculoId));

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            const string sql = @"
SELECT EXISTS(
    SELECT 1
    FROM wishlist_curtidas
    WHERE usuario_id = @UsuarioId
      AND espetaculo_id = @EspetaculoId
      AND tipo = 'wishlist')";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UsuarioId", usuarioId);
            command.Parameters.AddWithValue("@EspetaculoId", espetaculoId);

            return command.ExecuteScalar() is bool exists && exists;
        }

        public IReadOnlyList<Guid> GetItemIds(Guid usuarioId)
        {
            EntityGuards.NotEmpty(usuarioId, nameof(usuarioId));

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            const string sql = @"
SELECT espetaculo_id
FROM wishlist_curtidas
WHERE usuario_id = @UsuarioId
  AND tipo = 'wishlist'";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UsuarioId", usuarioId);

            using var reader = command.ExecuteReader();
            var ids = new List<Guid>();
            while (reader.Read())
            {
                ids.Add(reader.GetGuid(reader.GetOrdinal("espetaculo_id")));
            }

            return ids;
        }
    }
}
