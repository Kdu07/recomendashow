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
    // 1. GET /api/usuario/{id} — ID existente → 200 com dados corretos
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
    // 2. GET /api/usuario/{id} — ID inexistente → 404
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
    // 3. PUT sem CEP → 200, nome/idade atualizados, ViaCEP não chamado
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
    // 4. PUT com CEP válido → WireMock retorna endereço → 200 com endereço preenchido
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
    // 5. PUT com CEP inválido → WireMock retorna {erro: true} → 400
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
    // 6. GET de usuário com deleted_at preenchido → 404
    // -------------------------------------------------------------------------
    [Fact]
    public async Task GetById_DeletedUser_Returns404()
    {
        // Arrange
        var userId = await CreateDeletedUserAsync("Usuario Deletado", "deletado@test.com", 30);

        // Act
        var response = await _fixture.Client.GetAsync($"/api/usuario/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // -------------------------------------------------------------------------
    // 7. PUT só com nome, sem idade e sem CEP → 200, idade anterior mantida
    // -------------------------------------------------------------------------
    [Fact]
    public async Task Update_OnlyName_Returns200AndKeepsAge()
    {
        // Arrange
        _fixture.ViaCepServer.Reset();
        var userId = await CreateTestUserAsync("Nome Antigo", "somenome@test.com", 30);
        var request = new UsuarioUpdateRequest { Nome = "Nome Novo" }; // Idade = null, Cep = null

        // Act
        var response = await _fixture.Client.PutAsJsonAsync($"/api/usuario/{userId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await DeserializeAsync<UsuarioResponse>(response);
        dto!.Nome.Should().Be("Nome Novo");
        dto.Idade.Should().Be(30); // idade original preservada
    }

    // -------------------------------------------------------------------------
    // 8. PUT com nome e idade → GET logo depois → dados persistidos
    // -------------------------------------------------------------------------
    [Fact]
    public async Task Update_ThenGet_ConfirmsPersistence()
    {
        // Arrange
        var userId = await CreateTestUserAsync("Persistencia Original", "persistencia@test.com", 20);
        var request = new UsuarioUpdateRequest { Nome = "Persistencia Atualizado", Idade = 35 };

        // Act
        var putResponse = await _fixture.Client.PutAsJsonAsync($"/api/usuario/{userId}", request);
        var getResponse = await _fixture.Client.GetAsync($"/api/usuario/{userId}");

        // Assert
        putResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await DeserializeAsync<UsuarioResponse>(getResponse);
        dto!.Nome.Should().Be("Persistencia Atualizado");
        dto.Idade.Should().Be(35);
    }

    // -------------------------------------------------------------------------
    // 9. WireMock responde 503 → serviço retorna erro controlado (400)
    // -------------------------------------------------------------------------
    [Fact]
    public async Task Update_ViaCepReturns503_Returns400()
    {
        // Arrange
        _fixture.ViaCepServer.Reset();
        _fixture.ViaCepServer
            .Given(Request.Create().WithPath("/ws/01001000/json/").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(503));

        var userId = await CreateTestUserAsync("Servico Indisponivel", "unavailable@test.com", 25);
        var request = new UsuarioUpdateRequest { Nome = "Servico Indisponivel", Idade = 25, Cep = "01001000" };

        // Act
        var response = await _fixture.Client.PutAsJsonAsync($"/api/usuario/{userId}", request);

        // Assert — ViaCepService retorna null para HTTP não-2xx → ArgumentException → 400
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // -------------------------------------------------------------------------
    // 10. WireMock com delay de 10s → HttpClient timeout de 3s → retorna 400 antes de travar
    // -------------------------------------------------------------------------
    [Fact]
    public async Task Update_ViaCepTimeout_DoesNotHang()
    {
        // Arrange
        _fixture.ViaCepServer.Reset();
        _fixture.ViaCepServer
            .Given(Request.Create().WithPath("/ws/02002000/json/").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithDelay(TimeSpan.FromSeconds(10))); // maior que o timeout do HttpClient (3s)

        var userId = await CreateTestUserAsync("Timeout User", "timeout@test.com", 25);
        var request = new UsuarioUpdateRequest { Nome = "Timeout User", Idade = 25, Cep = "02002000" };

        // Act — HttpClient cancela após 3s; catch em BuscarAsync captura TaskCanceledException → null → 400
        var response = await _fixture.Client.PutAsJsonAsync($"/api/usuario/{userId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // -------------------------------------------------------------------------
    // 11. WireMock retorna logradouro e bairro vazios → salva sem erro → 200
    // -------------------------------------------------------------------------
    [Fact]
    public async Task Update_ViaCepReturnsEmptyFields_SavesWithoutError()
    {
        // Arrange
        _fixture.ViaCepServer.Reset();
        _fixture.ViaCepServer
            .Given(Request.Create().WithPath("/ws/03003000/json/").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""
                    {
                        "cep": "03003-000",
                        "logradouro": "",
                        "bairro": "",
                        "localidade": "",
                        "uf": "SP"
                    }
                    """));

        var userId = await CreateTestUserAsync("Empty Fields User", "emptyfields@test.com", 25);
        var request = new UsuarioUpdateRequest { Nome = "Empty Fields User", Idade = 25, Cep = "03003000" };

        // Act
        var response = await _fixture.Client.PutAsJsonAsync($"/api/usuario/{userId}", request);

        // Assert — strings vazias são não-nulas; AtualizarPerfil aceita sem lançar exceção
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // -------------------------------------------------------------------------
    // 12. PUT com CEP válido → GET → endereço retornado bate com o que WireMock devolveu
    // -------------------------------------------------------------------------
    [Fact]
    public async Task Update_WithValidCep_ThenGet_ConfirmsAddressPersisted()
    {
        // Arrange
        _fixture.ViaCepServer.Reset();
        _fixture.ViaCepServer
            .Given(Request.Create().WithPath("/ws/04004000/json/").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""
                    {
                        "cep": "04004-000",
                        "logradouro": "Rua Vergueiro",
                        "bairro": "Vila Mariana",
                        "localidade": "São Paulo",
                        "uf": "SP"
                    }
                    """));

        var userId = await CreateTestUserAsync("Persist Address User", "persistaddress@test.com", 25);
        var putRequest = new UsuarioUpdateRequest
        {
            Nome = "Persist Address User",
            Idade = 25,
            Cep = "04004000",
            Numero = "1500"
        };

        // Act
        await _fixture.Client.PutAsJsonAsync($"/api/usuario/{userId}", putRequest);
        var getResponse = await _fixture.Client.GetAsync($"/api/usuario/{userId}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await DeserializeAsync<UsuarioResponse>(getResponse);
        dto!.Cep.Should().Be("04004000");
        dto.Logradouro.Should().Be("Rua Vergueiro");
        dto.Bairro.Should().Be("Vila Mariana");
        dto.Cidade.Should().Be("São Paulo");
        dto.Numero.Should().Be("1500");
    }

    // -------------------------------------------------------------------------
    // 13. PUT com CEP A → PUT com CEP B → GET → endereço final é o do CEP B
    // -------------------------------------------------------------------------
    [Fact]
    public async Task Update_ChangeCep_OverwritesPreviousAddress()
    {
        // Arrange
        _fixture.ViaCepServer.Reset();
        _fixture.ViaCepServer
            .Given(Request.Create().WithPath("/ws/01001000/json/").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""
                    {
                        "cep": "01001-000",
                        "logradouro": "Praça da Sé",
                        "bairro": "Sé",
                        "localidade": "São Paulo",
                        "uf": "SP"
                    }
                    """));
        _fixture.ViaCepServer
            .Given(Request.Create().WithPath("/ws/20040020/json/").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""
                    {
                        "cep": "20040-020",
                        "logradouro": "Avenida Rio Branco",
                        "bairro": "Centro",
                        "localidade": "Rio de Janeiro",
                        "uf": "RJ"
                    }
                    """));

        var userId = await CreateTestUserAsync("Change Cep User", "changecep@test.com", 25);

        // Act — primeiro PUT com CEP A
        await _fixture.Client.PutAsJsonAsync($"/api/usuario/{userId}",
            new UsuarioUpdateRequest { Nome = "Change Cep User", Idade = 25, Cep = "01001000" });

        // segundo PUT com CEP B
        await _fixture.Client.PutAsJsonAsync($"/api/usuario/{userId}",
            new UsuarioUpdateRequest { Nome = "Change Cep User", Idade = 25, Cep = "20040020" });

        var getResponse = await _fixture.Client.GetAsync($"/api/usuario/{userId}");

        // Assert — endereço final deve refletir CEP B
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await DeserializeAsync<UsuarioResponse>(getResponse);
        dto!.Cep.Should().Be("20040020");
        dto.Logradouro.Should().Be("Avenida Rio Branco");
        dto.Bairro.Should().Be("Centro");
        dto.Cidade.Should().Be("Rio de Janeiro");
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

    private async Task<Guid> CreateDeletedUserAsync(string nome, string email, int idade)
    {
        await using var conn = new NpgsqlConnection(_fixture.ConnectionString);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(@"
            INSERT INTO usuarios(nome, email, senha_hash, idade, consentimento_aceito, deleted_at)
            VALUES (@nome, @email, 'testhash', @idade, true, NOW())
            ON CONFLICT(email) DO UPDATE SET deleted_at = NOW()
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
