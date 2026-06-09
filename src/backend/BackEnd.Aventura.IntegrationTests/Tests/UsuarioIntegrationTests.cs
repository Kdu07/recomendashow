using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BackEnd.Aventura.Dto;
using BackEnd.Aventura.IntegrationTests.Fixtures;
using FluentAssertions;
using Npgsql;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;

namespace BackEnd.Aventura.IntegrationTests.Tests;

public class UsuarioIntegrationTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public UsuarioIntegrationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    // -------------------------------------------------------------------------
    // GET /api/usuario/{id} — ID existente → 200 com dados corretos
    // -------------------------------------------------------------------------
    [Fact]
    public async Task GetById_ExistingUser_Returns200WithCorrectData()
    {
        // Arrange
        var userId = await GetUserIdByEmailAsync("alex@email.com");

        // Act
        var response = await _fixture.Client.GetAsync($"/api/usuario/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await DeserializeAsync<UsuarioResponse>(response);
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(userId);
        dto.Nome.Should().Be("Alexsander Barbosa");
        dto.Email.Should().Be("alex@email.com");
        dto.Idade.Should().Be(21);
    }

    // -------------------------------------------------------------------------
    // GET /api/usuario/{id} — ID inexistente → 404
    // -------------------------------------------------------------------------
    [Fact]
    public async Task GetById_NonExistentUser_Returns404()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _fixture.Client.GetAsync($"/api/usuario/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // -------------------------------------------------------------------------
    // PUT /api/usuario/{id} sem CEP → 200, nome/idade atualizados, ViaCEP não chamado
    // -------------------------------------------------------------------------
    [Fact]
    public async Task Update_WithoutCep_Returns200AndDoesNotCallViaCep()
    {
        // Arrange
        _fixture.ViaCepServer.Reset();
        var userId = await CreateTestUserAsync("Sem Cep Original", "semcep@test.com", 20);
        var request = new UsuarioUpdateRequest { Nome = "Sem Cep Atualizado", Idade = 28 };

        // Act
        var response = await _fixture.Client.PutAsJsonAsync($"/api/usuario/{userId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await DeserializeAsync<UsuarioResponse>(response);
        dto!.Nome.Should().Be("Sem Cep Atualizado");
        dto.Idade.Should().Be(28);
        _fixture.ViaCepServer.LogEntries.Should().BeEmpty();
    }

    // -------------------------------------------------------------------------
    // PUT /api/usuario/{id} com CEP válido → WireMock retorna endereço → 200 com endereço preenchido
    // -------------------------------------------------------------------------
    [Fact]
    public async Task Update_WithValidCep_CallsViaCepAndReturns200WithAddress()
    {
        // Arrange
        _fixture.ViaCepServer.Reset();
        _fixture.ViaCepServer
            .Given(Request.Create().WithPath("/ws/01310100/json/").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""
                    {
                        "cep": "01310-100",
                        "logradouro": "Praça da Sé",
                        "complemento": "lado ímpar",
                        "bairro": "Sé",
                        "localidade": "São Paulo",
                        "uf": "SP"
                    }
                    """));

        var userId = await CreateTestUserAsync("Cep Valido User", "cepvalido@test.com", 25);
        var request = new UsuarioUpdateRequest { Nome = "Cep Valido User", Idade = 25, Cep = "01310100", Numero = "1" };

        // Act
        var response = await _fixture.Client.PutAsJsonAsync($"/api/usuario/{userId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await DeserializeAsync<UsuarioResponse>(response);
        dto!.Cep.Should().Be("01310100");
        dto.Logradouro.Should().Be("Praça da Sé");
        dto.Bairro.Should().Be("Sé");
        dto.Cidade.Should().Be("São Paulo");
    }

    // -------------------------------------------------------------------------
    // PUT /api/usuario/{id} com CEP inválido → WireMock retorna {erro: true} → 400
    // -------------------------------------------------------------------------
    [Fact]
    public async Task Update_WithInvalidCep_Returns400()
    {
        // Arrange
        _fixture.ViaCepServer.Reset();
        _fixture.ViaCepServer
            .Given(Request.Create().WithPath("/ws/00000000/json/").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(@"{""erro"": true}"));

        var userId = await CreateTestUserAsync("Cep Invalido User", "cepinvalido@test.com", 25);
        var request = new UsuarioUpdateRequest { Nome = "Cep Invalido User", Idade = 25, Cep = "00000000" };

        // Act
        var response = await _fixture.Client.PutAsJsonAsync($"/api/usuario/{userId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private async Task<Guid> GetUserIdByEmailAsync(string email)
    {
        await using var conn = new NpgsqlConnection(_fixture.ConnectionString);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            "SELECT id FROM usuarios WHERE email = @email AND deleted_at IS NULL", conn);
        cmd.Parameters.AddWithValue("@email", email);
        var result = await cmd.ExecuteScalarAsync();
        return (Guid)result!;
    }

    private async Task<Guid> CreateTestUserAsync(string nome, string email, int idade)
    {
        await using var conn = new NpgsqlConnection(_fixture.ConnectionString);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(@"
            INSERT INTO usuarios(nome, email, senha_hash, idade, consentimento_aceito)
            VALUES (@nome, @email, 'testhash', @idade, true)
            ON CONFLICT(email) DO UPDATE SET nome = EXCLUDED.nome
            RETURNING id", conn);
        cmd.Parameters.AddWithValue("@nome", nome);
        cmd.Parameters.AddWithValue("@email", email);
        cmd.Parameters.AddWithValue("@idade", idade);
        var result = await cmd.ExecuteScalarAsync();
        return (Guid)result!;
    }

    private static async Task<T?> DeserializeAsync<T>(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, JsonOptions);
    }
}
