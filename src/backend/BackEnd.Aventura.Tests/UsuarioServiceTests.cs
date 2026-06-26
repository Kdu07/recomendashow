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
            var usuario = Usuario.Create("João Silva", "joao@example.com", "hash123", 25);
            _repository.Create(usuario);

            var response = _service.GetById(usuario.Id);

            Assert.Equal("João Silva", response.Nome);
            Assert.Equal("joao@example.com", response.Email);
            Assert.Equal(25, response.Idade);
        }

        [Fact]
        public void GetById_NonExistentUser_ShouldThrow()
        {
            var usuarioId = Guid.NewGuid();

            var ex = Assert.Throws<NotFoundException>(() => _service.GetById(usuarioId));
            Assert.Contains("não encontrado", ex.Message);
        }

        [Fact]
        public void Update_ExistingUserWithValidData_ShouldUpdate()
        {
            var usuario = Usuario.Create("João Silva", "joao@example.com", "hash123", 25);
            _repository.Create(usuario);

            var updateRequest = new UsuarioUpdateRequest
            {
                Nome = "João Silva Updated",
                Idade = 26
            };

            var response = _service.Update(usuario.Id, updateRequest);

            Assert.Equal("João Silva Updated", response.Nome);
            Assert.Equal(26, response.Idade);
        }

        [Fact]
        public void Update_NonExistentUser_ShouldThrow()
        {
            var usuarioId = Guid.NewGuid();
            var updateRequest = new UsuarioUpdateRequest
            {
                Nome = "New Name",
                Idade = 30
            };

            var ex = Assert.Throws<NotFoundException>(() => _service.Update(usuarioId, updateRequest));
            Assert.Contains("não encontrado", ex.Message);
        }

        [Fact]
        public void Update_UserWithInvalidAge_ShouldThrow()
        {
            var usuario = Usuario.Create("João Silva", "joao@example.com", "hash123", 25);
            _repository.Create(usuario);

            var updateRequest = new UsuarioUpdateRequest
            {
                Nome = "João Silva",
                Idade = 10 // Invalid age
            };

            Assert.Throws<ArgumentException>(() => _service.Update(usuario.Id, updateRequest));
        }

        [Fact]
        public void Update_OnlyName_ShouldUpdateName()
        {
            var usuario = Usuario.Create("João Silva", "joao@example.com", "hash123", 25);
            _repository.Create(usuario);

            var updateRequest = new UsuarioUpdateRequest
            {
                Nome = "João Silva Updated",
                Idade = null
            };

            var response = _service.Update(usuario.Id, updateRequest);

            Assert.Equal("João Silva Updated", response.Nome);
            Assert.Equal(25, response.Idade);
        }

        [Fact]
        public void GetById_PreservesAllUserData()
        {
            var usuario = Usuario.Create("Maria Santos", "maria@example.com", "hash456", 30);
            _repository.Create(usuario);

            var response = _service.GetById(usuario.Id);

            Assert.Equal(usuario.Id, response.Id);
            Assert.Equal("Maria Santos", response.Nome);
            Assert.Equal("maria@example.com", response.Email);
            Assert.Equal(30, response.Idade);
            Assert.Equal(usuario.DataCadastro, response.DataCadastro);
        }

        [Fact]
        public void Update_WithNewUniqueEmail_ShouldUpdateEmail()
        {
            var usuario = Usuario.Create("João Silva", "joao@example.com", "hash123", 25);
            _repository.Create(usuario);

            var updateRequest = new UsuarioUpdateRequest
            {
                Email = "joao.novo@example.com"
            };

            var response = _service.Update(usuario.Id, updateRequest);

            Assert.Equal("joao.novo@example.com", response.Email);
        }

        [Fact]
        public void Update_WithEmailAlreadyUsedByAnotherUser_ShouldThrowConflict()
        {
            var usuario1 = Usuario.Create("João Silva", "joao@example.com", "hash123", 25);
            var usuario2 = Usuario.Create("Maria Souza", "maria@example.com", "hash456", 30);
            _repository.Create(usuario1);
            _repository.Create(usuario2);

            var updateRequest = new UsuarioUpdateRequest
            {
                Email = "maria@example.com"
            };

            var ex = Assert.Throws<ConflictException>(() => _service.Update(usuario1.Id, updateRequest));
            Assert.Equal("E-mail já cadastrado.", ex.Message);
        }

        [Fact]
        public void Update_WithOwnSameEmail_ShouldNotThrowConflict()
        {
            var usuario = Usuario.Create("João Silva", "joao@example.com", "hash123", 25);
            _repository.Create(usuario);

            var updateRequest = new UsuarioUpdateRequest
            {
                Email = "joao@example.com",
                Nome = "João Silva Jr"
            };

            var response = _service.Update(usuario.Id, updateRequest);

            Assert.Equal("joao@example.com", response.Email);
            Assert.Equal("João Silva Jr", response.Nome);
        }

        [Fact]
        public void SolicitarExclusao_ExistingUser_ShouldMarkDeleted()
        {
            var usuario = Usuario.Create("João Silva", "joao@example.com", "hash123", 25);
            _repository.Create(usuario);

            _service.SolicitarExclusao(usuario.Id);

            var armazenado = _repository.GetById(usuario.Id);
            Assert.NotNull(armazenado!.DeletedAt);
        }

        [Fact]
        public void SolicitarExclusao_NonExistentUser_ShouldThrow()
        {
            var usuarioId = Guid.NewGuid();

            var ex = Assert.Throws<NotFoundException>(() => _service.SolicitarExclusao(usuarioId));
            Assert.Contains("não encontrado", ex.Message);
        }
    }
}