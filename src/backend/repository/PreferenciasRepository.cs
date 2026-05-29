using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace BackEnd.Aventura.Repository
{
    public class PreferenciasRepository : IPreferenciasRepository
    {
        private readonly string _connectionString;

        public PreferenciasRepository(IConfiguration configuration)
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

        public Preferencias GetPreferencias(Guid usuarioId)
        {
            if (usuarioId == Guid.Empty) throw new ArgumentException("Id de usuário inválido.", nameof(usuarioId));

            using var connection = CreateConnection();
            connection.Open();

            var preferencias = ReadPreferencias(connection, usuarioId);
            if (preferencias == null)
            {
                CreateDefaultPreferencias(connection, usuarioId);
                preferencias = ReadPreferencias(connection, usuarioId);
            }

            if (preferencias == null)
            {
                throw new InvalidOperationException("Não foi possível carregar as preferências do usuário.");
            }

            var generos = ReadGeneros(connection, usuarioId).ToList();
            preferencias.SetGeneros(generos);

            var tags = ReadTags(connection, usuarioId);
            preferencias.SetTags(tags);

            return preferencias;
        }

        public Preferencias? GetPreferenciasOrNull(Guid usuarioId)
        {
            if (usuarioId == Guid.Empty) throw new ArgumentException("Id de usuário inválido.", nameof(usuarioId));

            using var connection = CreateConnection();
            connection.Open();
            return ReadPreferencias(connection, usuarioId);
        }

        public bool Save(Preferencias preferencias)
        {
            if (preferencias is null) throw new ArgumentNullException(nameof(preferencias));
            if (preferencias.UsuarioId == Guid.Empty) throw new ArgumentException("Id de usuário inválido.", nameof(preferencias.UsuarioId));

            using var connection = CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            const string sql = @"
INSERT INTO preferencias(usuario_id, cidade, tags, faixa_preco_min, faixa_preco_max, receber_notificacoes, data_atualizacao)
VALUES (@UsuarioId, @Cidade, @Tags, @FaixaPrecoMin, @FaixaPrecoMax, @ReceberNotificacoes, NOW())
ON CONFLICT(usuario_id) DO UPDATE SET
    cidade = EXCLUDED.cidade,
    tags = EXCLUDED.tags,
    faixa_preco_min = EXCLUDED.faixa_preco_min,
    faixa_preco_max = EXCLUDED.faixa_preco_max,
    receber_notificacoes = EXCLUDED.receber_notificacoes,
    data_atualizacao = NOW();";

            using var command = new NpgsqlCommand(sql, connection, transaction);
            command.Parameters.AddWithValue("@UsuarioId", preferencias.UsuarioId);
            command.Parameters.AddWithValue("@Cidade", preferencias.Cidade is null ? DBNull.Value : preferencias.Cidade);
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

        public bool Delete(Guid usuarioId)
        {
            if (usuarioId == Guid.Empty) throw new ArgumentException("Id de usuário inválido.", nameof(usuarioId));

            using var connection = CreateConnection();
            connection.Open();

            const string sql = @"DELETE FROM preferencias WHERE usuario_id = @UsuarioId";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UsuarioId", usuarioId);
            return command.ExecuteNonQuery() > 0;
        }

        private static Preferencias? ReadPreferencias(NpgsqlConnection connection, Guid usuarioId)
        {
            const string sql = @"
SELECT id, usuario_id, cidade, tags, faixa_preco_min, faixa_preco_max, receber_notificacoes, data_atualizacao
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
            var tags = reader.IsDBNull(reader.GetOrdinal("tags")) ? Array.Empty<string>() : reader.GetFieldValue<string[]>(reader.GetOrdinal("tags"));
            var faixaMin = reader.IsDBNull(reader.GetOrdinal("faixa_preco_min")) ? null : reader.GetFieldValue<decimal?>(reader.GetOrdinal("faixa_preco_min"));
            var faixaMax = reader.IsDBNull(reader.GetOrdinal("faixa_preco_max")) ? null : reader.GetFieldValue<decimal?>(reader.GetOrdinal("faixa_preco_max"));
            var receberNotificacoes = reader.IsDBNull(reader.GetOrdinal("receber_notificacoes")) ? true : reader.GetBoolean(reader.GetOrdinal("receber_notificacoes"));
            var dataAtualizacao = reader.GetDateTime(reader.GetOrdinal("data_atualizacao"));
            var generos = ReadGeneros(connection, usuarioId);

            return Preferencias.FromData(id, usuarioId, cidade, generos, tags, faixaMin, faixaMax, receberNotificacoes, dataAtualizacao);
        }

        private static void CreateDefaultPreferencias(NpgsqlConnection connection, Guid usuarioId)
        {
            const string sql = @"
INSERT INTO preferencias(usuario_id, receber_notificacoes, data_atualizacao)
VALUES (@UsuarioId, TRUE, NOW())
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
            while (reader.Read())
            {
                yield return reader.GetString(reader.GetOrdinal("nome"));
            }
        }

        private static string[] ReadTags(NpgsqlConnection connection, Guid usuarioId)
        {
            const string sql = @"SELECT tags FROM preferencias WHERE usuario_id = @UsuarioId";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UsuarioId", usuarioId);
            using var reader = command.ExecuteReader();
            if (!reader.Read() || reader.IsDBNull(0))
            {
                return Array.Empty<string>();
            }

            return reader.GetFieldValue<string[]>(0);
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
