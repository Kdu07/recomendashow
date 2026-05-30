using System;
using BackEnd.Aventura.Controller;
using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);
        }

        [Fact]
        public void Register_ShouldReturnCreatedAndResponse()
        {
            // Arrange
            var request = new RegisterRequest { Nome = "User", Email = "user@test.com", Senha = "123", Idade = 18 };
            var response = new AuthResponse { UsuarioId = Guid.NewGuid(), Nome = "User", Email = "user@test.com" };
            _authServiceMock.Setup(s => s.Register(request)).Returns(response);

            // Act
            var result = _controller.Register(request);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(response, actionResult.Value);
        }

        [Fact]
        public void Login_ShouldReturnOkAndResponse()
        {
            // Arrange
            var request = new LoginRequest { Email = "user@test.com", Senha = "123" };
            var response = new AuthResponse { UsuarioId = Guid.NewGuid(), Nome = "User", Email = "user@test.com" };
            _authServiceMock.Setup(s => s.Login(request)).Returns(response);

            // Act
            var result = _controller.Login(request);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response, actionResult.Value);
        }
    }
}
