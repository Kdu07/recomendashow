using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using BackEnd.Aventura.Tests.Fixtures;
using Xunit;

namespace BackEnd.Aventura.Tests;

public class PreferenceNegativeTests : IClassFixture<LightweightFixture>
{
    private readonly HttpClient _client;

    public PreferenceNegativeTests(LightweightFixture fixture) =>
        _client = fixture.Client;

    // NEG-PREF-01 — Tipos vazio retorna 400 Bad Request
    [Fact]
    [Trait("Fixture", "Lightweight")]
    public async Task Pref_EmptyTipos_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/preference/1",
            new { tipos = Array.Empty<string>() });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // NEG-PREF-02 — Tipos null retorna 400 Bad Request
    [Fact]
    [Trait("Fixture", "Lightweight")]
    public async Task Pref_NullTipos_Returns400()
    {
        var json = JsonSerializer.Serialize(new { tipos = (string[]?)null });
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/preference/1", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // NEG-PREF-03 — Body ausente retorna 400
    [Fact]
    [Trait("Fixture", "Lightweight")]
    public async Task Pref_AbsentBody_Returns400()
    {
        var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/preference/1", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // NEG-PREF-04 — Corpo do erro menciona a validação
    [Fact]
    [Trait("Fixture", "Lightweight")]
    public async Task Pref_EmptyTipos_ErrorBodyMentionsValidation()
    {
        var response = await _client.PostAsJsonAsync("/api/preference/1",
            new { tipos = Array.Empty<string>() });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Informe ao menos um tipo de evento", body);
    }

    // NEG-PREF-05 — Preferência com categoria inexistente é aceita pelo controller
    [Fact]
    [Trait("Fixture", "Lightweight")]
    public async Task Pref_UnknownCategory_Returns204()
    {
        var response = await _client.PostAsJsonAsync("/api/preference/1",
            new { tipos = new[] { "CATEGORIA_INEXISTENTE_XYZ" } });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
