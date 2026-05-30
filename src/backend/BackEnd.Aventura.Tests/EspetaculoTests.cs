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
    }
}