using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BackEnd.Aventura.ApiTests.Fixtures;
using BackEnd.Aventura.Dto;
using FluentAssertions;
using Npgsql;
using Xunit;

namespace BackEnd.Aventura.ApiTests.E2E;

[Collection("Api")]
[Trait("Category", "E2E")]
public class EndToEndApplicationTests : IAsyncLifetime
{
    private readonly ApiFixture _api;

    public EndToEndApplicationTests(ApiFixture api) => _api = api;

    public async Task InitializeAsync() => await _api.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact(DisplayName = "E2E-01: usuário novo completa onboarding, descobre espetáculo, favorita e segue para bilheteria")]
    public async Task NewUser_DiscoveryJourney_CompletesCoreProductFlow()
    {
        await using var conn = _api.CreateConnection();
        await conn.OpenAsync();

        var generoMusical = await InsertGeneroAsync(conn, "Musical");
        var teatroId = await InsertTeatroAsync(conn, "Teatro Renault", "São Paulo");
        var espetaculoId = await InsertEspetaculoAsync(
            conn,
            nome: "A Noviça Rebelde",
            tipoEvento: "Musical",
            urlBilheteria: "https://ticketera.com/novica-rebelde",
            preco: 120m);
        await LinkEspetaculoGeneroAsync(conn, espetaculoId, generoMusical);
        await LinkTeatroEspetaculoAsync(conn, teatroId, espetaculoId);

        var auth = await RegisterAsync("Ana Luiza", "ana.e2e@aventura.test", "Senha@123");
        UseToken(auth.Token);

        var profile = await _api.HttpClient.GetFromJsonAsync<UsuarioResponse>($"/api/usuario/{auth.UsuarioId}");
        profile.Should().NotBeNull();
        profile!.Nome.Should().Be("Ana Luiza");
        profile.Cidade.Should().Be("São Paulo");

        var preferenciasResponse = await _api.HttpClient.PutAsJsonAsync(
            $"/api/preferencias/{auth.UsuarioId}",
            new PreferenciasUpdateDto
            {
                Cep = "01310-100",
                Cidade = "São Paulo",
                Generos = ["Musical"],
                Tags = ["família", "clássico"],
                FaixaPrecoMin = 50m,
                FaixaPrecoMax = 200m,
                ReceberNotificacoes = true
            });
        preferenciasResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var preferencias = await preferenciasResponse.Content.ReadFromJsonAsync<PreferenciasDto>();
        preferencias!.Cidade.Should().Be("São Paulo");
        preferencias.Generos.Should().Contain("Musical");
        preferencias.Tags.Should().Contain("família");

        var generos = await _api.HttpClient.GetFromJsonAsync<List<string>>("/api/genero");
        generos.Should().Contain("Musical");

        var catalogo = await _api.HttpClient.GetFromJsonAsync<List<EspetaculoDto>>("/api/espetaculo");
        catalogo.Should().Contain(e => e.Id == espetaculoId);

        var busca = await _api.HttpClient.GetFromJsonAsync<List<EspetaculoDto>>("/api/espetaculo/search?nome=novi%C3%A7a&genero=Musical");
        busca.Should().ContainSingle(e => e.Id == espetaculoId);

        var detalhe = await _api.HttpClient.GetFromJsonAsync<EspetaculoDto>($"/api/espetaculo/{espetaculoId}");
        detalhe!.UrlBilheteria.Should().Be("https://ticketera.com/novica-rebelde");
        detalhe.EmCartaz.Should().BeTrue();

        var teatro = await _api.HttpClient.GetFromJsonAsync<TeatroDto>($"/api/teatro/{teatroId}");
        teatro!.Nome.Should().Be("Teatro Renault");

        var espetaculosDoTeatro = await _api.HttpClient.GetFromJsonAsync<List<EspetaculoDto>>($"/api/teatro/{teatroId}/espetaculos");
        espetaculosDoTeatro.Should().Contain(e => e.Id == espetaculoId);

        var addWishlist = await _api.HttpClient.PostAsync($"/api/wishlist/{auth.UsuarioId}/{espetaculoId}", null);
        addWishlist.StatusCode.Should().Be(HttpStatusCode.Created);

        var contains = await _api.HttpClient.GetFromJsonAsync<bool>($"/api/wishlist/{auth.UsuarioId}/contains/{espetaculoId}");
        contains.Should().BeTrue();

        var wishlist = await _api.HttpClient.GetFromJsonAsync<List<EspetaculoDto>>($"/api/wishlist/{auth.UsuarioId}");
        wishlist.Should().ContainSingle(e => e.Id == espetaculoId);
    }

    [Fact(DisplayName = "E2E-02: usuário recorrente recebe recomendações persistidas e remove item da wishlist")]
    public async Task ReturningUser_RecommendationJourney_UsesStoredRecommendationsAndUpdatesWishlist()
    {
        await using var conn = _api.CreateConnection();
        await conn.OpenAsync();

        var generoComedia = await InsertGeneroAsync(conn, "Comédia");
        var teatroId = await InsertTeatroAsync(conn, "Teatro Riachuelo", "Rio de Janeiro");
        var altaRelevancia = await InsertEspetaculoAsync(conn, "Comédia em Pé", "Stand-Up", "https://ticketera.com/comedia", 90m);
        var baixaRelevancia = await InsertEspetaculoAsync(conn, "Improviso da Noite", "Stand-Up", "https://ticketera.com/improviso", 70m);
        await LinkEspetaculoGeneroAsync(conn, altaRelevancia, generoComedia);
        await LinkEspetaculoGeneroAsync(conn, baixaRelevancia, generoComedia);
        await LinkTeatroEspetaculoAsync(conn, teatroId, altaRelevancia);
        await LinkTeatroEspetaculoAsync(conn, teatroId, baixaRelevancia);

        var auth = await RegisterAsync("Marcos Abrantes", "marcos.e2e@aventura.test", "Senha@123");
        UseToken(auth.Token);

        await InsertRecomendacaoAsync(conn, auth.UsuarioId, altaRelevancia, 0.96m, "Alta afinidade com Stand-Up");
        await InsertRecomendacaoAsync(conn, auth.UsuarioId, baixaRelevancia, 0.62m, "Também combina com seu histórico");

        var recomendacoes = await _api.HttpClient.GetFromJsonAsync<List<RecomendacaoItemDto>>(
            $"/api/recomendacao/{auth.UsuarioId}?limit=2");

        recomendacoes.Should().HaveCount(2);
        recomendacoes![0].Espetaculo.Id.Should().Be(altaRelevancia);
        recomendacoes[0].Score.Should().BeGreaterThan(recomendacoes[1].Score);
        recomendacoes[0].Motivo.Should().Contain("Stand-Up");

        var addWishlist = await _api.HttpClient.PostAsync($"/api/wishlist/{auth.UsuarioId}/{altaRelevancia}", null);
        addWishlist.StatusCode.Should().Be(HttpStatusCode.Created);

        var duplicate = await _api.HttpClient.PostAsync($"/api/wishlist/{auth.UsuarioId}/{altaRelevancia}", null);
        duplicate.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var remove = await _api.HttpClient.DeleteAsync($"/api/wishlist/{auth.UsuarioId}/{altaRelevancia}");
        remove.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var contains = await _api.HttpClient.GetFromJsonAsync<bool>($"/api/wishlist/{auth.UsuarioId}/contains/{altaRelevancia}");
        contains.Should().BeFalse();
    }

    [Fact(DisplayName = "E2E-03: autenticação protege recursos, perfil é atualizado e conta encerrada bloqueia novo login")]
    public async Task AccountSecurityJourney_ProtectsResourcesAndBlocksDeletedAccount()
    {
        var auth = await RegisterAsync("Carla Segurança", "carla.e2e@aventura.test", "Senha@123");

        using var anonymousClient = _api.CreateClient();
        var anonymousCatalog = await anonymousClient.GetAsync("/api/espetaculo");
        anonymousCatalog.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        UseToken(auth.Token);

        var update = await _api.HttpClient.PutAsJsonAsync(
            $"/api/usuario/{auth.UsuarioId}",
            new UsuarioUpdateRequest
            {
                Nome = "Carla Segurança Atualizada",
                Email = "carla.atualizada@aventura.test",
                Idade = 34,
                Cep = "22040-002",
                Logradouro = "Avenida Atlântica",
                Numero = "1000",
                Bairro = "Copacabana",
                Cidade = "Rio de Janeiro"
            });

        update.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await update.Content.ReadFromJsonAsync<UsuarioResponse>();
        updated!.Nome.Should().Be("Carla Segurança Atualizada");
        updated.Cidade.Should().Be("Rio de Janeiro");

        var delete = await _api.HttpClient.DeleteAsync($"/api/usuario/{auth.UsuarioId}");
        delete.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var loginAfterDelete = await _api.HttpClient.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest { Email = "carla.atualizada@aventura.test", Senha = "Senha@123" });
        loginAfterDelete.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact(DisplayName = "E2E-04: cadastro e login rejeitam consentimento ausente, menor de idade, duplicidade e senha incorreta")]
    public async Task AuthenticationValidationJourney_RejectsInvalidIdentityFlows()
    {
        var withoutConsent = await _api.HttpClient.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest
            {
                Nome = "Sem Consentimento",
                Email = "sem.consentimento@aventura.test",
                Senha = "Senha@123",
                Idade = 25,
                ConsentimentoAceito = false
            });
        withoutConsent.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var underAge = await _api.HttpClient.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest
            {
                Nome = "Menor Idade",
                Email = "menor@aventura.test",
                Senha = "Senha@123",
                Idade = 12,
                ConsentimentoAceito = true
            });
        underAge.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await RegisterAsync("Usuário Duplicado", "duplicado@aventura.test", "Senha@123");

        var duplicate = await _api.HttpClient.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest
            {
                Nome = "Usuário Duplicado 2",
                Email = "duplicado@aventura.test",
                Senha = "Senha@123",
                Idade = 28,
                ConsentimentoAceito = true
            });
        duplicate.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var wrongPassword = await _api.HttpClient.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest { Email = "duplicado@aventura.test", Senha = "SenhaErrada" });
        wrongPassword.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var wrongPasswordBody = await wrongPassword.Content.ReadAsStringAsync();
        wrongPasswordBody.Should().NotContain("StackTrace");
        wrongPasswordBody.Should().NotContain("senha_hash");
    }

    [Fact(DisplayName = "E2E-05: catálogo, busca e detalhe tratam filtros, vazio e recurso inexistente")]
    public async Task CatalogSearchJourney_HandlesFiltersEmptyResultsAndMissingResources()
    {
        await using var conn = _api.CreateConnection();
        await conn.OpenAsync();

        var musical = await InsertGeneroAsync(conn, "Musical");
        var drama = await InsertGeneroAsync(conn, "Drama");
        var musicalId = await InsertEspetaculoAsync(conn, "Mamma Mia E2E", "Musical", "https://ticketera.com/mamma-e2e", 150m);
        var dramaId = await InsertEspetaculoAsync(conn, "Hamlet E2E", "Drama", "https://ticketera.com/hamlet-e2e", 80m);
        await LinkEspetaculoGeneroAsync(conn, musicalId, musical);
        await LinkEspetaculoGeneroAsync(conn, dramaId, drama);

        var auth = await RegisterAsync("Catálogo E2E", "catalogo.e2e@aventura.test", "Senha@123");
        UseToken(auth.Token);

        var all = await _api.HttpClient.GetFromJsonAsync<List<EspetaculoDto>>("/api/espetaculo");
        all.Should().Contain(e => e.Id == musicalId);
        all.Should().Contain(e => e.Id == dramaId);

        var onlyMusicals = await _api.HttpClient.GetFromJsonAsync<List<EspetaculoDto>>("/api/espetaculo/search?genero=Musical");
        onlyMusicals.Should().ContainSingle(e => e.Id == musicalId);
        onlyMusicals.Should().NotContain(e => e.Id == dramaId);

        var byName = await _api.HttpClient.GetFromJsonAsync<List<EspetaculoDto>>("/api/espetaculo/search?nome=hamlet");
        byName.Should().ContainSingle(e => e.Id == dramaId);

        var empty = await _api.HttpClient.GetFromJsonAsync<List<EspetaculoDto>>("/api/espetaculo/search?nome=nao-existe");
        empty.Should().BeEmpty();

        var missingShow = await _api.HttpClient.GetAsync($"/api/espetaculo/{Guid.NewGuid()}");
        missingShow.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var missingTheater = await _api.HttpClient.GetAsync($"/api/teatro/{Guid.NewGuid()}");
        missingTheater.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "E2E-06: recomendação usa fallback por gêneros favoritos quando não há recomendação persistida")]
    public async Task RecommendationFallbackJourney_UsesFavoriteGenresWhenStoredRecommendationsAreUnavailable()
    {
        await using var conn = _api.CreateConnection();
        await conn.OpenAsync();

        var musical = await InsertGeneroAsync(conn, "Musical");
        var first = await InsertEspetaculoAsync(conn, "Musical Favorito 1", "Musical", "https://ticketera.com/fav-1", 110m);
        var second = await InsertEspetaculoAsync(conn, "Musical Favorito 2", "Musical", "https://ticketera.com/fav-2", 95m);
        await LinkEspetaculoGeneroAsync(conn, first, musical);
        await LinkEspetaculoGeneroAsync(conn, second, musical);

        var auth = await RegisterAsync("Fallback E2E", "fallback.e2e@aventura.test", "Senha@123");
        UseToken(auth.Token);

        var preferencias = await _api.HttpClient.PutAsJsonAsync(
            $"/api/preferencias/{auth.UsuarioId}",
            new PreferenciasUpdateDto { Generos = ["Musical"], Cidade = "São Paulo" });
        preferencias.StatusCode.Should().Be(HttpStatusCode.OK);

        var recommendations = await _api.HttpClient.GetFromJsonAsync<List<RecomendacaoItemDto>>(
            $"/api/recomendacao/{auth.UsuarioId}?limit=5");

        recommendations.Should().HaveCountGreaterOrEqualTo(2);
        recommendations.Should().Contain(r => r.Espetaculo.Id == first);
        recommendations.Should().Contain(r => r.Espetaculo.Id == second);
        recommendations!.Select(r => r.Motivo).Should().OnlyContain(m => m == "Baseado nos seus gêneros favoritos");
    }

    [Fact(DisplayName = "E2E-07: recomendação valida limites e retorna vazio quando não há dados nem preferências")]
    public async Task RecommendationValidationJourney_RejectsInvalidLimitsAndSupportsEmptyState()
    {
        var auth = await RegisterAsync("Recomendação Vazia", "recomendacao.vazia@aventura.test", "Senha@123");
        UseToken(auth.Token);

        var empty = await _api.HttpClient.GetFromJsonAsync<List<RecomendacaoItemDto>>(
            $"/api/recomendacao/{auth.UsuarioId}?limit=10");
        empty.Should().BeEmpty();

        var zero = await _api.HttpClient.GetAsync($"/api/recomendacao/{auth.UsuarioId}?limit=0");
        zero.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var overLimit = await _api.HttpClient.GetAsync($"/api/recomendacao/{auth.UsuarioId}?limit=51");
        overLimit.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        using var anonymousClient = _api.CreateClient();
        var anonymous = await anonymousClient.GetAsync($"/api/recomendacao/{auth.UsuarioId}");
        anonymous.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact(DisplayName = "E2E-08: wishlist cobre lista vazia, item inexistente, duplicidade e remoção inexistente")]
    public async Task WishlistValidationJourney_CoversEmptyStateConflictsAndNotFound()
    {
        await using var conn = _api.CreateConnection();
        await conn.OpenAsync();

        var showId = await InsertEspetaculoAsync(conn, "Wishlist E2E", "Drama", "https://ticketera.com/wishlist-e2e", 75m);
        var auth = await RegisterAsync("Wishlist E2E", "wishlist.e2e@aventura.test", "Senha@123");
        UseToken(auth.Token);

        var initial = await _api.HttpClient.GetFromJsonAsync<List<EspetaculoDto>>($"/api/wishlist/{auth.UsuarioId}");
        initial.Should().BeEmpty();

        var missingAdd = await _api.HttpClient.PostAsync($"/api/wishlist/{auth.UsuarioId}/{Guid.NewGuid()}", null);
        missingAdd.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var add = await _api.HttpClient.PostAsync($"/api/wishlist/{auth.UsuarioId}/{showId}", null);
        add.StatusCode.Should().Be(HttpStatusCode.Created);

        var duplicate = await _api.HttpClient.PostAsync($"/api/wishlist/{auth.UsuarioId}/{showId}", null);
        duplicate.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var remove = await _api.HttpClient.DeleteAsync($"/api/wishlist/{auth.UsuarioId}/{showId}");
        remove.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var removeAgain = await _api.HttpClient.DeleteAsync($"/api/wishlist/{auth.UsuarioId}/{showId}");
        removeAgain.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "E2E-09: preferências suportam criação automática, atualização parcial e validação de faixa de preço")]
    public async Task PreferencesJourney_CreatesUpdatesAndValidatesUserPreferences()
    {
        await using var conn = _api.CreateConnection();
        await conn.OpenAsync();
        await InsertGeneroAsync(conn, "Drama");
        await InsertGeneroAsync(conn, "Ballet");

        var auth = await RegisterAsync("Preferências E2E", "preferencias.e2e@aventura.test", "Senha@123");
        UseToken(auth.Token);

        var initial = await _api.HttpClient.GetFromJsonAsync<PreferenciasDto>($"/api/preferencias/{auth.UsuarioId}");
        initial.Should().NotBeNull();
        initial!.UsuarioId.Should().Be(auth.UsuarioId);
        initial.Generos.Should().BeEmpty();
        initial.ReceberNotificacoes.Should().BeTrue();

        var firstUpdate = await _api.HttpClient.PutAsJsonAsync(
            $"/api/preferencias/{auth.UsuarioId}",
            new PreferenciasUpdateDto
            {
                Generos = ["Drama", "Ballet"],
                Tags = ["estreia", "fim-de-semana"],
                Cep = "20040-020",
                Cidade = "Rio de Janeiro",
                FaixaPrecoMin = 40m,
                FaixaPrecoMax = 180m,
                ReceberNotificacoes = false
            });
        firstUpdate.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await firstUpdate.Content.ReadFromJsonAsync<PreferenciasDto>();
        updated!.Generos.Should().BeEquivalentTo(["Drama", "Ballet"]);
        updated.Tags.Should().BeEquivalentTo(["estreia", "fim-de-semana"]);
        updated.Cidade.Should().Be("Rio de Janeiro");
        updated.ReceberNotificacoes.Should().BeFalse();

        var partialUpdate = await _api.HttpClient.PutAsJsonAsync(
            $"/api/preferencias/{auth.UsuarioId}",
            new PreferenciasUpdateDto { ReceberNotificacoes = true });
        partialUpdate.StatusCode.Should().Be(HttpStatusCode.OK);

        var afterPartial = await partialUpdate.Content.ReadFromJsonAsync<PreferenciasDto>();
        afterPartial!.Generos.Should().BeEquivalentTo(["Drama", "Ballet"]);
        afterPartial.ReceberNotificacoes.Should().BeTrue();

        var invalidRange = await _api.HttpClient.PutAsJsonAsync(
            $"/api/preferencias/{auth.UsuarioId}",
            new PreferenciasUpdateDto { FaixaPrecoMin = 200m, FaixaPrecoMax = 50m });
        invalidRange.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "E2E-10: health check é público e endpoints de negócio continuam protegidos")]
    public async Task OperationalReadinessJourney_ExposesPublicHealthAndProtectsBusinessEndpoints()
    {
        using var anonymousClient = _api.CreateClient();

        var health = await anonymousClient.GetAsync("/health");
        health.StatusCode.Should().Be(HttpStatusCode.OK);

        var catalog = await anonymousClient.GetAsync("/api/espetaculo");
        catalog.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var genres = await anonymousClient.GetAsync("/api/genero");
        genres.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var theaters = await anonymousClient.GetAsync("/api/teatro");
        theaters.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact(DisplayName = "E2E-11: dados de recomendações e wishlist permanecem isolados entre usuários")]
    public async Task MultiUserIsolationJourney_DoesNotLeakRecommendationsOrWishlistAcrossAccounts()
    {
        await using var conn = _api.CreateConnection();
        await conn.OpenAsync();

        var showA = await InsertEspetaculoAsync(conn, "Show Usuário A", "Musical", "https://ticketera.com/a", 100m);
        var showB = await InsertEspetaculoAsync(conn, "Show Usuário B", "Drama", "https://ticketera.com/b", 80m);

        var userA = await RegisterAsync("Usuário A", "usuario.a@aventura.test", "Senha@123");
        var userB = await RegisterAsync("Usuário B", "usuario.b@aventura.test", "Senha@123");

        await InsertRecomendacaoAsync(conn, userA.UsuarioId, showA, 0.91m, "Recomendação exclusiva A");
        await InsertRecomendacaoAsync(conn, userB.UsuarioId, showB, 0.88m, "Recomendação exclusiva B");

        UseToken(userA.Token);
        var addA = await _api.HttpClient.PostAsync($"/api/wishlist/{userA.UsuarioId}/{showA}", null);
        addA.StatusCode.Should().Be(HttpStatusCode.Created);

        var recsA = await _api.HttpClient.GetFromJsonAsync<List<RecomendacaoItemDto>>($"/api/recomendacao/{userA.UsuarioId}");
        recsA.Should().ContainSingle(r => r.Espetaculo.Id == showA);
        recsA.Should().NotContain(r => r.Espetaculo.Id == showB);

        var wishlistA = await _api.HttpClient.GetFromJsonAsync<List<EspetaculoDto>>($"/api/wishlist/{userA.UsuarioId}");
        wishlistA.Should().ContainSingle(e => e.Id == showA);

        UseToken(userB.Token);
        var recsB = await _api.HttpClient.GetFromJsonAsync<List<RecomendacaoItemDto>>($"/api/recomendacao/{userB.UsuarioId}");
        recsB.Should().ContainSingle(r => r.Espetaculo.Id == showB);
        recsB.Should().NotContain(r => r.Espetaculo.Id == showA);

        var wishlistB = await _api.HttpClient.GetFromJsonAsync<List<EspetaculoDto>>($"/api/wishlist/{userB.UsuarioId}");
        wishlistB.Should().BeEmpty();
    }

    private async Task<AuthResponse> RegisterAsync(string nome, string email, string senha)
    {
        var response = await _api.HttpClient.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest
            {
                Nome = nome,
                Email = email,
                Senha = senha,
                Idade = 29,
                Cep = "01310-100",
                Logradouro = "Avenida Paulista",
                Numero = "1000",
                Bairro = "Bela Vista",
                Cidade = "São Paulo",
                ConsentimentoAceito = true
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        auth.Should().NotBeNull();
        auth!.Token.Should().NotBeNullOrWhiteSpace();
        auth.UsuarioId.Should().NotBeEmpty();
        return auth;
    }

    private void UseToken(string token)
    {
        _api.HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    private static async Task<Guid> InsertGeneroAsync(NpgsqlConnection conn, string nome)
    {
        var id = Guid.NewGuid();
        const string sql = @"
INSERT INTO generos (id, nome)
VALUES (@Id, @Nome)
ON CONFLICT (nome) DO UPDATE SET nome = EXCLUDED.nome
RETURNING id";
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@Nome", nome);
        return (Guid)(await cmd.ExecuteScalarAsync())!;
    }

    private static async Task<Guid> InsertTeatroAsync(NpgsqlConnection conn, string nome, string cidade)
    {
        var id = Guid.NewGuid();
        const string sql = @"
INSERT INTO teatros (id, nome, endereco, cidade, horario_funcionamento)
VALUES (@Id, @Nome, 'Rua de Teste, 100', @Cidade, '10h às 22h')
ON CONFLICT (nome) DO UPDATE
SET cidade = EXCLUDED.cidade
RETURNING id";
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@Nome", nome);
        cmd.Parameters.AddWithValue("@Cidade", cidade);
        return (Guid)(await cmd.ExecuteScalarAsync())!;
    }

    private static async Task<Guid> InsertEspetaculoAsync(
        NpgsqlConnection conn,
        string nome,
        string tipoEvento,
        string urlBilheteria,
        decimal preco)
    {
        var id = Guid.NewGuid();
        const string sql = @"
INSERT INTO espetaculos (id, nome, tipo_evento, descricao, url_bilheteria,
                         data_inicio, data_fim, valor_ingresso)
VALUES (@Id, @Nome, @TipoEvento, 'Descrição E2E', @Url,
        NOW() - INTERVAL '1 day', NOW() + INTERVAL '30 days', @Preco)";
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@Nome", nome);
        cmd.Parameters.AddWithValue("@TipoEvento", tipoEvento);
        cmd.Parameters.AddWithValue("@Url", urlBilheteria);
        cmd.Parameters.AddWithValue("@Preco", preco);
        await cmd.ExecuteNonQueryAsync();
        return id;
    }

    private static async Task LinkEspetaculoGeneroAsync(NpgsqlConnection conn, Guid espetaculoId, Guid generoId)
    {
        const string sql = @"
INSERT INTO espetaculo_genero (espetaculo_id, genero_id)
VALUES (@EspetaculoId, @GeneroId)
ON CONFLICT DO NOTHING";
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@EspetaculoId", espetaculoId);
        cmd.Parameters.AddWithValue("@GeneroId", generoId);
        await cmd.ExecuteNonQueryAsync();
    }

    private static async Task LinkTeatroEspetaculoAsync(NpgsqlConnection conn, Guid teatroId, Guid espetaculoId)
    {
        const string sql = @"
INSERT INTO teatro_espetaculo (teatro_id, espetaculo_id)
VALUES (@TeatroId, @EspetaculoId)
ON CONFLICT DO NOTHING";
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@TeatroId", teatroId);
        cmd.Parameters.AddWithValue("@EspetaculoId", espetaculoId);
        await cmd.ExecuteNonQueryAsync();
    }

    private static async Task InsertRecomendacaoAsync(
        NpgsqlConnection conn,
        Guid usuarioId,
        Guid espetaculoId,
        decimal score,
        string motivo)
    {
        const string sql = @"
INSERT INTO recomendacoes (usuario_id, espetaculo_id, score, motivo)
VALUES (@UsuarioId, @EspetaculoId, @Score, @Motivo)";
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
        cmd.Parameters.AddWithValue("@EspetaculoId", espetaculoId);
        cmd.Parameters.AddWithValue("@Score", score);
        cmd.Parameters.AddWithValue("@Motivo", motivo);
        await cmd.ExecuteNonQueryAsync();
    }
}
