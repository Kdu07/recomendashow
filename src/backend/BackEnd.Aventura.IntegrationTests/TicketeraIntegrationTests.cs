using System.Net;
using System.Net.Http.Json;
using BackEnd.Aventura.ApiTests.Fixtures;
using BackEnd.Aventura.ApiTests.Helpers;
using BackEnd.Aventura.Dto;
using FluentAssertions;
using Xunit;

namespace BackEnd.Aventura.ApiTests.TI04;

// O fluxo real da ticketera: o espetáculo tem url_bilheteria no banco.
// GET /api/espetaculo/{id} retorna essa URL no campo UrlBilheteria.
// O frontend usa essa URL para redirecionar o usuário à ticketera.
// Não há endpoint separado de validação.

[Collection("Api")]
[Trait("Category", "TI-04")]
public class TicketeraIntegrationTests : IAsyncLifetime
{
    private readonly ApiFixture _api;
    private Guid _userId;

    public TicketeraIntegrationTests(ApiFixture api) => _api = api;

    public async Task InitializeAsync()
    {
        await _api.ResetDatabaseAsync();
        _userId = Guid.NewGuid();
        await using var conn = _api.CreateConnection();
        await conn.OpenAsync();
        await DbSeedHelper.InsertUsuarioAsync(conn, _userId);
        await DbSeedHelper.InsertPreferenciasAsync(conn, _userId);
        _api.HttpClient.SetBearerToken(_userId);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact(DisplayName = "TI-04-P1: Espetáculo retorna url_bilheteria correta no response")]
    public async Task GetEspetaculo_ReturnsBilheteriaUrl()
    {
        await using var conn = _api.CreateConnection();
        await conn.OpenAsync();
        var espId = await DbSeedHelper.InsertEspetaculoAsync(conn,
            nome: "Show com Link",
            urlBilheteria: "https://ticketera.com/show-123");

        var r = await _api.HttpClient.GetAsync($"/api/espetaculo/{espId}");

        r.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await r.Content.ReadFromJsonAsync<EspetaculoDto>();
        body!.UrlBilheteria.Should().Be("https://ticketera.com/show-123");
    }

    [Fact(DisplayName = "TI-04-P2: Todos espetáculos retornam url_bilheteria preenchida")]
    public async Task GetAll_AllEspetaculosHaveUrl()
    {
        await using var conn = _api.CreateConnection();
        await conn.OpenAsync();
        await DbSeedHelper.InsertEspetaculoAsync(conn, "Show A", "https://ticketera.com/a");
        await DbSeedHelper.InsertEspetaculoAsync(conn, "Show B", "https://ticketera.com/b");

        var r    = await _api.HttpClient.GetAsync("/api/espetaculo");
        var body = await r.Content.ReadFromJsonAsync<List<EspetaculoDto>>();

        r.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body!.Should().OnlyContain(e => !string.IsNullOrWhiteSpace(e.UrlBilheteria));
    }

    [Fact(DisplayName = "TI-04-N1: Espetáculo inexistente retorna 404")]
    public async Task GetEspetaculo_UnknownId_Returns404()
    {
        var r = await _api.HttpClient.GetAsync($"/api/espetaculo/{Guid.NewGuid()}");
        r.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "TI-04-N2: Sem autenticação retorna 401")]
    public async Task GetEspetaculo_WithoutToken_Returns401()
    {
        using var client = _api.CreateClient();
    
        var r = await client.GetAsync("/api/espetaculo");
    
        r.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}