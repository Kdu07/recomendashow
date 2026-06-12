using BackEnd.Aventura.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using Xunit;

namespace BackEnd.Aventura.ApiTests.Fixtures;

public class ApiFixture : IAsyncLifetime
{
    public const string JwtKey      = "test-jwt-key-with-at-least-32-chars!!";
    public const string JwtIssuer   = "BackEnd.Aventura";
    public const string JwtAudience = "BackEnd.AventuraClient";

    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .Build();

    private WebApplicationFactory<Program>? _factory;
    private Respawner?                       _respawner;

    public HttpClient HttpClient { get; private set; } = null!;
    public string     ConnString { get; private set; } = string.Empty;

    public HttpClient CreateClient()
    {
        return _factory!.CreateClient();
    }

    public async Task InitializeAsync()
    {
        Environment.SetEnvironmentVariable("JWT_KEY", JwtKey);
        Environment.SetEnvironmentVariable("Jwt__Key", JwtKey);
        Environment.SetEnvironmentVariable("Jwt__Issuer", JwtIssuer);
        Environment.SetEnvironmentVariable("Jwt__Audience", JwtAudience);

        await _postgres.StartAsync();
        ConnString = _postgres.GetConnectionString();

        await ApplySqlAsync("01_create_database.sql");
        await ApplySqlAsync("02_sprint4_improvements.sql");

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(host =>
            {
                host.UseEnvironment("Test");
                host.ConfigureAppConfiguration((_, cfg) =>
                    cfg.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:DefaultConnection"] = ConnString,
                        ["JWT_KEY"]           = JwtKey,
                        ["Jwt:Key"]           = JwtKey,
                        ["Jwt:Issuer"]        = JwtIssuer,
                        ["Jwt:Audience"]      = JwtAudience,
                        ["Jwt:ExpiryMinutes"] = "60",
                        ["ViaCep:BaseUrl"]    = "https://viacep.com.br/",
                    }));

                host.ConfigureServices(services =>
                {
                    var d = services.SingleOrDefault(
                        x => x.ServiceType == typeof(IDbConnectionFactory));

                    if (d is not null)
                        services.Remove(d);

                    services.AddSingleton<IDbConnectionFactory>(_ =>
                        new TestDbConnectionFactory(ConnString));
                });
            });

        HttpClient = _factory.CreateClient();

        await using var conn = new NpgsqlConnection(ConnString);
        await conn.OpenAsync();

        _respawner = await Respawner.CreateAsync(conn, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"],
        });
    }

    public async Task DisposeAsync()
    {
        HttpClient?.Dispose();

        if (_factory is not null)
            await _factory.DisposeAsync();

        await _postgres.DisposeAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        await using var conn = new NpgsqlConnection(ConnString);
        await conn.OpenAsync();
        await _respawner!.ResetAsync(conn);
    }

    public NpgsqlConnection CreateConnection() => new(ConnString);

    private async Task ApplySqlAsync(string fileName)
    {
        var path = Path.Combine(FindSqlDir(), fileName);
        var sql  = await File.ReadAllTextAsync(path);
        await using var conn = new NpgsqlConnection(ConnString);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    private static string FindSqlDir()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var c = Path.Combine(dir.FullName, "src", "database");
            if (Directory.Exists(c)) return c;
            dir = dir.Parent;
        }
        throw new DirectoryNotFoundException("src/database não encontrado");
    }
}

public class TestDbConnectionFactory : IDbConnectionFactory
{
    private readonly string _cs;
    public TestDbConnectionFactory(string cs) => _cs = cs;
    public NpgsqlConnection CreateConnection() => new(_cs);
}