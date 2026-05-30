using System;
using BackEnd.Aventura.Controller;
using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class PreferenciasControllerTests
    {
        private readonly Mock<IPreferenciasService> _serviceMock;
        private readonly PreferenciasController _controller;

        public PreferenciasControllerTests()
        {
            _serviceMock = new Mock<IPreferenciasService>();
            _controller = new PreferenciasController(_serviceMock.Object);
        }

        [Fact]
        public void GetPreferencias_ShouldReturnOkAndDto()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var pref = new Preferencias(userId);
            _serviceMock.Setup(s => s.GetByUsuario(userId)).Returns(pref);

            // Act
            var result = _controller.GetPreferencias(userId);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<PreferenciasDto>(actionResult.Value);
            Assert.Equal(userId, returnedDto.UsuarioId);
        }

        [Fact]
        public void UpdatePreferencias_ShouldReturnOkAndUpdatedDto()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var pref = new Preferencias(userId);
            var dto = new PreferenciasUpdateDto { Cidade = "São Paulo", ReceberNotificacoes = false };
            pref.SetLocalizacao("", "São Paulo");
            pref.SetReceberNotificacoes(false);

            _serviceMock.Setup(s => s.Update(userId, dto)).Returns(pref);

            // Act
            var result = _controller.UpdatePreferencias(userId, dto);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<PreferenciasDto>(actionResult.Value);
            Assert.Equal("São Paulo", returnedDto.Cidade);
            Assert.False(returnedDto.ReceberNotificacoes);
        }
    }
}
