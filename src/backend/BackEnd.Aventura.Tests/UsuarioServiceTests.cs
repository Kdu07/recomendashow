using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Exceptions;
using BackEnd.Aventura.Services;
using BackEnd.Aventura.Tests.Fakes;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class UsuarioServiceTests
    {
        private readonly FakeUsuarioRepository _repository;
        private readonly UsuarioService _service;

        public UsuarioServiceTests()
        {
            _repository = new FakeUsuarioRepository();
            _service = new UsuarioService(_repository);
        }

        [Fact]
        public void GetById_ExistingUser_ShouldReturnUser()
        {
            // Arrange
            var usuario = Usuario.Create("João Silva", "joao@example.com", "hash123", 25);
            _repository.Create(usuario);

            // Act
            var response = _service.GetById(usuario.Id);

            // Assert
            Assert.Equal("João Silva", response.Nome);
            Assert.Equal("joao@example.com", response.Email);
            Assert.Equal(25, response.Idade);
        }

        [Fact]
        public void GetById_NonExistentUser_ShouldThrow()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();

            // Act & Assert
            var ex = Assert.Throws<NotFoundException>(() => _service.GetById(usuarioId));
            Assert.Contains("não encontrado", ex.Message);
        }

        [Fact]
        public void Update_ExistingUserWithValidData_ShouldUpdate()
        {
            // Arrange
            var usuario = Usuario.Create("João Silva", "joao@example.com", "hash123", 25);
            _repository.Create(usuario);

            var updateRequest = new UsuarioUpdateRequest
            {
                Nome = "João Silva Updated",
                Idade = 26
            };

            // Act
            var response = _service.Update(usuario.Id, updateRequest);

            // Assert
            Assert.Equal("João Silva Updated", response.Nome);
            Assert.Equal(26, response.Idade);
        }

        [Fact]
        public void Update_NonExistentUser_ShouldThrow()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var updateRequest = new UsuarioUpdateRequest
            {
                Nome = "New Name",
                Idade = 30
            };

            // Act & Assert
            var ex = Assert.Throws<NotFoundException>(() => _service.Update(usuarioId, updateRequest));
            Assert.Contains("não encontrado", ex.Message);
        }

        [Fact]
        public void Update_UserWithInvalidAge_ShouldThrow()
        {
            // Arrange
            var usuario = Usuario.Create("João Silva", "joao@example.com", "hash123", 25);
            _repository.Create(usuario);

            var updateRequest = new UsuarioUpdateRequest
            {
                Nome = "João Silva",
                Idade = 10 // Invalid age
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.Update(usuario.Id, updateRequest));
        }

        [Fact]
        public void Update_OnlyName_ShouldUpdateName()
        {
            // Arrange
            var usuario = Usuario.Create("João Silva", "joao@example.com", "hash123", 25);
            _repository.Create(usuario);

            var updateRequest = new UsuarioUpdateRequest
            {
                Nome = "João Silva Updated",
                Idade = null
            };

            // Act
            var response = _service.Update(usuario.Id, updateRequest);

            // Assert
            Assert.Equal("João Silva Updated", response.Nome);
            Assert.Equal(25, response.Idade);
        }

        [Fact]
        public void GetById_PreservesAllUserData()
        {
            // Arrange
            var usuario = Usuario.Create("Maria Santos", "maria@example.com", "hash456", 30);
            _repository.Create(usuario);

            // Act
            var response = _service.GetById(usuario.Id);

            // Assert
            Assert.Equal(usuario.Id, response.Id);
            Assert.Equal("Maria Santos", response.Nome);
            Assert.Equal("maria@example.com", response.Email);
            Assert.Equal(30, response.Idade);
            Assert.Equal(usuario.DataCadastro, response.DataCadastro);
        }
    }
}
