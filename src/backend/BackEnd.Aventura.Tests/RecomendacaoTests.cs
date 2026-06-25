using System;
using BackEnd.Aventura.Domain;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class RecomendacaoTests
    {
        [Fact]
        public void Constructor_SetsProperties()
        {
            var e = new Espetaculo(Guid.NewGuid(), "Show", "Genero", "Tipo", "Desc", "http://buy", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 20);
            var r = new Recomendacao(e, 0.75m, "porque");

            Assert.Equal(e, r.Espetaculo);
            Assert.Equal(0.75m, r.Score);
            Assert.Equal("porque", r.Motivo);
        }

        [Fact]
        public void Constructor_NullEspetaculo_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new Recomendacao(null!, 0.5m, "motivo"));
        }
    }
}