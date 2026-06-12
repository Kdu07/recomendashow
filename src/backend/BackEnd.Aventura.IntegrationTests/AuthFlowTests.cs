using System.Net;
using System.Net.Http.Json;
using BackEnd.Aventura.ApiTests.Fixtures;
using BackEnd.Aventura.ApiTests.Helpers;
using BackEnd.Aventura.Dto;
using FluentAssertions;
using Xunit;

namespace BackEnd.Aventura.ApiTests.TI01;

[Collection("Api")]
[Trait("Category", "TI-01")]
public class AuthFlowTests : IAsyncLifetime
{
    private readonly ApiFixture _api;
    public AuthFlowTests(ApiFixture api) => _api = api;
    public async Task InitializeAsync() => await _api.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact(DisplayName = "TI-01-P1: Login com credenciais válidas retorna 200 e token JWT")]
    public async Task Login_ValidCredentials_Returns200AndToken()
    {
        await _api.HttpClient.PostAsJsonAsync("/api/auth/register",
            new { nome = "Ana Paula", email = "ana@aventura.test", senha = "Senha@123",
                  idade = 28, consentimentoAceito = true });

        var response = await _api.HttpClient.PostAsJsonAsync("/api/auth/login",
            new { email = "ana@aventura.test", senha = "Senha@123" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        body!.Token.Should().NotBeNullOrWhiteSpace();
        body.Token.Split('.').Should().HaveCount(3, "JWT tem 3 partes separadas por ponto");
        body.UsuarioId.Should().NotBeEmpty();
    }

    [Fact(DisplayName = "TI-01-N1: Login com senha errada retorna 401 sem expor StackTrace")]
    public async Task Login_WrongPassword_Returns401()
    {
        await _api.HttpClient.PostAsJsonAsync("/api/auth/register",
            new { nome = "Bruno", email = "bruno@aventura.test", senha = "Certo@123",
                  idade = 30, consentimentoAceito = true });

        var response = await _api.HttpClient.PostAsJsonAsync("/api/auth/login",
            new { email = "bruno@aventura.test", senha = "Errado@999" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().NotContain("StackTrace");
        body.Should().NotContain("token");
    }

    [Fact(DisplayName = "TI-01-N2: Login com e-mail inexistente retorna 401")]
    public async Task Login_UnknownEmail_Returns401()
    {
        var response = await _api.HttpClient.PostAsJsonAsync("/api/auth/login",
            new { email = "naoexiste@aventura.test", senha = "Qualquer@1" });
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact(DisplayName = "TI-01-P2: Register com dados válidos retorna 201 com token")]
    public async Task Register_ValidData_Returns201()
    {
        var response = await _api.HttpClient.PostAsJsonAsync("/api/auth/register",
            new { nome = "Carla", email = "carla@aventura.test", senha = "Senha@123",
                  idade = 22, consentimentoAceito = true });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        body!.UsuarioId.Should().NotBeEmpty();
        body.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact(DisplayName = "TI-01-N3: Register com e-mail duplicado retorna 409")]
    public async Task Register_DuplicateEmail_Returns409()
    {
        await _api.HttpClient.PostAsJsonAsync("/api/auth/register",
            new { nome = "Diego", email = "diego@aventura.test", senha = "Senha@123",
                  idade = 25, consentimentoAceito = true });

        var response = await _api.HttpClient.PostAsJsonAsync("/api/auth/register",
            new { nome = "Diego2", email = "diego@aventura.test", senha = "Outra@456",
                  idade = 26, consentimentoAceito = true });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact(DisplayName = "TI-01-N4: Endpoint protegido sem token retorna 401")]
    public async Task ProtectedEndpoint_WithoutToken_Returns401()
    {
        _api.HttpClient.DefaultRequestHeaders.Authorization = null;
    
        var response = await _api.HttpClient.GetAsync("/api/espetaculo");
    
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact(DisplayName = "TI-01-P3: Catálogo com token válido retorna 200")]
    public async Task Catalog_WithValidToken_Returns200()
    {
        _api.HttpClient.SetBearerToken(Guid.NewGuid());
        var response = await _api.HttpClient.GetAsync("/api/espetaculo");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

[Collection("Api")]
[Trait("Category", "TI-01")]
public class ExploracaoFlowTests : IAsyncLifetime
{
    private readonly ApiFixture _api;
    private Guid _userId;
    private Guid _espetaculoId;

    public ExploracaoFlowTests(ApiFixture api) => _api = api;

    public async Task InitializeAsync()
    {
        await _api.ResetDatabaseAsync();
        _userId = Guid.NewGuid();
        await using var conn = _api.CreateConnection();
        await conn.OpenAsync();
        await DbSeedHelper.InsertUsuarioAsync(conn, _userId);
        await DbSeedHelper.InsertPreferenciasAsync(conn, _userId);
        _espetaculoId = await DbSeedHelper.InsertEspetaculoAsync(conn, "O Fantasma da Ópera");
        _api.HttpClient.SetBearerToken(_userId);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact(DisplayName = "TI-01-P4: Busca de espetáculos retorna 200")]
    public async Task Search_Returns200()
    {
        var response = await _api.HttpClient.GetAsync("/api/espetaculo/search");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "TI-01-P5: GET espetáculo por ID retorna dados corretos")]
    public async Task GetById_ReturnsCorrectData()
    {
        var response = await _api.HttpClient.GetAsync($"/api/espetaculo/{_espetaculoId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Fantasma");
    }

    [Fact(DisplayName = "TI-01-P6: Adicionar à wishlist retorna 201")]
    public async Task Wishlist_Add_Returns201()
    {
        var response = await _api.HttpClient
            .PostAsync($"/api/wishlist/{_userId}/{_espetaculoId}", null);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact(DisplayName = "TI-01-N5: Wishlist duplicada retorna 409")]
    public async Task Wishlist_AddDuplicate_Returns409()
    {
        await _api.HttpClient.PostAsync($"/api/wishlist/{_userId}/{_espetaculoId}", null);
        var response = await _api.HttpClient
            .PostAsync($"/api/wishlist/{_userId}/{_espetaculoId}", null);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}