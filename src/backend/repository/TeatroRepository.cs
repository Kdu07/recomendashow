using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace BackEnd.Aventura.Repository
{
    public class TeatroRepository : ITeatroRepository
    {
        private readonly string _connectionString;

        public TeatroRepository(IConfiguration configuration)
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

        public IEnumerable<Teatro> GetAll()
        {
            using var connection = CreateConnection();
            connection.Open();

            const string sql = @"
SELECT id, nome, endereco, cidade, horario_funcionamento
FROM teatros";

            using var command = new NpgsqlCommand(sql, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                yield return MapTeatro(reader);
            }
        }

        public Teatro? GetById(Guid id)
        {
            if (id == Guid.Empty) throw new ArgumentException("Id de teatro inválido.", nameof(id));

            using var connection = CreateConnection();
            connection.Open();

            const string sql = @"
SELECT id, nome, endereco, cidade, horario_funcionamento
FROM teatros
WHERE id = @Id";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            var teatro = MapTeatro(reader);
            reader.Close();
            return LoadEspetaculos(teatro, connection);
        }

        private static Teatro MapTeatro(NpgsqlDataReader reader)
        {
            return new Teatro(
                reader.GetGuid(reader.GetOrdinal("id")),
                reader.GetString(reader.GetOrdinal("nome")),
                reader.GetString(reader.GetOrdinal("endereco")),
                reader.GetString(reader.GetOrdinal("cidade")),
                reader.GetString(reader.GetOrdinal("horario_funcionamento")));
        }

        private static Teatro LoadEspetaculos(Teatro teatro, NpgsqlConnection connection)
        {
            const string sql = @"
SELECT e.id, e.nome, e.tipo_evento, e.descricao, e.url_bilheteria, e.data_inicio, e.data_fim, e.valor_ingresso
FROM espetaculos e
JOIN teatro_espetaculo te ON te.espetaculo_id = e.id
WHERE te.teatro_id = @TeatroId";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@TeatroId", teatro.Id);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var espetaculo = new Espetaculo(
                    reader.GetGuid(reader.GetOrdinal("id")),
                    reader.GetString(reader.GetOrdinal("nome")),
                    reader.GetString(reader.GetOrdinal("tipo_evento")),
                    reader.GetString(reader.GetOrdinal("tipo_evento")),
                    reader.GetString(reader.GetOrdinal("descricao")),
                    reader.GetString(reader.GetOrdinal("url_bilheteria")),
                    reader.GetDateTime(reader.GetOrdinal("data_inicio")),
                    reader.GetDateTime(reader.GetOrdinal("data_fim")),
                    reader.GetDecimal(reader.GetOrdinal("valor_ingresso")));

                teatro.AdicionarEspetaculo(espetaculo);
            }

            return teatro;
        }
    }
}
