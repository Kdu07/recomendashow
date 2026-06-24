using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace InferenceService.Tests;

/// <summary>
/// Sobe um PostgreSQL efêmero (Testcontainers) com o schema real do projeto
/// (01_create_database + 02_sprint4_improvements) para testar o
/// <see cref="InferenceService.Data.RecommendationWriteRepository"/> contra um banco de verdade.
/// Requer Docker em execução.
/// </summary>
public sealed class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await ApplyScriptAsync("01_create_database.sql");
        await ApplyScriptAsync("02_sprint4_improvements.sql");
    }

    public Task DisposeAsync() => _container.DisposeAsync().AsTask();

    private async Task ApplyScriptAsync(string fileName)
    {
        var sql = await File.ReadAllTextAsync(Path.Combine(FindSqlDirectory(), fileName));
        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    private static string FindSqlDirectory()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, "src", "database");
            if (Directory.Exists(candidate))
                return candidate;
            dir = dir.Parent;
        }
        throw new DirectoryNotFoundException(
            $"Não foi possível localizar src/database a partir de {AppContext.BaseDirectory}.");
    }
}
