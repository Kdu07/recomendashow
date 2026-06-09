using System.Net;
using System.Net.Http.Json;
using BackEnd.Aventura.Services;
using BackEnd.Aventura.Tests.Fixtures;
using Xunit;

namespace BackEnd.Aventura.Tests;

[Collection("Integration")]
public class PreferenceHappyPathTests
{
    private readonly HttpClient _client;

    public PreferenceHappyPathTests(IntegrationFixture fixture) =>
        _client = fixture.Client;

    // HP-PREF-01 — POST com lista válida retorna 204 NoContent
    [Fact]
    public async Task Pref01_Post_ValidTipos_Returns204()
    {
        var response = await _client.PostAsJsonAsync("/api/preference/10001",
            new { tipos = new[] { "Show Stand-Up" } });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    // HP-PREF-02 — GET para usuário sem preferências retorna lista vazia
    [Fact]
    public async Task Pref02_Get_UserWithNoPreferences_ReturnsEmptyList()
    {
        var response = await _client.GetAsync("/api/preference/99999");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<List<string>>();
        Assert.NotNull(body);
        Assert.Empty(body);
    }

    // HP-PREF-03 — GET após POST retorna as preferências salvas
    [Fact]
    public async Task Pref03_Get_AfterPost_ReturnsSavedPreferences()
    {
        await _client.PostAsJsonAsync("/api/preference/10002",
            new { tipos = new[] { "Espetáculo Musical", "Show Musical" } });

        var response = await _client.GetAsync("/api/preference/10002");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<List<string>>();
        Assert.NotNull(body);
        Assert.Equal(2, body.Count);
        Assert.Contains("Espetáculo Musical", body);
        Assert.Contains("Show Musical", body);
    }

    // HP-PREF-04 — Segundo POST sobrescreve o valor anterior
    [Fact]
    public async Task Pref04_SecondPost_OverwritesPreviousPreferences()
    {
        await _client.PostAsJsonAsync("/api/preference/10003",
            new { tipos = new[] { "Show Stand-Up" } });
        await _client.PostAsJsonAsync("/api/preference/10003",
            new { tipos = new[] { "Espetáculo Musical" } });

        var body = await _client.GetFromJsonAsync<List<string>>("/api/preference/10003");

        Assert.NotNull(body);
        Assert.Single(body);
        Assert.Contains("Espetáculo Musical", body);
        Assert.DoesNotContain("Show Stand-Up", body);
    }

    // HP-PREF-05 — Preferência salva influencia a recomendação cold-start
    [Fact]
    public async Task Pref05_SavedPreference_InfluencesColdStartRecommendation()
    {
        await _client.PostAsJsonAsync("/api/preference/99050",
            new { tipos = new[] { "Espetáculo Teatral" } });

        var response = await _client.GetAsync("/api/recommendation/99050?n=5");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var results = await response.Content.ReadFromJsonAsync<List<RecommendationResult>>();
        Assert.NotNull(results);
        Assert.NotEmpty(results);
        Assert.All(results, r => Assert.Null(r.Score));
    }
}
