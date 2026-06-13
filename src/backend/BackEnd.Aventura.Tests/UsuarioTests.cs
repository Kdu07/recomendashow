using System;
using BackEnd.Aventura.Domain;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class UsuarioTests
    {
        [Fact]
        public void Create_ValidParameters_ShouldCreateUsuario()
        {
            var usuario = Usuario.Create("Joao", "joao@example.com", "hash", 25);

            Assert.Equal("joao@example.com", usuario.Email);
            Assert.Equal("Joao", usuario.Nome);
            Assert.Equal(25, usuario.Idade);
            Assert.NotEqual(Guid.Empty, usuario.Id);
        }

        [Fact]
        public void Create_InvalidAge_Throws()
        {
            Assert.Throws<ArgumentException>(() => Usuario.Create("Ana", "ana@example.com", "hash", 12));
        }

        [Fact]
        public void AtualizarPerfil_UpdateNameAndAge_ShouldUpdate()
        {
            var usuario = Usuario.Create("Carlos", "carlos@example.com", "hash", 30);
            usuario.AtualizarPerfil("Carlos Silva", 31);

            Assert.Equal("Carlos Silva", usuario.Nome);
            Assert.Equal(31, usuario.Idade);
        }

        [Fact]
        public void AtualizarPerfil_InvalidAge_Throws()
        {
            var usuario = Usuario.Create("Pedro", "pedro@example.com", "hash", 20);
            Assert.Throws<ArgumentException>(() => usuario.AtualizarPerfil(null, 10));
        }

        [Fact]
        public void AceitarConsentimento_ShouldSetFlagAndTimestamp()
        {
            var usuario = Usuario.Create("Ana", "ana@example.com", "hash", 20);
            Assert.False(usuario.ConsentimentoAceito);
            Assert.Null(usuario.DataConsentimento);

            usuario.AceitarConsentimento();

            Assert.True(usuario.ConsentimentoAceito);
            Assert.NotNull(usuario.DataConsentimento);
        }

        [Fact]
        public void SolicitarExclusao_ShouldSetDeletedAt()
        {
            var usuario = Usuario.Create("Ana", "ana@example.com", "hash", 20);
            Assert.Null(usuario.DeletedAt);

            usuario.SolicitarExclusao();

            Assert.NotNull(usuario.DeletedAt);
        }

    }
}