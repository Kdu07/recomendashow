using BackEnd.Aventura.ApiTests.Fixtures;
using BackEnd.Aventura.ApiTests.Helpers;
using BackEnd.Aventura.Infrastructure;
using BackEnd.Aventura.Repository;
using FluentAssertions;
using Xunit;

namespace BackEnd.Aventura.IntegrationTests;

/// <summary>
/// Exercita o SQL real das queries que sustentam o uso da wishlist no motor de recomendações:
/// EspetaculoRepository.GetByGenerosDaWishlist (Fase 0/2) e
/// WishlistRepository.GetGenerosDaWishlist (Fase 3). Os testes de serviço usam fakes em memória,
/// então estes garantem que os JOINs/UNION/exclusão executam de fato no Postgres.
/// </summary>
[Collection("Database")]
public class WishlistRecommendationQueriesTests
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly DatabaseFixture _fixture;

    public WishlistRecommendationQueriesTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _connectionFactory = new TestDbConnectionFactory(fixture.ConnectionString);
    }

    [Fact(DisplayName = "GetByGenerosDaWishlist recomenda itens do mesmo gênero e exclui os já salvos")]
    public async Task GetByGenerosDaWishlist_RecomendaMesmoGenero_ExcluiSalvos()
    {
        var usuarioId = Guid.NewGuid();
        await using (var conn = new Npgsql.NpgsqlConnection(_fixture.ConnectionString))
        {
            await conn.OpenAsync();
            await DbSeedHelper.InsertUsuarioAsync(conn, usuarioId);

            var generoComedia = await DbSeedHelper.InsertGeneroAsync(conn, "Comédia");
            var generoDrama   = await DbSeedHelper.InsertGeneroAsync(conn, "Drama");

            // Item salvo na wishlist (gênero Comédia) — deve ser EXCLUÍDO do resultado.
            var salvo = await DbSeedHelper.InsertEspetaculoAsync(conn, "Salvo Comédia");
            await DbSeedHelper.LinkEspetaculoGeneroAsync(conn, salvo, generoComedia);
            await DbSeedHelper.InsertWishlistAsync(conn, usuarioId, salvo);

            // Candidato do mesmo gênero — deve aparecer.
            var candidato = await DbSeedHelper.InsertEspetaculoAsync(conn, "Candidato Comédia");
            await DbSeedHelper.LinkEspetaculoGeneroAsync(conn, candidato, generoComedia);

            // Candidato de outro gênero — não deve aparecer.
            var outro = await DbSeedHelper.InsertEspetaculoAsync(conn, "Outro Drama");
            await DbSeedHelper.LinkEspetaculoGeneroAsync(conn, outro, generoDrama);
        }

        var repo = new EspetaculoRepository(_connectionFactory);
        var resultado = repo.GetByGenerosDaWishlist(usuarioId, 10);

        var nomes = resultado.Select(e => e.Nome).ToList();
        nomes.Should().Contain("Candidato Comédia");
        nomes.Should().NotContain("Salvo Comédia", "o item já salvo deve ser excluído");
        nomes.Should().NotContain("Outro Drama", "gênero diferente do da wishlist");
    }

    [Fact(DisplayName = "GetByGenerosDaWishlist retorna vazio quando a wishlist está vazia")]
    public async Task GetByGenerosDaWishlist_WishlistVazia_RetornaVazio()
    {
        var usuarioId = Guid.NewGuid();
        await using (var conn = new Npgsql.NpgsqlConnection(_fixture.ConnectionString))
        {
            await conn.OpenAsync();
            await DbSeedHelper.InsertUsuarioAsync(conn, usuarioId);
            var genero = await DbSeedHelper.InsertGeneroAsync(conn, "Drama");
            var esp = await DbSeedHelper.InsertEspetaculoAsync(conn, "Qualquer");
            await DbSeedHelper.LinkEspetaculoGeneroAsync(conn, esp, genero);
        }

        var repo = new EspetaculoRepository(_connectionFactory);
        repo.GetByGenerosDaWishlist(usuarioId, 10).Should().BeEmpty();
    }

    [Fact(DisplayName = "GetGenerosDaWishlist retorna gênero nomeado e tipo_evento como fallback")]
    public async Task GetGenerosDaWishlist_RetornaGeneroNomeado_EFallbackTipoEvento()
    {
        var usuarioId = Guid.NewGuid();
        await using (var conn = new Npgsql.NpgsqlConnection(_fixture.ConnectionString))
        {
            await conn.OpenAsync();
            await DbSeedHelper.InsertUsuarioAsync(conn, usuarioId);

            var genero = await DbSeedHelper.InsertGeneroAsync(conn, "Comédia");

            // Item com gênero nomeado.
            var comGenero = await DbSeedHelper.InsertEspetaculoAsync(conn, "Com Gênero");
            await DbSeedHelper.LinkEspetaculoGeneroAsync(conn, comGenero, genero);
            await DbSeedHelper.InsertWishlistAsync(conn, usuarioId, comGenero);

            // Item sem gênero cadastrado — deve cair no tipo_evento ('Teatro' no seed helper).
            var semGenero = await DbSeedHelper.InsertEspetaculoAsync(conn, "Sem Gênero");
            await DbSeedHelper.InsertWishlistAsync(conn, usuarioId, semGenero);
        }

        var repo = new WishlistRepository(_connectionFactory);
        var generos = repo.GetGenerosDaWishlist(usuarioId);

        generos.Should().Contain("Comédia");
        generos.Should().Contain("Teatro", "tipo_evento é o fallback quando não há gênero cadastrado");
    }
}
