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
    }
}