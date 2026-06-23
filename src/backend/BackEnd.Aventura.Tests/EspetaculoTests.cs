using System;
using BackEnd.Aventura.Domain;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class EspetaculoTests
    {
        [Fact]
        public void PertenceAoGenero_IsCaseInsensitive()
        {
            var e = new Espetaculo(Guid.NewGuid(), "Show", "Rock", "Concerto", "Desc", "http://buy", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), 50);
            Assert.True(e.PertenceAoGenero("rock"));
        }

        [Fact]
        public void EstaEmCartaz_ReturnsTrueWhenBetweenDates()
        {
            var e = new Espetaculo(Guid.NewGuid(), "Show", "Genero", "Tipo", "Desc", "http://buy", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), 30);
            Assert.True(e.EstaEmCartaz());
        }

        [Fact]
        public void ObterUrlCompra_ReturnsUrl()
        {
            var url = "http://comprar";
            var e = new Espetaculo(Guid.NewGuid(), "Show", "Genero", "Tipo", "Desc", url, DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 10);
            Assert.Equal(url, e.ObterUrlCompra());
        }

        [Fact]
        public void EstaEmCartaz_ReturnsFalseWhenBeforeDataInicio()
        {
            var e = new Espetaculo(Guid.NewGuid(), "Show", "Genero", "Tipo", "Desc", "http://buy",
                DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(10), 30);
            Assert.False(e.EstaEmCartaz());
        }

        [Fact]
        public void EstaEmCartaz_ReturnsFalseWhenAfterDataFim()
        {
            var e = new Espetaculo(Guid.NewGuid(), "Show", "Genero", "Tipo", "Desc", "http://buy",
                DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(-5), 30);
            Assert.False(e.EstaEmCartaz());
        }

        [Fact]
        public void Constructor_NullNome_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Espetaculo(Guid.NewGuid(), null!, "Genero", "Tipo", "Desc", "http://buy", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 10));
        }

        [Fact]
        public void Constructor_NullGenero_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Espetaculo(Guid.NewGuid(), "Show", null!, "Tipo", "Desc", "http://buy", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 10));
        }

        [Fact]
        public void Constructor_NullTipoEvento_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Espetaculo(Guid.NewGuid(), "Show", "Genero", null!, "Desc", "http://buy", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 10));
        }

        [Fact]
        public void Constructor_NullDescricao_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Espetaculo(Guid.NewGuid(), "Show", "Genero", "Tipo", null!, "http://buy", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 10));
        }

        [Fact]
        public void Constructor_NullUrlBilheteria_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Espetaculo(Guid.NewGuid(), "Show", "Genero", "Tipo", "Desc", null!, DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 10));
        }
    }
}