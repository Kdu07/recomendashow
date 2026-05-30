using System;
using BackEnd.Aventura.Domain;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class TeatroTests
    {
        [Fact]
        public void AdicionarEspetaculo_AddsSuccessfully()
        {
            var t = new Teatro(Guid.NewGuid(), "Teatro X", "Rua A", "Cidade", "9-18");
            var e = new Espetaculo(Guid.NewGuid(), "Show", "Genero", "Tipo", "Desc", "http://buy", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 20);
            t.AdicionarEspetaculo(e);
            Assert.Contains(e, t.Espetaculos);
        }

        [Fact]
        public void AdicionarEspetaculo_Duplicate_Throws()
        {
            var t = new Teatro(Guid.NewGuid(), "Teatro X", "Rua A", "Cidade", "9-18");
            var id = Guid.NewGuid();
            var e1 = new Espetaculo(id, "Show1", "Genero", "Tipo", "Desc", "http://buy", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 20);
            var e2 = new Espetaculo(id, "Show2", "Genero", "Tipo", "Desc", "http://buy", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 20);

            t.AdicionarEspetaculo(e1);
            Assert.Throws<InvalidOperationException>(() => t.AdicionarEspetaculo(e2));
        }
    }
}