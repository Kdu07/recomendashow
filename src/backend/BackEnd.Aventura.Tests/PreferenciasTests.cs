using System;
using System.Collections.Generic;
using BackEnd.Aventura.Domain;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class PreferenciasTests
    {
        [Fact]
        public void Constructor_SetsDefaults()
        {
            var userId = Guid.NewGuid();
            var p = new Preferencias(userId);

            Assert.Equal(userId, p.UsuarioId);
            Assert.True(p.ReceberNotificacoes);
            Assert.NotEqual(Guid.Empty, p.Id);
        }

        [Fact]
        public void SetGeneros_AddsUniqueAndTrim()
        {
            var p = new Preferencias(Guid.NewGuid());
            p.SetGeneros(new[] { "Rock", " rock ", "" });

            Assert.Single(p.Generos);
            Assert.Contains("Rock", p.Generos);
        }

        [Fact]
        public void AtualizarFaixaPreco_Invalid_Throws()
        {
            var p = new Preferencias(Guid.NewGuid());
            Assert.Throws<ArgumentException>(() => p.AtualizarFaixaPreco(100, 50));
        }

        [Fact]
        public void SetLocalizacao_TrimsAndNulls()
        {
            var p = new Preferencias(Guid.NewGuid());
            p.SetLocalizacao(" 12345 ", " Sao Paulo ");

            Assert.Equal("12345", p.Cep);
            Assert.Equal("Sao Paulo", p.Cidade);
        }

        [Fact]
        public void SetGeneros_Null_Throws()
        {
            var p = new Preferencias(Guid.NewGuid());
            Assert.Throws<ArgumentNullException>(() => p.SetGeneros(null!));
        }

        [Fact]
        public void SetTags_AddsUniqueAndTrim()
        {
            var p = new Preferencias(Guid.NewGuid());
            p.SetTags(new[] { "Musical", " musical ", "  ", "Classico" });

            Assert.Equal(2, p.Tags.Count);
            Assert.Contains("Musical", p.Tags);
            Assert.Contains("Classico", p.Tags);
        }

        [Fact]
        public void SetTags_Null_Throws()
        {
            var p = new Preferencias(Guid.NewGuid());
            Assert.Throws<ArgumentNullException>(() => p.SetTags(null!));
        }

        [Fact]
        public void AtualizarFaixaPreco_ValidRange_ShouldUpdate()
        {
            var p = new Preferencias(Guid.NewGuid());
            p.AtualizarFaixaPreco(50m, 200m);

            Assert.Equal(50m, p.FaixaPrecoMin);
            Assert.Equal(200m, p.FaixaPrecoMax);
        }

        [Fact]
        public void AtualizarFaixaPreco_OnlyMin_ShouldUpdate()
        {
            var p = new Preferencias(Guid.NewGuid());
            p.AtualizarFaixaPreco(50m, null);

            Assert.Equal(50m, p.FaixaPrecoMin);
            Assert.Null(p.FaixaPrecoMax);
        }

        [Fact]
        public void SetReceberNotificacoes_ShouldUpdateValue()
        {
            var p = new Preferencias(Guid.NewGuid());
            Assert.True(p.ReceberNotificacoes);

            p.SetReceberNotificacoes(false);

            Assert.False(p.ReceberNotificacoes);
        }

        [Fact]
        public void SetLocalizacao_NullValues_ShouldKeepCurrent()
        {
            var p = new Preferencias(Guid.NewGuid());
            p.SetLocalizacao("12345", "Sao Paulo");

            p.SetLocalizacao(null, null);

            Assert.Equal("12345", p.Cep);
            Assert.Equal("Sao Paulo", p.Cidade);
        }

        [Fact]
        public void SetLocalizacao_EmptyValues_ShouldClear()
        {
            var p = new Preferencias(Guid.NewGuid());
            p.SetLocalizacao("12345", "Sao Paulo");

            p.SetLocalizacao("   ", "   ");

            Assert.Null(p.Cep);
            Assert.Null(p.Cidade);
        }

        [Fact]
        public void FromData_ShouldBuildPreferenciasWithAllFields()
        {
            var id = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var dataAtualizacao = DateTime.UtcNow.AddDays(-1);

            var p = Preferencias.FromData(
                id,
                usuarioId,
                cep: " 04567-000 ",
                cidade: " Rio de Janeiro ",
                generos: new[] { "Drama", "Comedia" },
                tags: new[] { "infantil" },
                faixaPrecoMin: 10m,
                faixaPrecoMax: 100m,
                receberNotificacoes: false,
                dataAtualizacao: dataAtualizacao);

            Assert.Equal(id, p.Id);
            Assert.Equal(usuarioId, p.UsuarioId);
            Assert.Equal("04567-000", p.Cep);
            Assert.Equal("Rio de Janeiro", p.Cidade);
            Assert.Contains("Drama", p.Generos);
            Assert.Contains("Comedia", p.Generos);
            Assert.Contains("infantil", p.Tags);
            Assert.Equal(10m, p.FaixaPrecoMin);
            Assert.Equal(100m, p.FaixaPrecoMax);
            Assert.False(p.ReceberNotificacoes);
        }

        [Fact]
        public void FromData_WithNullOrWhiteSpaceCepCidade_ShouldNormalizeToNull()
        {
            var p = Preferencias.FromData(
                Guid.NewGuid(),
                Guid.NewGuid(),
                cep: "   ",
                cidade: null,
                generos: Array.Empty<string>(),
                tags: Array.Empty<string>(),
                faixaPrecoMin: null,
                faixaPrecoMax: null,
                receberNotificacoes: true,
                dataAtualizacao: DateTime.UtcNow);

            Assert.Null(p.Cep);
            Assert.Null(p.Cidade);
        }
    }
}