using System.Net;
using System.Net.Http.Json;
using BackEnd.Aventura.Dto;
using BackEnd.Aventura.E2ETests.Fixtures;
using Xunit;

namespace BackEnd.Aventura.E2ETests
{
    public sealed class EspetaculoWishlistFluxoAlternativoE2ETests : IClassFixture<EspetaculoWishlistFixture>
    {
        private readonly EspetaculoWishlistFixture _fixture;

        public EspetaculoWishlistFluxoAlternativoE2ETests(EspetaculoWishlistFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task EspetaculoInexistenteNaoDeveEntrarNaWishlistERetornaErro()
        {
            await _fixture.ResetDatabaseAsync();
            using var client = _fixture.CreateClient();
            var espetaculoInexistenteId = Guid.Parse("99999999-9999-9999-9999-999999999999");

            var salvarResponse = await client.PostAsync(
                $"/api/Wishlist/{_fixture.UsuarioId}/{espetaculoInexistenteId}",
                content: null);

            Assert.Equal(HttpStatusCode.NotFound, salvarResponse.StatusCode);

            var wishlistResponse = await client.GetAsync($"/api/Wishlist/{_fixture.UsuarioId}");
            Assert.Equal(HttpStatusCode.OK, wishlistResponse.StatusCode);

            var wishlist = await wishlistResponse.Content.ReadFromJsonAsync<List<EspetaculoDto>>();
            Assert.NotNull(wishlist);
            Assert.Empty(wishlist!);
        }

        [Fact]
        public async Task EspetaculoDuplicadoNaoDeveDuplicarWishlistERetornaConflito()
        {
            await _fixture.ResetDatabaseAsync();
            using var client = _fixture.CreateClient();

            var primeiraResposta = await client.PostAsync(
                $"/api/Wishlist/{_fixture.UsuarioId}/{_fixture.EspetaculoId}",
                content: null);
            var segundaResposta = await client.PostAsync(
                $"/api/Wishlist/{_fixture.UsuarioId}/{_fixture.EspetaculoId}",
                content: null);

            Assert.Equal(HttpStatusCode.Created, primeiraResposta.StatusCode);
            Assert.Equal(HttpStatusCode.Conflict, segundaResposta.StatusCode);

            var wishlistResponse = await client.GetAsync($"/api/Wishlist/{_fixture.UsuarioId}");
            Assert.Equal(HttpStatusCode.OK, wishlistResponse.StatusCode);

            var wishlist = await wishlistResponse.Content.ReadFromJsonAsync<List<EspetaculoDto>>();
            var item = Assert.Single(wishlist!);
            Assert.Equal(_fixture.EspetaculoId, item.Id);
        }
    }
}
