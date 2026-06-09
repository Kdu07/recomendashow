// ============================================================
//  Testes E2E — Preferencias
//  Camada: HTTP (Controller) → Service → Repository → PostgreSQL
//
//  Ferramentas:
//    • WebApplicationFactory  → sobe a API ASP.NET inteira in-process
//    • Testcontainers         → dependência POSSUÍDA (PostgreSQL real)
//    • WireMock.Net           → dependência CONSUMIDA (API ViaCEP externa)
//
//  Pacotes necessários no projeto de testes:
//    dotnet add package Microsoft.AspNetCore.Mvc.Testing
//    dotnet add package Testcontainers.PostgreSql
//    dotnet add package WireMock.Net
//    dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
//
//  RF coberto  : RF08 — Gestão de Perfil e Preferências
//  RNF coberto : RNF05 — Confiabilidade (ISO 25010)
// ============================================================

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BackEnd.Aventura.Dto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Testcontainers.PostgreSql;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace BackEnd.Aventura.Tests.Integration
{
    // ──────────────────────────────────────────────────────────
    //  CONSTANTES DE TESTE
    // ──────────────────────────────────────────────────────────

    internal static class TestJwt
    {
        // Chave usada SOMENTE nos testes — não é a chave de produção
        public const string Key      = "aventura-test-secret-key-32-chars!!";
        public const string Issuer   = "BackEnd.Aventura";
        public const string Audience = "BackEnd.AventuraClient";

        /// <summary>Gera um token JWT válido para o usuário informado.</summary>
        public static string Generate(Guid userId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            var credentials  = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,   userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, $"teste_{userId}@aventura.com"),
                new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer:             Issuer,
                audience:           Audience,
                claims:             claims,
                expires:            DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // ──────────────────────────────────────────────────────────
    //  FIXTURE — PostgreSQL via Testcontainers
    // ──────────────────────────────────────────────────────────

    public class PostgresFixture : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _db = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("aventura_test")
            .WithUsername("app")
            .WithPassword("app")
            .WithCleanUp(true)
            .Build();

        public string ConnectionString => _db.GetConnectionString();

        public async Task InitializeAsync()
        {
            await _db.StartAsync();
            await CreateSchemaAsync();
        }

        public Task DisposeAsync() => _db.DisposeAsync().AsTask();

        public async Task ResetAsync()
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                DELETE FROM usuario_genero_favorito;
                DELETE FROM preferencias;
                DELETE FROM generos;
                DELETE FROM usuarios;";
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task SeedGenerosAsync(params string[] nomes)
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();
            foreach (var nome in nomes)
            {
                await using var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO generos(nome) VALUES (@n) ON CONFLICT DO NOTHING";
                cmd.Parameters.AddWithValue("@n", nome);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task CreateSchemaAsync()
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS usuarios (
                    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                    nome                TEXT NOT NULL,
                    email               TEXT NOT NULL UNIQUE,
                    senha_hash          TEXT NOT NULL,
                    idade               INT,
                    cep                 TEXT,
                    logradouro          TEXT,
                    numero              TEXT,
                    complemento         TEXT,
                    bairro              TEXT,
                    cidade              TEXT,
                    consentimento_aceito BOOLEAN NOT NULL DEFAULT FALSE,
                    data_cadastro       TIMESTAMP NOT NULL DEFAULT NOW(),
                    deleted_at          TIMESTAMP,
                    anonimizado         BOOLEAN NOT NULL DEFAULT FALSE,
                    ml_id               INT
                );

                CREATE TABLE IF NOT EXISTS generos (
                    id   UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                    nome TEXT NOT NULL UNIQUE
                );

                CREATE TABLE IF NOT EXISTS preferencias (
                    id                   UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                    usuario_id           UUID NOT NULL UNIQUE,
                    cidade               TEXT,
                    cep                  TEXT,
                    tags                 TEXT[],
                    faixa_preco_min      NUMERIC,
                    faixa_preco_max      NUMERIC,
                    receber_notificacoes BOOLEAN NOT NULL DEFAULT TRUE,
                    data_atualizacao     TIMESTAMP NOT NULL DEFAULT NOW()
                );

                CREATE TABLE IF NOT EXISTS usuario_genero_favorito (
                    usuario_id UUID NOT NULL,
                    genero_id  UUID NOT NULL REFERENCES generos(id) ON DELETE CASCADE,
                    PRIMARY KEY (usuario_id, genero_id)
                );";
            await cmd.ExecuteNonQueryAsync();
        }
    }

    // ──────────────────────────────────────────────────────────
    //  FIXTURE — WireMock (stub ViaCEP)
    // ──────────────────────────────────────────────────────────

    public class ViaCepApiFixture : IAsyncLifetime
    {
        public WireMockServer Server { get; private set; } = null!;
        public string Url => Server.Url!;

        public Task InitializeAsync()
        {
            Server = WireMockServer.Start(new WireMock.Settings.WireMockServerSettings { Port = 0 });
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            Server.Stop();
            Server.Dispose();
            return Task.CompletedTask;
        }

        public void StubCepValido(string cep, string cidade, string uf)
        {
            Server.Reset();
            Server
                .Given(Request.Create().UsingGet().WithPath($"/ws/{cep}/json/"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyAsJson(new { cep, localidade = cidade, uf, erro = (bool?)null }));
        }

        public void StubCepNaoEncontrado(string cep)
        {
            Server.Reset();
            Server
                .Given(Request.Create().UsingGet().WithPath($"/ws/{cep}/json/"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyAsJson(new { erro = true }));
        }

        public void StubViaCepIndisponivel(string cep)
        {
            Server.Reset();
            Server
                .Given(Request.Create().UsingGet().WithPath($"/ws/{cep}/json/"))
                .RespondWith(Response.Create().WithStatusCode(503).WithBody("unavailable"));
        }
    }

    // ──────────────────────────────────────────────────────────
    //  FACTORY — Sobe a API inteira substituindo o banco
    // ──────────────────────────────────────────────────────────

    public class AventuraWebFactory : WebApplicationFactory<Program>
    {
        private readonly string _connectionString;

        public AventuraWebFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Define a variável de ambiente ANTES de qualquer coisa do Program.cs rodar
            Environment.SetEnvironmentVariable("JWT_KEY", TestJwt.Key);
        
            builder.UseEnvironment("Testing");
        
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = _connectionString,
                    ["Jwt:Key"]      = TestJwt.Key,
                    ["Jwt:Issuer"]   = TestJwt.Issuer,
                    ["Jwt:Audience"] = TestJwt.Audience,
                });
            });
        }
    }

    // ──────────────────────────────────────────────────────────
    //  COLLECTION — compartilha fixtures entre todas as suites
    // ──────────────────────────────────────────────────────────

    [CollectionDefinition("preferencias-e2e")]
    public class PreferenciasE2ECollection
        : ICollectionFixture<PostgresFixture>,
          ICollectionFixture<ViaCepApiFixture>
    { }

    // ──────────────────────────────────────────────────────────
    //  HELPERS
    // ──────────────────────────────────────────────────────────

    internal static class HttpClientExtensions
    {
        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static async Task<T?> ReadAsAsync<T>(this HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, JsonOpts);
        }
    }

    // ══════════════════════════════════════════════════════════
    //  SUITE 1 — GET /api/preferencias/{usuarioId}
    //  Fluxo: HTTP → Controller → Service → Repository → PostgreSQL
    // ══════════════════════════════════════════════════════════

    [Collection("preferencias-e2e")]
    public class GetPreferenciasE2ETests : IAsyncLifetime
    {
        private readonly PostgresFixture _pg;
        private readonly AventuraWebFactory _factory;
        private readonly HttpClient _client;

        public GetPreferenciasE2ETests(PostgresFixture pg, ViaCepApiFixture _)
        {
            _pg      = pg;
            _factory = new AventuraWebFactory(pg.ConnectionString);
            _client  = _factory.CreateClient();
        }

        public Task InitializeAsync() => _pg.ResetAsync();
        public async Task DisposeAsync()
        {
            _client.Dispose();
            await _factory.DisposeAsync();
        }

        private void Authenticate(Guid userId)
        {
            var token = TestJwt.Generate(userId);
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        // ── Cenários de sucesso ──────────────────────────────

        [Fact]
        public async Task Get_UsuarioNovo_Retorna200ComPreferenciasDefault()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);

            // Act
            var response = await _client.GetAsync($"/api/preferencias/{userId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var dto = await response.ReadAsAsync<PreferenciasDto>();
            Assert.NotNull(dto);
            Assert.Equal(userId, dto!.UsuarioId);
            Assert.True(dto.ReceberNotificacoes);
            Assert.Empty(dto.Generos);
            Assert.Empty(dto.Tags);
        }

        [Fact]
        public async Task Get_AposUpdate_RetornaEstadoAtualizado()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);

            var putBody = new PreferenciasUpdateDto
            {
                Cidade = "Recife",
                Cep    = "50060-001"
            };
            await _client.PutAsJsonAsync($"/api/preferencias/{userId}", putBody);

            // Act
            var response = await _client.GetAsync($"/api/preferencias/{userId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var dto = await response.ReadAsAsync<PreferenciasDto>();
            Assert.Equal("Recife",    dto!.Cidade);
            Assert.Equal("50060-001", dto.Cep);
        }

        [Fact]
        public async Task Get_DoisUsuariosDiferentes_NaoMisturaDados()
        {
            // Arrange
            var userA = Guid.NewGuid();
            var userB = Guid.NewGuid();

            Authenticate(userA);
            await _client.PutAsJsonAsync($"/api/preferencias/{userA}",
                new PreferenciasUpdateDto { Cidade = "São Paulo" });

            Authenticate(userB);
            await _client.PutAsJsonAsync($"/api/preferencias/{userB}",
                new PreferenciasUpdateDto { Cidade = "Fortaleza" });

            // Act
            Authenticate(userA);
            var respA = await _client.GetAsync($"/api/preferencias/{userA}");
            Authenticate(userB);
            var respB = await _client.GetAsync($"/api/preferencias/{userB}");

            // Assert
            var dtoA = await respA.ReadAsAsync<PreferenciasDto>();
            var dtoB = await respB.ReadAsAsync<PreferenciasDto>();
            Assert.Equal("São Paulo", dtoA!.Cidade);
            Assert.Equal("Fortaleza", dtoB!.Cidade);
        }

        [Fact]
        public async Task Get_ChamadoDuasVezes_RetornaConsistente()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);

            // Act
            var r1 = await _client.GetAsync($"/api/preferencias/{userId}");
            var r2 = await _client.GetAsync($"/api/preferencias/{userId}");

            // Assert — idempotência
            Assert.Equal(HttpStatusCode.OK, r1.StatusCode);
            Assert.Equal(HttpStatusCode.OK, r2.StatusCode);
            var d1 = await r1.ReadAsAsync<PreferenciasDto>();
            var d2 = await r2.ReadAsAsync<PreferenciasDto>();
            Assert.Equal(d1!.UsuarioId, d2!.UsuarioId);
        }

        [Fact]
        public async Task Get_ResponseContemCamposObrigatorios()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);

            // Act
            var response = await _client.GetAsync($"/api/preferencias/{userId}");
            var dto      = await response.ReadAsAsync<PreferenciasDto>();

            // Assert — contrato do DTO
            Assert.NotNull(dto);
            Assert.NotEqual(Guid.Empty, dto!.UsuarioId);
            Assert.True(dto.DataAtualizacao > DateTime.MinValue);
            Assert.NotNull(dto.Generos);
            Assert.NotNull(dto.Tags);
        }

        // ── Cenários de erro ────────────────────────────────

        [Fact]
        public async Task Get_SemToken_Retorna401()
        {
            // Arrange — sem autenticação
            var userId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/preferencias/{userId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Get_TokenInvalido_Retorna401()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "token.invalido.aqui");

            // Act
            var response = await _client.GetAsync($"/api/preferencias/{userId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Get_GuidVazio_Retorna400OuNotFound()
        {
            // Arrange — guid vazio na rota não é roteado pelo ASP.NET
            var userId = Guid.NewGuid();
            Authenticate(userId);

            // Act — guid "00000000-0000-0000-0000-000000000000" deve ser barrado
            var response = await _client.GetAsync($"/api/preferencias/00000000-0000-0000-0000-000000000000");

            // Assert
            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest ||
                response.StatusCode == HttpStatusCode.NotFound,
                $"Esperava 400 ou 404, recebeu {(int)response.StatusCode}");
        }

        [Fact]
        public async Task Get_ContentTypeEhJson()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);

            // Act
            var response = await _client.GetAsync($"/api/preferencias/{userId}");

            // Assert
            Assert.Equal("application/json",
                response.Content.Headers.ContentType?.MediaType);
        }
    }

    // ══════════════════════════════════════════════════════════
    //  SUITE 2 — PUT /api/preferencias/{usuarioId}
    //  Fluxo: HTTP → Controller → Service → Repository → PostgreSQL
    // ══════════════════════════════════════════════════════════

    [Collection("preferencias-e2e")]
    public class PutPreferenciasE2ETests : IAsyncLifetime
    {
        private readonly PostgresFixture _pg;
        private readonly AventuraWebFactory _factory;
        private readonly HttpClient _client;

        public PutPreferenciasE2ETests(PostgresFixture pg, ViaCepApiFixture _)
        {
            _pg      = pg;
            _factory = new AventuraWebFactory(pg.ConnectionString);
            _client  = _factory.CreateClient();
        }

        public Task InitializeAsync() => _pg.ResetAsync();
        public async Task DisposeAsync()
        {
            _client.Dispose();
            await _factory.DisposeAsync();
        }

        private void Authenticate(Guid userId)
        {
            var token = TestJwt.Generate(userId);
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        // ── Cenários de sucesso ──────────────────────────────

        [Fact]
        public async Task Put_Cidade_Retorna200ComCidadeAtualizada()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);
            var dto = new PreferenciasUpdateDto { Cidade = "Manaus" };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/preferencias/{userId}", dto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.ReadAsAsync<PreferenciasDto>();
            Assert.Equal("Manaus", result!.Cidade);
        }

        [Fact]
        public async Task Put_Cep_Retorna200ComCepAtualizado()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);
            var dto = new PreferenciasUpdateDto { Cep = "69005-141" };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/preferencias/{userId}", dto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.ReadAsAsync<PreferenciasDto>();
            Assert.Equal("69005-141", result!.Cep);
        }

        [Fact]
        public async Task Put_CepECidade_AmbosAtualizados()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);
            var dto = new PreferenciasUpdateDto { Cep = "40020-010", Cidade = "Salvador" };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/preferencias/{userId}", dto);
            var result   = await response.ReadAsAsync<PreferenciasDto>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("40020-010", result!.Cep);
            Assert.Equal("Salvador",  result.Cidade);
        }

        [Fact]
        public async Task Put_FaixaPrecoValida_Retorna200()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);
            var dto = new PreferenciasUpdateDto { FaixaPrecoMin = 20m, FaixaPrecoMax = 300m };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/preferencias/{userId}", dto);
            var result   = await response.ReadAsAsync<PreferenciasDto>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(20m,  result!.FaixaPrecoMin);
            Assert.Equal(300m, result.FaixaPrecoMax);
        }

        [Fact]
        public async Task Put_ReceberNotificacoesFalse_Persistido()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);
            var dto = new PreferenciasUpdateDto { ReceberNotificacoes = false };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/preferencias/{userId}", dto);
            var result   = await response.ReadAsAsync<PreferenciasDto>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(result!.ReceberNotificacoes);
        }

        [Fact]
        public async Task Put_Tags_PersistidoCorretamente()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);
            var dto = new PreferenciasUpdateDto
            {
                Tags = new[] { "familiar", "ao-vivo" }
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/preferencias/{userId}", dto);
            var result   = await response.ReadAsAsync<PreferenciasDto>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("familiar", result!.Tags);
            Assert.Contains("ao-vivo",  result.Tags);
        }

        [Fact]
        public async Task Put_Generos_PersistidosViaJoin()
        {
            // Arrange
            await _pg.SeedGenerosAsync("Drama", "Musical");
            var userId = Guid.NewGuid();
            Authenticate(userId);
            var dto = new PreferenciasUpdateDto
            {
                Generos = new[] { "Drama", "Musical" }
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/preferencias/{userId}", dto);
            var result   = await response.ReadAsAsync<PreferenciasDto>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Drama",   result!.Generos);
            Assert.Contains("Musical", result.Generos);
        }

        [Fact]
        public async Task Put_DoisUpdatesSequenciais_UltimoEstadoPrevalesce()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);

            await _client.PutAsJsonAsync($"/api/preferencias/{userId}",
                new PreferenciasUpdateDto { Cidade = "Porto Alegre" });

            // Act
            var response = await _client.PutAsJsonAsync($"/api/preferencias/{userId}",
                new PreferenciasUpdateDto { Cidade = "Curitiba" });
            var result = await response.ReadAsAsync<PreferenciasDto>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Curitiba", result!.Cidade);
        }

        [Fact]
        public async Task Put_ApenasUmCampo_OutrosNaoSaoApagados()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);

            await _client.PutAsJsonAsync($"/api/preferencias/{userId}",
                new PreferenciasUpdateDto { Cidade = "Belém", Cep = "66010-020" });

            // Act — atualiza só notificações
            var response = await _client.PutAsJsonAsync($"/api/preferencias/{userId}",
                new PreferenciasUpdateDto { ReceberNotificacoes = false });
            var result = await response.ReadAsAsync<PreferenciasDto>();

            // Assert — cidade e cep preservados
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Belém",     result!.Cidade);
            Assert.Equal("66010-020", result.Cep);
            Assert.False(result.ReceberNotificacoes);
        }

        [Fact]
        public async Task Put_RetornaEstadoAposUpdate_NaoAntigo()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);

            await _client.PutAsJsonAsync($"/api/preferencias/{userId}",
                new PreferenciasUpdateDto { Cidade = "Natal" });

            // Act
            var response = await _client.PutAsJsonAsync($"/api/preferencias/{userId}",
                new PreferenciasUpdateDto { Cidade = "Maceió" });
            var result = await response.ReadAsAsync<PreferenciasDto>();

            // Assert — retorno é o estado APÓS o update
            Assert.Equal("Maceió", result!.Cidade);
        }

        [Fact]
        public async Task Put_TodosCampos_TodosPersistidos()
        {
            // Arrange
            await _pg.SeedGenerosAsync("Jazz", "Ballet");
            var userId = Guid.NewGuid();
            Authenticate(userId);
            var dto = new PreferenciasUpdateDto
            {
                Generos            = new[] { "Jazz", "Ballet" },
                Tags               = new[] { "ao-vivo", "clássico" },
                Cidade             = "São Paulo",
                Cep                = "04538-133",
                FaixaPrecoMin      = 30m,
                FaixaPrecoMax      = 500m,
                ReceberNotificacoes = false
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/preferencias/{userId}", dto);
            var result   = await response.ReadAsAsync<PreferenciasDto>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("São Paulo",  result!.Cidade);
            Assert.Equal("04538-133",  result.Cep);
            Assert.Equal(30m,          result.FaixaPrecoMin);
            Assert.Equal(500m,         result.FaixaPrecoMax);
            Assert.False(result.ReceberNotificacoes);
            Assert.Contains("Jazz",     result.Generos);
            Assert.Contains("Ballet",   result.Generos);
            Assert.Contains("ao-vivo",  result.Tags);
            Assert.Contains("clássico", result.Tags);
        }

        // ── Cenários de erro ────────────────────────────────

        [Fact]
        public async Task Put_SemToken_Retorna401()
        {
            // Arrange — sem autenticação
            var userId = Guid.NewGuid();
            var dto    = new PreferenciasUpdateDto { Cidade = "Goiânia" };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/preferencias/{userId}", dto);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Put_FaixaPrecoInvalida_Retorna400()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);
            var dto = new PreferenciasUpdateDto { FaixaPrecoMin = 500m, FaixaPrecoMax = 10m };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/preferencias/{userId}", dto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Put_BodyNulo_Retorna400()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);

            // Act — envia corpo vazio
            var response = await _client.PutAsync(
                $"/api/preferencias/{userId}",
                new StringContent("null", Encoding.UTF8, "application/json"));

            // Assert
            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest ||
                response.StatusCode == HttpStatusCode.UnsupportedMediaType,
                $"Esperava 400, recebeu {(int)response.StatusCode}");
        }

        [Fact]
        public async Task Put_ContentTypeEhJson()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);

            // Act
            var response = await _client.PutAsJsonAsync($"/api/preferencias/{userId}",
                new PreferenciasUpdateDto { Cidade = "Teresina" });

            // Assert
            Assert.Equal("application/json",
                response.Content.Headers.ContentType?.MediaType);
        }
    }

    // ══════════════════════════════════════════════════════════
    //  SUITE 3 — Fluxo GET → PUT → GET (ciclo completo)
    //  Verifica consistência ponta a ponta via HTTP
    // ══════════════════════════════════════════════════════════

    [Collection("preferencias-e2e")]
    public class FluxoCompletoE2ETests : IAsyncLifetime
    {
        private readonly PostgresFixture _pg;
        private readonly AventuraWebFactory _factory;
        private readonly HttpClient _client;

        public FluxoCompletoE2ETests(PostgresFixture pg, ViaCepApiFixture _)
        {
            _pg      = pg;
            _factory = new AventuraWebFactory(pg.ConnectionString);
            _client  = _factory.CreateClient();
        }

        public Task InitializeAsync() => _pg.ResetAsync();
        public async Task DisposeAsync()
        {
            _client.Dispose();
            await _factory.DisposeAsync();
        }

        private void Authenticate(Guid userId)
        {
            var token = TestJwt.Generate(userId);
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        [Fact]
        public async Task FluxoGetPutGet_EstadoConsistente()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);

            // Act — 1) GET inicial
            var getInicial = await _client.GetAsync($"/api/preferencias/{userId}");
            var inicial    = await getInicial.ReadAsAsync<PreferenciasDto>();

            // Act — 2) PUT com novos dados
            var putResponse = await _client.PutAsJsonAsync($"/api/preferencias/{userId}",
                new PreferenciasUpdateDto { Cidade = "Vitória", FaixaPrecoMin = 10m, FaixaPrecoMax = 100m });

            // Act — 3) GET após update
            var getFinal = await _client.GetAsync($"/api/preferencias/{userId}");
            var final    = await getFinal.ReadAsAsync<PreferenciasDto>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, getInicial.StatusCode);
            Assert.Null(inicial!.Cidade);           // default: sem cidade
            Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, getFinal.StatusCode);
            Assert.Equal("Vitória", final!.Cidade);
            Assert.Equal(10m,       final.FaixaPrecoMin);
            Assert.Equal(100m,      final.FaixaPrecoMax);
        }

        [Fact]
        public async Task FluxoMultiplosUpdates_SemContaminacaoCruzada()
        {
            // Arrange
            var users = Enumerable.Range(0, 5).Select(_ => Guid.NewGuid()).ToList();

            // Act — cada usuário recebe sua própria cidade
            foreach (var (uid, i) in users.Select((u, i) => (u, i)))
            {
                Authenticate(uid);
                await _client.PutAsJsonAsync($"/api/preferencias/{uid}",
                    new PreferenciasUpdateDto { Cidade = $"Cidade{i}" });
            }

            // Assert — cada usuário lê só a sua cidade
            for (var i = 0; i < users.Count; i++)
            {
                Authenticate(users[i]);
                var response = await _client.GetAsync($"/api/preferencias/{users[i]}");
                var dto      = await response.ReadAsAsync<PreferenciasDto>();
                Assert.Equal($"Cidade{i}", dto!.Cidade);
            }
        }

        [Fact]
        public async Task FluxoGeneros_AddSubstitui_SemOrfaos()
        {
            // Arrange
            await _pg.SeedGenerosAsync("Drama", "Musical");
            var userId = Guid.NewGuid();
            Authenticate(userId);

            // Act — primeiro salva Drama
            await _client.PutAsJsonAsync($"/api/preferencias/{userId}",
                new PreferenciasUpdateDto { Generos = new[] { "Drama" } });

            // Act — substitui por Musical
            await _client.PutAsJsonAsync($"/api/preferencias/{userId}",
                new PreferenciasUpdateDto { Generos = new[] { "Musical" } });

            // Act — GET final
            var response = await _client.GetAsync($"/api/preferencias/{userId}");
            var result   = await response.ReadAsAsync<PreferenciasDto>();

            // Assert — Drama removido, Musical presente
            Assert.DoesNotContain("Drama",   result!.Generos);
            Assert.Contains("Musical",       result.Generos);
        }

        [Fact]
        public async Task FluxoNotificacoes_MudaDirecoesOpostas()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);

            // Act — desliga
            await _client.PutAsJsonAsync($"/api/preferencias/{userId}",
                new PreferenciasUpdateDto { ReceberNotificacoes = false });
            var r1 = await _client.GetAsync($"/api/preferencias/{userId}");
            var d1 = await r1.ReadAsAsync<PreferenciasDto>();

            // Act — religa
            await _client.PutAsJsonAsync($"/api/preferencias/{userId}",
                new PreferenciasUpdateDto { ReceberNotificacoes = true });
            var r2 = await _client.GetAsync($"/api/preferencias/{userId}");
            var d2 = await r2.ReadAsAsync<PreferenciasDto>();

            // Assert
            Assert.False(d1!.ReceberNotificacoes);
            Assert.True(d2!.ReceberNotificacoes);
        }

        [Fact]
        public async Task FluxoTags_UpdateSubstitui()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);

            await _client.PutAsJsonAsync($"/api/preferencias/{userId}",
                new PreferenciasUpdateDto { Tags = new[] { "familiar" } });

            // Act
            await _client.PutAsJsonAsync($"/api/preferencias/{userId}",
                new PreferenciasUpdateDto { Tags = new[] { "clássico", "ao-vivo" } });

            var response = await _client.GetAsync($"/api/preferencias/{userId}");
            var result   = await response.ReadAsAsync<PreferenciasDto>();

            // Assert
            Assert.DoesNotContain("familiar", result!.Tags);
            Assert.Contains("clássico", result.Tags);
            Assert.Contains("ao-vivo",  result.Tags);
        }
    }

    // ══════════════════════════════════════════════════════════
    //  SUITE 4 — ViaCEP via WireMock + API real
    //  Dependência CONSUMIDA testada junto ao fluxo HTTP
    // ══════════════════════════════════════════════════════════

    [Collection("preferencias-e2e")]
    public class ViaCepE2ETests : IAsyncLifetime
    {
        private readonly PostgresFixture  _pg;
        private readonly ViaCepApiFixture _cep;
        private readonly AventuraWebFactory _factory;
        private readonly HttpClient _client;
        private readonly HttpClient _viaCepClient;

        public ViaCepE2ETests(PostgresFixture pg, ViaCepApiFixture cep)
        {
            _pg      = pg;
            _cep     = cep;
            _factory = new AventuraWebFactory(pg.ConnectionString);
            _client  = _factory.CreateClient();
            _viaCepClient = new HttpClient
            {
                BaseAddress = new Uri(cep.Url),
                Timeout     = TimeSpan.FromSeconds(3)
            };
        }

        public Task InitializeAsync() => _pg.ResetAsync();
        public async Task DisposeAsync()
        {
            _client.Dispose();
            _viaCepClient.Dispose();
            await _factory.DisposeAsync();
        }

        private void Authenticate(Guid userId)
        {
            var token = TestJwt.Generate(userId);
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        // ── ViaCEP stub ──────────────────────────────────────

        [Fact]
        public async Task ViaCep_CepValido_RetornaCidade()
        {
            // Arrange
            _cep.StubCepValido("01310-100", "São Paulo", "SP");

            // Act — consulta o stub como o sistema faria
            var response = await _viaCepClient.GetAsync("/ws/01310-100/json/");
            var json     = await response.Content.ReadAsStringAsync();
            var doc      = System.Text.Json.JsonDocument.Parse(json);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("São Paulo", doc.RootElement.GetProperty("localidade").GetString());
            Assert.Equal("SP",        doc.RootElement.GetProperty("uf").GetString());
        }

        [Fact]
        public async Task ViaCep_CepNaoEncontrado_RetornaErroTrue()
        {
            // Arrange
            _cep.StubCepNaoEncontrado("99999-999");

            // Act
            var response = await _viaCepClient.GetAsync("/ws/99999-999/json/");
            var json     = await response.Content.ReadAsStringAsync();
            var doc      = System.Text.Json.JsonDocument.Parse(json);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(doc.RootElement.GetProperty("erro").GetBoolean());
        }

        [Fact]
        public async Task ViaCep_Indisponivel_Retorna503()
        {
            // Arrange
            _cep.StubViaCepIndisponivel("01310-100");

            // Act
            var response = await _viaCepClient.GetAsync("/ws/01310-100/json/");

            // Assert
            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        }

        // ── Fluxo combinado: ViaCEP + API + banco ───────────

        [Fact]
        public async Task FluxoCompleto_ViaCepValidaCep_ApiPersisteCidade()
        {
            // Este teste cobre as DUAS dependências em um único fluxo:
            //   WireMock → simula ViaCEP retornando cidade para o CEP
            //   Testcontainers → PostgreSQL real persiste a preferência
            //   WebApplicationFactory → API inteira in-process

            // Arrange
            _cep.StubCepValido("04538-133", "São Paulo", "SP");
            var userId = Guid.NewGuid();
            Authenticate(userId);

            // Act — 1) consulta ViaCEP para obter a cidade
            var cepResponse = await _viaCepClient.GetAsync("/ws/04538-133/json/");
            var cepJson     = await cepResponse.Content.ReadAsStringAsync();
            var cepDoc      = System.Text.Json.JsonDocument.Parse(cepJson);
            var cidade      = cepDoc.RootElement.GetProperty("localidade").GetString();

            // Act — 2) persiste via API (banco real)
            var putResponse = await _client.PutAsJsonAsync($"/api/preferencias/{userId}",
                new PreferenciasUpdateDto { Cep = "04538-133", Cidade = cidade });

            // Act — 3) lê de volta via API
            var getResponse = await _client.GetAsync($"/api/preferencias/{userId}");
            var result      = await getResponse.ReadAsAsync<PreferenciasDto>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            Assert.Equal("São Paulo", result!.Cidade);
            Assert.Equal("04538-133", result.Cep);
        }

        [Fact]
        public async Task FluxoFalha_ViaCepIndisponivel_ApiPersisteDadosAntigos()
        {
            // Arrange — primeiro salva uma cidade
            var userId = Guid.NewGuid();
            Authenticate(userId);
            await _client.PutAsJsonAsync($"/api/preferencias/{userId}",
                new PreferenciasUpdateDto { Cidade = "Brasília" });

            // Arrange — ViaCEP fica fora do ar
            _cep.StubViaCepIndisponivel("70040-020");

            // Act — tenta consultar o ViaCEP indisponível
            var cepResponse = await _viaCepClient.GetAsync("/ws/70040-020/json/");

            // Act — sem atualizar as preferências (falha antes de chamar a API)
            // lê as preferências existentes
            var getResponse = await _client.GetAsync($"/api/preferencias/{userId}");
            var result      = await getResponse.ReadAsAsync<PreferenciasDto>();

            // Assert — preferências anteriores preservadas, não corrompidas
            Assert.Equal(HttpStatusCode.ServiceUnavailable, cepResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            Assert.Equal("Brasília", result!.Cidade);
        }

        [Fact]
        public async Task ViaCep_WireMockRegistraChamadasCorretamente()
        {
            // Arrange
            _cep.StubCepValido("20040-020", "Rio de Janeiro", "RJ");

            // Act — duas chamadas ao mesmo CEP
            await _viaCepClient.GetAsync("/ws/20040-020/json/");
            await _viaCepClient.GetAsync("/ws/20040-020/json/");

            // Assert — WireMock registrou exatamente 2 chamadas
            Assert.Equal(2, _cep.Server.LogEntries.Count());
        }
    }

    // ══════════════════════════════════════════════════════════
    //  SUITE 5 — Edge Cases via HTTP
    // ══════════════════════════════════════════════════════════

    [Collection("preferencias-e2e")]
    public class EdgeCasesE2ETests : IAsyncLifetime
    {
        private readonly PostgresFixture _pg;
        private readonly AventuraWebFactory _factory;
        private readonly HttpClient _client;

        public EdgeCasesE2ETests(PostgresFixture pg, ViaCepApiFixture _)
        {
            _pg      = pg;
            _factory = new AventuraWebFactory(pg.ConnectionString);
            _client  = _factory.CreateClient();
        }

        public Task InitializeAsync() => _pg.ResetAsync();
        public async Task DisposeAsync()
        {
            _client.Dispose();
            await _factory.DisposeAsync();
        }

        private void Authenticate(Guid userId)
        {
            var token = TestJwt.Generate(userId);
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        [Fact]
        public async Task Put_CidadeComAcento_PersistidaCorretamente()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);

            // Act
            await _client.PutAsJsonAsync($"/api/preferencias/{userId}",
                new PreferenciasUpdateDto { Cidade = "Águas de Lindóia" });
            var response = await _client.GetAsync($"/api/preferencias/{userId}");
            var result   = await response.ReadAsAsync<PreferenciasDto>();

            // Assert
            Assert.Equal("Águas de Lindóia", result!.Cidade);
        }

        [Fact]
        public async Task Put_TagComCaracterEspecial_Persistida()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);

            // Act
            await _client.PutAsJsonAsync($"/api/preferencias/{userId}",
                new PreferenciasUpdateDto { Tags = new[] { "família & crianças" } });
            var response = await _client.GetAsync($"/api/preferencias/{userId}");
            var result   = await response.ReadAsAsync<PreferenciasDto>();

            // Assert
            Assert.Contains("família & crianças", result!.Tags);
        }

        [Fact]
        public async Task Put_TagsDuplicadas_SalvaUmaVez()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);

            // Act
            await _client.PutAsJsonAsync($"/api/preferencias/{userId}",
                new PreferenciasUpdateDto { Tags = new[] { "musical", "musical", "MUSICAL" } });
            var response = await _client.GetAsync($"/api/preferencias/{userId}");
            var result   = await response.ReadAsAsync<PreferenciasDto>();

            // Assert
            Assert.Single(result!.Tags);
        }

        [Fact]
        public async Task Put_FaixaPrecoMinIgualMax_NaoLanca()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);

            // Act
            var response = await _client.PutAsJsonAsync($"/api/preferencias/{userId}",
                new PreferenciasUpdateDto { FaixaPrecoMin = 100m, FaixaPrecoMax = 100m });

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Put_FaixaPrecoDecimalAlta_Persistida()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);

            // Act
            await _client.PutAsJsonAsync($"/api/preferencias/{userId}",
                new PreferenciasUpdateDto { FaixaPrecoMin = 0.01m, FaixaPrecoMax = 99999.99m });
            var response = await _client.GetAsync($"/api/preferencias/{userId}");
            var result   = await response.ReadAsAsync<PreferenciasDto>();

            // Assert
            Assert.Equal(0.01m,     result!.FaixaPrecoMin);
            Assert.Equal(99999.99m, result.FaixaPrecoMax);
        }

        [Fact]
        public async Task Get_10VezesSequenciais_SemException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);

            // Act & Assert — 10 GETs seguidos sem falha
            for (var i = 0; i < 10; i++)
            {
                var response = await _client.GetAsync($"/api/preferencias/{userId}");
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Put_CepVazio_GravaNull()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);

            // Act
            await _client.PutAsJsonAsync($"/api/preferencias/{userId}",
                new PreferenciasUpdateDto { Cep = "" });
            var response = await _client.GetAsync($"/api/preferencias/{userId}");
            var result   = await response.ReadAsAsync<PreferenciasDto>();

            // Assert
            Assert.Null(result!.Cep);
        }

        [Fact]
        public async Task Put_GenerosVazios_ListaVazia()
        {
            // Arrange
            await _pg.SeedGenerosAsync("Drama");
            var userId = Guid.NewGuid();
            Authenticate(userId);

            // Primeiro adiciona Drama
            await _client.PutAsJsonAsync($"/api/preferencias/{userId}",
                new PreferenciasUpdateDto { Generos = new[] { "Drama" } });

            // Act — remove todos
            await _client.PutAsJsonAsync($"/api/preferencias/{userId}",
                new PreferenciasUpdateDto { Generos = Array.Empty<string>() });
            var response = await _client.GetAsync($"/api/preferencias/{userId}");
            var result   = await response.ReadAsAsync<PreferenciasDto>();

            // Assert
            Assert.Empty(result!.Generos);
        }

        [Theory]
        [InlineData("familiar")]
        [InlineData("ao-vivo")]
        [InlineData("clássico")]
        public async Task Put_CadaTagIndividual_PersistidaViaHttp(string tag)
        {
            // Arrange
            var userId = Guid.NewGuid();
            Authenticate(userId);

            // Act
            await _client.PutAsJsonAsync($"/api/preferencias/{userId}",
                new PreferenciasUpdateDto { Tags = new[] { tag } });
            var response = await _client.GetAsync($"/api/preferencias/{userId}");
            var result   = await response.ReadAsAsync<PreferenciasDto>();

            // Assert
            Assert.Contains(tag, result!.Tags);
        }
    }
}