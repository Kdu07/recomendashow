using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Testcontainers.PostgreSql;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace BackEnd.Aventura.IntegrationTests;

public sealed class GeneroValidatedIntegrationTests : IClassFixture<GeneroValidatedFixture>
{
    private readonly GeneroValidatedFixture _fixture;

    public GeneroValidatedIntegrationTests(GeneroValidatedFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Should_ReturnValidatedGenres_When_ExternalCatalogSucceeds()
    {
        await _fixture.ResetGenresAsync("Drama", "Musical", "Terror");
        _fixture.ConfigureExternalCatalogSuccess();

        using var client = _fixture.CreateClient();
        var response = await client.GetAsync("/api/Genero/validated");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var genres = await response.Content.ReadFromJsonAsync<IReadOnlyList<string>>();
        genres.Should().NotBeNull();
        genres.Should().Contain("Drama");
        genres.Should().Contain("Musical");
        genres.Should().NotContain("Terror");
    }

    [Fact]
    public async Task Should_ReturnServiceUnavailable_When_ExternalCatalogFails()
    {
        await _fixture.ResetGenresAsync("Drama");
        _fixture.ConfigureExternalCatalogFailure();

        using var client = _fixture.CreateClient();
        var response = await client.GetAsync("/api/Genero/validated");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        body.Should().Contain("Serviço externo de catálogo de gêneros indisponível.");
        body.Should().NotContain("StackTrace");
        body.Should().NotContain("System.");
    }
}

public sealed class GeneroValidatedFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("aventura_tests")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private WireMockServer? _catalogServer;
    private WebApplicationFactory<Program>? _factory;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        await ExecuteSchemaAsync();

        _catalogServer = WireMockServer.Start();
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:DefaultConnection"] = _postgres.GetConnectionString(),
                        ["GeneroCatalogApi:BaseUrl"] = _catalogServer.Url,
                    });
                });
            });
    }

    public async Task DisposeAsync()
    {
        _factory?.Dispose();
        _catalogServer?.Stop();
        _catalogServer?.Dispose();
        await _postgres.DisposeAsync();
    }

    public HttpClient CreateClient()
    {
        if (_factory is null)
        {
            throw new InvalidOperationException("Factory de teste não inicializada.");
        }

        return _factory.CreateClient();
    }

    public async Task ResetGenresAsync(params string[] genres)
    {
        await using var connection = new NpgsqlConnection(_postgres.GetConnectionString());
        await connection.OpenAsync();

        await using (var deleteCommand = new NpgsqlCommand("DELETE FROM generos", connection))
        {
            await deleteCommand.ExecuteNonQueryAsync();
        }

        foreach (var genre in genres)
        {
            await using var insertCommand = new NpgsqlCommand(
                "INSERT INTO generos(nome) VALUES (@Nome)",
                connection);
            insertCommand.Parameters.AddWithValue("@Nome", genre);
            await insertCommand.ExecuteNonQueryAsync();
        }
    }

    public void ConfigureExternalCatalogSuccess()
    {
        EnsureCatalogServer().Reset();
        EnsureCatalogServer()
            .Given(Request.Create()
                .WithPath("/api/genre-catalog/validate")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""
                {
                  "genres": [
                    { "name": "Drama", "active": true },
                    { "name": "Musical", "active": true },
                    { "name": "Terror", "active": false }
                  ]
                }
                """));
    }

    public void ConfigureExternalCatalogFailure()
    {
        EnsureCatalogServer().Reset();
        EnsureCatalogServer()
            .Given(Request.Create()
                .WithPath("/api/genre-catalog/validate")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(503)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""{ "message": "catalog unavailable" }"""));
    }

    private async Task ExecuteSchemaAsync()
    {
        var schema = File.ReadAllText(FindDatabaseScript("01_create_database.sql"));

        await using var connection = new NpgsqlConnection(_postgres.GetConnectionString());
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(schema, connection);
        await command.ExecuteNonQueryAsync();
    }

    private WireMockServer EnsureCatalogServer()
    {
        return _catalogServer ?? throw new InvalidOperationException("WireMock não inicializado.");
    }

    private static string FindDatabaseScript(string fileName)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, "src", "database", fileName);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"Script de banco não encontrado: {fileName}");
    }
}
