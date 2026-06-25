using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace BackEnd.Aventura.Infrastructure
{
    public sealed class MongoConnectionFactory : IMongoConnectionFactory
    {
        private readonly IMongoDatabase _database;

        public MongoConnectionFactory(IConfiguration configuration)
        {
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));

            var databaseName = configuration["MONGO_DATABASE"] ?? "aventura_ml";
            var client = new MongoClient(BuildClientSettings(configuration));
            _database = client.GetDatabase(databaseName);
        }

        public IMongoDatabase GetDatabase() => _database;

        private static MongoClientSettings BuildClientSettings(IConfiguration configuration)
        {
            var explicitConnectionString = configuration.GetConnectionString("MongoConnection")
                ?? configuration["MONGO_CONNECTION_STRING"];

            if (!string.IsNullOrWhiteSpace(explicitConnectionString))
            {
                return MongoClientSettings.FromConnectionString(explicitConnectionString);
            }

            var host = configuration["MONGO_HOST"];
            var port = configuration["MONGO_PORT"] ?? "27017";
            var user = configuration["MONGO_USER"];
            var password = configuration["MONGO_PASSWORD"];
            var authSource = configuration["MONGO_AUTH_SOURCE"] ?? "admin";

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(user) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "As variáveis MONGO_HOST, MONGO_USER e MONGO_PASSWORD são necessárias (ou MONGO_CONNECTION_STRING).");
            }

            return new MongoClientSettings
            {
                Server = new MongoServerAddress(host, int.TryParse(port, out var parsedPort) ? parsedPort : 27017),
                Credential = MongoCredential.CreateCredential(authSource, user, password),
                DirectConnection = true
            };
        }
    }
}