using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using BackEnd.Aventura.Dto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace BackEnd.Aventura.E2ETests
{
    public sealed class EspetaculoWishlistE2ETests : IClassFixture<EspetaculoWishlistFixture>
    {
        private readonly EspetaculoWishlistFixture _fixture;

        public EspetaculoWishlistE2ETests(EspetaculoWishlistFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task FluxoPerfeito_DeveSalvarEspetaculoNaWishlistEConsultarDadosPersistidos()
        {
            await _fixture.ResetDatabaseAsync();
            using var client = _fixture.CreateClient();

            var detalhesResponse = await client.GetAsync($"/api/Espetaculo/{_fixture.EspetaculoId}");
            Assert.Equal(HttpStatusCode.OK, detalhesResponse.StatusCode);

            var detalhes = await detalhesResponse.Content.ReadFromJsonAsync<EspetaculoDto>();
            Assert.NotNull(detalhes);
            Assert.Equal("Mamma Mia!", detalhes!.Nome);
            Assert.Equal("Musical", detalhes.Genero);
            Assert.True(detalhes.EmCartaz);

            var salvarResponse = await client.PostAsync(
                $"/api/Wishlist/{_fixture.UsuarioId}/{_fixture.EspetaculoId}",
                content: null);
            Assert.Equal(HttpStatusCode.Created, salvarResponse.StatusCode);

            var wishlistResponse = await client.GetAsync($"/api/Wishlist/{_fixture.UsuarioId}");
            Assert.Equal(HttpStatusCode.OK, wishlistResponse.StatusCode);

            var wishlist = await wishlistResponse.Content.ReadFromJsonAsync<List<EspetaculoDto>>();
            var item = Assert.Single(wishlist!);
            Assert.Equal(_fixture.EspetaculoId, item.Id);
            Assert.Equal("Mamma Mia!", item.Nome);
            Assert.Equal("Musical", item.Genero);
            Assert.Equal("https://ingressos.example/mamma-mia", item.UrlBilheteria);
        }

        [Fact]
        public async Task FluxoAlternativo_EspetaculoInexistenteNaoEntraNaWishlistERetornaErro()
        {
            await _fixture.ResetDatabaseAsync();
            using var client = _fixture.CreateClient();
            var espetaculoInexistenteId = Guid.Parse("99999999-9999-9999-9999-999999999999");

            var salvarResponse = await client.PostAsync(
                $"/api/Wishlist/{_fixture.UsuarioId}/{espetaculoInexistenteId}",
                content: null);

            Assert.Equal(HttpStatusCode.NotFound, salvarResponse.StatusCode);

            var wishlistResponse = await client.GetAsync($"/api/Wishlist/{_fixture.UsuarioId}");
            Assert.Equal(HttpStatusCode.OK, wishlistResponse.StatusCode);

            var wishlist = await wishlistResponse.Content.ReadFromJsonAsync<List<EspetaculoDto>>();
            Assert.NotNull(wishlist);
            Assert.Empty(wishlist!);
        }

        [Fact]
        public async Task FluxoAlternativo_EspetaculoDuplicadoNaoDuplicaWishlistERetornaConflito()
        {
            await _fixture.ResetDatabaseAsync();
            using var client = _fixture.CreateClient();

            var primeiraResposta = await client.PostAsync(
                $"/api/Wishlist/{_fixture.UsuarioId}/{_fixture.EspetaculoId}",
                content: null);
            var segundaResposta = await client.PostAsync(
                $"/api/Wishlist/{_fixture.UsuarioId}/{_fixture.EspetaculoId}",
                content: null);

            Assert.Equal(HttpStatusCode.Created, primeiraResposta.StatusCode);
            Assert.Equal(HttpStatusCode.Conflict, segundaResposta.StatusCode);

            var wishlistResponse = await client.GetAsync($"/api/Wishlist/{_fixture.UsuarioId}");
            Assert.Equal(HttpStatusCode.OK, wishlistResponse.StatusCode);

            var wishlist = await wishlistResponse.Content.ReadFromJsonAsync<List<EspetaculoDto>>();
            var item = Assert.Single(wishlist!);
            Assert.Equal(_fixture.EspetaculoId, item.Id);
        }
    }

    public sealed class EspetaculoWishlistFixture : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("aventura_e2e")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        private AventuraApiFactory? _factory;

        public Guid UsuarioId { get; } = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public Guid EspetaculoId { get; } = Guid.Parse("22222222-2222-2222-2222-222222222222");

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
            await using var command = new NpgsqlCommand(BuildSchemaAndSeedSql(), connection);
            await command.ExecuteNonQueryAsync();
        }

        private string BuildSchemaAndSeedSql() => $@"
DROP TABLE IF EXISTS wishlist_curtidas;
DROP TABLE IF EXISTS espetaculo_genero;
DROP TABLE IF EXISTS espetaculos;
DROP TABLE IF EXISTS generos;
DROP TABLE IF EXISTS usuarios;
DROP TYPE IF EXISTS tipo_lista;

CREATE TYPE tipo_lista AS ENUM ('curtida', 'wishlist');

CREATE TABLE usuarios(
    id UUID PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    senha_hash TEXT NOT NULL,
    idade INT CHECK(idade >= 13),
    data_cadastro TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE generos(
    id UUID PRIMARY KEY,
    nome VARCHAR(50) UNIQUE NOT NULL
);

CREATE TABLE espetaculos(
    id UUID PRIMARY KEY,
    nome VARCHAR(150) UNIQUE NOT NULL,
    tipo_evento VARCHAR(80),
    descricao TEXT,
    url_bilheteria TEXT,
    data_inicio TIMESTAMP NOT NULL,
    data_fim TIMESTAMP NOT NULL,
    valor_ingresso DECIMAL(10,2) CHECK(valor_ingresso >= 0),
    CONSTRAINT chk_datas CHECK(data_fim > data_inicio)
);

CREATE TABLE espetaculo_genero(
    espetaculo_id UUID REFERENCES espetaculos(id) ON DELETE CASCADE,
    genero_id UUID REFERENCES generos(id) ON DELETE CASCADE,
    PRIMARY KEY(espetaculo_id, genero_id)
);

CREATE TABLE wishlist_curtidas(
    usuario_id UUID REFERENCES usuarios(id) ON DELETE CASCADE,
    espetaculo_id UUID REFERENCES espetaculos(id) ON DELETE CASCADE,
    tipo tipo_lista,
    PRIMARY KEY(usuario_id, espetaculo_id, tipo)
);

INSERT INTO usuarios(id, nome, email, senha_hash, idade)
VALUES ('{UsuarioId}', 'Ana Teste', 'ana.e2e@example.com', 'hash', 28);

INSERT INTO generos(id, nome)
VALUES ('33333333-3333-3333-3333-333333333333', 'Musical');

INSERT INTO espetaculos(id, nome, tipo_evento, descricao, url_bilheteria, data_inicio, data_fim, valor_ingresso)
VALUES (
    '{EspetaculoId}',
    'Mamma Mia!',
    'Espetaculo Musical',
    'Musical em cartaz para validacao E2E.',
    'https://ingressos.example/mamma-mia',
    NOW() - INTERVAL '1 day',
    NOW() + INTERVAL '30 days',
    120.00
);

INSERT INTO espetaculo_genero(espetaculo_id, genero_id)
VALUES ('{EspetaculoId}', '33333333-3333-3333-3333-333333333333');
";
    }

    internal sealed class AventuraApiFactory : WebApplicationFactory<Program>
    {
        private readonly string _connectionString;

        public AventuraApiFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(configurationBuilder =>
            {
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = _connectionString,
                    ["Jwt:Key"] = "e2e-tests-secret-key-with-enough-length"
                });
            });

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IHostedService>();
                services
                    .AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthHandler.AuthScheme;
                        options.DefaultChallengeScheme = TestAuthHandler.AuthScheme;
                    })
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.AuthScheme, _ => { });
            });
        }
    }

    internal sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string AuthScheme = "Test";

        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var identity = new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, "e2e-user") },
                AuthScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, AuthScheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
