using System;
using BackEnd.Aventura.Controller;
using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class UsuarioControllerTests
    {
        private readonly Mock<IUsuarioService> _serviceMock;
        private readonly UsuarioController _controller;

        public UsuarioControllerTests()
        {
            _serviceMock = new Mock<IUsuarioService>();
            _controller = new UsuarioController(_serviceMock.Object);
        }

        [Fact]
        public void GetById_ShouldReturnOkAndDto()
        {
            var userId = Guid.NewGuid();
            var response = new UsuarioResponse { Id = userId, Nome = "User", Email = "user@test.com" };
            _serviceMock.Setup(s => s.GetById(userId)).Returns(response);

            var result = _controller.GetById(userId);

            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<UsuarioResponse>(actionResult.Value);
            Assert.Equal(userId, returnedDto.Id);
        }

        [Fact]
        public async Task Update_ShouldReturnOkAndUpdatedDto()
        {
            var userId = Guid.NewGuid();
            var request = new UsuarioUpdateRequest { Nome = "User Updated", Idade = 25 };
            var response = new UsuarioResponse { Id = userId, Nome = "User Updated", Email = "user@test.com" };
            _serviceMock.Setup(s => s.Update(userId, request)).ReturnsAsync(response);

            var result = await _controller.Update(userId, request);

            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<UsuarioResponse>(actionResult.Value);
            Assert.Equal("User Updated", returnedDto.Nome);
        }
    }
}
