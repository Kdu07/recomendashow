using System;
using System.Collections.Generic;
using BackEnd.Aventura.Controller;
using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class RecomendacaoControllerTests
    {
        private readonly Mock<IRecomendacaoService> _serviceMock;
        private readonly RecomendacaoController _controller;

        public RecomendacaoControllerTests()
        {
            _serviceMock = new Mock<IRecomendacaoService>();
            _controller = new RecomendacaoController(_serviceMock.Object);
        }

        [Fact]
        public void GetRecomendacoes_ShouldReturnOkAndList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var list = new List<RecomendacaoItemDto>
            {
                new RecomendacaoItemDto
                {
                    Espetaculo = new EspetaculoDto { Id = Guid.NewGuid(), Nome = "Espetaculo A" },
                    Score = 0.9m,
                    Motivo = "Recomendação"
                }
            };
            _serviceMock.Setup(s => s.GetRecomendacoes(userId, 10)).Returns(list);

            // Act
            var result = _controller.GetRecomendacoes(userId, 10);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedList = Assert.IsAssignableFrom<IReadOnlyList<RecomendacaoItemDto>>(actionResult.Value);
            Assert.Single(returnedList);
            Assert.Equal(0.9m, returnedList[0].Score);
        }
    }
}
