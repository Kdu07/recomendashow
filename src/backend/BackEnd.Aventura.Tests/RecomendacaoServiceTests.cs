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
        private readonly RecomendacaoService _service;

        public RecomendacaoServiceTests()
        {
            _recomendacaoRepository = new FakeRecomendacaoRepository();
            _espetaculoRepository = new FakeEspetaculoRepository();
            _service = CreateService();
        }

        private RecomendacaoService CreateService()
        {
            return new RecomendacaoService(new IRecommendationStrategy[]
            {
                new StoredRecommendationStrategy(_recomendacaoRepository),
                new FavoriteGenresRecommendationStrategy(_espetaculoRepository)
            });
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
        public void GetRecomendacoes_NoStrategyHasData_ShouldReturnEmpty()
        {
            var usuarioId = Guid.NewGuid();

            var recomendacoes = _service.GetRecomendacoes(usuarioId, 10);

            Assert.Empty(recomendacoes);
        }

        [Fact]
        public void Constructor_NullStrategies_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new RecomendacaoService(null!));
        }

        [Fact]
        public void Constructor_EmptyStrategies_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => new RecomendacaoService(Array.Empty<IRecommendationStrategy>()));
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
    }
}