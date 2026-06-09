using Microsoft.AspNetCore.Mvc.Testing;
using Npgsql;
using Testcontainers.PostgreSql;

namespace BackEnd.Aventura.Tests.Fixtures;

public sealed class IntegrationFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _db = new PostgreSqlBuilder().Build();
    public WebApplicationFactory<Program> Factory { get; private set; } = null!;
    public HttpClient Client { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _db.StartAsync();
        await ApplySchemaAsync();

        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("ConnectionStrings:DefaultConnection", _db.GetConnectionString());
                builder.UseSetting("ML:ModelPath",               ModelPath("model.zip"));
                builder.UseSetting("ML:ColdStartPath",           ModelPath("cold_start.json"));
                builder.UseSetting("ML:DatasetPath",             DatasetPath());
                builder.UseSetting("ML:ItemCategoriesPath",      ModelPath("item_categories.json"));
                builder.UseSetting("ML:CategoryAffinityPath",    ModelPath("category_affinity.json"));
                builder.UseSetting("ML:ColdStartByCategoryPath", ModelPath("cold_start_by_category.json"));
            });

        Client = Factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        Factory.Dispose();
        await _db.DisposeAsync();
    }

    private async Task ApplySchemaAsync()
    {
        await using var conn = new NpgsqlConnection(_db.GetConnectionString());
        await conn.OpenAsync();

        foreach (var sqlFile in new[] { "01_create_database.sql", "02_sprint4_improvements.sql" })
        {
            var path = Path.Combine(SolutionRoot(), "src", "database", sqlFile);
            var sql = await File.ReadAllTextAsync(path);
            await using var cmd = new NpgsqlCommand(sql, conn);
            await cmd.ExecuteNonQueryAsync();
        }
    }

    // AppContext.BaseDirectory ends with a trailing slash, so the first GetDirectoryName call only
    // strips the slash without going up a level — we need 7 calls to reach g02/ from net9.0/.
    private static string SolutionRoot()
    {
        var dir = AppContext.BaseDirectory;
        for (var i = 0; i < 7; i++)
            dir = Path.GetDirectoryName(dir)!;
        return dir;
    }

    private static string ModelPath(string file) =>
        Path.Combine(SolutionRoot(), "data", "models", file);

    private static string DatasetPath() =>
        Path.Combine(SolutionRoot(), "data", "dataset", "dataset.csv");
}

[CollectionDefinition("Integration")]
public class IntegrationCollection : ICollectionFixture<IntegrationFixture> { }
