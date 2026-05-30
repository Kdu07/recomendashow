using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Exceptions;
using BackEnd.Aventura.Services;
using BackEnd.Aventura.Tests.Fakes;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class TeatroServiceTests
    {
        private readonly FakeTeatroRepository _repository;
        private readonly TeatroService _service;

        public TeatroServiceTests()
        {
            _repository = new FakeTeatroRepository();
            _service = new TeatroService(_repository);
        }

        [Fact]
        public void GetAll_NoTeatros_ShouldReturnEmpty()
        {
            // Act
            var teatros = _service.GetAll();

            // Assert
            Assert.Empty(teatros);
        }

        [Fact]
        public void GetAll_WithTeatros_ShouldReturnAll()
        {
            // Arrange
            var teatro1 = new Teatro(Guid.NewGuid(), "Theatro Municipal", "Av. Paulista, 1578", "São Paulo", "10:00-22:00");
            var teatro2 = new Teatro(Guid.NewGuid(), "Teatro da Paz", "Av. Nazaré, 705", "Belém", "10:00-22:00");

            _repository.AddTeatro(teatro1);
            _repository.AddTeatro(teatro2);

            // Act
            var teatros = _service.GetAll();

            // Assert
            Assert.Equal(2, teatros.Count);
        }

        [Fact]
        public void GetById_ExistingTeatro_ShouldReturn()
        {
            // Arrange
            var teatro = new Teatro(Guid.NewGuid(), "Theatro Municipal", "Av. Paulista, 1578", "São Paulo", "10:00-22:00");
            _repository.AddTeatro(teatro);

            // Act
            var result = _service.GetById(teatro.Id);

            // Assert
            Assert.Equal("Theatro Municipal", result.Nome);
            Assert.Equal("Av. Paulista, 1578", result.Endereco);
        }

        [Fact]
        public void GetById_NonExistentTeatro_ShouldThrow()
        {
            // Arrange
            var teatroId = Guid.NewGuid();

            // Act & Assert
            var ex = Assert.Throws<NotFoundException>(() => _service.GetById(teatroId));
            Assert.Contains("não encontrado", ex.Message);
        }

        [Fact]
        public void GetEspetaculos_TeatroWithEspetaculos_ShouldReturnAll()
        {
            // Arrange
            var teatro = new Teatro(Guid.NewGuid(), "Theatro Municipal", "Av. Paulista, 1578", "São Paulo", "10:00-22:00");
            var espetaculo = new Espetaculo(Guid.NewGuid(), "Macbeth", "Drama", "Show", "Uma tragédia de Shakespeare", "https://example.com/buy", DateTime.UtcNow, DateTime.UtcNow.AddMonths(3), 100m);
            teatro.AdicionarEspetaculo(espetaculo);
            _repository.AddTeatro(teatro);

            // Act
            var espetaculos = _service.GetEspetaculos(teatro.Id);

            // Assert
            Assert.Single(espetaculos);
            Assert.Equal("Macbeth", espetaculos.First().Nome);
        }

        [Fact]
        public void GetEspetaculos_TeatroWithoutEspetaculos_ShouldReturnEmpty()
        {
            // Arrange
            var teatro = new Teatro(Guid.NewGuid(), "Theatro Municipal", "Av. Paulista, 1578", "São Paulo", "10:00-22:00");
            _repository.AddTeatro(teatro);

            // Act
            var espetaculos = _service.GetEspetaculos(teatro.Id);

            // Assert
            Assert.Empty(espetaculos);
        }

        [Fact]
        public void GetEspetaculos_NonExistentTeatro_ShouldThrow()
        {
            // Arrange
            var teatroId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<NotFoundException>(() => _service.GetEspetaculos(teatroId));
        }
    }
}
