using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Exceptions;
using BackEnd.Aventura.Services;
using BackEnd.Aventura.Tests.Fakes;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class EspetaculoServiceTests
    {
        private readonly FakeEspetaculoRepository _repository;
        private readonly EspetaculoService _service;

        public EspetaculoServiceTests()
        {
            _repository = new FakeEspetaculoRepository();
            _service = new EspetaculoService(_repository);
        }

        [Fact]
        public void GetAll_NoEspetaculos_ShouldReturnEmpty()
        {
            // Act
            var espetaculos = _service.GetAll();

            // Assert
            Assert.Empty(espetaculos);
        }

        [Fact]
        public void GetAll_WithEspetaculos_ShouldReturnAll()
        {
            // Arrange
            var teatro = new Teatro(Guid.NewGuid(), "Theatro Municipal", "Av. Paulista, 1578", "São Paulo", "10:00-22:00");
            var espetaculo1 = new Espetaculo(Guid.NewGuid(), "Macbeth", "Drama", "Show", "Uma tragédia de Shakespeare", "https://example.com/buy", DateTime.UtcNow, DateTime.UtcNow.AddMonths(3), 100m);
            var espetaculo2 = new Espetaculo(Guid.NewGuid(), "Hamlet", "Drama", "Show", "Uma tragédia de Shakespeare", "https://example.com/buy", DateTime.UtcNow, DateTime.UtcNow.AddMonths(2), 100m);

            _repository.AddEspetaculo(espetaculo1);
            _repository.AddEspetaculo(espetaculo2);

            // Act
            var espetaculos = _service.GetAll();

            // Assert
            Assert.Equal(2, espetaculos.Count);
        }

        [Fact]
        public void GetById_ExistingEspetaculo_ShouldReturn()
        {
            // Arrange
            var teatro = new Teatro(Guid.NewGuid(), "Theatro Municipal", "Av. Paulista, 1578", "São Paulo", "10:00-22:00");
            var espetaculo = new Espetaculo(Guid.NewGuid(), "Macbeth", "Drama", "Show", "Uma tragédia de Shakespeare", "https://example.com/buy", DateTime.UtcNow, DateTime.UtcNow.AddMonths(3), 100m);
            _repository.AddEspetaculo(espetaculo);

            // Act
            var result = _service.GetById(espetaculo.Id);

            // Assert
            Assert.Equal("Macbeth", result.Nome);
            Assert.Equal("Drama", result.Genero);
        }

        [Fact]
        public void GetById_NonExistentEspetaculo_ShouldThrow()
        {
            // Arrange
            var espetaculoId = Guid.NewGuid();

            // Act & Assert
            var ex = Assert.Throws<NotFoundException>(() => _service.GetById(espetaculoId));
            Assert.Contains("não encontrado", ex.Message);
        }

        [Fact]
        public void Search_ByGenero_ShouldReturnMatches()
        {
            // Arrange
            var teatro = new Teatro(Guid.NewGuid(), "Theatro Municipal", "Av. Paulista, 1578", "São Paulo", "10:00-22:00");
            var drama = new Espetaculo(Guid.NewGuid(), "Macbeth", "Drama", "Show", "Uma tragédia de Shakespeare", "https://example.com/buy", DateTime.UtcNow, DateTime.UtcNow.AddMonths(3), 100m);
            var comedia = new Espetaculo(Guid.NewGuid(), "The Importance of Being Earnest", "Comédia", "Show", "Uma comédia de Oscar Wilde", "https://example.com/buy", DateTime.UtcNow, DateTime.UtcNow.AddMonths(3), 100m);

            _repository.AddEspetaculo(drama);
            _repository.AddEspetaculo(comedia);

            // Act
            var resultado = _service.Search("Drama", null);

            // Assert
            Assert.Single(resultado);
            Assert.Equal("Macbeth", resultado.First().Nome);
        }

        [Fact]
        public void Search_ByNome_ShouldReturnMatches()
        {
            // Arrange
            var teatro = new Teatro(Guid.NewGuid(), "Theatro Municipal", "Av. Paulista, 1578", "São Paulo", "10:00-22:00");
            var espetaculo1 = new Espetaculo(Guid.NewGuid(), "Macbeth", "Drama", "Show", "Uma tragédia de Shakespeare", "https://example.com/buy", DateTime.UtcNow, DateTime.UtcNow.AddMonths(3), 100m);
            var espetaculo2 = new Espetaculo(Guid.NewGuid(), "Hamlet", "Drama", "Show", "Uma tragédia de Shakespeare", "https://example.com/buy", DateTime.UtcNow, DateTime.UtcNow.AddMonths(2), 100m);

            _repository.AddEspetaculo(espetaculo1);
            _repository.AddEspetaculo(espetaculo2);

            // Act
            var resultado = _service.Search(null, "Macbeth");

            // Assert
            Assert.Single(resultado);
            Assert.Equal("Macbeth", resultado.First().Nome);
        }

        [Fact]
        public void Search_ByGeneroAndNome_ShouldReturnMatches()
        {
            // Arrange
            var teatro = new Teatro(Guid.NewGuid(), "Theatro Municipal", "Av. Paulista, 1578", "São Paulo", "10:00-22:00");
            var drama1 = new Espetaculo(Guid.NewGuid(), "Macbeth", "Drama", "Show", "Uma tragédia de Shakespeare", "https://example.com/buy", DateTime.UtcNow, DateTime.UtcNow.AddMonths(3), 100m);
            var drama2 = new Espetaculo(Guid.NewGuid(), "Hamlet", "Drama", "Show", "Uma tragédia de Shakespeare", "https://example.com/buy", DateTime.UtcNow, DateTime.UtcNow.AddMonths(2), 100m);
            var comedia = new Espetaculo(Guid.NewGuid(), "The Importance of Being Earnest", "Comédia", "Show", "Uma comédia de Oscar Wilde", "https://example.com/buy", DateTime.UtcNow, DateTime.UtcNow.AddMonths(3), 100m);

            _repository.AddEspetaculo(drama1);
            _repository.AddEspetaculo(drama2);
            _repository.AddEspetaculo(comedia);

            // Act
            var resultado = _service.Search("Drama", "Macbeth");

            // Assert
            Assert.Single(resultado);
            Assert.Equal("Macbeth", resultado.First().Nome);
            Assert.Equal("Drama", resultado.First().Genero);
        }

        [Fact]
        public void Search_NoMatches_ShouldReturnEmpty()
        {
            // Arrange
            var teatro = new Teatro(Guid.NewGuid(), "Theatro Municipal", "Av. Paulista, 1578", "São Paulo", "10:00-22:00");
            var espetaculo = new Espetaculo(Guid.NewGuid(), "Macbeth", "Drama", "Show", "Uma tragédia de Shakespeare", "https://example.com/buy", DateTime.UtcNow, DateTime.UtcNow.AddMonths(3), 100m);
            _repository.AddEspetaculo(espetaculo);

            // Act
            var resultado = _service.Search(null, "Nonexistent");

            // Assert
            Assert.Empty(resultado);
        }

        [Fact]
        public void Search_NomeIsCaseInsensitive()
        {
            // Arrange
            var teatro = new Teatro(Guid.NewGuid(), "Theatro Municipal", "Av. Paulista, 1578", "São Paulo", "10:00-22:00");
            var espetaculo = new Espetaculo(Guid.NewGuid(), "Macbeth", "Drama", "Show", "Uma tragédia de Shakespeare", "https://example.com/buy", DateTime.UtcNow, DateTime.UtcNow.AddMonths(3), 100m);
            _repository.AddEspetaculo(espetaculo);

            // Act
            var resultado = _service.Search(null, "macbeth");

            // Assert
            Assert.Single(resultado);
        }
    }
}
