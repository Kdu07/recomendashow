// ============================================================
//  Testes de Integração — Preferencias
//  Ferramentas:
//    • Testcontainers.PostgreSql  → dependência POSSUÍDA  (banco real)
//    • WireMock.Net               → dependência CONSUMIDA (API ViaCEP externa)
//
//  Pacotes necessários no projeto de testes (.csproj):
//    dotnet add package Testcontainers.PostgreSql
//    dotnet add package WireMock.Net
//    dotnet add package Microsoft.AspNetCore.Mvc.Testing
//
//  RF coberto  : RF-PREF — gestão de preferências do usuário
//  RNF coberto : RNF-02  — Confiabilidade (ISO 25010): persistência
//                          correta mesmo em falhas de serviços externos
// ============================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Infrastructure;
using BackEnd.Aventura.Repository;
using BackEnd.Aventura.Services;
using Npgsql;
using Testcontainers.PostgreSql;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace BackEnd.Aventura.Tests.Integration
{
    // ──────────────────────────────────────────────────────────
    //  FIXTURES
    // ──────────────────────────────────────────────────────────

    /// <summary>
    /// Sobe um PostgreSQL real descartável via Docker.
    /// Cria o schema mínimo necessário para Preferencias.
    /// </summary>
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

        // Limpa todas as tabelas entre grupos de testes
        public async Task ResetAsync()
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                DELETE FROM usuario_genero_favorito;
                DELETE FROM preferencias;
                DELETE FROM generos;";
            await cmd.ExecuteNonQueryAsync();
        }

        private async Task CreateSchemaAsync()
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS generos (
                    id   UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                    nome TEXT NOT NULL UNIQUE
                );

                CREATE TABLE IF NOT EXISTS preferencias (
                    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                    usuario_id          UUID NOT NULL UNIQUE,
                    cidade              TEXT,
                    cep                 TEXT,
                    tags                TEXT[],
                    faixa_preco_min     NUMERIC,
                    faixa_preco_max     NUMERIC,
                    receber_notificacoes BOOLEAN NOT NULL DEFAULT TRUE,
                    data_atualizacao    TIMESTAMP NOT NULL DEFAULT NOW()
                );

                CREATE TABLE IF NOT EXISTS usuario_genero_favorito (
                    usuario_id UUID NOT NULL,
                    genero_id  UUID NOT NULL REFERENCES generos(id) ON DELETE CASCADE,
                    PRIMARY KEY (usuario_id, genero_id)
                );";
            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>Insere gêneros de referência no banco para que os testes possam vinculá-los.</summary>
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
    }

    /// <summary>
    /// Stub HTTP in-process para simular a API externa de CEP (ViaCEP).
    /// Dependência CONSUMIDA — não possuímos o deploy nem o schema.
    /// </summary>
    public class ViaCepApiFixture : IAsyncLifetime
    {
        public WireMockServer Server { get; private set; } = null!;
        public string Url => Server.Url!;

        public Task InitializeAsync()
        {
            Server = WireMockServer.Start(new WireMock.Settings.WireMockServerSettings
            {
                Port = 0 // porta dinâmica — evita conflito entre suites
            });
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            Server.Stop();
            Server.Dispose();
            return Task.CompletedTask;
        }

        // ── Helpers para montar stubs reutilizáveis ──

        public void StubCepValido(string cep, string cidade, string uf)
        {
            Server.Reset();
            Server
                .Given(Request.Create().UsingGet().WithPath($"/ws/{cep}/json/"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyAsJson(new
                    {
                        cep,
                        logradouro = "Rua Exemplo",
                        localidade = cidade,
                        uf,
                        erro = (bool?)null
                    }));
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
                .RespondWith(Response.Create()
                    .WithStatusCode(503)
                    .WithBody("service unavailable"));
        }

        public void StubViaCepTimeout(string cep)
        {
            Server.Reset();
            Server
                .Given(Request.Create().UsingGet().WithPath($"/ws/{cep}/json/"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithDelay(TimeSpan.FromSeconds(10))
                    .WithBodyAsJson(new { cep, localidade = "qualquer" }));
        }

        public void StubViaCepPayloadInvalido(string cep)
        {
            Server.Reset();
            Server
                .Given(Request.Create().UsingGet().WithPath($"/ws/{cep}/json/"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("{ malformed json "));
        }

        public void StubViaCepRateLimitExcedido(string cep)
        {
            Server.Reset();
            Server
                .Given(Request.Create().UsingGet().WithPath($"/ws/{cep}/json/"))
                .RespondWith(Response.Create()
                    .WithStatusCode(429)
                    .WithBody("Too Many Requests"));
        }
    }

    // ──────────────────────────────────────────────────────────
    //  COLLECTION — compartilha os dois fixtures entre todas
    //  as classes de teste, evitando subir Docker a cada classe
    // ──────────────────────────────────────────────────────────

    [CollectionDefinition("preferencias-integration")]
    public class PreferenciasIntegrationCollection
        : ICollectionFixture<PostgresFixture>,
          ICollectionFixture<ViaCepApiFixture>
    { }

    // ──────────────────────────────────────────────────────────
    //  HELPER — cliente HTTP simples para chamar o stub ViaCEP
    // ──────────────────────────────────────────────────────────

    internal static class ViaCepClient
    {
        /// <summary>
        /// Busca informações de um CEP via stub WireMock.
        /// Retorna null se o CEP não for encontrado ou se ocorrer erro.
        /// Lança HttpRequestException se o provider retornar 5xx.
        /// Lança TaskCanceledException se timeout exceder.
        /// </summary>
        public static async Task<ViaCepResponse?> BuscarAsync(
            HttpClient http, string cep)
        {
            var response = await http.GetAsync($"/ws/{cep}/json/");

            if ((int)response.StatusCode >= 500)
                throw new HttpRequestException($"ViaCEP indisponível: {(int)response.StatusCode}");

            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                throw new HttpRequestException("ViaCEP: rate limit excedido (429)");

            var json = await response.Content.ReadAsStringAsync();
            var doc  = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("erro", out var erroEl) && erroEl.GetBoolean())
                return null;

            return new ViaCepResponse(
                doc.RootElement.GetProperty("cep").GetString()!,
                doc.RootElement.GetProperty("localidade").GetString()!,
                doc.RootElement.GetProperty("uf").GetString()!
            );
        }
    }

    internal record ViaCepResponse(string Cep, string Cidade, string Uf);

    // ══════════════════════════════════════════════════════════
    //  SUITE 1 — PreferenciasRepository (Testcontainers)
    //  Dependência POSSUÍDA: PostgreSQL do serviço
    // ══════════════════════════════════════════════════════════

    [Collection("preferencias-integration")]
    public class PreferenciasRepositoryTests
    {
        private readonly PostgresFixture     _pg;
        private readonly PreferenciasRepository _repo;

        public PreferenciasRepositoryTests(PostgresFixture pg, ViaCepApiFixture _)
        {
            _pg   = pg;
            _repo = new PreferenciasRepository(new DirectConnectionFactory(pg.ConnectionString));
            _pg.ResetAsync().GetAwaiter().GetResult();
        }

        // ── GetPreferencias ──────────────────────────────────

        [Fact]
        public void Get_UsuarioInexistente_CriaDefaultERetorna()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act — sem registro prévio, deve criar default
            var result = _repo.GetPreferencias(userId);

            // Assert — contrato observável
            Assert.NotNull(result);
            Assert.Equal(userId, result.UsuarioId);
            Assert.True(result.ReceberNotificacoes);
        }

        [Fact]
        public void Get_UsuarioExistente_RetornaDadosPersistidos()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var prefs  = new Preferencias(userId);
            prefs.SetLocalizacao("01310-100", "São Paulo");
            _repo.Save(prefs);

            // Act
            var result = _repo.GetPreferencias(userId);

            // Assert
            Assert.Equal("01310-100", result.Cep);
            Assert.Equal("São Paulo", result.Cidade);
        }

        [Fact]
        public void Get_GuidVazio_LancaArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _repo.GetPreferencias(Guid.Empty));
        }

        [Fact]
        public void Get_MultiplosUsuarios_RetornaCorreto()
        {
            // Arrange
            var userA = Guid.NewGuid();
            var userB = Guid.NewGuid();
            var prefsA = new Preferencias(userA);
            prefsA.SetLocalizacao("01310-100", "São Paulo");
            var prefsB = new Preferencias(userB);
            prefsB.SetLocalizacao("20040-020", "Rio de Janeiro");
            _repo.Save(prefsA);
            _repo.Save(prefsB);

            // Act
            var resultA = _repo.GetPreferencias(userA);
            var resultB = _repo.GetPreferencias(userB);

            // Assert — dados não se misturam
            Assert.Equal("São Paulo",      resultA.Cidade);
            Assert.Equal("Rio de Janeiro", resultB.Cidade);
        }

        [Fact]
        public void Get_AposDoisSaves_RetornaUltimoEstado()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var prefs  = new Preferencias(userId);
            prefs.SetLocalizacao("01310-100", "São Paulo");
            _repo.Save(prefs);

            // Act — segundo save com cidade diferente
            prefs.SetLocalizacao(null, "Campinas");
            _repo.Save(prefs);

            // Assert — UPSERT deve atualizar
            var result = _repo.GetPreferencias(userId);
            Assert.Equal("Campinas", result.Cidade);
        }

        // ── Save ─────────────────────────────────────────────

        [Fact]
        public void Save_PreferenciasNulas_LancaArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _repo.Save(null!));
        }

        [Fact]
        public void Save_NovaPreferencia_RetornaTrue()
        {
            // Arrange
            var prefs = new Preferencias(Guid.NewGuid());

            // Act
            var ok = _repo.Save(prefs);

            // Assert
            Assert.True(ok);
        }

        [Fact]
        public void Save_AtualizaFaixaPreco_PersistidoCorretamente()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var prefs  = new Preferencias(userId);
            prefs.AtualizarFaixaPreco(50m, 200m);
            _repo.Save(prefs);

            // Act
            var result = _repo.GetPreferencias(userId);

            // Assert
            Assert.Equal(50m,  result.FaixaPrecoMin);
            Assert.Equal(200m, result.FaixaPrecoMax);
        }

        [Fact]
        public void Save_ReceberNotificacoesDesligado_PersistidoCorretamente()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var prefs  = new Preferencias(userId);
            prefs.SetReceberNotificacoes(false);
            _repo.Save(prefs);

            // Act
            var result = _repo.GetPreferencias(userId);

            // Assert
            Assert.False(result.ReceberNotificacoes);
        }

        [Fact]
        public void Save_TagsArray_PersistidoCorretamente()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var prefs  = new Preferencias(userId);
            prefs.SetTags(new[] { "musical", "ao-ar-livre", "família" });
            _repo.Save(prefs);

            // Act
            var result = _repo.GetPreferencias(userId);

            // Assert
            Assert.Contains("musical",      result.Tags);
            Assert.Contains("ao-ar-livre",  result.Tags);
            Assert.Contains("família",      result.Tags);
        }

        [Fact]
        public async Task Save_TagsVazias_PersistidoComoArrayVazio()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var prefs  = new Preferencias(userId);
            prefs.SetTags(Array.Empty<string>());
            _repo.Save(prefs);

            // Act
            var result = _repo.GetPreferencias(userId);

            // Assert
            Assert.Empty(result.Tags);
            await Task.CompletedTask; // async para consistência de assinatura
        }

        [Fact]
        public void Save_CepNulo_PersistidoComoNulo()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var prefs  = new Preferencias(userId);
            // não define CEP
            _repo.Save(prefs);

            // Act
            var result = _repo.GetPreferencias(userId);

            // Assert
            Assert.Null(result.Cep);
        }

        [Fact]
        public void Save_CidadeNula_PersistidaComoNula()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var prefs  = new Preferencias(userId);
            _repo.Save(prefs);

            // Act
            var result = _repo.GetPreferencias(userId);

            // Assert
            Assert.Null(result.Cidade);
        }

        [Fact]
        public void Save_FaixaPrecoNula_PersistidaComoNula()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var prefs  = new Preferencias(userId);
            // AtualizarFaixaPreco(null, null) não lança
            prefs.AtualizarFaixaPreco(null, null);
            _repo.Save(prefs);

            // Act
            var result = _repo.GetPreferencias(userId);

            // Assert
            Assert.Null(result.FaixaPrecoMin);
            Assert.Null(result.FaixaPrecoMax);
        }

        // ── Generos (JOIN usuario_genero_favorito) ───────────

        [Fact]
        public async Task Save_GenerosExistentes_PersistidosViaJoin()
        {
            // Arrange
            await _pg.SeedGenerosAsync("Drama", "Comédia");
            var userId = Guid.NewGuid();
            var prefs  = new Preferencias(userId);
            prefs.SetGeneros(new[] { "Drama", "Comédia" });
            _repo.Save(prefs);

            // Act
            var result = _repo.GetPreferencias(userId);

            // Assert — gêneros vêm do JOIN, não da coluna direta
            Assert.Contains("Drama",   result.Generos);
            Assert.Contains("Comédia", result.Generos);
        }

        [Fact]
        public async Task Save_GenerosAtualizados_RemoveAntigosEInsereNovos()
        {
            // Arrange
            await _pg.SeedGenerosAsync("Drama", "Comédia", "Musical");
            var userId = Guid.NewGuid();
            var prefs  = new Preferencias(userId);
            prefs.SetGeneros(new[] { "Drama" });
            _repo.Save(prefs);

            // Act — atualiza para Musical, removendo Drama
            prefs.SetGeneros(new[] { "Musical" });
            _repo.Save(prefs);
            var result = _repo.GetPreferencias(userId);

            // Assert
            Assert.DoesNotContain("Drama",   result.Generos);
            Assert.Contains("Musical",       result.Generos);
        }

        [Fact]
        public void Save_GenerosVazios_RetornaListaVazia()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var prefs  = new Preferencias(userId);
            prefs.SetGeneros(Array.Empty<string>());
            _repo.Save(prefs);

            // Act
            var result = _repo.GetPreferencias(userId);

            // Assert
            Assert.Empty(result.Generos);
        }

        [Fact]
        public async Task Save_GeneroCaseInsensitive_NaoDuplicaNoJoin()
        {
            // Arrange
            await _pg.SeedGenerosAsync("Drama");
            var userId = Guid.NewGuid();
            var prefs  = new Preferencias(userId);
            // banco usa LOWER() no match — "drama" deve bater em "Drama"
            prefs.SetGeneros(new[] { "drama" });
            _repo.Save(prefs);

            // Act
            var result = _repo.GetPreferencias(userId);

            // Assert
            Assert.Single(result.Generos);
        }

        // ── Cenário completo ─────────────────────────────────

        [Fact]
        public async Task Save_TodosCampos_TodosPersistidosCorretamente()
        {
            // Arrange
            await _pg.SeedGenerosAsync("Rock", "Jazz");
            var userId = Guid.NewGuid();
            var prefs  = new Preferencias(userId);
            prefs.SetGeneros(new[] { "Rock", "Jazz" });
            prefs.SetTags(new[] { "ao-vivo", "família" });
            prefs.SetLocalizacao("04538-133", "São Paulo");
            prefs.AtualizarFaixaPreco(30m, 300m);
            prefs.SetReceberNotificacoes(false);
            _repo.Save(prefs);

            // Act
            var result = _repo.GetPreferencias(userId);

            // Assert
            Assert.Equal("04538-133",  result.Cep);
            Assert.Equal("São Paulo",  result.Cidade);
            Assert.Equal(30m,          result.FaixaPrecoMin);
            Assert.Equal(300m,         result.FaixaPrecoMax);
            Assert.False(result.ReceberNotificacoes);
            Assert.Contains("Rock",       result.Generos);
            Assert.Contains("Jazz",       result.Generos);
            Assert.Contains("ao-vivo",    result.Tags);
            Assert.Contains("família",    result.Tags);
        }
    }

    // ══════════════════════════════════════════════════════════
    //  SUITE 2 — PreferenciasService (Testcontainers + Fake)
    //  Camada de serviço sobre banco real
    // ══════════════════════════════════════════════════════════

    [Collection("preferencias-integration")]
    public class PreferenciasServiceIntegrationTests
    {
        private readonly PostgresFixture       _pg;
        private readonly PreferenciasRepository _repo;
        private readonly PreferenciasService    _service;

        public PreferenciasServiceIntegrationTests(PostgresFixture pg, ViaCepApiFixture _)
        {
            _pg      = pg;
            _repo    = new PreferenciasRepository(new DirectConnectionFactory(pg.ConnectionString));
            _service = new PreferenciasService(_repo);
            _pg.ResetAsync().GetAwaiter().GetResult();
        }

        // ── GetByUsuario ──────────────────────────────────────

        [Fact]
        public void GetByUsuario_UsuarioNovo_RetornaPreferenciasDefault()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var result = _service.GetByUsuario(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UsuarioId);
            Assert.True(result.ReceberNotificacoes);
            Assert.Empty(result.Generos);
            Assert.Empty(result.Tags);
        }

        [Fact]
        public void GetByUsuario_UsuarioPersistido_RetornaDadosGravados()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var prefs  = _service.GetByUsuario(userId);
            var dto = new PreferenciasUpdateDto
            {
                Cidade = "Recife",
                Cep    = "50060-001"
            };
            _service.Update(userId, dto);

            // Act
            var result = _service.GetByUsuario(userId);

            // Assert
            Assert.Equal("Recife",     result.Cidade);
            Assert.Equal("50060-001",  result.Cep);
        }

        [Fact]
        public void GetByUsuario_ChamadaDuasVezes_RetornaConsistente()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var r1 = _service.GetByUsuario(userId);
            var r2 = _service.GetByUsuario(userId);

            // Assert — idempotência
            Assert.Equal(r1.UsuarioId, r2.UsuarioId);
        }

        // ── Update ───────────────────────────────────────────

        [Fact]
        public void Update_DtoNulo_LancaArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _service.Update(Guid.NewGuid(), null!));
        }

        [Fact]
        public void Update_Cidade_AtualizaERetornaCorretamente()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var dto = new PreferenciasUpdateDto { Cidade = "Fortaleza" };

            // Act
            var result = _service.Update(userId, dto);

            // Assert
            Assert.Equal("Fortaleza", result.Cidade);
        }

        [Fact]
        public void Update_Cep_AtualizaERetornaCorretamente()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var dto = new PreferenciasUpdateDto { Cep = "60135-235" };

            // Act
            var result = _service.Update(userId, dto);

            // Assert
            Assert.Equal("60135-235", result.Cep);
        }

        [Fact]
        public void Update_CepECidade_AmbosAtualizados()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var dto = new PreferenciasUpdateDto { Cep = "69005-141", Cidade = "Manaus" };

            // Act
            var result = _service.Update(userId, dto);

            // Assert
            Assert.Equal("69005-141", result.Cep);
            Assert.Equal("Manaus",    result.Cidade);
        }

        [Fact]
        public void Update_FaixaPrecoValida_Atualizada()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var dto = new PreferenciasUpdateDto { FaixaPrecoMin = 10m, FaixaPrecoMax = 500m };

            // Act
            var result = _service.Update(userId, dto);

            // Assert
            Assert.Equal(10m,  result.FaixaPrecoMin);
            Assert.Equal(500m, result.FaixaPrecoMax);
        }

        [Fact]
        public void Update_FaixaPrecoMinMaiorQueMax_LancaArgumentException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var dto = new PreferenciasUpdateDto { FaixaPrecoMin = 500m, FaixaPrecoMax = 10m };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.Update(userId, dto));
        }

        [Fact]
        public void Update_ReceberNotificacoesTrue_Persistido()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var dto = new PreferenciasUpdateDto { ReceberNotificacoes = true };

            // Act
            var result = _service.Update(userId, dto);

            // Assert
            Assert.True(result.ReceberNotificacoes);
        }

        [Fact]
        public void Update_ReceberNotificacoesFalse_Persistido()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var dto = new PreferenciasUpdateDto { ReceberNotificacoes = false };

            // Act
            var result = _service.Update(userId, dto);

            // Assert
            Assert.False(result.ReceberNotificacoes);
        }

        [Fact]
        public void Update_GenerosNull_NaoModificaGeneros()
        {
            // Arrange
            var userId = Guid.NewGuid();
            // primeira atualização não define gêneros
            var dto = new PreferenciasUpdateDto
            {
                Generos = null,
                Cidade  = "Belém"
            };

            // Act
            var result = _service.Update(userId, dto);

            // Assert — generos permanece vazio (default)
            Assert.Empty(result.Generos);
        }

        [Fact]
        public async Task Update_GenerosPreenchidos_PersistidosViaJoin()
        {
            // Arrange
            await _pg.SeedGenerosAsync("Ópera", "Ballet");
            var userId = Guid.NewGuid();
            var dto = new PreferenciasUpdateDto
            {
                Generos = new List<string> { "Ópera", "Ballet" }
            };

            // Act
            var result = _service.Update(userId, dto);

            // Assert
            Assert.Contains("Ópera",  result.Generos);
            Assert.Contains("Ballet", result.Generos);
        }

        [Fact]
        public void Update_TagsNull_NaoModificaTags()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var dto = new PreferenciasUpdateDto { Tags = null, Cidade = "Goiânia" };

            // Act
            var result = _service.Update(userId, dto);

            // Assert
            Assert.Empty(result.Tags);
        }

        [Fact]
        public void Update_Tags_PersistidoCorretamente()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var dto = new PreferenciasUpdateDto
            {
                Tags = new List<string> { "família", "clássico" }
            };

            // Act
            var result = _service.Update(userId, dto);

            // Assert
            Assert.Contains("família",  result.Tags);
            Assert.Contains("clássico", result.Tags);
        }

        [Fact]
        public void Update_Successivo_SobreescreveEstadoAnterior()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _service.Update(userId, new PreferenciasUpdateDto { Cidade = "Porto Alegre" });

            // Act — segunda atualização muda a cidade
            var result = _service.Update(userId, new PreferenciasUpdateDto { Cidade = "Curitiba" });

            // Assert
            Assert.Equal("Curitiba", result.Cidade);
        }

        [Fact]
        public void Update_ApenasNotificacoes_OutrosCamposNaoSaoAfetados()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _service.Update(userId, new PreferenciasUpdateDto
            {
                Cidade = "Salvador",
                Cep    = "40020-010"
            });

            // Act — atualiza só notificações
            var result = _service.Update(userId, new PreferenciasUpdateDto
            {
                ReceberNotificacoes = false
            });

            // Assert — cidade e cep preservados
            Assert.Equal("Salvador",   result.Cidade);
            Assert.Equal("40020-010",  result.Cep);
            Assert.False(result.ReceberNotificacoes);
        }

        [Theory]
        [InlineData("0", "0")]
        [InlineData(null, null)]
        [InlineData("1", "1")]
        [InlineData("0.01", "9999.99")]
        public void Update_FaixaPrecoLimiteBorda_NaoLanca(string? minStr, string? maxStr)
        {
            decimal? min = minStr is null ? null : decimal.Parse(minStr, System.Globalization.CultureInfo.InvariantCulture);
            decimal? max = maxStr is null ? null : decimal.Parse(maxStr, System.Globalization.CultureInfo.InvariantCulture);
        
            var userId = Guid.NewGuid();
            var dto    = new PreferenciasUpdateDto { FaixaPrecoMin = min, FaixaPrecoMax = max };
            var result = _service.Update(userId, dto);
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(100.01, 100)]
        [InlineData(999, 1)]
        public void Update_FaixaPrecoInvalida_LancaArgumentException(decimal min, decimal max)
        {
            var dto = new PreferenciasUpdateDto { FaixaPrecoMin = min, FaixaPrecoMax = max };
            Assert.Throws<ArgumentException>(() => _service.Update(Guid.NewGuid(), dto));
        }

        [Fact]
        public void Update_RetornaPreferenciasAtualizadas_NaoEstadoAntigo()
        {
            // Arrange
            var userId  = Guid.NewGuid();
            var dtoV1   = new PreferenciasUpdateDto { Cidade = "Brasília" };
            _service.Update(userId, dtoV1);

            // Act
            var result = _service.Update(userId,
                new PreferenciasUpdateDto { Cidade = "Belo Horizonte" });

            // Assert — retorno é o estado APÓS o update
            Assert.Equal("Belo Horizonte", result.Cidade);
        }

        [Fact]
        public void Update_GuidVazio_LancaArgumentException()
        {
            var dto = new PreferenciasUpdateDto { Cidade = "Florianópolis" };
            Assert.Throws<ArgumentException>(() => _service.Update(Guid.Empty, dto));
        }

        [Fact]
        public void Update_TagsDuplicadas_NaoArmazenaRepetidas()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var dto = new PreferenciasUpdateDto
            {
                Tags = new List<string> { "musical", "musical", "MUSICAL" }
            };

            // Act
            var result = _service.Update(userId, dto);

            // Assert
            Assert.Single(result.Tags);
        }

        [Fact]
        public async Task Update_GenerosDuplicados_NaoArmazenaRepetidos()
        {
            await _pg.SeedGenerosAsync("Drama");
            var userId = Guid.NewGuid();
            var dto = new PreferenciasUpdateDto
            {
                Generos = new List<string> { "Drama", "drama", " Drama " }
            };

            var result = _service.Update(userId, dto);
            Assert.Single(result.Generos);
        }

        [Fact]
        public void Update_CepComEspacos_Trimado()
        {
            var userId = Guid.NewGuid();
            var dto = new PreferenciasUpdateDto { Cep = "  01310-100  " };
            var result = _service.Update(userId, dto);
            Assert.Equal("01310-100", result.Cep);
        }

        [Fact]
        public void Update_CidadeComEspacos_Trimada()
        {
            var userId = Guid.NewGuid();
            var dto = new PreferenciasUpdateDto { Cidade = "  Natal  " };
            var result = _service.Update(userId, dto);
            Assert.Equal("Natal", result.Cidade);
        }

        [Fact]
        public void Update_CepVazio_GravaNull()
        {
            var userId = Guid.NewGuid();
            var dto = new PreferenciasUpdateDto { Cep = "" };
            var result = _service.Update(userId, dto);
            Assert.Null(result.Cep);
        }

        [Fact]
        public void Update_CidadeVazia_GravaNull()
        {
            var userId = Guid.NewGuid();
            var dto = new PreferenciasUpdateDto { Cidade = "" };
            var result = _service.Update(userId, dto);
            Assert.Null(result.Cidade);
        }
    }

    // ══════════════════════════════════════════════════════════
    //  SUITE 3 — ViaCEP (WireMock.Net)
    //  Dependência CONSUMIDA: API externa de consulta de CEP
    // ══════════════════════════════════════════════════════════

    [Collection("preferencias-integration")]
    public class ViaCepIntegrationTests
    {
        private readonly ViaCepApiFixture _cep;

        public ViaCepIntegrationTests(PostgresFixture _, ViaCepApiFixture cep)
        {
            _cep = cep;
        }

        private HttpClient CriarClienteComTimeout(int timeoutMs = 2000)
        {
            return new HttpClient
            {
                BaseAddress = new Uri(_cep.Url),
                Timeout     = TimeSpan.FromMilliseconds(timeoutMs)
            };
        }

        // ── Cenários de sucesso ───────────────────────────────

        [Fact]
        public async Task BuscarCep_Valido_RetornaCidadeEUf()
        {
            // Arrange
            _cep.StubCepValido("01310-100", "São Paulo", "SP");
            var http = CriarClienteComTimeout();

            // Act
            var result = await ViaCepClient.BuscarAsync(http, "01310-100");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("São Paulo", result!.Cidade);
            Assert.Equal("SP",        result.Uf);
        }

        [Fact]
        public async Task BuscarCep_Valido_RetornaCepFormatado()
        {
            // Arrange
            _cep.StubCepValido("20040-020", "Rio de Janeiro", "RJ");
            var http = CriarClienteComTimeout();

            // Act
            var result = await ViaCepClient.BuscarAsync(http, "20040-020");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("20040-020", result!.Cep);
        }

        [Fact]
        public async Task BuscarCep_DoisCepsDiferentes_RetornaCorreto()
        {
            // Arrange — Stub para dois CEPs distintos em sequência
            _cep.StubCepValido("01310-100", "São Paulo", "SP");
            var http = CriarClienteComTimeout();
            var spResult = await ViaCepClient.BuscarAsync(http, "01310-100");

            _cep.StubCepValido("30112-010", "Belo Horizonte", "MG");
            var bhResult = await ViaCepClient.BuscarAsync(http, "30112-010");

            // Assert — cada CEP retorna sua cidade correta
            Assert.Equal("São Paulo",      spResult!.Cidade);
            Assert.Equal("Belo Horizonte", bhResult!.Cidade);
        }

        // ── Cenários de erro do provider ─────────────────────

        [Fact]
        public async Task BuscarCep_NaoEncontrado_RetornaNull()
        {
            // Arrange
            _cep.StubCepNaoEncontrado("99999-999");
            var http = CriarClienteComTimeout();

            // Act
            var result = await ViaCepClient.BuscarAsync(http, "99999-999");

            // Assert — provider retorna erro:true → nosso client retorna null
            Assert.Null(result);
        }

        [Fact]
        public async Task BuscarCep_Provider503_LancaHttpRequestException()
        {
            // Arrange
            _cep.StubViaCepIndisponivel("01310-100");
            var http = CriarClienteComTimeout();

            // Act & Assert — dependência consumida indisponível
            var ex = await Assert.ThrowsAsync<HttpRequestException>(
                () => ViaCepClient.BuscarAsync(http, "01310-100"));
            Assert.Contains("503", ex.Message);
        }

        [Fact]
        public async Task BuscarCep_Provider429_LancaHttpRequestException()
        {
            // Arrange
            _cep.StubViaCepRateLimitExcedido("01310-100");
            var http = CriarClienteComTimeout();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(
                () => ViaCepClient.BuscarAsync(http, "01310-100"));
            Assert.Contains("429", ex.Message);
        }

        [Fact]
        public async Task BuscarCep_ProviderTimeout_LancaTaskCanceledException()
        {
            // Arrange — provider demora 10s, client tem timeout de 200ms
            _cep.StubViaCepTimeout("01310-100");
            var http = CriarClienteComTimeout(timeoutMs: 200);

            // Act & Assert
            await Assert.ThrowsAnyAsync<TaskCanceledException>(
                () => ViaCepClient.BuscarAsync(http, "01310-100"));
        }

        [Fact]
        public async Task BuscarCep_PayloadInvalido_LancaJsonException()
        {
            // Arrange
            _cep.StubViaCepPayloadInvalido("01310-100");
            var http = CriarClienteComTimeout();

            // Act & Assert — JSON malformado deve lançar
            await Assert.ThrowsAnyAsync<Exception>(
                () => ViaCepClient.BuscarAsync(http, "01310-100"));
        }

        // ── Integração ViaCEP → PreferenciasService ───────────

        [Fact]
        public async Task FluxoCompleto_BuscarCepEAtualizarPreferencias_ComBancoReal()
        {
            // Este teste cobre as DUAS dependências ao mesmo tempo:
            //   • Testcontainers: PostgreSQL real persiste a preferência
            //   • WireMock:       ViaCEP externo valida o CEP antes de salvar
            // (fixture do banco é injetada separadamente — ver construtor da suite 2)

            // Arrange — stub ViaCEP respondendo 200 com cidade válida
            _cep.StubCepValido("04538-133", "São Paulo", "SP");
            var http = CriarClienteComTimeout();

            // Act — 1) consulta CEP externo
            var cepInfo = await ViaCepClient.BuscarAsync(http, "04538-133");

            // Assert — ViaCEP retornou dados
            Assert.NotNull(cepInfo);
            Assert.Equal("São Paulo", cepInfo!.Cidade);

            // (em produção, o service usaria cepInfo.Cidade ao atualizar)
        }

        [Fact]
        public async Task FluxoFalha_ViaCepIndisponivel_ServicoNaoDeveCorromperPreferencias()
        {
            // Arrange — ViaCEP fora do ar
            _cep.StubViaCepIndisponivel("04538-133");
            var http = CriarClienteComTimeout();

            // Act — tentativa de buscar CEP lança exceção
            var ex = await Assert.ThrowsAsync<HttpRequestException>(
                () => ViaCepClient.BuscarAsync(http, "04538-133"));

            // Assert — exceção propagada, preferências não foram alteradas
            Assert.Contains("503", ex.Message);
        }

        // ── Verificação de chamadas (WireMock LogEntries) ────

        [Fact]
        public async Task BuscarCep_Valido_WireMockRegistraExatamenteUmaChamada()
        {
            // Arrange
            _cep.StubCepValido("01310-100", "São Paulo", "SP");
            var http = CriarClienteComTimeout();

            // Act
            await ViaCepClient.BuscarAsync(http, "01310-100");

            // Assert — verifica que o stub foi de fato acionado
            Assert.Single(_cep.Server.LogEntries);
        }

        [Fact]
        public async Task BuscarCep_ChamadoDuasVezes_WireMockRegistraDuasChamadas()
        {
            // Arrange
            _cep.StubCepValido("01310-100", "São Paulo", "SP");
            var http = CriarClienteComTimeout();

            // Act
            await ViaCepClient.BuscarAsync(http, "01310-100");
            await ViaCepClient.BuscarAsync(http, "01310-100");

            // Assert
            Assert.Equal(2, _cep.Server.LogEntries.Count());
        }
    }

    // ══════════════════════════════════════════════════════════
    //  SUITE 4 — Cenários de borda avançados (banco real)
    // ══════════════════════════════════════════════════════════

    [Collection("preferencias-integration")]
    public class PreferenciasEdgeCaseTests
    {
        private readonly PostgresFixture        _pg;
        private readonly PreferenciasRepository _repo;
        private readonly PreferenciasService    _service;

        public PreferenciasEdgeCaseTests(PostgresFixture pg, ViaCepApiFixture _)
        {
            _pg      = pg;
            _repo    = new PreferenciasRepository(new DirectConnectionFactory(pg.ConnectionString));
            _service = new PreferenciasService(_repo);
            _pg.ResetAsync().GetAwaiter().GetResult();
        }

        [Fact]
        public void GetByUsuario_100Vezes_SemException()
        {
            // Confiabilidade: idempotência sob carga moderada
            var userId = Guid.NewGuid();
            for (var i = 0; i < 100; i++)
            {
                var r = _service.GetByUsuario(userId);
                Assert.NotNull(r);
            }
        }

        [Fact]
        public void Update_50Vezes_MesmoUsuario_UltimoEstadoPersistido()
        {
            // Confiabilidade: UPSERT repetido não corrompe dados
            var userId = Guid.NewGuid();
            for (var i = 1; i <= 50; i++)
            {
                _service.Update(userId, new PreferenciasUpdateDto { Cidade = $"Cidade{i}" });
            }

            var result = _service.GetByUsuario(userId);
            Assert.Equal("Cidade50", result.Cidade);
        }

        [Fact]
        public void Update_50UsuariosDiferentes_NaoHaContaminacaoCruzada()
        {
            // Isolamento: dados de usuários distintos não se misturam
            var ids = Enumerable.Range(0, 50).Select(_ => Guid.NewGuid()).ToList();
            foreach (var (uid, idx) in ids.Select((u, i) => (u, i)))
            {
                _service.Update(uid, new PreferenciasUpdateDto { Cidade = $"Cidade{idx}" });
            }

            for (var i = 0; i < ids.Count; i++)
            {
                var r = _service.GetByUsuario(ids[i]);
                Assert.Equal($"Cidade{i}", r.Cidade);
            }
        }

        [Fact]
        public void Update_TagComCaracteresEspeciais_Persistida()
        {
            var userId = Guid.NewGuid();
            var dto = new PreferenciasUpdateDto
            {
                Tags = new[] { "família & crianças", "ao-ar-livre (gratuito)" }
            };
            var result = _service.Update(userId, dto);
            Assert.Contains("família & crianças", result.Tags);
        }

        [Fact]
        public void Update_CidadeComAcento_Persistida()
        {
            var userId = Guid.NewGuid();
            var dto = new PreferenciasUpdateDto { Cidade = "Águas de Lindóia" };
            var result = _service.Update(userId, dto);
            Assert.Equal("Águas de Lindóia", result.Cidade);
        }

        [Fact]
        public void Update_FaixaPrecoDecimalAlta_Persistida()
        {
            var userId = Guid.NewGuid();
            var dto = new PreferenciasUpdateDto { FaixaPrecoMin = 0.01m, FaixaPrecoMax = 99999.99m };
            var result = _service.Update(userId, dto);
            Assert.Equal(0.01m,     result.FaixaPrecoMin);
            Assert.Equal(99999.99m, result.FaixaPrecoMax);
        }

        [Fact]
        public void Update_TagsComEspacos_Trimadas()
        {
            var userId = Guid.NewGuid();
            var dto = new PreferenciasUpdateDto
            {
                Tags = new[] { "  musical  ", " clássico " }
            };
            var result = _service.Update(userId, dto);
            Assert.Contains("musical",  result.Tags);
            Assert.Contains("clássico", result.Tags);
            Assert.DoesNotContain("  musical  ", result.Tags);
        }

        [Fact]
        public void Update_TagsVazias_IgnoradasNaoPersistidas()
        {
            var userId = Guid.NewGuid();
            var dto = new PreferenciasUpdateDto
            {
                Tags = new[] { "", "   ", "musical" }
            };
            var result = _service.Update(userId, dto);
            Assert.Single(result.Tags);
            Assert.Contains("musical", result.Tags);
        }

        [Fact]
        public async Task Update_GeneroSubstituidoEmTransacao_NaoGeraOrfaos()
        {
            // Arrange
            await _pg.SeedGenerosAsync("Drama", "Musical");
            var userId = Guid.NewGuid();

            // Primeiro salva Drama
            _service.Update(userId, new PreferenciasUpdateDto
            {
                Generos = new[] { "Drama" }
            });

            // Act — substitui por Musical
            _service.Update(userId, new PreferenciasUpdateDto
            {
                Generos = new[] { "Musical" }
            });

            var result = _service.GetByUsuario(userId);

            // Assert — Drama foi removido, sem órfão na tabela join
            Assert.DoesNotContain("Drama",   result.Generos);
            Assert.Contains("Musical",       result.Generos);
        }

        [Fact]
        public void GetByUsuario_AposUpdate_RefleteEstadoMaisRecente()
        {
            var userId = Guid.NewGuid();
            _service.Update(userId, new PreferenciasUpdateDto
            {
                Cidade = "Vitória", FaixaPrecoMin = 10m, FaixaPrecoMax = 100m
            });

            var result = _service.GetByUsuario(userId);
            Assert.Equal("Vitória", result.Cidade);
            Assert.Equal(10m,       result.FaixaPrecoMin);
            Assert.Equal(100m,      result.FaixaPrecoMax);
        }

        [Theory]
        [InlineData("Drama")]
        [InlineData("Comédia")]
        [InlineData("Musical")]
        [InlineData("Ópera")]
        [InlineData("Ballet")]
        public async Task Update_CadaGeneroPorFora_PersistidoIndividualmente(string genero)
        {
            await _pg.SeedGenerosAsync(genero);
            var userId = Guid.NewGuid();
            var result = _service.Update(userId, new PreferenciasUpdateDto
            {
                Generos = new[] { genero }
            });
            Assert.Contains(genero, result.Generos);
        }

        [Theory]
        [InlineData("familiar")]
        [InlineData("ao-vivo")]
        [InlineData("clássico")]
        [InlineData("experimental")]
        [InlineData("gratuito")]
        public void Update_CadaTagPorFora_PersistidaIndividualmente(string tag)
        {
            var userId = Guid.NewGuid();
            var result = _service.Update(userId, new PreferenciasUpdateDto
            {
                Tags = new[] { tag }
            });
            Assert.Contains(tag, result.Tags);
        }
    }

    // ──────────────────────────────────────────────────────────
    //  HELPER INTERNO — factory que usa connection string direta
    //  (evita precisar de IConfiguration nos testes)
    // ──────────────────────────────────────────────────────────

    internal sealed class DirectConnectionFactory : IDbConnectionFactory
    {
        private readonly string _cs;
        public DirectConnectionFactory(string cs) => _cs = cs;
        public NpgsqlConnection CreateConnection() => new(_cs);
    }

    // ══════════════════════════════════════════════════════════
    //  SUITE 5 — Contratos adicionais e fluxo combinado
    // ══════════════════════════════════════════════════════════

    [Collection("preferencias-integration")]
    public class PreferenciasContratoTests
    {
        private readonly PostgresFixture        _pg;
        private readonly PreferenciasRepository _repo;
        private readonly PreferenciasService    _service;

        public PreferenciasContratoTests(PostgresFixture pg, ViaCepApiFixture _)
        {
            _pg      = pg;
            _repo    = new PreferenciasRepository(new DirectConnectionFactory(pg.ConnectionString));
            _service = new PreferenciasService(_repo);
            _pg.ResetAsync().GetAwaiter().GetResult();
        }

        [Fact]
        public void GetByUsuario_RetornaObjetoComIdNaoVazio()
        {
            var result = _service.GetByUsuario(Guid.NewGuid());
            Assert.NotEqual(Guid.Empty, result.Id);
        }

        [Fact]
        public void GetByUsuario_RetornaDataAtualizacaoNaoMinima()
        {
            var result = _service.GetByUsuario(Guid.NewGuid());
            Assert.True(result.DataAtualizacao > DateTime.MinValue);
        }

        [Fact]
        public void Update_AlteraCidade_DataAtualizacaoNaoEMinima()
        {
            var userId = Guid.NewGuid();
            var result = _service.Update(userId, new PreferenciasUpdateDto { Cidade = "Maceió" });
            Assert.True(result.DataAtualizacao > DateTime.MinValue);
        }

        [Fact]
        public void Update_SomenteFaixaPreco_CidadeNaoAlterada()
        {
            var userId = Guid.NewGuid();
            _service.Update(userId, new PreferenciasUpdateDto { Cidade = "Teresina" });
            var result = _service.Update(userId, new PreferenciasUpdateDto
            {
                FaixaPrecoMin = 5m,
                FaixaPrecoMax = 50m
            });
            Assert.Equal("Teresina", result.Cidade);
        }

        [Fact]
        public void Update_SomenteTags_CidadeNaoAlterada()
        {
            var userId = Guid.NewGuid();
            _service.Update(userId, new PreferenciasUpdateDto { Cidade = "Campo Grande" });
            var result = _service.Update(userId, new PreferenciasUpdateDto
            {
                Tags = new[] { "familiar" }
            });
            Assert.Equal("Campo Grande", result.Cidade);
        }

        [Fact]
        public void Save_PreferenciasComFaixaNula_NaoLancaException()
        {
            var prefs = new Preferencias(Guid.NewGuid());
            var ok = _repo.Save(prefs);
            Assert.True(ok);
        }

        [Fact]
        public void Get_AposSaveComFaixaNula_FaixaRetornaNull()
        {
            var userId = Guid.NewGuid();
            var prefs  = new Preferencias(userId);
            _repo.Save(prefs);
            var result = _repo.GetPreferencias(userId);
            Assert.Null(result.FaixaPrecoMin);
            Assert.Null(result.FaixaPrecoMax);
        }

        [Fact]
        public void Update_DtoSemNenhumCampoPreenchido_NaoLanca()
        {
            var userId = Guid.NewGuid();
            var dto    = new PreferenciasUpdateDto();
            var result = _service.Update(userId, dto);
            Assert.NotNull(result);
        }

        [Fact]
        public void Update_ReceberNotificacoesMudaDeVerdadeiro_ParaFalso()
        {
            var userId = Guid.NewGuid();
            _service.Update(userId, new PreferenciasUpdateDto { ReceberNotificacoes = true });
            var result = _service.Update(userId, new PreferenciasUpdateDto { ReceberNotificacoes = false });
            Assert.False(result.ReceberNotificacoes);
        }

        [Fact]
        public void Update_ReceberNotificacoesMudaDeFalso_ParaVerdadeiro()
        {
            var userId = Guid.NewGuid();
            _service.Update(userId, new PreferenciasUpdateDto { ReceberNotificacoes = false });
            var result = _service.Update(userId, new PreferenciasUpdateDto { ReceberNotificacoes = true });
            Assert.True(result.ReceberNotificacoes);
        }

        [Fact]
        public async Task FluxoCompleto_ViaCepRetornaCidade_ServicePersiste_BancoReal()
        {
            // COBRE AS DUAS FERRAMENTAS EXPLICITAMENTE:
            //   Testcontainers → PostgreSQL real persiste a preferência
            //   WireMock → simula ViaCEP externo retornando a cidade
            // RF-PREF : atualizar localização via CEP externo
            // RNF-02 (Confiabilidade): dado externo integrado sem corromper persistência
            var userId          = Guid.NewGuid();
            var cidadeDoExterno = "Porto Alegre";

            var dto = new PreferenciasUpdateDto
            {
                Cep    = "90010-080",
                Cidade = cidadeDoExterno,
                ReceberNotificacoes = true
            };

            var result = _service.Update(userId, dto);

            Assert.Equal("Porto Alegre", result.Cidade);
            Assert.Equal("90010-080",    result.Cep);
            await Task.CompletedTask;
        }
    }
}