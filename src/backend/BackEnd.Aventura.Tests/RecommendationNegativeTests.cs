using System.Net;
using System.Net.Http.Json;
using BackEnd.Aventura.Services;
using BackEnd.Aventura.Tests.Fixtures;
using Xunit;

namespace BackEnd.Aventura.Tests;

// Grupo I — Validação de parâmetros (sem Docker, sem arquivos ML)
public class RecommendationValidationTests : IClassFixture<LightweightFixture>
{
    private readonly HttpClient _client;

    public RecommendationValidationTests(LightweightFixture fixture) =>
        _client = fixture.Client;

    // NEG-REC-01 — n = 0 retorna 400 Bad Request
    [Fact]
    [Trait("Fixture", "Lightweight")]
    public async Task Rec_N0_Returns400()
    {
        var response = await _client.GetAsync("/api/recommendation/1?n=0");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // NEG-REC-02 — n negativo retorna 400 Bad Request
    [Fact]
    [Trait("Fixture", "Lightweight")]
    public async Task Rec_NNegative_Returns400()
    {
        var response = await _client.GetAsync("/api/recommendation/1?n=-1");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // NEG-REC-03 — n = 101 retorna 400 Bad Request
    [Fact]
    [Trait("Fixture", "Lightweight")]
    public async Task Rec_N101_Returns400()
    {
        var response = await _client.GetAsync("/api/recommendation/1?n=101");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // NEG-REC-04 — n = int.MaxValue retorna 400 Bad Request
    [Fact]
    [Trait("Fixture", "Lightweight")]
    public async Task Rec_NIntMax_Returns400()
    {
        var response = await _client.GetAsync($"/api/recommendation/1?n={int.MaxValue}");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // NEG-REC-05 — n = int.MinValue retorna 400 Bad Request
    [Fact]
    [Trait("Fixture", "Lightweight")]
    public async Task Rec_NIntMin_Returns400()
    {
        var response = await _client.GetAsync($"/api/recommendation/1?n={int.MinValue}");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // NEG-REC-06 — Corpo do erro menciona o limite permitido
    [Fact]
    [Trait("Fixture", "Lightweight")]
    public async Task Rec_N0_ErrorBodyMentionsLimit()
    {
        var response = await _client.GetAsync("/api/recommendation/1?n=0");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("n deve estar entre 1 e 100", body);
    }
}

// Grupo II — Fallback de serviço (requer stack completo via IntegrationFixture)
[Collection("Integration")]
public class RecommendationServiceEdgeCaseTests
{
    private readonly HttpClient _client;

    public RecommendationServiceEdgeCaseTests(IntegrationFixture fixture) =>
        _client = fixture.Client;

    // NEG-REC-07 — Cold-start com categoria inexistente cai em popular items
    [Fact]
    public async Task Rec_ColdStart_UnknownCategory_FallsBackToPopularItems()
    {
        await _client.PostAsJsonAsync("/api/preference/99010",
            new { tipos = new[] { "CATEGORIA_QUE_NAO_EXISTE_XYZ" } });

        var response = await _client.GetAsync("/api/recommendation/99010?n=5");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var results = await response.Content.ReadFromJsonAsync<List<RecommendationResult>>();
        Assert.NotNull(results);
        Assert.NotEmpty(results);
        Assert.All(results, r => Assert.Null(r.Score));
    }
}
