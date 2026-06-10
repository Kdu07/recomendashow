using BackEnd.Aventura.E2ETests.Database;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace BackEnd.Aventura.E2ETests.Fixtures
{
    public sealed class EspetaculoWishlistFixture : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("aventura_e2e")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        private AventuraApiFactory? _factory;

        public Guid UsuarioId => EspetaculoWishlistSeed.UsuarioId;
        public Guid EspetaculoId => EspetaculoWishlistSeed.EspetaculoId;

        public async Task InitializeAsync()
        {
            await _postgres.StartAsync();
            _factory = new AventuraApiFactory(_postgres.GetConnectionString());
            await ResetDatabaseAsync();
        }

        public async Task DisposeAsync()
        {
            _factory?.Dispose();
            await _postgres.DisposeAsync();
        }

        public HttpClient CreateClient() => _factory!.CreateClient();

        public async Task ResetDatabaseAsync()
        {
            await using var connection = new NpgsqlConnection(_postgres.GetConnectionString());
            await connection.OpenAsync();
            await using var command = new NpgsqlCommand(EspetaculoWishlistSeed.BuildSchemaAndSeedSql(), connection);
            await command.ExecuteNonQueryAsync();
        }
    }
}
