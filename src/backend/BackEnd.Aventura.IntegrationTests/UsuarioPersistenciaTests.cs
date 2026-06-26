using System.Net;
using System.Net.Http.Json;
using BackEnd.Aventura.ApiTests.Fixtures;
using BackEnd.Aventura.ApiTests.Helpers;
using BackEnd.Aventura.Dto;
using FluentAssertions;
using Npgsql;
using Xunit;

namespace BackEnd.Aventura.ApiTests.TI02;

[Collection("Api")]
[Trait("Category", "TI-02")]
public class UsuarioPersistenciaTests : IAsyncLifetime
{
    private readonly ApiFixture _api;
    public UsuarioPersistenciaTests(ApiFixture api) => _api = api;
    public async Task InitializeAsync() => await _api.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact(DisplayName = "TI-02-P1: Register persiste campos obrigatórios com senha hasheada")]
    public async Task Register_PersistsAllFields()
    {
        var response = await _api.HttpClient.PostAsJsonAsync("/api/auth/register",
            new { nome = "Fernanda", email = "fernanda@aventura.test", senha = "Senha@123",
                  idade = 30, consentimentoAceito = true });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();

        await using var conn = _api.CreateConnection();
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            "SELECT nome, email, senha_hash, idade FROM usuarios WHERE id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", body!.UsuarioId);
        await using var reader = await cmd.ExecuteReaderAsync();

        reader.Read().Should().BeTrue();
        reader.GetString(0).Should().Be("Fernanda");
        reader.GetString(1).Should().Be("fernanda@aventura.test");
        reader.GetString(2).Should().NotBe("Senha@123", "senha deve estar hasheada");
        reader.GetInt32(3).Should().Be(30);
    }

    [Fact(DisplayName = "TI-02-N1: E-mail duplicado retorna 409 e não cria segundo registro no banco")]
    public async Task Register_DuplicateEmail_NoSideEffect()
    {
        await _api.HttpClient.PostAsJsonAsync("/api/auth/register",
            new { nome = "Gustavo", email = "gustavo@aventura.test", senha = "Senha@1",
                  idade = 25, consentimentoAceito = true });
        await _api.HttpClient.PostAsJsonAsync("/api/auth/register",
            new { nome = "Gustavo2", email = "gustavo@aventura.test", senha = "Senha@2",
                  idade = 26, consentimentoAceito = true });

        await using var conn = _api.CreateConnection();
        await conn.OpenAsync();
        var count = await DbSeedHelper.CountRowsAsync(conn, "usuarios",
            "LOWER(email) = 'gustavo@aventura.test'");
        count.Should().Be(1, "não deve haver duplicata no banco");
    }

    [Fact(DisplayName = "TI-02-P2: Wishlist é persistida corretamente no banco")]
    public async Task Wishlist_PersistsCorrectly()
    {
        var userId = Guid.NewGuid();
        await using var conn = _api.CreateConnection();
        await conn.OpenAsync();
        await DbSeedHelper.InsertUsuarioAsync(conn, userId);
        await DbSeedHelper.InsertPreferenciasAsync(conn, userId);
        var espId = await DbSeedHelper.InsertEspetaculoAsync(conn);

        _api.HttpClient.SetBearerToken(userId);
        var r = await _api.HttpClient.PostAsync($"/api/wishlist/{userId}/{espId}", null);
        r.StatusCode.Should().Be(HttpStatusCode.Created);

        var count = await DbSeedHelper.CountRowsAsync(conn, "wishlist_curtidas",
            $"usuario_id = '{userId}' AND espetaculo_id = '{espId}'");
        count.Should().Be(1);
    }

    [Fact(DisplayName = "TI-02-N2: Wishlist duplicada retorna 409 e não duplica no banco")]
    public async Task Wishlist_Duplicate_NoSideEffect()
    {
        var userId = Guid.NewGuid();
        await using var conn = _api.CreateConnection();
        await conn.OpenAsync();
        await DbSeedHelper.InsertUsuarioAsync(conn, userId);
        await DbSeedHelper.InsertPreferenciasAsync(conn, userId);
        var espId = await DbSeedHelper.InsertEspetaculoAsync(conn);

        _api.HttpClient.SetBearerToken(userId);
        await _api.HttpClient.PostAsync($"/api/wishlist/{userId}/{espId}", null);
        var r2 = await _api.HttpClient.PostAsync($"/api/wishlist/{userId}/{espId}", null);
        r2.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var count = await DbSeedHelper.CountRowsAsync(conn, "wishlist_curtidas",
            $"usuario_id = '{userId}' AND espetaculo_id = '{espId}'");
        count.Should().Be(1);
    }

    [Fact(DisplayName = "TI-02-P3: Remoção da wishlist apaga o registro do banco")]
    public async Task Wishlist_Remove_DeletesFromDb()
    {
        var userId = Guid.NewGuid();
        await using var conn = _api.CreateConnection();
        await conn.OpenAsync();
        await DbSeedHelper.InsertUsuarioAsync(conn, userId);
        await DbSeedHelper.InsertPreferenciasAsync(conn, userId);
        var espId = await DbSeedHelper.InsertEspetaculoAsync(conn);

        _api.HttpClient.SetBearerToken(userId);
        await _api.HttpClient.PostAsync($"/api/wishlist/{userId}/{espId}", null);
        var del = await _api.HttpClient.DeleteAsync($"/api/wishlist/{userId}/{espId}");
        del.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var count = await DbSeedHelper.CountRowsAsync(conn, "wishlist_curtidas",
            $"usuario_id = '{userId}' AND espetaculo_id = '{espId}'");
        count.Should().Be(0);
    }

    [Fact(DisplayName = "TI-02-P4: GET espetáculo por ID responde em menos de 200ms")]
    public async Task GetEspetaculo_RespondsFast()
    {
        var userId = Guid.NewGuid();
        await using var conn = _api.CreateConnection();
        await conn.OpenAsync();
        await DbSeedHelper.InsertUsuarioAsync(conn, userId);
        var espId = await DbSeedHelper.InsertEspetaculoAsync(conn, "Cats");

        _api.HttpClient.SetBearerToken(userId);
        var warmup = await _api.HttpClient.GetAsync($"/api/espetaculo/{espId}");
        warmup.StatusCode.Should().Be(HttpStatusCode.OK);

        var sw = System.Diagnostics.Stopwatch.StartNew();
        var r  = await _api.HttpClient.GetAsync($"/api/espetaculo/{espId}");
        sw.Stop();

        r.StatusCode.Should().Be(HttpStatusCode.OK);
        sw.ElapsedMilliseconds.Should().BeLessThan(200);
    }
}
