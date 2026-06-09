using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;
using Xunit.Abstractions;

namespace BackEnd.Aventura.Tests;

public sealed class RecommendationLoadTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _db = new PostgreSqlBuilder().Build();
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private readonly ITestOutputHelper _output;

    public RecommendationLoadTests(ITestOutputHelper output) => _output = output;

    // -------------------------------------------------------------------------
    // Setup / teardown
    // -------------------------------------------------------------------------

    public async Task InitializeAsync()
    {
        await _db.StartAsync();
        await ApplySchemaAsync();

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting(
                    "ConnectionStrings:DefaultConnection",
                    _db.GetConnectionString());

                builder.UseSetting("ML:ModelPath",               ModelPath("model.zip"));
                builder.UseSetting("ML:ColdStartPath",           ModelPath("cold_start.json"));
                builder.UseSetting("ML:DatasetPath",             DatasetPath());
                builder.UseSetting("ML:ItemCategoriesPath",      ModelPath("item_categories.json"));
                builder.UseSetting("ML:CategoryAffinityPath",    ModelPath("category_affinity.json"));
                builder.UseSetting("ML:ColdStartByCategoryPath", ModelPath("cold_start_by_category.json"));
            });

        _client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        _factory.Dispose();
        await _db.DisposeAsync();
    }

    // -------------------------------------------------------------------------
    // Teste 1 — Corretude sob concorrência
    // -------------------------------------------------------------------------

    [Fact]
    public async Task Recommend_ConcurrentRequests_AllReturnValidScores()
    {
        var tasks = Enumerable.Range(1, 50)
            .Select(id => _client.GetAsync($"/api/recommendation/{id}?n=10"))
            .ToList();

        var responses = await Task.WhenAll(tasks);

        foreach (var resp in responses)
        {
            resp.EnsureSuccessStatusCode();
            var results = await resp.Content
                .ReadFromJsonAsync<List<RecommendationResult>>();

            Assert.NotNull(results);
            Assert.All(results, r =>
            {
                if (!r.Score.HasValue) return;
                Assert.False(float.IsNaN(r.Score.Value),       "Score não pode ser NaN");
                Assert.False(float.IsInfinity(r.Score.Value),  "Score não pode ser infinito");
                Assert.InRange(r.Score.Value, 0f, 1f);
            });
        }
    }

    // -------------------------------------------------------------------------
    // Teste 2 — Curva de latência sob carga crescente
    // -------------------------------------------------------------------------

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(200)]
    public async Task Recommend_MeasureLatencyUnderConcurrency(int concurrency)
    {
        // warm-up: garante que o modelo já está carregado antes de medir
        await _client.GetAsync("/api/recommendation/1?n=10");

        var sw = Stopwatch.StartNew();

        var tasks = Enumerable.Range(1, concurrency)
            .Select(id => _client.GetAsync($"/api/recommendation/{id}?n=10"))
            .ToList();

        await Task.WhenAll(tasks);
        sw.Stop();

        _output.WriteLine($"concurrency={concurrency,3}  elapsed={sw.ElapsedMilliseconds}ms");

        // RNF01: p95 ≤ 3 s com 200 usuários; threshold genérico de 500 ms/req para demais níveis
        var limitMs = concurrency switch
        {
            200 => 3_000L,
            _   => (long)concurrency * 500
        };

        Assert.True(
            sw.ElapsedMilliseconds < limitMs,
            $"Latência total {sw.ElapsedMilliseconds}ms excede limite {limitMs}ms para concurrency={concurrency}");
    }

    // -------------------------------------------------------------------------
    // Teste 3 — Cold-start sem deadlock sob concorrência
    // -------------------------------------------------------------------------

    [Fact]
    public async Task Recommend_ColdStartUsers_NoCrashUnderConcurrency()
    {
        // userIds 99000+ não existem no dataset → consumed.Count == 0 → cold-start path
        var tasks = Enumerable.Range(99000, 30)
            .Select(id => _client.GetAsync($"/api/recommendation/{id}?n=5"))
            .ToList();

        var responses = await Task.WhenAll(tasks);

        Assert.All(responses, r =>
            Assert.Equal(HttpStatusCode.OK, r.StatusCode));
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

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
