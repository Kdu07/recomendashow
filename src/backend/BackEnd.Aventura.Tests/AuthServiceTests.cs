using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Exceptions;
using BackEnd.Aventura.Services;
using BackEnd.Aventura.Tests.Fakes;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class AuthServiceTests
    {
        private readonly FakeUsuarioRepository _usuarioRepository;
        private readonly FakePreferenciasRepository _preferenciasRepository;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _usuarioRepository = new FakeUsuarioRepository();
            _preferenciasRepository = new FakePreferenciasRepository();
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "test-jwt-key-with-at-least-32-chars!",
                    ["Jwt:Issuer"] = "BackEnd.Aventura",
                    ["Jwt:Audience"] = "BackEnd.AventuraClient",
                    ["Jwt:ExpiryMinutes"] = "60",
                })
                .Build();
            _service = new AuthService(_usuarioRepository, _preferenciasRepository, config);
        }

        [Fact]
        public void Register_ValidData_ShouldCreateUser()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Nome = "João Silva",
                Email = "joao@example.com",
                Senha = "Senha@123",
                Idade = 25
            };

            // Act
            var response = _service.Register(request);

            // Assert
            Assert.NotEqual(Guid.Empty, response.UsuarioId);
            Assert.Equal("João Silva", response.Nome);
            Assert.Equal("joao@example.com", response.Email);
        }

        [Fact]
        public void Register_DuplicateEmail_ShouldThrow()
        {
            // Arrange
            var request1 = new RegisterRequest
            {
                Nome = "João Silva",
                Email = "joao@example.com",
                Senha = "Senha@123",
                Idade = 25
            };

            var request2 = new RegisterRequest
            {
                Nome = "Maria Silva",
                Email = "joao@example.com",
                Senha = "Senha@456",
                Idade = 22
            };

            _service.Register(request1);

            // Act & Assert
            var ex = Assert.Throws<ConflictException>(() => _service.Register(request2));
            Assert.Equal("E-mail já cadastrado.", ex.Message);
        }

        [Fact]
        public void Register_InvalidEmail_ShouldThrow()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Nome = "João Silva",
                Email = "invalid-email",
                Senha = "Senha@123",
                Idade = 25
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.Register(request));
        }

        [Fact]
        public void Register_PasswordTooShort_ShouldThrow()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Nome = "João Silva",
                Email = "joao@example.com",
                Senha = "123",
                Idade = 25
            };

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _service.Register(request));
            Assert.Contains("no mínimo 6 caracteres", ex.Message);
        }

        [Fact]
        public void Register_UnderageUser_ShouldThrow()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Nome = "João Silva",
                Email = "joao@example.com",
                Senha = "Senha@123",
                Idade = 10
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.Register(request));
        }

        [Fact]
        public void Login_ValidCredentials_ShouldReturnAuthResponse()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Nome = "João Silva",
                Email = "joao@example.com",
                Senha = "Senha@123",
                Idade = 25
            };

            _service.Register(registerRequest);

            var loginRequest = new LoginRequest
            {
                Email = "joao@example.com",
                Senha = "Senha@123"
            };

            // Act
            var response = _service.Login(loginRequest);

            // Assert
            Assert.Equal("João Silva", response.Nome);
            Assert.Equal("joao@example.com", response.Email);
        }

        [Fact]
        public void Login_InvalidEmail_ShouldThrow()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "nonexistent@example.com",
                Senha = "Senha@123"
            };

            // Act & Assert
            Assert.Throws<UnauthorizedAccessException>(() => _service.Login(loginRequest));
        }

        [Fact]
        public void Login_InvalidPassword_ShouldThrow()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Nome = "João Silva",
                Email = "joao@example.com",
                Senha = "Senha@123",
                Idade = 25
            };

            _service.Register(registerRequest);

            var loginRequest = new LoginRequest
            {
                Email = "joao@example.com",
                Senha = "WrongPassword"
            };

            // Act & Assert
            Assert.Throws<UnauthorizedAccessException>(() => _service.Login(loginRequest));
        }

        [Fact]
        public void Register_EmptyName_ShouldThrow()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Nome = "",
                Email = "joao@example.com",
                Senha = "Senha@123",
                Idade = 25
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.Register(request));
        }
    }
}
