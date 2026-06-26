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

        [Fact]
        public void Constructor_NullNome_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Usuario(Guid.NewGuid(), null!, "email@example.com", "hash", 20, DateTime.UtcNow));
        }

        [Fact]
        public void Constructor_NullEmail_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Usuario(Guid.NewGuid(), "Nome", null!, "hash", 20, DateTime.UtcNow));
        }

        [Fact]
        public void Constructor_NullSenhaHash_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Usuario(Guid.NewGuid(), "Nome", "email@example.com", null!, 20, DateTime.UtcNow));
        }

        [Fact]
        public void Create_WithDataNascimento_ShouldCalculateIdade()
        {
            // 30 dias antes do equivalente a "20 anos atrás, hoje" garante que o aniversário
            // já passou neste ano, tornando o cálculo de idade determinístico em qualquer data de execução.
            var dataNascimento = DateTime.UtcNow.Date.AddYears(-20).AddDays(-30);

            var usuario = Usuario.Create("Bia", "bia@example.com", "hash", dataNascimento: dataNascimento);

            Assert.Equal(20, usuario.Idade);
            Assert.Equal(dataNascimento.Date, usuario.DataNascimento);
        }

        [Fact]
        public void Create_WithDataNascimentoMakingUnderage_ShouldThrow()
        {
            var dataNascimento = DateTime.UtcNow.AddYears(-10);

            Assert.Throws<ArgumentException>(() =>
                Usuario.Create("Joaozinho", "joaozinho@example.com", "hash", dataNascimento: dataNascimento));
        }

        [Fact]
        public void Create_EmptyNome_Throws()
        {
            Assert.Throws<ArgumentException>(() => Usuario.Create("", "valido@example.com", "hash", 20));
        }

        [Fact]
        public void AtualizarPerfil_WithEmail_ShouldNormalizeAndUpdate()
        {
            var usuario = Usuario.Create("Carlos", "carlos@example.com", "hash", 30);

            usuario.AtualizarPerfil(nome: null, email: "  NOVO@Example.com  ", idade: null);

            Assert.Equal("novo@example.com", usuario.Email);
        }

        [Fact]
        public void AtualizarPerfil_WithInvalidEmail_ShouldThrow()
        {
            var usuario = Usuario.Create("Carlos", "carlos@example.com", "hash", 30);

            var ex = Assert.Throws<ArgumentException>(() =>
                usuario.AtualizarPerfil(nome: null, email: "email-invalido", idade: null));
            Assert.Contains("E-mail inválido", ex.Message);
        }

        [Fact]
        public void AtualizarPerfil_WithValidDataNascimento_ShouldUpdateIdadeAndData()
        {
            var usuario = Usuario.Create("Carlos", "carlos@example.com", "hash", 30);
            var novaData = DateTime.UtcNow.AddYears(-22).Date;

            usuario.AtualizarPerfil(nome: null, idade: null, dataNascimento: novaData);

            Assert.Equal(22, usuario.Idade);
            Assert.Equal(novaData, usuario.DataNascimento);
        }

        [Fact]
        public void AtualizarPerfil_WithDataNascimentoMakingUnderage_ShouldThrow()
        {
            var usuario = Usuario.Create("Carlos", "carlos@example.com", "hash", 30);
            var dataNascimento = DateTime.UtcNow.AddYears(-5);

            Assert.Throws<ArgumentException>(() =>
                usuario.AtualizarPerfil(nome: null, idade: null, dataNascimento: dataNascimento));
        }

        [Fact]
        public void AtualizarPerfil_WithAddressFields_ShouldUpdateAll()
        {
            var usuario = Usuario.Create("Carlos", "carlos@example.com", "hash", 30);

            usuario.AtualizarPerfil(
                nome: null,
                idade: null,
                cep: "01000-000",
                logradouro: "Rua A",
                numero: "100",
                complemento: "Apto 1",
                bairro: "Centro",
                cidade: "São Paulo");

            Assert.Equal("01000-000", usuario.Cep);
            Assert.Equal("Rua A", usuario.Logradouro);
            Assert.Equal("100", usuario.Numero);
            Assert.Equal("Apto 1", usuario.Complemento);
            Assert.Equal("Centro", usuario.Bairro);
            Assert.Equal("São Paulo", usuario.Cidade);
        }
    }
}