using BackEnd.Aventura.Services;
using BackEnd.Aventura.Tests.Fakes;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class GeneroServiceTests
    {
        private readonly FakeGeneroRepository _repository;
        private readonly GeneroService _service;

        public GeneroServiceTests()
        {
            _repository = new FakeGeneroRepository();
            _service = new GeneroService(_repository);
        }

        [Fact]
        public void GetAll_NoGeneros_ShouldReturnEmpty()
        {
            // Act
            var generos = _service.GetAll();

            // Assert
            Assert.Empty(generos);
        }

        [Fact]
        public void GetAll_WithGeneros_ShouldReturnAll()
        {
            // Arrange
            _repository.AddGenero("Drama");
            _repository.AddGenero("Comédia");
            _repository.AddGenero("Musical");

            // Act
            var generos = _service.GetAll();

            // Assert
            Assert.Equal(3, generos.Count);
            Assert.Contains("Drama", generos);
            Assert.Contains("Comédia", generos);
            Assert.Contains("Musical", generos);
        }

        [Fact]
        public void GetAll_ReturnsSameCountAsRepository()
        {
            // Arrange
            _repository.AddGenero("Drama");
            _repository.AddGenero("Comédia");

            // Act
            var generos = _service.GetAll();

            // Assert
            Assert.Equal(_repository.GetAllNomes().Count(), generos.Count);
        }

        [Fact]
        public void GetAll_ReturnsReadOnlyList()
        {
            // Arrange
            _repository.AddGenero("Drama");

            // Act
            var generos = _service.GetAll();

            // Assert
            Assert.IsAssignableFrom<IReadOnlyList<string>>(generos);
        }
    }
}
