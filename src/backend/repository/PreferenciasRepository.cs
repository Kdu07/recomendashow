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
            EnsurePreferenciasTable(connection);

            var preferencias = ReadPreferencias(connection, usuarioId) ?? new Preferencias(usuarioId);
            preferencias.SetGeneros(ReadGeneros(connection, usuarioId));

            return preferencias;
        }

        public bool Save(Preferencias preferencias)
        {
            if (preferencias is null) throw new ArgumentNullException(nameof(preferencias));
            EntityGuards.NotEmpty(preferencias.UsuarioId, nameof(preferencias.UsuarioId));

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            EnsurePreferenciasTable(connection);

            using var transaction = connection.BeginTransaction();
            UpsertPreferencias(connection, preferencias, transaction);
            DeleteGeneros(connection, preferencias.UsuarioId, transaction);
            InsertGeneros(connection, preferencias.UsuarioId, preferencias.Generos, transaction);
            transaction.Commit();

            return true;
        }

        private static void EnsurePreferenciasTable(NpgsqlConnection connection)
        {
            const string sql = @"
CREATE EXTENSION IF NOT EXISTS ""pgcrypto"";

CREATE TABLE IF NOT EXISTS preferencias(
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    usuario_id UUID UNIQUE,
    cidade VARCHAR(100),
    tags TEXT[],
    faixa_preco_min DECIMAL(10,2),
    faixa_preco_max DECIMAL(10,2),
    receber_notificacoes BOOLEAN DEFAULT TRUE,
    data_atualizacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    cep VARCHAR(9)
);

ALTER TABLE preferencias
    ADD COLUMN IF NOT EXISTS tags TEXT[],
    ADD COLUMN IF NOT EXISTS faixa_preco_min DECIMAL(10,2),
    ADD COLUMN IF NOT EXISTS faixa_preco_max DECIMAL(10,2),
    ADD COLUMN IF NOT EXISTS receber_notificacoes BOOLEAN DEFAULT TRUE,
    ADD COLUMN IF NOT EXISTS cep VARCHAR(9);";

            using var command = new NpgsqlCommand(sql, connection);
            command.ExecuteNonQuery();
        }

        private static Preferencias? ReadPreferencias(NpgsqlConnection connection, Guid usuarioId)
        {
            const string sql = @"
SELECT id, usuario_id, cep, cidade, tags, faixa_preco_min, faixa_preco_max,
       receber_notificacoes, data_atualizacao
FROM preferencias
WHERE usuario_id = @UsuarioId";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UsuarioId", usuarioId);

            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            var tags = reader.IsDBNull(reader.GetOrdinal("tags"))
                ? Array.Empty<string>()
                : reader.GetFieldValue<string[]>(reader.GetOrdinal("tags"));

            return Preferencias.FromData(
                reader.GetGuid(reader.GetOrdinal("id")),
                reader.GetGuid(reader.GetOrdinal("usuario_id")),
                reader.IsDBNull(reader.GetOrdinal("cep")) ? null : reader.GetString(reader.GetOrdinal("cep")),
                reader.IsDBNull(reader.GetOrdinal("cidade")) ? null : reader.GetString(reader.GetOrdinal("cidade")),
                Array.Empty<string>(),
                tags,
                reader.IsDBNull(reader.GetOrdinal("faixa_preco_min")) ? null : reader.GetDecimal(reader.GetOrdinal("faixa_preco_min")),
                reader.IsDBNull(reader.GetOrdinal("faixa_preco_max")) ? null : reader.GetDecimal(reader.GetOrdinal("faixa_preco_max")),
                reader.IsDBNull(reader.GetOrdinal("receber_notificacoes")) || reader.GetBoolean(reader.GetOrdinal("receber_notificacoes")),
                reader.GetDateTime(reader.GetOrdinal("data_atualizacao")));
        }

        private static void UpsertPreferencias(NpgsqlConnection connection, Preferencias preferencias, NpgsqlTransaction transaction)
        {
            const string sql = @"
INSERT INTO preferencias(
    id, usuario_id, cep, cidade, tags, faixa_preco_min, faixa_preco_max,
    receber_notificacoes, data_atualizacao)
VALUES (
    @Id, @UsuarioId, @Cep, @Cidade, @Tags, @FaixaPrecoMin, @FaixaPrecoMax,
    @ReceberNotificacoes, @DataAtualizacao)
ON CONFLICT (usuario_id) DO UPDATE
SET cep = @Cep,
    cidade = @Cidade,
    tags = @Tags,
    faixa_preco_min = @FaixaPrecoMin,
    faixa_preco_max = @FaixaPrecoMax,
    receber_notificacoes = @ReceberNotificacoes,
    data_atualizacao = @DataAtualizacao";

            using var command = new NpgsqlCommand(sql, connection, transaction);
            command.Parameters.AddWithValue("@Id", preferencias.Id);
            command.Parameters.AddWithValue("@UsuarioId", preferencias.UsuarioId);
            command.Parameters.AddWithValue("@Cep", (object?)preferencias.Cep ?? DBNull.Value);
            command.Parameters.AddWithValue("@Cidade", (object?)preferencias.Cidade ?? DBNull.Value);
            command.Parameters.AddWithValue("@Tags", preferencias.Tags.ToArray());
            command.Parameters.AddWithValue("@FaixaPrecoMin", (object?)preferencias.FaixaPrecoMin ?? DBNull.Value);
            command.Parameters.AddWithValue("@FaixaPrecoMax", (object?)preferencias.FaixaPrecoMax ?? DBNull.Value);
            command.Parameters.AddWithValue("@ReceberNotificacoes", preferencias.ReceberNotificacoes);
            command.Parameters.AddWithValue("@DataAtualizacao", preferencias.DataAtualizacao);
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
