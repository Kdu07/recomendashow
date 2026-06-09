using BackEnd.Aventura.Infrastructure;
using BackEnd.Aventura.Repository;
using BackEnd.Aventura.Services;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Testcontainers.PostgreSql;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace BackEnd.Aventura.Tests.Integration
{
    // RF06 - Gestao de Wishlist:
    // estes testes validam a adicao de espetaculo a wishlist e a consulta posterior
    // dos itens salvos para o usuario, cobrindo o fluxo funcional documentado no projeto.
    // RNF05 - Disponibilidade da plataforma:
    // o cenario com WireMock retornando HTTP 503 valida que uma falha no servico
    // consumido de recomendacao e tratada sem persistir estado inconsistente.
    public sealed class PostgresFixture : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("aventura_tests")
            .WithUsername("aventura")
            .WithPassword("aventura")
            .Build();

        public string ConnectionString => _container.GetConnectionString();

        public async Task InitializeAsync()
        {
            await _container.StartAsync();
            await ExecuteScriptAsync("src/database/01_create_database.sql");
            await ExecuteScriptAsync("src/database/02_sprint4_improvements.sql");
        }

        public Task DisposeAsync() => _container.DisposeAsync().AsTask();

        public IDbConnectionFactory CreateConnectionFactory()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = ConnectionString
                })
                .Build();

            return new DbConnectionFactory(configuration);
        }

        public async Task InsertUsuarioAsync(Guid usuarioId)
        {
            const string sql = @"
INSERT INTO usuarios(id, nome, email, senha_hash, idade, data_cadastro, consentimento_aceito, data_consentimento)
VALUES (@Id, @Nome, @Email, @SenhaHash, @Idade, @DataCadastro, TRUE, @DataConsentimento)";

            await using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();
            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", usuarioId);
            command.Parameters.AddWithValue("@Nome", "Usuario Integracao Wishlist");
            command.Parameters.AddWithValue("@Email", $"wishlist-{usuarioId:N}@teste.local");
            command.Parameters.AddWithValue("@SenhaHash", "$2a$11$k3kD5xX7mL3lB1kB0eC7wOjIVJ5XW8UMd4ScLXpYp7qZQ7R3F8ufG");
            command.Parameters.AddWithValue("@Idade", 25);
            command.Parameters.AddWithValue("@DataCadastro", DateTime.UtcNow);
            command.Parameters.AddWithValue("@DataConsentimento", DateTime.UtcNow);
            await command.ExecuteNonQueryAsync();
        }

        public async Task InsertEspetaculoAsync(Guid espetaculoId)
        {
            const string sql = @"
INSERT INTO espetaculos(id, nome, tipo_evento, descricao, url_bilheteria, data_inicio, data_fim, valor_ingresso)
VALUES (@Id, @Nome, @TipoEvento, @Descricao, @UrlBilheteria, @DataInicio, @DataFim, @ValorIngresso)";

            await using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();
            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", espetaculoId);
            command.Parameters.AddWithValue("@Nome", $"Espetaculo Integracao {espetaculoId:N}");
            command.Parameters.AddWithValue("@TipoEvento", "Musical");
            command.Parameters.AddWithValue("@Descricao", "Espetaculo usado no teste de integracao da wishlist.");
            command.Parameters.AddWithValue("@UrlBilheteria", "https://ingressos.example/wishlist");
            command.Parameters.AddWithValue("@DataInicio", DateTime.UtcNow.AddDays(1));
            command.Parameters.AddWithValue("@DataFim", DateTime.UtcNow.AddDays(2));
            command.Parameters.AddWithValue("@ValorIngresso", 120m);
            await command.ExecuteNonQueryAsync();
        }

        private async Task ExecuteScriptAsync(string relativePath)
        {
            var root = FindRepositoryRoot();
            var path = Path.Combine(root, relativePath.Replace('/', Path.DirectorySeparatorChar));
            var sql = await File.ReadAllTextAsync(path);
            sql = RemoveConflictMarkers(sql);

            await using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();
            await using var command = new NpgsqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync();
        }

        private static string RemoveConflictMarkers(string sql)
        {
            var lines = sql
                .Split(Environment.NewLine)
                .Where(line =>
                    !line.StartsWith("<<<<<<<", StringComparison.Ordinal) &&
                    !line.StartsWith("=======", StringComparison.Ordinal) &&
                    !line.StartsWith(">>>>>>>", StringComparison.Ordinal));

            return string.Join(Environment.NewLine, lines);
        }

        private static string FindRepositoryRoot()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);

            while (directory is not null)
            {
                if (Directory.Exists(Path.Combine(directory.FullName, "src", "database")))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }

            throw new DirectoryNotFoundException("Nao foi possivel localizar a raiz do repositorio.");
        }
    }

    public sealed class WireMockFixture : IAsyncLifetime
    {
        public WireMockServer Server { get; private set; } = null!;

        public string Url => Server.Url!;

        public Task InitializeAsync()
        {
            Server = WireMockServer.Start();
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            Server.Stop();
            Server.Dispose();
            return Task.CompletedTask;
        }

        public void Reset() => Server.Reset();
    }

    public class WishlistIntegrationTests :
        IClassFixture<PostgresFixture>,
        IClassFixture<WireMockFixture>
    {
        private readonly PostgresFixture _postgres;
        private readonly WireMockFixture _wireMock;

        public WishlistIntegrationTests(PostgresFixture postgres, WireMockFixture wireMock)
        {
            _postgres = postgres;
            _wireMock = wireMock;
            _wireMock.Reset();
        }

        [Fact]
        public async Task Add_WhenRecommendationServiceReturnsActive_ShouldPersistWishlistItem()
        {
            var usuarioId = Guid.NewGuid();
            var espetaculoId = Guid.NewGuid();

            _wireMock.Server
                .Given(Request.Create()
                    .WithPath($"/api/recommendations/espetaculos/{espetaculoId}/ativo")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("{ \"ativo\": true }"));

            await _postgres.InsertUsuarioAsync(usuarioId);
            await _postgres.InsertEspetaculoAsync(espetaculoId);

            var service = CreateWishlistService();

            await service.AdicionarAsync(usuarioId, espetaculoId);

            var wishlist = await service.ObterPorUsuarioAsync(usuarioId);
            Assert.Contains(wishlist, espetaculo => espetaculo.Id == espetaculoId);
        }

        [Fact]
        public async Task Add_WhenRecommendationServiceReturnsHttpError_ShouldThrowInvalidOperationException()
        {
            var usuarioId = Guid.NewGuid();
            var espetaculoId = Guid.NewGuid();

            _wireMock.Server
                .Given(Request.Create()
                    .WithPath($"/api/recommendations/espetaculos/{espetaculoId}/ativo")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(503));

            await _postgres.InsertUsuarioAsync(usuarioId);
            await _postgres.InsertEspetaculoAsync(espetaculoId);

            var service = CreateWishlistService();

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.AdicionarAsync(usuarioId, espetaculoId));
        }

        private WishlistService CreateWishlistService()
        {
            var connectionFactory = _postgres.CreateConnectionFactory();
            var wishlistRepository = new WishlistRepository(connectionFactory);
            var espetaculoRepository = new EspetaculoRepository(connectionFactory);
            var recommendationClient = new RecommendationHttpClient(new HttpClient
            {
                BaseAddress = new Uri(_wireMock.Url)
            });

            return new WishlistService(wishlistRepository, espetaculoRepository, recommendationClient);
        }
    }
}
