using System.Net;
using System.Net.Http.Json;
using BackEnd.Aventura.ApiTests.Fixtures;
using BackEnd.Aventura.ApiTests.Helpers;
using BackEnd.Aventura.Dto;
using FluentAssertions;
using Npgsql;
using Xunit;

namespace BackEnd.Aventura.ApiTests.TI03;

[Collection("Api")]
[Trait("Category", "TI-03")]
public class RecomendacaoTests : IAsyncLifetime
{
    private readonly ApiFixture _api;
    private Guid _userId;

    public RecomendacaoTests(ApiFixture api) => _api = api;

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

    [Fact(DisplayName = "TI-03-P1: Usuário sem histórico retorna 200 sem erro 500")]
    public async Task GetRecomendacoes_NoHistory_Returns200()
    {
        var r = await _api.HttpClient.GetAsync($"/api/recomendacao/{_userId}");
        r.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "TI-03-P2: Com recomendações salvas retorna lista ordenada por score")]
    public async Task GetRecomendacoes_WithStoredData_ReturnsSortedList()
    {
        await using var conn = _api.CreateConnection();
        await conn.OpenAsync();
        var esp1 = await DbSeedHelper.InsertEspetaculoAsync(conn, "Show Alta Relevância");
        var esp2 = await DbSeedHelper.InsertEspetaculoAsync(conn, "Show Baixa Relevância");

        await using var cmd1 = new NpgsqlCommand(
            "INSERT INTO recomendacoes (usuario_id, espetaculo_id, score) VALUES (@U, @E, @S)", conn);
        cmd1.Parameters.AddWithValue("@U", _userId);
        cmd1.Parameters.AddWithValue("@E", esp1);
        cmd1.Parameters.AddWithValue("@S", 0.95m);
        await cmd1.ExecuteNonQueryAsync();

        await using var cmd2 = new NpgsqlCommand(
            "INSERT INTO recomendacoes (usuario_id, espetaculo_id, score) VALUES (@U, @E, @S)", conn);
        cmd2.Parameters.AddWithValue("@U", _userId);
        cmd2.Parameters.AddWithValue("@E", esp2);
        cmd2.Parameters.AddWithValue("@S", 0.60m);
        await cmd2.ExecuteNonQueryAsync();

        var r    = await _api.HttpClient.GetAsync($"/api/recomendacao/{_userId}?limit=10");
        var body = await r.Content.ReadFromJsonAsync<List<RecomendacaoItemDto>>();

        r.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().HaveCountGreaterOrEqualTo(2);
        body![0].Score.Should().BeGreaterOrEqualTo(body[1].Score, "deve vir ordenado por score DESC");
    }

    [Fact(DisplayName = "TI-03-P3: Campos obrigatórios (espetaculo, score) presentes na resposta")]
    public async Task GetRecomendacoes_RequiredFieldsPresent()
    {
        await using var conn = _api.CreateConnection();
        await conn.OpenAsync();
        var espId = await DbSeedHelper.InsertEspetaculoAsync(conn, "Show Obrigatório");

        await using var cmd = new NpgsqlCommand(
            "INSERT INTO recomendacoes (usuario_id, espetaculo_id, score) VALUES (@U, @E, @S)", conn);
        cmd.Parameters.AddWithValue("@U", _userId);
        cmd.Parameters.AddWithValue("@E", espId);
        cmd.Parameters.AddWithValue("@S", 0.80m);
        await cmd.ExecuteNonQueryAsync();

        var r    = await _api.HttpClient.GetAsync($"/api/recomendacao/{_userId}");
        var body = await r.Content.ReadFromJsonAsync<List<RecomendacaoItemDto>>();

        body![0].Espetaculo.Should().NotBeNull();
        body[0].Espetaculo.Id.Should().NotBeEmpty();
        body[0].Score.Should().BeGreaterThan(0);
    }

    [Fact(DisplayName = "TI-03-N1: Limit 0 retorna 400")]
    public async Task GetRecomendacoes_LimitZero_Returns400()
    {
        var r = await _api.HttpClient.GetAsync($"/api/recomendacao/{_userId}?limit=0");
        r.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "TI-03-N2: Limit acima de 50 retorna 400")]
    public async Task GetRecomendacoes_LimitOver50_Returns400()
    {
        var r = await _api.HttpClient.GetAsync($"/api/recomendacao/{_userId}?limit=51");
        r.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "TI-03-N3: Sem autenticação retorna 401")]
    public async Task GetRecomendacoes_WithoutToken_Returns401()
    {
        using var client = _api.CreateClient();
    
        var r = await client.GetAsync($"/api/recomendacao/{_userId}");
    
        r.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}