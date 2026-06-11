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

            var preferencias = ReadPreferencias(connection, usuarioId);
            if (preferencias is null)
            {
                CreateDefaultPreferencias(connection, usuarioId);
                preferencias = ReadPreferencias(connection, usuarioId);
            }

            if (preferencias is null)
            {
                throw new InvalidOperationException("Não foi possível carregar as preferências do usuário.");
            }

            preferencias.SetGeneros(ReadGeneros(connection, usuarioId));
            return preferencias;
        }

        public bool Save(Preferencias preferencias)
        {
            if (preferencias is null) throw new ArgumentNullException(nameof(preferencias));
            EntityGuards.NotEmpty(preferencias.UsuarioId, nameof(preferencias.UsuarioId));

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            const string sql = @"
INSERT INTO preferencias(usuario_id, cidade, cep, tags, faixa_preco_min, faixa_preco_max, receber_notificacoes)
VALUES (@UsuarioId, @Cidade, @Cep, @Tags, @FaixaPrecoMin, @FaixaPrecoMax, @ReceberNotificacoes)
ON CONFLICT(usuario_id) DO UPDATE SET
    cidade = EXCLUDED.cidade,
    cep = EXCLUDED.cep,
    tags = EXCLUDED.tags,
    faixa_preco_min = EXCLUDED.faixa_preco_min,
    faixa_preco_max = EXCLUDED.faixa_preco_max,
    receber_notificacoes = EXCLUDED.receber_notificacoes;";

            using var command = new NpgsqlCommand(sql, connection, transaction);
            command.Parameters.AddWithValue("@UsuarioId", preferencias.UsuarioId);
            command.Parameters.AddWithValue("@Cidade", preferencias.Cidade is null ? DBNull.Value : preferencias.Cidade);
            command.Parameters.AddWithValue("@Cep", preferencias.Cep is null ? DBNull.Value : preferencias.Cep);
            command.Parameters.AddWithValue("@Tags", preferencias.Tags.Any() ? preferencias.Tags.ToArray() : Array.Empty<string>());
            command.Parameters.AddWithValue("@FaixaPrecoMin", (object?)preferencias.FaixaPrecoMin ?? DBNull.Value);
            command.Parameters.AddWithValue("@FaixaPrecoMax", (object?)preferencias.FaixaPrecoMax ?? DBNull.Value);
            command.Parameters.AddWithValue("@ReceberNotificacoes", preferencias.ReceberNotificacoes);
            command.ExecuteNonQuery();

            DeleteGeneros(connection, preferencias.UsuarioId, transaction);
            InsertGeneros(connection, preferencias.UsuarioId, preferencias.Generos, transaction);
            transaction.Commit();

            return true;
        }

        private static Preferencias? ReadPreferencias(NpgsqlConnection connection, Guid usuarioId)
        {
            const string sql = @"
SELECT id, usuario_id, cidade, cep, tags, faixa_preco_min, faixa_preco_max, receber_notificacoes, data_atualizacao
FROM preferencias
WHERE usuario_id = @UsuarioId";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UsuarioId", usuarioId);
            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            var id = reader.GetGuid(reader.GetOrdinal("id"));
            var cidade = reader.IsDBNull(reader.GetOrdinal("cidade")) ? null : reader.GetString(reader.GetOrdinal("cidade"));
            var cep = reader.IsDBNull(reader.GetOrdinal("cep")) ? null : reader.GetString(reader.GetOrdinal("cep"));
            var tags = reader.IsDBNull(reader.GetOrdinal("tags")) ? Array.Empty<string>() : reader.GetFieldValue<string[]>(reader.GetOrdinal("tags"));
            var faixaMin = reader.IsDBNull(reader.GetOrdinal("faixa_preco_min")) ? null : reader.GetFieldValue<decimal?>(reader.GetOrdinal("faixa_preco_min"));
            var faixaMax = reader.IsDBNull(reader.GetOrdinal("faixa_preco_max")) ? null : reader.GetFieldValue<decimal?>(reader.GetOrdinal("faixa_preco_max"));
            var receberNotificacoes = reader.IsDBNull(reader.GetOrdinal("receber_notificacoes")) ? true : reader.GetBoolean(reader.GetOrdinal("receber_notificacoes"));
            var dataAtualizacao = reader.GetDateTime(reader.GetOrdinal("data_atualizacao"));

            return Preferencias.FromData(id, usuarioId, cep, cidade, Array.Empty<string>(), tags, faixaMin, faixaMax, receberNotificacoes, dataAtualizacao);
        }

        private static void CreateDefaultPreferencias(NpgsqlConnection connection, Guid usuarioId)
        {
            const string sql = @"
INSERT INTO preferencias(usuario_id, receber_notificacoes)
VALUES (@UsuarioId, TRUE)
ON CONFLICT(usuario_id) DO NOTHING";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UsuarioId", usuarioId);
            command.ExecuteNonQuery();
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

        private static void DeleteGeneros(NpgsqlConnection connection, Guid usuarioId, NpgsqlTransaction transaction)
        {
            const string sql = @"DELETE FROM usuario_genero_favorito WHERE usuario_id = @UsuarioId";
            using var command = new NpgsqlCommand(sql, connection, transaction);
            command.Parameters.AddWithValue("@UsuarioId", usuarioId);
            command.ExecuteNonQuery();
        }

        private static void InsertGeneros(NpgsqlConnection connection, Guid usuarioId, IEnumerable<string> generos, NpgsqlTransaction transaction)
        {
            const string sql = @"INSERT INTO usuario_genero_favorito(usuario_id, genero_id)
VALUES (@UsuarioId, (SELECT id FROM generos WHERE LOWER(nome) = LOWER(@Genero)))";
            foreach (var genero in generos)
            {
                using var command = new NpgsqlCommand(sql, connection, transaction);
                command.Parameters.AddWithValue("@UsuarioId", usuarioId);
                command.Parameters.AddWithValue("@Genero", genero.Trim());
                command.ExecuteNonQuery();
            }
        }
    }
}
