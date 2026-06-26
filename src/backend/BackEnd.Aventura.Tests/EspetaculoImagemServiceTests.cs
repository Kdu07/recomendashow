using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Exceptions;
using BackEnd.Aventura.Repository;
using BackEnd.Aventura.Services;
using Moq;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class EspetaculoImagemServiceTests
    {
        private readonly Mock<IEspetaculoService> _espetaculoServiceMock;
        private readonly Mock<IEspetaculoImagemRepository> _imagemRepositoryMock;
        private readonly EspetaculoImagemService _service;

        public EspetaculoImagemServiceTests()
        {
            _espetaculoServiceMock = new Mock<IEspetaculoService>();
            _imagemRepositoryMock = new Mock<IEspetaculoImagemRepository>();
            _service = new EspetaculoImagemService(_espetaculoServiceMock.Object, _imagemRepositoryMock.Object);
        }

        private static Espetaculo CreateEspetaculo(Guid id, string nome) => new(
            id, nome, "Drama", "Show", "Descricao", "http://buy",
            DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(5), 50m);

        [Fact]
        public void GetImagem_EspetaculoExisteComImagem_ShouldReturnImagem()
        {
            // Arrange
            var espetaculoId = Guid.NewGuid();
            var espetaculo = CreateEspetaculo(espetaculoId, "Hamlet");
            _espetaculoServiceMock.Setup(s => s.GetById(espetaculoId)).Returns(espetaculo);

            using var conteudo = new MemoryStream(new byte[] { 1, 2, 3 });
            var imagemEsperada = new EspetaculoImagem(conteudo, "image/jpeg", "hamlet.jpg");
            _imagemRepositoryMock.Setup(r => r.GetByEspetaculoNome("Hamlet")).Returns(imagemEsperada);

            // Act
            var resultado = _service.GetImagem(espetaculoId);

            // Assert
            Assert.Same(imagemEsperada, resultado);
            _imagemRepositoryMock.Verify(r => r.GetByEspetaculoNome("Hamlet"), Times.Once);
        }

        [Fact]
        public void GetImagem_EspetaculoNaoExiste_ShouldPropagateNotFound()
        {
            // Arrange
            var espetaculoId = Guid.NewGuid();
            _espetaculoServiceMock.Setup(s => s.GetById(espetaculoId))
                .Throws(new NotFoundException($"Espetáculo com ID {espetaculoId} não encontrado."));

            // Act & Assert
            Assert.Throws<NotFoundException>(() => _service.GetImagem(espetaculoId));
            _imagemRepositoryMock.Verify(r => r.GetByEspetaculoNome(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void GetImagem_EspetaculoExisteSemImagem_ShouldThrowNotFound()
        {
            // Arrange
            var espetaculoId = Guid.NewGuid();
            var espetaculo = CreateEspetaculo(espetaculoId, "Sem Imagem");
            _espetaculoServiceMock.Setup(s => s.GetById(espetaculoId)).Returns(espetaculo);
            _imagemRepositoryMock.Setup(r => r.GetByEspetaculoNome("Sem Imagem")).Returns((EspetaculoImagem?)null);

            // Act & Assert
            var ex = Assert.Throws<NotFoundException>(() => _service.GetImagem(espetaculoId));
            Assert.Contains("Sem Imagem", ex.Message);
        }

        [Fact]
        public void Constructor_NullEspetaculoService_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new EspetaculoImagemService(null!, _imagemRepositoryMock.Object));
        }

        [Fact]
        public void Constructor_NullImagemRepository_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new EspetaculoImagemService(_espetaculoServiceMock.Object, null!));
        }
    }
}