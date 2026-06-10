using System.Net;
using System.Net.Http.Json;
using BackEnd.Aventura.Dto;
using BackEnd.Aventura.E2ETests.Fixtures;
using Xunit;

namespace BackEnd.Aventura.E2ETests
{
    public sealed class EspetaculoWishlistFluxoPerfeitoE2ETests : IClassFixture<EspetaculoWishlistFixture>
    {
        private readonly EspetaculoWishlistFixture _fixture;

        public EspetaculoWishlistFluxoPerfeitoE2ETests(EspetaculoWishlistFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task DeveSalvarEspetaculoNaWishlistEConsultarDadosPersistidos()
        {
            await _fixture.ResetDatabaseAsync();
            using var client = _fixture.CreateClient();

            var detalhesResponse = await client.GetAsync($"/api/Espetaculo/{_fixture.EspetaculoId}");
            Assert.Equal(HttpStatusCode.OK, detalhesResponse.StatusCode);

            var detalhes = await detalhesResponse.Content.ReadFromJsonAsync<EspetaculoDto>();
            Assert.NotNull(detalhes);
            Assert.Equal("Mamma Mia!", detalhes!.Nome);
            Assert.Equal("Musical", detalhes.Genero);
            Assert.True(detalhes.EmCartaz);

            var salvarResponse = await client.PostAsync(
                $"/api/Wishlist/{_fixture.UsuarioId}/{_fixture.EspetaculoId}",
                content: null);
            Assert.Equal(HttpStatusCode.Created, salvarResponse.StatusCode);

            var wishlistResponse = await client.GetAsync($"/api/Wishlist/{_fixture.UsuarioId}");
            Assert.Equal(HttpStatusCode.OK, wishlistResponse.StatusCode);

            var wishlist = await wishlistResponse.Content.ReadFromJsonAsync<List<EspetaculoDto>>();
            var item = Assert.Single(wishlist!);
            Assert.Equal(_fixture.EspetaculoId, item.Id);
            Assert.Equal("Mamma Mia!", item.Nome);
            Assert.Equal("Musical", item.Genero);
            Assert.Equal("https://ingressos.example/mamma-mia", item.UrlBilheteria);
        }
    }
}
