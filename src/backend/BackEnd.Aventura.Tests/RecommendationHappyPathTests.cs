using System.Net;
using System.Net.Http.Json;
using BackEnd.Aventura.Services;
using BackEnd.Aventura.Tests.Fixtures;
using Xunit;

namespace BackEnd.Aventura.Tests;

[Collection("Integration")]
public class RecommendationHappyPathTests
{
    private readonly HttpClient _client;

    public RecommendationHappyPathTests(IntegrationFixture fixture) =>
        _client = fixture.Client;

    // HP-REC-01 — Usuário warm retorna HTTP 200
    [Fact]
    public async Task Rec01_WarmUser_Returns200()
    {
        var response = await _client.GetAsync("/api/recommendation/1?n=10");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // HP-REC-02 — Scores de usuário warm são numéricos (não null)
    [Fact]
    public async Task Rec02_WarmUser_ScoresAreNotNull()
    {
        var results = await _client.GetFromJsonAsync<List<RecommendationResult>>(
            "/api/recommendation/1?n=10");

        Assert.NotNull(results);
        Assert.All(results, r => Assert.NotNull(r.Score));
    }

    // HP-REC-03 — Scores de usuário warm estão em [0, 1]
    [Fact]
    public async Task Rec03_WarmUser_ScoresInRange()
    {
        var results = await _client.GetFromJsonAsync<List<RecommendationResult>>(
            "/api/recommendation/1?n=10");

        Assert.NotNull(results);
        Assert.All(results, r => Assert.InRange(r.Score!.Value, 0f, 1f));
    }

    // HP-REC-04 — Scores de usuário warm são finitos
    [Fact]
    public async Task Rec04_WarmUser_ScoresAreFinite()
    {
        var results = await _client.GetFromJsonAsync<List<RecommendationResult>>(
            "/api/recommendation/1?n=10");

        Assert.NotNull(results);
        Assert.All(results, r =>
        {
            Assert.False(float.IsNaN(r.Score!.Value),      "Score não pode ser NaN");
            Assert.False(float.IsInfinity(r.Score!.Value), "Score não pode ser infinito");
        });
    }

    // HP-REC-05 — Resultados não contêm duplicatas
    [Fact]
    public async Task Rec05_WarmUser_NoDuplicateShows()
    {
        var results = await _client.GetFromJsonAsync<List<RecommendationResult>>(
            "/api/recommendation/1?n=10");

        Assert.NotNull(results);
        Assert.Equal(results.Count, results.DistinctBy(r => r.Show).Count());
    }

    // HP-REC-06 — Parâmetro n limita a quantidade exata
    [Fact]
    public async Task Rec06_ParameterN_LimitsResultCount()
    {
        var results = await _client.GetFromJsonAsync<List<RecommendationResult>>(
            "/api/recommendation/1?n=5");

        Assert.NotNull(results);
        Assert.Equal(5, results.Count);
    }

    // HP-REC-07 — n = 1 retorna exatamente um item
    [Fact]
    public async Task Rec07_N1_ReturnsExactlyOneItem()
    {
        var results = await _client.GetFromJsonAsync<List<RecommendationResult>>(
            "/api/recommendation/1?n=1");

        Assert.NotNull(results);
        Assert.Single(results);
    }

    // HP-REC-08 — n = 100 (limite máximo) retorna até 100 itens
    [Fact]
    public async Task Rec08_N100_ReturnsUpTo100Items()
    {
        var response = await _client.GetAsync("/api/recommendation/1?n=100");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var results = await response.Content.ReadFromJsonAsync<List<RecommendationResult>>();
        Assert.NotNull(results);
        Assert.InRange(results.Count, 1, 100);
    }

    // HP-REC-09 — n padrão (sem parâmetro) retorna no máximo 10 itens
    [Fact]
    public async Task Rec09_DefaultN_ReturnsAtMost10Items()
    {
        var response = await _client.GetAsync("/api/recommendation/1");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var results = await response.Content.ReadFromJsonAsync<List<RecommendationResult>>();
        Assert.NotNull(results);
        Assert.True(results.Count <= 10);
    }

    // HP-REC-10 — Cold-start sem preferências retorna popular items com score null
    [Fact]
    public async Task Rec10_ColdStart_NoPreferences_PopularItemsWithNullScore()
    {
        var response = await _client.GetAsync("/api/recommendation/99001?n=5");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var results = await response.Content.ReadFromJsonAsync<List<RecommendationResult>>();
        Assert.NotNull(results);
        Assert.NotEmpty(results);
        Assert.All(results, r => Assert.Null(r.Score));
    }

    // HP-REC-11 — Cold-start sem preferências retorna até n popular items
    [Fact]
    public async Task Rec11_ColdStart_NoPreferences_RespectsLimit()
    {
        var results = await _client.GetFromJsonAsync<List<RecommendationResult>>(
            "/api/recommendation/99002?n=3");

        Assert.NotNull(results);
        Assert.InRange(results.Count, 1, 3);
    }

    // HP-REC-12 — Cold-start com preferências retorna scores null
    [Fact]
    public async Task Rec12_ColdStart_WithPreferences_ReturnsNullScores()
    {
        await _client.PostAsJsonAsync("/api/preference/99003",
            new { tipos = new[] { "Espetáculo Teatral" } });

        var response = await _client.GetAsync("/api/recommendation/99003?n=5");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var results = await response.Content.ReadFromJsonAsync<List<RecommendationResult>>();
        Assert.NotNull(results);
        Assert.All(results, r => Assert.Null(r.Score));
    }

    // HP-REC-13 — Cold-start com preferências respeita o limite n
    [Fact]
    public async Task Rec13_ColdStart_WithPreferences_RespectsLimit()
    {
        await _client.PostAsJsonAsync("/api/preference/99004",
            new { tipos = new[] { "Show Musical" } });

        var results = await _client.GetFromJsonAsync<List<RecommendationResult>>(
            "/api/recommendation/99004?n=3");

        Assert.NotNull(results);
        Assert.InRange(results.Count, 1, 3);
    }

    // HP-REC-14 — Cold-start com preferências retorna itens distintos do popular items global
    [Fact]
    public async Task Rec14_ColdStart_WithPreferences_ReturnsDifferentSetThanGlobal()
    {
        // Passo 1: popular items globais sem preferência
        var popularItems = await _client.GetFromJsonAsync<List<RecommendationResult>>(
            "/api/recommendation/99005?n=5");

        // Passo 2: items filtrados pela categoria preferida
        await _client.PostAsJsonAsync("/api/preference/99006",
            new { tipos = new[] { "Espetáculo Teatral" } });
        var categoryItems = await _client.GetFromJsonAsync<List<RecommendationResult>>(
            "/api/recommendation/99006?n=5");

        Assert.NotNull(popularItems);
        Assert.NotNull(categoryItems);
        Assert.NotEmpty(categoryItems);

        var popularShows  = popularItems.Select(r => r.Show).ToHashSet();
        var categoryShows = categoryItems.Select(r => r.Show).ToHashSet();

        Assert.False(popularShows.SetEquals(categoryShows),
            "A preferência por 'Espetáculo Teatral' deve alterar o conjunto de shows retornado");
    }
}
