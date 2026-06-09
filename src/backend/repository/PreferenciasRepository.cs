using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Infrastructure;
using Npgsql;

namespace BackEnd.Aventura.Repository
{
    public class PreferenciasRepository : IPreferenciasRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PreferenciasRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public Preferencias GetPreferencias(Guid usuarioId)
        {
            EntityGuards.NotEmpty(usuarioId, nameof(usuarioId));

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            // There is no `preferencias` table in this schema. Build a default
            // Preferencias object and populate only the favorite genres from
            // the genre-favorite relation table.
            var preferencias = new Preferencias(usuarioId);
            preferencias.SetGeneros(ReadGeneros(connection, usuarioId));
            preferencias.SetTags(ReadTags(connection, usuarioId));
            return preferencias;
        }

        public bool Save(Preferencias preferencias)
        {
            if (preferencias is null) throw new ArgumentNullException(nameof(preferencias));
            EntityGuards.NotEmpty(preferencias.UsuarioId, nameof(preferencias.UsuarioId));

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            // Persist only the user's favorite genres. The legacy `preferencias`
            // table does not exist in this schema, so avoid touching it.
            DeleteGeneros(connection, preferencias.UsuarioId, transaction);
            InsertGeneros(connection, preferencias.UsuarioId, preferencias.Generos, transaction);
            transaction.Commit();

            return true;
        }

        private static IEnumerable<string> ReadGeneros(NpgsqlConnection connection, Guid usuarioId)
        {
            const string sql = @"
SELECT g.nome
FROM generos g
JOIN usuario_genero_favorito ugf ON ugf.genero_id = g.id
WHERE ugf.usuario_id = @UsuarioId";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UsuarioId", usuarioId);
            using var reader = command.ExecuteReader();
            var generos = new List<string>();
            while (reader.Read())
            {
                generos.Add(reader.GetString(reader.GetOrdinal("nome")));
            }

            return generos;
        }

        private static string[] ReadTags(NpgsqlConnection connection, Guid usuarioId)
        {
            // The schema does not include a `preferencias` table for tags.
            // Return an empty tag set to keep behavior compatible.
            return Array.Empty<string>();
        }

        private static void DeleteGeneros(NpgsqlConnection connection, Guid usuarioId, NpgsqlTransaction transaction)
        {
            const string sql = @"DELETE FROM usuario_genero_favorito WHERE usuario_id = @UsuarioId";
            using var command = new NpgsqlCommand(sql, connection, transaction);
            command.Parameters.AddWithValue("@UsuarioId", usuarioId);
            command.ExecuteNonQuery();
        }

        private static void InsertGeneros(NpgsqlConnection connection, Guid usuarioId, IEnumerable<string> generos, NpgsqlTransaction transaction)
        {
            const string sqlByName = @"INSERT INTO usuario_genero_favorito(usuario_id, genero_id)
VALUES (@UsuarioId, (SELECT id FROM generos WHERE LOWER(nome) = LOWER(@Genero)))";

            const string sqlById = @"INSERT INTO usuario_genero_favorito(usuario_id, genero_id)
VALUES (@UsuarioId, @GeneroId)";

            foreach (var genero in generos)
            {
                var trimmed = genero.Trim();
                if (Guid.TryParse(trimmed, out var generoGuid))
                {
                    using var command = new NpgsqlCommand(sqlById, connection, transaction);
                    command.Parameters.AddWithValue("@UsuarioId", usuarioId);
                    command.Parameters.AddWithValue("@GeneroId", generoGuid);
                    command.ExecuteNonQuery();
                }
                else
                {
                    using var command = new NpgsqlCommand(sqlByName, connection, transaction);
                    command.Parameters.AddWithValue("@UsuarioId", usuarioId);
                    command.Parameters.AddWithValue("@Genero", trimmed);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
