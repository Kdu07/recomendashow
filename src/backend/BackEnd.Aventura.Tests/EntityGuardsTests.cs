using System;
using BackEnd.Aventura.Domain;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class EntityGuardsTests
    {
        [Fact]
        public void NotEmpty_EmptyGuid_Throws()
        {
            Assert.Throws<ArgumentException>(() => EntityGuards.NotEmpty(Guid.Empty, "id"));
        }

        [Fact]
        public void NotNullOrWhiteSpace_Invalid_Throws()
        {
            Assert.Throws<ArgumentException>(() => EntityGuards.NotNullOrWhiteSpace("   ", "nome"));
        }

        [Fact]
        public void NotNullOrWhiteSpace_Valid_Trims()
        {
            var result = EntityGuards.NotNullOrWhiteSpace("  teste  ", "campo");
            Assert.Equal("teste", result);
        }
    }
}