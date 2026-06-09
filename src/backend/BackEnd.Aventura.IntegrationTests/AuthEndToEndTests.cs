using System.Net;
using System.Net.Http.Json;
using BackEnd.Aventura.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace BackEnd.Aventura.IntegrationTests
{
    public sealed class AuthEndToEndTests : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("aventura_auth_e2e")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        public async Task InitializeAsync()
        {
            await _postgres.StartAsync();
            await using var connection = new NpgsqlConnection(_postgres.GetConnectionString());
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(AuthSchemaSql, connection);
            await command.ExecuteNonQueryAsync();
        }

        public async Task DisposeAsync()
        {
            await _postgres.DisposeAsync();
        }

        [Fact]
        public async Task Register_ELogin_ComDadosValidos_DeveCriarUsuarioEAutenticarViaHttp()
        {
            using var factory = CreateFactory();
            using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost")
            });

            var registerResponse = await client.PostAsJsonAsync("/api/Auth/register", new RegisterRequest
            {
                Nome = "Usuario E2E",
                Email = "usuario-e2e@example.com",
                Senha = "Senha@123",
                Idade = 23,
                Cep = "01310-100",
                Logradouro = "Avenida Paulista",
                Numero = "1000",
                Bairro = "Bela Vista",
                Cidade = "Sao Paulo",
                ConsentimentoAceito = true
            });

            Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);
            var registerBody = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
            Assert.NotNull(registerBody);
            Assert.NotEqual(Guid.Empty, registerBody.UsuarioId);
            Assert.Equal("usuario-e2e@example.com", registerBody.Email);
            Assert.False(string.IsNullOrWhiteSpace(registerBody.Token));
            Assert.Equal(1L, await CountUsuariosByEmailAsync("usuario-e2e@example.com"));

            var loginResponse = await client.PostAsJsonAsync("/api/Auth/login", new LoginRequest
            {
                Email = "usuario-e2e@example.com",
                Senha = "Senha@123"
            });

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
            var loginBody = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            Assert.NotNull(loginBody);
            Assert.Equal(registerBody.UsuarioId, loginBody.UsuarioId);
            Assert.Equal("Usuario E2E", loginBody.Nome);
            Assert.Equal("usuario-e2e@example.com", loginBody.Email);
            Assert.False(string.IsNullOrWhiteSpace(loginBody.Token));
        }

        [Fact]
        public async Task Register_ComEmailDuplicado_DeveRetornarConflictENaoDuplicarUsuario()
        {
            using var factory = CreateFactory();
            using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost")
            });

            var request = new RegisterRequest
            {
                Nome = "Usuario Duplicado",
                Email = "duplicado-e2e@example.com",
                Senha = "Senha@123",
                Idade = 24,
                ConsentimentoAceito = true
            };

            var firstResponse = await client.PostAsJsonAsync("/api/Auth/register", request);
            Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

            var duplicateResponse = await client.PostAsJsonAsync("/api/Auth/register", request);

            Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);
            var problem = await duplicateResponse.Content.ReadFromJsonAsync<ProblemDetails>();
            Assert.NotNull(problem);
            Assert.Equal((int)HttpStatusCode.Conflict, problem.Status);
            Assert.Contains("cadastrado", problem.Detail ?? string.Empty);
            Assert.Equal(1L, await CountUsuariosByEmailAsync("duplicado-e2e@example.com"));
        }

        [Fact]
        public async Task Login_ComEmailNaoCadastrado_DeveRetornarUnauthorized()
        {
            using var factory = CreateFactory();
            using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost")
            });

            var response = await client.PostAsJsonAsync("/api/Auth/login", new LoginRequest
            {
                Email = "nao-cadastrado-e2e@example.com",
                Senha = "Senha@123"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            Assert.NotNull(problem);
            Assert.Equal((int)HttpStatusCode.Unauthorized, problem.Status);
        }

        private WebApplicationFactory<Program> CreateFactory()
        {
            return new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureAppConfiguration((_, configuration) =>
                    {
                        configuration.AddInMemoryCollection(new Dictionary<string, string?>
                        {
                            ["ConnectionStrings:DefaultConnection"] = _postgres.GetConnectionString(),
                            ["Jwt:Key"] = "test-jwt-key-with-at-least-32-chars!",
                            ["Jwt:Issuer"] = "BackEnd.Aventura",
                            ["Jwt:Audience"] = "BackEnd.AventuraClient",
                            ["Jwt:ExpiryMinutes"] = "60",
                        });
                    });
                });
        }

        private async Task<long> CountUsuariosByEmailAsync(string email)
        {
            await using var connection = new NpgsqlConnection(_postgres.GetConnectionString());
            await connection.OpenAsync();
            await using var command = new NpgsqlCommand(
                "SELECT COUNT(*) FROM usuarios WHERE LOWER(email) = LOWER(@Email)",
                connection);
            command.Parameters.AddWithValue("@Email", email);

            return (long)(await command.ExecuteScalarAsync() ?? 0L);
        }

        private const string AuthSchemaSql = """
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

CREATE TABLE IF NOT EXISTS usuarios(
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nome VARCHAR(100) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    senha_hash TEXT NOT NULL,
    idade INT CHECK(idade >= 13),
    data_cadastro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    cep VARCHAR(9),
    logradouro VARCHAR(255),
    numero VARCHAR(10),
    complemento VARCHAR(255),
    bairro VARCHAR(100),
    cidade VARCHAR(100),
    consentimento_aceito BOOLEAN NOT NULL DEFAULT FALSE,
    data_consentimento TIMESTAMP,
    deleted_at TIMESTAMP,
    anonimizado BOOLEAN NOT NULL DEFAULT FALSE,
    ml_id SERIAL
);

CREATE INDEX IF NOT EXISTS idx_usuario_email ON usuarios(email);
CREATE UNIQUE INDEX IF NOT EXISTS idx_usuarios_ml_id ON usuarios(ml_id);

CREATE TABLE IF NOT EXISTS generos(
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nome VARCHAR(50) UNIQUE NOT NULL
);

CREATE TABLE IF NOT EXISTS preferencias(
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    usuario_id UUID UNIQUE,
    cidade VARCHAR(100),
    cep VARCHAR(9),
    tags TEXT[],
    faixa_preco_min DECIMAL(10,2),
    faixa_preco_max DECIMAL(10,2),
    receber_notificacoes BOOLEAN DEFAULT TRUE,
    data_atualizacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_preferencia_usuario
        FOREIGN KEY(usuario_id)
        REFERENCES usuarios(id)
        ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS usuario_genero_favorito(
    usuario_id UUID,
    genero_id UUID,
    PRIMARY KEY(usuario_id, genero_id),
    FOREIGN KEY(usuario_id) REFERENCES usuarios(id) ON DELETE CASCADE,
    FOREIGN KEY(genero_id) REFERENCES generos(id) ON DELETE CASCADE
);
""";
    }
}
