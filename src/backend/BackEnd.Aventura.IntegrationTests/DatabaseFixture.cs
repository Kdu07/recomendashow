using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace BackEnd.Aventura.IntegrationTests;

public class DatabaseFixture : IAsyncLifetime
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

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    public async Task ApplyScriptAsync(string fileName)
    {
        var path = Path.Combine(FindSqlDirectory(), fileName);
        var sql = await File.ReadAllTextAsync(path);

        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
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
            "Não foi possível localizar o diretório src/database. " +
            $"Busca iniciada em: {AppContext.BaseDirectory}");
    }
}
