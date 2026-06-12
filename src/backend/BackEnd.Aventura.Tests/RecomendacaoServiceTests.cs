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
            // Arrange
            var usuarioId = Guid.Empty;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.GetRecomendacoes(usuarioId, 10));
        }

        [Fact]
        public void GetRecomendacoes_InvalidLimit_Zero_ShouldThrow()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _service.GetRecomendacoes(usuarioId, 0));
            Assert.Contains("entre 1 e 50", ex.Message);
        }

        [Fact]
        public void GetRecomendacoes_InvalidLimit_OverFifty_ShouldThrow()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _service.GetRecomendacoes(usuarioId, 51));
            Assert.Contains("entre 1 e 50", ex.Message);
        }

        [Fact]
        public void GetRecomendacoes_NoStoredData_ShouldReturnFallback()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();

            // Create some shows for fallback
            var teatro = new Teatro(Guid.NewGuid(), "Theatro Municipal", "Av. Paulista, 1578", "São Paulo", "10:00-22:00");
            var espetaculo = new Espetaculo(Guid.NewGuid(), "Macbeth", "Drama", "Show", "Uma tragédia de Shakespeare", "https://example.com/buy", DateTime.UtcNow, DateTime.UtcNow.AddMonths(3), 100m);
            _espetaculoRepository.AddEspetaculoParaUsuario(usuarioId, espetaculo);

            // Act
            var recomendacoes = _service.GetRecomendacoes(usuarioId, 10);

            // Assert
            Assert.NotEmpty(recomendacoes);
            Assert.Single(recomendacoes);
            Assert.NotNull(recomendacoes.First().Espetaculo);
        }

        [Fact]
        public void GetRecomendacoes_WithStoredData_ShouldReturnStored()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var teatro = new Teatro(Guid.NewGuid(), "Theatro Municipal", "Av. Paulista, 1578", "São Paulo", "10:00-22:00");
            var espetaculo = new Espetaculo(Guid.NewGuid(), "Hamlet", "Drama", "Show", "Uma tragédia de Shakespeare", "https://example.com/buy", DateTime.UtcNow, DateTime.UtcNow.AddMonths(3), 100m);
            var recomendacao = new Recomendacao(espetaculo, 0.95m, "Recomendação teste");

            _recomendacaoRepository.AddRecomendacao(usuarioId, recomendacao);

            // Act
            var recomendacoes = _service.GetRecomendacoes(usuarioId, 10);

            // Assert
            Assert.Single(recomendacoes);
            Assert.Equal("Hamlet", recomendacoes.First().Espetaculo.Nome);
            Assert.Equal(0.95m, recomendacoes.First().Score);
        }

        [Fact]
        public void GetRecomendacoes_RespectLimit()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var teatro = new Teatro(Guid.NewGuid(), "Theatro Municipal", "Av. Paulista, 1578", "São Paulo", "10:00-22:00");

            for (int i = 0; i < 20; i++)
            {
                var espetaculo = new Espetaculo(Guid.NewGuid(), $"Show {i}", "Drama", "Show", "Uma tragédia de Shakespeare", "https://example.com/buy", DateTime.UtcNow, DateTime.UtcNow.AddMonths(3), 100m);
                var recomendacao = new Recomendacao(espetaculo, 0.9m - i * 0.01m, "Test");
                _recomendacaoRepository.AddRecomendacao(usuarioId, recomendacao);
            }

            // Act
            var recomendacoes = _service.GetRecomendacoes(usuarioId, 5);

            // Assert
            Assert.Equal(5, recomendacoes.Count);
        }

        [Fact]
        public void GetRecomendacoes_MaxLimit_ShouldAllow()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();

            // Act
            var recomendacoes = _service.GetRecomendacoes(usuarioId, 50);

            // Assert - Should not throw
            Assert.NotNull(recomendacoes);
        }
    }
}
