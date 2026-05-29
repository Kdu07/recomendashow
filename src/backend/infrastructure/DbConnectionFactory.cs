using Microsoft.Extensions.Configuration;
using Npgsql;

namespace BackEnd.Aventura.Infrastructure
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(IConfiguration configuration)
        {
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));
            _connectionString = BuildConnectionString(configuration);
        }

        public NpgsqlConnection CreateConnection() => new(_connectionString);

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

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(database) ||
                string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "As variáveis POSTGRES_HOST, POSTGRES_DB, POSTGRES_USER e POSTGRES_PASSWORD são necessárias.");
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
    }
}
