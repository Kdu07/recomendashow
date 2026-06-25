using System;
using System.Collections.Generic;
using System.Linq;
using BackEnd.Aventura.Controller;
using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class EspetaculoControllerTests
    {
        private readonly Mock<IEspetaculoService> _serviceMock;
        private readonly Mock<IEspetaculoImagemService> _imagemServiceMock;
        private readonly EspetaculoController _controller;

        public EspetaculoControllerTests()
        {
            _serviceMock = new Mock<IEspetaculoService>();
            _imagemServiceMock = new Mock<IEspetaculoImagemService>();
            _controller = new EspetaculoController(_serviceMock.Object, _imagemServiceMock.Object);
        }

        private Espetaculo CreateEspetaculo(string nome)
        {
            return new Espetaculo(
                Guid.NewGuid(),
                nome,
                "genero",
                "tipo",
                "desc",
                "url",
                DateTime.UtcNow.AddDays(-1),
                DateTime.UtcNow.AddDays(5),
                50m
            );
        }

        [Fact]
        public void GetAll_ShouldReturnOkAndList()
        {
            // Arrange
            var list = new List<Espetaculo> { CreateEspetaculo("A"), CreateEspetaculo("B") };
            _serviceMock.Setup(s => s.GetAll()).Returns(list);

            // Act
            var result = _controller.GetAll();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedList = Assert.IsAssignableFrom<IReadOnlyList<EspetaculoDto>>(actionResult.Value);
            Assert.Equal(2, returnedList.Count);
        }

        [Fact]
        public void GetById_ShouldReturnOkAndDto()
        {
            // Arrange
            var espetaculo = CreateEspetaculo("A");
            _serviceMock.Setup(s => s.GetById(espetaculo.Id)).Returns(espetaculo);

            // Act
            var result = _controller.GetById(espetaculo.Id);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<EspetaculoDto>(actionResult.Value);
            Assert.Equal(espetaculo.Id, returnedDto.Id);
        }

        [Fact]
        public void Search_ShouldReturnOkAndFilteredList()
        {
            // Arrange
            var list = new List<Espetaculo> { CreateEspetaculo("A") };
            _serviceMock.Setup(s => s.Search("Rock", "A")).Returns(list);

            // Act
            var result = _controller.Search("Rock", "A");

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedList = Assert.IsAssignableFrom<IReadOnlyList<EspetaculoDto>>(actionResult.Value);
            Assert.Single(returnedList);
            Assert.Equal("A", returnedList[0].Nome);
        }

        [Fact]
        public void GetImagem_ShouldReturnFileStreamResultWithContentType()
        {
            // Arrange
            var espetaculoId = Guid.NewGuid();
            using var conteudo = new MemoryStream(new byte[] { 1, 2, 3 });
            var imagem = new EspetaculoImagem(conteudo, "image/jpeg", "show.jpg");
            _imagemServiceMock.Setup(s => s.GetImagem(espetaculoId)).Returns(imagem);

            // Act
            var result = _controller.GetImagem(espetaculoId);

            // Assert
            var fileResult = Assert.IsType<FileStreamResult>(result);
            Assert.Equal("image/jpeg", fileResult.ContentType);
            Assert.Equal("show.jpg", fileResult.FileDownloadName);
        }

        [Fact]
        public void GetImagem_NotFound_ShouldPropagateException()
        {
            // Arrange
            var espetaculoId = Guid.NewGuid();
            _imagemServiceMock.Setup(s => s.GetImagem(espetaculoId))
                .Throws(new BackEnd.Aventura.Exceptions.NotFoundException("Imagem não encontrada."));

            // Act & Assert
            Assert.Throws<BackEnd.Aventura.Exceptions.NotFoundException>(() => _controller.GetImagem(espetaculoId));
        }
    }
}