using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Exceptions;
using BackEnd.Aventura.Services;
using BackEnd.Aventura.Tests.Fakes;
using Moq;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class UsuarioServiceTests
    {
        private readonly FakeUsuarioRepository _repository;
        private readonly Mock<IViaCepService> _viaCepMock;
        private readonly UsuarioService _service;

        public UsuarioServiceTests()
        {
            _repository = new FakeUsuarioRepository();
            _viaCepMock = new Mock<IViaCepService>();
            _service = new UsuarioService(_repository, _viaCepMock.Object);
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
        public async Task Update_ExistingUserWithValidData_ShouldUpdate()
        {
            var usuario = Usuario.Create("João Silva", "joao@example.com", "hash123", 25);
            _repository.Create(usuario);

            var updateRequest = new UsuarioUpdateRequest { Nome = "João Silva Updated", Idade = 26 };

            var response = await _service.Update(usuario.Id, updateRequest);

            Assert.Equal("João Silva Updated", response.Nome);
            Assert.Equal(26, response.Idade);
        }

        [Fact]
        public async Task Update_NonExistentUser_ShouldThrow()
        {
            var usuarioId = Guid.NewGuid();
            var updateRequest = new UsuarioUpdateRequest { Nome = "New Name", Idade = 30 };

            var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.Update(usuarioId, updateRequest));
            Assert.Contains("não encontrado", ex.Message);
        }

        [Fact]
        public async Task Update_UserWithInvalidAge_ShouldThrow()
        {
            var usuario = Usuario.Create("João Silva", "joao@example.com", "hash123", 25);
            _repository.Create(usuario);

            var updateRequest = new UsuarioUpdateRequest { Nome = "João Silva", Idade = 10 };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.Update(usuario.Id, updateRequest));
        }

        [Fact]
        public async Task Update_OnlyName_ShouldUpdateName()
        {
            var usuario = Usuario.Create("João Silva", "joao@example.com", "hash123", 25);
            _repository.Create(usuario);

            var updateRequest = new UsuarioUpdateRequest { Nome = "João Silva Updated", Idade = null };

            var response = await _service.Update(usuario.Id, updateRequest);

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
    }
}
