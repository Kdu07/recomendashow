using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Testcontainers.PostgreSql;
using WireMock.Server;
using Xunit;

namespace BackEnd.Aventura.IntegrationTests.Fixtures;

public class IntegrationTestFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .Build();

    public WireMockServer ViaCepServer { get; private set; } = null!;
    public HttpClient Client { get; private set; } = null!;
    public string ConnectionString => _postgres.GetConnectionString();

    async Task IAsyncLifetime.InitializeAsync()
    {
        await _postgres.StartAsync();

        await ApplySqlScriptAsync("01_create_database.sql");
        await ApplySqlScriptAsync("02_sprint4_improvements.sql");
        await ApplySqlScriptAsync("02_initial_data.sql");

        ViaCepServer = WireMockServer.Start();
        Client = CreateClient();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["POSTGRES_HOST"] = _postgres.Hostname,
                ["POSTGRES_PORT"] = _postgres.GetMappedPublicPort(5432).ToString(),
                ["POSTGRES_DB"] = "postgres",
                ["POSTGRES_USER"] = "postgres",
                ["POSTGRES_PASSWORD"] = "postgres",
                ["ViaCep:BaseUrl"] = ViaCepServer.Urls[0],
            });
        });

        // Timeout curto no HttpClient do ViaCepService para que timeouts reais sejam
        // capturados pelo catch de BuscarAsync e devolvidos como 400 dentro do prazo do teste
        builder.ConfigureServices(services =>
        {
            services.AddHttpClient<ViaCepService>()
                .ConfigureHttpClient(client => client.Timeout = TimeSpan.FromSeconds(3));
        });
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        Client?.Dispose();
        ViaCepServer?.Stop();
        ViaCepServer?.Dispose();
        await _postgres.DisposeAsync();
        // WebApplicationFactory.Dispose() é chamado pelo xUnit via IDisposable
    }

    private async Task ApplySqlScriptAsync(string filename)
    {
        var sql = await File.ReadAllTextAsync(GetSqlPath(filename));
        await using var connection = new NpgsqlConnection(_postgres.GetConnectionString());
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    // AppContext.BaseDirectory = .../BackEnd.Aventura.IntegrationTests/bin/Debug/net10.0/
    // 5 níveis acima chega em src/, depois entra em database/
    private static string GetSqlPath(string filename) =>
        Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "database", filename));
}
