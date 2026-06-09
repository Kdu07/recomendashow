using System;
using System.Collections.Generic;
using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Mappings;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class DtoMapperTests
    {
        [Fact]
        public void ToDto_Espetaculo_ShouldMapCorrectly()
        {
            // Arrange
            var espetaculo = new Espetaculo(
                Guid.NewGuid(),
                "Nome Espetaculo",
                "Teatro",
                "Musical",
                "Descricao",
                "http://bilheteria",
                DateTime.UtcNow.AddDays(-1),
                DateTime.UtcNow.AddDays(5),
                100m
            );

            // Act
            var dto = espetaculo.ToDto();

            // Assert
            Assert.Equal(espetaculo.Id, dto.Id);
            Assert.Equal(espetaculo.Nome, dto.Nome);
            Assert.Equal(espetaculo.Genero, dto.Genero);
            Assert.Equal(espetaculo.TipoEvento, dto.TipoEvento);
            Assert.Equal(espetaculo.Descricao, dto.Descricao);
            Assert.Equal(espetaculo.UrlBilheteria, dto.UrlBilheteria);
            Assert.Equal(espetaculo.DataInicio, dto.DataInicio);
            Assert.Equal(espetaculo.DataFim, dto.DataFim);
            Assert.Equal(espetaculo.ValorIngresso, dto.ValorIngresso);
            Assert.True(dto.EmCartaz);
        }

        [Fact]
        public void ToDto_Teatro_ShouldMapCorrectly()
        {
            // Arrange
            var teatro = new Teatro(
                Guid.NewGuid(),
                "Nome Teatro",
                "Endereco Teatro",
                "Cidade Teatro",
                "Horario Funcionamento"
            );
            var espetaculo = new Espetaculo(
                Guid.NewGuid(),
                "Nome Espetaculo",
                "Teatro",
                "Musical",
                "Descricao",
                "http://bilheteria",
                DateTime.UtcNow.AddDays(-1),
                DateTime.UtcNow.AddDays(5),
                100m
            );
            teatro.AdicionarEspetaculo(espetaculo);

            // Act
            var dto = teatro.ToDto();

            // Assert
            Assert.Equal(teatro.Id, dto.Id);
            Assert.Equal(teatro.Nome, dto.Nome);
            Assert.Equal(teatro.Endereco, dto.Endereco);
            Assert.Equal(teatro.Cidade, dto.Cidade);
            Assert.Equal(teatro.HorarioFuncionamento, dto.HorarioFuncionamento);
            Assert.Single(dto.Espetaculos);
            Assert.Equal(espetaculo.Id, dto.Espetaculos[0].Id);
        }

        [Fact]
        public void ToDto_Preferencias_ShouldMapCorrectly()
        {
            // Arrange
            var pref = new Preferencias(Guid.NewGuid());
            pref.SetLocalizacao("12345678", "Cidade");
            pref.SetGeneros(new[] { "Rock", "Pop" });
            pref.SetTags(new[] { "tag1" });
            pref.AtualizarFaixaPreco(10m, 200m);
            pref.SetReceberNotificacoes(false);

            // Act
            var dto = pref.ToDto();

            // Assert
            Assert.Equal(pref.UsuarioId, dto.UsuarioId);
            Assert.Equal(pref.Cep, dto.Cep);
            Assert.Equal(pref.Cidade, dto.Cidade);
            Assert.Equal(2, dto.Generos.Count);
            Assert.Single(dto.Tags);
            Assert.Equal(10m, dto.FaixaPrecoMin);
            Assert.Equal(200m, dto.FaixaPrecoMax);
            Assert.False(dto.ReceberNotificacoes);
        }

    }
}
