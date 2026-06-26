using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Services;
using BackEnd.Aventura.Tests.Fakes;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class RecomendacaoServiceTests
    {
        private readonly FakeRecomendacaoRepository _recomendacaoRepository;
        private readonly FakeEspetaculoRepository _espetaculoRepository;
        private readonly FakeWishlistRepository _wishlistRepository;
        private readonly RecomendacaoService _service;

        public RecomendacaoServiceTests()
        {
            _recomendacaoRepository = new FakeRecomendacaoRepository();
            _espetaculoRepository = new FakeEspetaculoRepository();
            _wishlistRepository = new FakeWishlistRepository();
            _service = CreateService();
        }

        private RecomendacaoService CreateService()
        {
            return new RecomendacaoService(
                new IRecommendationStrategy[]
                {
                    new StoredRecommendationStrategy(_recomendacaoRepository),
                    new WishlistGenresRecommendationStrategy(_espetaculoRepository),
                    new FavoriteGenresRecommendationStrategy(_espetaculoRepository)
                },
                _wishlistRepository,
                new WishlistRecommendationPostProcessor());
        }

        [Fact]
        public void GetRecomendacoes_EmptyGuId_ShouldThrow()
        {
            var usuarioId = Guid.Empty;

            Assert.Throws<ArgumentException>(() => _service.GetRecomendacoes(usuarioId, 10));
        }

        [Fact]
        public void GetRecomendacoes_InvalidLimit_Zero_ShouldThrow()
        {
            var usuarioId = Guid.NewGuid();

            var ex = Assert.Throws<ArgumentException>(() => _service.GetRecomendacoes(usuarioId, 0));
            Assert.Contains("entre 1 e 50", ex.Message);
        }

        [Fact]
        public void GetRecomendacoes_InvalidLimit_OverFifty_ShouldThrow()
        {
            var usuarioId = Guid.NewGuid();

            var ex = Assert.Throws<ArgumentException>(() => _service.GetRecomendacoes(usuarioId, 51));
            Assert.Contains("entre 1 e 50", ex.Message);
        }

        [Fact]
        public void GetRecomendacoes_NoStoredData_ShouldReturnFallback()
        {
            var usuarioId = Guid.NewGuid();

            var teatro = new Teatro(Guid.NewGuid(), "Theatro Municipal", "Av. Paulista, 1578", "São Paulo", "10:00-22:00");
            var espetaculo = new Espetaculo(Guid.NewGuid(), "Macbeth", "Drama", "Show", "Uma tragédia de Shakespeare", "https://example.com/buy", DateTime.UtcNow, DateTime.UtcNow.AddMonths(3), 100m);
            _espetaculoRepository.AddEspetaculoParaUsuario(usuarioId, espetaculo);

            var recomendacoes = _service.GetRecomendacoes(usuarioId, 10);

            Assert.NotEmpty(recomendacoes);
            Assert.Single(recomendacoes);
            Assert.NotNull(recomendacoes.First().Espetaculo);
        }

        [Fact]
        public void GetRecomendacoes_WithStoredData_ShouldReturnStored()
        {
            var usuarioId = Guid.NewGuid();
            var teatro = new Teatro(Guid.NewGuid(), "Theatro Municipal", "Av. Paulista, 1578", "São Paulo", "10:00-22:00");
            var espetaculo = new Espetaculo(Guid.NewGuid(), "Hamlet", "Drama", "Show", "Uma tragédia de Shakespeare", "https://example.com/buy", DateTime.UtcNow, DateTime.UtcNow.AddMonths(3), 100m);
            var recomendacao = new Recomendacao(espetaculo, 0.95m, "Recomendação teste");

            _recomendacaoRepository.AddRecomendacao(usuarioId, recomendacao);

            var recomendacoes = _service.GetRecomendacoes(usuarioId, 10);

            Assert.Single(recomendacoes);
            Assert.Equal("Hamlet", recomendacoes.First().Espetaculo.Nome);
            Assert.Equal(0.95m, recomendacoes.First().Score);
        }

        [Fact]
        public void GetRecomendacoes_RespectLimit()
        {
            var usuarioId = Guid.NewGuid();
            var teatro = new Teatro(Guid.NewGuid(), "Theatro Municipal", "Av. Paulista, 1578", "São Paulo", "10:00-22:00");

            for (int i = 0; i < 20; i++)
            {
                var espetaculo = new Espetaculo(Guid.NewGuid(), $"Show {i}", "Drama", "Show", "Uma tragédia de Shakespeare", "https://example.com/buy", DateTime.UtcNow, DateTime.UtcNow.AddMonths(3), 100m);
                var recomendacao = new Recomendacao(espetaculo, 0.9m - i * 0.01m, "Test");
                _recomendacaoRepository.AddRecomendacao(usuarioId, recomendacao);
            }

            var recomendacoes = _service.GetRecomendacoes(usuarioId, 5);

            Assert.Equal(5, recomendacoes.Count);
        }

        [Fact]
        public void GetRecomendacoes_MaxLimit_ShouldAllow()
        {
            var usuarioId = Guid.NewGuid();

            var recomendacoes = _service.GetRecomendacoes(usuarioId, 50);

            Assert.NotNull(recomendacoes);
        }

        [Fact]
        public void GetRecomendacoes_WhenItemInWishlist_ShouldExcludeIt()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var naWishlist = CriarEspetaculo("Hamlet");
            var disponivel = CriarEspetaculo("Macbeth");
            _recomendacaoRepository.AddRecomendacao(usuarioId, new Recomendacao(naWishlist, 0.95m, "Test"));
            _recomendacaoRepository.AddRecomendacao(usuarioId, new Recomendacao(disponivel, 0.90m, "Test"));
            _wishlistRepository.AddItem(usuarioId, naWishlist.Id);

            // Act
            var recomendacoes = _service.GetRecomendacoes(usuarioId, 10);

            // Assert
            Assert.Single(recomendacoes);
            Assert.Equal("Macbeth", recomendacoes.First().Espetaculo.Nome);
        }

        [Fact]
        public void GetRecomendacoes_WhenWishlistEmpty_ShouldReturnAllStored()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            _recomendacaoRepository.AddRecomendacao(usuarioId, new Recomendacao(CriarEspetaculo("Hamlet"), 0.95m, "Test"));
            _recomendacaoRepository.AddRecomendacao(usuarioId, new Recomendacao(CriarEspetaculo("Macbeth"), 0.90m, "Test"));

            // Act
            var recomendacoes = _service.GetRecomendacoes(usuarioId, 10);

            // Assert
            Assert.Equal(2, recomendacoes.Count);
        }

        [Fact]
        public void GetRecomendacoes_AfterExclusion_ShouldStillFillLimit()
        {
            // Arrange - 6 recomendações, sendo a primeira já na wishlist; o buffer deve
            // garantir que o limite de 5 ainda seja preenchido após a exclusão.
            var usuarioId = Guid.NewGuid();
            for (int i = 0; i < 6; i++)
            {
                var espetaculo = CriarEspetaculo($"Show {i}");
                _recomendacaoRepository.AddRecomendacao(usuarioId, new Recomendacao(espetaculo, 0.9m - i * 0.01m, "Test"));
                if (i == 0)
                {
                    _wishlistRepository.AddItem(usuarioId, espetaculo.Id);
                }
            }

            // Act
            var recomendacoes = _service.GetRecomendacoes(usuarioId, 5);

            // Assert
            Assert.Equal(5, recomendacoes.Count);
            Assert.DoesNotContain(recomendacoes, r => r.Espetaculo.Nome == "Show 0");
        }

        [Fact]
        public void GetRecomendacoes_NoStored_WithWishlistGenres_ShouldUseWishlistFallback()
        {
            // Arrange - sem dados do modelo, mas com candidatos por gênero da wishlist
            var usuarioId = Guid.NewGuid();
            _espetaculoRepository.AddEspetaculoWishlistParaUsuario(usuarioId, CriarEspetaculo("Othello"));

            // Act
            var recomendacoes = _service.GetRecomendacoes(usuarioId, 10);

            // Assert
            Assert.Single(recomendacoes);
            Assert.Equal("Othello", recomendacoes.First().Espetaculo.Nome);
            Assert.Equal("Baseado nos itens da sua wishlist", recomendacoes.First().Motivo);
        }

        [Fact]
        public void GetRecomendacoes_WishlistGenres_ShouldPrecedeFavoriteGenres()
        {
            // Arrange - usuário tem candidatos tanto por wishlist quanto por gêneros favoritos;
            // a estratégia de wishlist deve vencer por estar antes na cadeia.
            var usuarioId = Guid.NewGuid();
            _espetaculoRepository.AddEspetaculoWishlistParaUsuario(usuarioId, CriarEspetaculo("Por Wishlist"));
            _espetaculoRepository.AddEspetaculoParaUsuario(usuarioId, CriarEspetaculo("Por Favoritos"));

            // Act
            var recomendacoes = _service.GetRecomendacoes(usuarioId, 10);

            // Assert
            Assert.Single(recomendacoes);
            Assert.Equal("Por Wishlist", recomendacoes.First().Espetaculo.Nome);
        }

        [Fact]
        public void GetRecomendacoes_NoStoredNoWishlist_ShouldFallbackToFavoriteGenres()
        {
            // Arrange - sem modelo e sem wishlist, cai para gêneros favoritos
            var usuarioId = Guid.NewGuid();
            _espetaculoRepository.AddEspetaculoParaUsuario(usuarioId, CriarEspetaculo("Por Favoritos"));

            // Act
            var recomendacoes = _service.GetRecomendacoes(usuarioId, 10);

            // Assert
            Assert.Single(recomendacoes);
            Assert.Equal("Por Favoritos", recomendacoes.First().Espetaculo.Nome);
        }

        [Fact]
        public void GetRecomendacoes_ShouldBoostItemsMatchingWishlistGenres_AndReorder()
        {
            // Arrange - modelo devolve Comédia (0.80) acima de Drama (0.70); a wishlist é de
            // Drama, então o boost (x1.15 => 0.805) deve levar o Drama para o topo.
            var usuarioId = Guid.NewGuid();
            _recomendacaoRepository.AddRecomendacao(usuarioId, new Recomendacao(CriarEspetaculo("Comédia A", "Comédia"), 0.80m, "modelo"));
            _recomendacaoRepository.AddRecomendacao(usuarioId, new Recomendacao(CriarEspetaculo("Drama B", "Drama"), 0.70m, "modelo"));

            // wishlist não-vazia (para o serviço buscar os gêneros) sem colidir com os candidatos
            _wishlistRepository.AddItem(usuarioId, Guid.NewGuid());
            _wishlistRepository.SetGenerosDaWishlist(usuarioId, "Drama");

            // Act
            var recomendacoes = _service.GetRecomendacoes(usuarioId, 10);

            // Assert
            Assert.Equal(2, recomendacoes.Count);
            Assert.Equal("Drama B", recomendacoes.First().Espetaculo.Nome);
            Assert.Equal("Combina com a sua wishlist", recomendacoes.First().Motivo);
            Assert.Equal("Comédia A", recomendacoes.Last().Espetaculo.Nome);
        }

        [Fact]
        public void GetRecomendacoes_BoostMatch_ShouldBeCaseInsensitive()
        {
            // Arrange - gênero da recomendação ("Drama") e da wishlist ("drama") diferem só na caixa
            var usuarioId = Guid.NewGuid();
            _recomendacaoRepository.AddRecomendacao(usuarioId, new Recomendacao(CriarEspetaculo("Comédia A", "Comédia"), 0.80m, "modelo"));
            _recomendacaoRepository.AddRecomendacao(usuarioId, new Recomendacao(CriarEspetaculo("Drama B", "Drama"), 0.70m, "modelo"));
            _wishlistRepository.AddItem(usuarioId, Guid.NewGuid());
            _wishlistRepository.SetGenerosDaWishlist(usuarioId, "drama");

            // Act
            var recomendacoes = _service.GetRecomendacoes(usuarioId, 10);

            // Assert
            Assert.Equal("Drama B", recomendacoes.First().Espetaculo.Nome);
            Assert.Equal("Combina com a sua wishlist", recomendacoes.First().Motivo);
        }

        [Fact]
        public void GetRecomendacoes_NoStrategyHasData_ShouldReturnEmpty()
        {
            var usuarioId = Guid.NewGuid();

            var recomendacoes = _service.GetRecomendacoes(usuarioId, 10);

            Assert.Empty(recomendacoes);
        }

        [Fact]
        public void Constructor_NullStrategies_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new RecomendacaoService(null!, _wishlistRepository, new WishlistRecommendationPostProcessor()));
        }

        [Fact]
        public void Constructor_EmptyStrategies_Throws()
        {
            Assert.Throws<InvalidOperationException>(() =>
                new RecomendacaoService(Array.Empty<IRecommendationStrategy>(), _wishlistRepository, new WishlistRecommendationPostProcessor()));
        }

        [Fact]
        public void StoredRecommendationStrategy_Name_ShouldBeStored()
        {
            var strategy = new StoredRecommendationStrategy(_recomendacaoRepository);
            Assert.Equal("stored", strategy.Name);
        }

        [Fact]
        public void FavoriteGenresRecommendationStrategy_Name_ShouldBeFavoriteGenres()
        {
            var strategy = new FavoriteGenresRecommendationStrategy(_espetaculoRepository);
            Assert.Equal("favorite-genres", strategy.Name);
        }

        private static Espetaculo CriarEspetaculo(string nome) => CriarEspetaculo(nome, "Drama");

        private static Espetaculo CriarEspetaculo(string nome, string genero) =>
            new(Guid.NewGuid(), nome, genero, "Show", "Uma tragédia de Shakespeare",
                "https://example.com/buy", DateTime.UtcNow, DateTime.UtcNow.AddMonths(3), 100m);
    }
}