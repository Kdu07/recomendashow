using System;
using System.Collections.Generic;
using BackEnd.Aventura.Controller;
using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class WishlistControllerTests
    {
        private readonly Mock<IWishlistService> _serviceMock;
        private readonly WishlistController _controller;

        public WishlistControllerTests()
        {
            _serviceMock = new Mock<IWishlistService>();
            _controller = new WishlistController(_serviceMock.Object);
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
        public void GetByUsuario_ShouldReturnOkAndList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var list = new List<Espetaculo> { CreateEspetaculo("A") };
            _serviceMock.Setup(s => s.GetWishlist(userId)).Returns(list);

            // Act
            var result = _controller.GetByUsuario(userId);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedList = Assert.IsAssignableFrom<IReadOnlyList<EspetaculoDto>>(actionResult.Value);
            Assert.Single(returnedList);
            Assert.Equal("A", returnedList[0].Nome);
        }

        [Fact]
        public void Contains_ShouldReturnOkAndBool()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var espetaculoId = Guid.NewGuid();
            _serviceMock.Setup(s => s.Contains(userId, espetaculoId)).Returns(true);

            // Act
            var result = _controller.Contains(userId, espetaculoId);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.True((bool)actionResult.Value!);
        }

        [Fact]
        public void Add_ShouldReturnCreated()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var espetaculoId = Guid.NewGuid();

            // Act
            var result = _controller.Add(userId, espetaculoId);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result);
            _serviceMock.Verify(s => s.Add(userId, espetaculoId), Times.Once);
        }

        [Fact]
        public void Remove_ShouldReturnNoContent()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var espetaculoId = Guid.NewGuid();

            // Act
            var result = _controller.Remove(userId, espetaculoId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _serviceMock.Verify(s => s.Remove(userId, espetaculoId), Times.Once);
        }
    }
}
