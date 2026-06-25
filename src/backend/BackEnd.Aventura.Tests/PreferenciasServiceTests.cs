using System;
using System.Collections.Generic;
using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Services;
using BackEnd.Aventura.Tests.Fakes;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class PreferenciasServiceTests
    {
        private readonly FakePreferenciasRepository _repository;
        private readonly PreferenciasService _service;

        public PreferenciasServiceTests()
        {
            _repository = new FakePreferenciasRepository();
            _service = new PreferenciasService(_repository);
        }

        [Fact]
        public void GetByUsuario_ShouldReturnPreferencias()
        {
            var userId = Guid.NewGuid();

            var result = _service.GetByUsuario(userId);

            Assert.NotNull(result);
            Assert.Equal(userId, result.UsuarioId);
        }

        [Fact]
        public void Update_NullDto_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _service.Update(Guid.NewGuid(), null!));
        }

        [Fact]
        public void Update_ValidDto_ShouldUpdateAndSave()
        {
            var userId = Guid.NewGuid();
            var dto = new PreferenciasUpdateDto
            {
                Generos = new List<string> { "Drama", "Comedia" },
                Tags = new List<string> { "musical", "classico" },
                Cidade = "Rio de Janeiro",
                Cep = "22000-000",
                FaixaPrecoMin = 20m,
                FaixaPrecoMax = 150m,
                ReceberNotificacoes = false
            };

            var result = _service.Update(userId, dto);

            Assert.NotNull(result);
            Assert.Equal(userId, result.UsuarioId);
            Assert.Equal("Rio de Janeiro", result.Cidade);
            Assert.Equal("22000-000", result.Cep);
            Assert.Equal(20m, result.FaixaPrecoMin);
            Assert.Equal(150m, result.FaixaPrecoMax);
            Assert.False(result.ReceberNotificacoes);
            Assert.Contains("Drama", result.Generos);
            Assert.Contains("musical", result.Tags);
        }

        [Fact]
        public void Update_WithoutReceberNotificacoes_ShouldKeepCurrentValue()
        {
            var userId = Guid.NewGuid();
            var dto = new PreferenciasUpdateDto
            {
                Cidade = "Curitiba",
                ReceberNotificacoes = null
            };

            var result = _service.Update(userId, dto);

            // Valor default do domínio (true) deve permanecer intocado.
            Assert.True(result.ReceberNotificacoes);
            Assert.Equal("Curitiba", result.Cidade);
        }
    }
}