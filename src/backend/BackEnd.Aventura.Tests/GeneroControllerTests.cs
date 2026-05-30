using System.Collections.Generic;
using BackEnd.Aventura.Controller;
using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class GeneroControllerTests
    {
        private readonly Mock<IGeneroService> _serviceMock;
        private readonly GeneroController _controller;

        public GeneroControllerTests()
        {
            _serviceMock = new Mock<IGeneroService>();
            _controller = new GeneroController(_serviceMock.Object);
        }

        [Fact]
        public void GetAll_ShouldReturnOkAndList()
        {
            // Arrange
            var list = new List<string> { "Drama", "Musical" };
            _serviceMock.Setup(s => s.GetAll()).Returns(list);

            // Act
            var result = _controller.GetAll();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedList = Assert.IsAssignableFrom<IReadOnlyList<string>>(actionResult.Value);
            Assert.Equal(2, returnedList.Count);
            Assert.Contains("Drama", returnedList);
        }
    }
}
