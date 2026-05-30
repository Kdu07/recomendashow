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
    public class TeatroControllerTests
    {
        private readonly Mock<ITeatroService> _serviceMock;
        private readonly TeatroController _controller;

        public TeatroControllerTests()
        {
            _serviceMock = new Mock<ITeatroService>();
            _controller = new TeatroController(_serviceMock.Object);
        }

        private Teatro CreateTeatro(string nome)
        {
            return new Teatro(
                Guid.NewGuid(),
                nome,
                "Endereco",
                "Cidade",
                "Horario"
            );
        }

        [Fact]
        public void GetAll_ShouldReturnOkAndList()
        {
            // Arrange
            var list = new List<Teatro> { CreateTeatro("A"), CreateTeatro("B") };
            _serviceMock.Setup(s => s.GetAll()).Returns(list);

            // Act
            var result = _controller.GetAll();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedList = Assert.IsAssignableFrom<IReadOnlyList<TeatroDto>>(actionResult.Value);
            Assert.Equal(2, returnedList.Count);
        }

        [Fact]
        public void GetById_ShouldReturnOkAndDto()
        {
            // Arrange
            var teatro = CreateTeatro("A");
            _serviceMock.Setup(s => s.GetById(teatro.Id)).Returns(teatro);

            // Act
            var result = _controller.GetById(teatro.Id);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<TeatroDto>(actionResult.Value);
            Assert.Equal(teatro.Id, returnedDto.Id);
        }

        [Fact]
        public void GetEspetaculos_ShouldReturnOkAndEspetaculosList()
        {
            // Arrange
            var teatroId = Guid.NewGuid();
            var list = new List<Espetaculo>
            {
                new Espetaculo(
                    Guid.NewGuid(),
                    "Espetaculo A",
                    "Drama",
                    "Show",
                    "desc",
                    "url",
                    DateTime.UtcNow.AddDays(-1),
                    DateTime.UtcNow.AddDays(5),
                    50m
                )
            };
            _serviceMock.Setup(s => s.GetEspetaculos(teatroId)).Returns(list);

            // Act
            var result = _controller.GetEspetaculos(teatroId);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedList = Assert.IsAssignableFrom<IReadOnlyList<EspetaculoDto>>(actionResult.Value);
            Assert.Single(returnedList);
            Assert.Equal("Espetaculo A", returnedList[0].Nome);
        }
    }
}
