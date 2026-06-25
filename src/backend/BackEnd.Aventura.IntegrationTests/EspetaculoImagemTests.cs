using System.Net;
using BackEnd.Aventura.ApiTests.Fixtures;
using BackEnd.Aventura.ApiTests.Helpers;
using FluentAssertions;
using Xunit;

namespace BackEnd.Aventura.ApiTests.TI01;

[Collection("Api")]
[Trait("Category", "TI-01")]
public class EspetaculoImagemTests : IAsyncLifetime
{
    private readonly ApiFixture _api;
    public EspetaculoImagemTests(ApiFixture api) => _api = api;
    public async Task InitializeAsync() => await _api.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact(DisplayName = "TI-01-P7: GET imagem de espetáculo sem token não retorna 401 (endpoint público)")]
    public async Task GetImagem_SemToken_NaoDeveRetornar401()
    {
        // Arrange
        Guid espetaculoId;
        await using (var conn = _api.CreateConnection())
        {
            await conn.OpenAsync();
            espetaculoId = await DbSeedHelper.InsertEspetaculoAsync(conn, nome: "Hamlet Teste Imagem");
        }

        // Act — usa um HttpClient novo, sem qualquer Authorization header configurado.
        using var anonymousClient = _api.CreateClient();
        var response = await anonymousClient.GetAsync($"/api/Espetaculo/{espetaculoId}/imagem");

        // Assert — sem o [AllowAnonymous], isso retornaria 401 antes de qualquer outra lógica.
        // Como não há Mongo real configurado neste ambiente de teste, o esperado é 404
        // (imagem não encontrada), nunca 401 (não autenticado).
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "TI-01-N6: GET imagem de espetáculo inexistente retorna 404")]
    public async Task GetImagem_EspetaculoInexistente_Retorna404()
    {
        // Act
        using var anonymousClient = _api.CreateClient();
        var response = await anonymousClient.GetAsync($"/api/Espetaculo/{Guid.NewGuid()}/imagem");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}