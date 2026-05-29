using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace BackEnd.Aventura.Repository
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly string _connectionString;

        public WishlistRepository(IConfiguration configuration)
        {
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));
            _connectionString = BuildConnectionString(configuration);
        }

        private static string BuildConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                return connectionString;
            }

            var host = configuration["POSTGRES_HOST"] ?? configuration["DB_HOST"];
            var port = configuration["POSTGRES_PORT"] ?? configuration["DB_PORT"] ?? "5432";
            var database = configuration["POSTGRES_DB"] ?? configuration["DB_NAME"];
            var user = configuration["POSTGRES_USER"] ?? configuration["DB_USER"];
            var password = configuration["POSTGRES_PASSWORD"] ?? configuration["DB_PASSWORD"];

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(database) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException("As variáveis de ambiente POSTGRES_HOST, POSTGRES_DB, POSTGRES_USER e POSTGRES_PASSWORD são necessárias para conectar ao PostgreSQL.");
            }

            return new NpgsqlConnectionStringBuilder
            {
                Host = host,
                Port = int.TryParse(port, out var parsedPort) ? parsedPort : 5432,
                Database = database,
                Username = user,
                Password = password,
                SslMode = SslMode.Prefer
            }.ConnectionString;
        }

        private NpgsqlConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        public Wishlist GetOrCreateWishlist(Guid usuarioId)
        {
            if (usuarioId == Guid.Empty) throw new ArgumentException("Id de usuário inválido.", nameof(usuarioId));

            var wishlist = new Wishlist(usuarioId);
            foreach (var itemId in GetItemIds(usuarioId))
            {
                wishlist.AdicionarEspetaculo(itemId);
            }

            return wishlist;
        }

        public Wishlist? GetWishlist(Guid usuarioId)
        {
            if (usuarioId == Guid.Empty) throw new ArgumentException("Id de usuário inválido.", nameof(usuarioId));
            return GetOrCreateWishlist(usuarioId);
        }

        public bool AddItem(Guid usuarioId, Guid espetaculoId)
        {
            if (usuarioId == Guid.Empty) throw new ArgumentException("Id de usuário inválido.", nameof(usuarioId));
            if (espetaculoId == Guid.Empty) throw new ArgumentException("Id de espetáculo inválido.", nameof(espetaculoId));

            using var connection = CreateConnection();
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
            if (usuarioId == Guid.Empty) throw new ArgumentException("Id de usuário inválido.", nameof(usuarioId));
            if (espetaculoId == Guid.Empty) throw new ArgumentException("Id de espetáculo inválido.", nameof(espetaculoId));

            using var connection = CreateConnection();
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
            if (usuarioId == Guid.Empty) throw new ArgumentException("Id de usuário inválido.", nameof(usuarioId));
            if (espetaculoId == Guid.Empty) throw new ArgumentException("Id de espetáculo inválido.", nameof(espetaculoId));

            using var connection = CreateConnection();
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

            var result = command.ExecuteScalar();
            return result is bool liked && liked;
        }

        public IEnumerable<Guid> GetItemIds(Guid usuarioId)
        {
            if (usuarioId == Guid.Empty) throw new ArgumentException("Id de usuário inválido.", nameof(usuarioId));

            using var connection = CreateConnection();
            connection.Open();

            const string sql = @"
SELECT espetaculo_id
FROM wishlist_curtidas
WHERE usuario_id = @UsuarioId
  AND tipo = 'wishlist'";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UsuarioId", usuarioId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                yield return reader.GetGuid(reader.GetOrdinal("espetaculo_id"));
            }
        }
    }
}
