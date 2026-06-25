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
            var request = new RegisterRequest
            {
                Nome = "João Silva",
                Email = "joao@example.com",
                Senha = "Senha@123",
                Idade = 25,
                ConsentimentoAceito = true
            };

            var response = _service.Register(request);

            Assert.NotEqual(Guid.Empty, response.UsuarioId);
            Assert.Equal("João Silva", response.Nome);
            Assert.Equal("joao@example.com", response.Email);
        }

        [Fact]
        public void Register_DuplicateEmail_ShouldThrow()
        {
            var request1 = new RegisterRequest
            {
                Nome = "João Silva",
                Email = "joao@example.com",
                Senha = "Senha@123",
                Idade = 25,
                ConsentimentoAceito = true
            };

            var request2 = new RegisterRequest
            {
                Nome = "Maria Silva",
                Email = "joao@example.com",
                Senha = "Senha@456",
                Idade = 22,
                ConsentimentoAceito = true
            };

            _service.Register(request1);

            var ex = Assert.Throws<ConflictException>(() => _service.Register(request2));
            Assert.Equal("E-mail já cadastrado.", ex.Message);
        }

        [Fact]
        public void Register_InvalidEmail_ShouldThrow()
        {
            var request = new RegisterRequest
            {
                Nome = "João Silva",
                Email = "invalid-email",
                Senha = "Senha@123",
                Idade = 25,
                ConsentimentoAceito = true
            };

            Assert.Throws<ArgumentException>(() => _service.Register(request));
        }

        [Fact]
        public void Register_PasswordTooShort_ShouldThrow()
        {
            var request = new RegisterRequest
            {
                Nome = "João Silva",
                Email = "joao@example.com",
                Senha = "123",
                Idade = 25,
                ConsentimentoAceito = true
            };

            var ex = Assert.Throws<ArgumentException>(() => _service.Register(request));
            Assert.Contains("no mínimo 6 caracteres", ex.Message);
        }

        [Fact]
        public void Register_UnderageUser_ShouldThrow()
        {
            var request = new RegisterRequest
            {
                Nome = "João Silva",
                Email = "joao@example.com",
                Senha = "Senha@123",
                Idade = 10,
                ConsentimentoAceito = true
            };

            Assert.Throws<ArgumentException>(() => _service.Register(request));
        }

        [Fact]
        public void Login_ValidCredentials_ShouldReturnAuthResponse()
        {
            var registerRequest = new RegisterRequest
            {
                Nome = "João Silva",
                Email = "joao@example.com",
                Senha = "Senha@123",
                Idade = 25,
                ConsentimentoAceito = true
            };

            _service.Register(registerRequest);

            var loginRequest = new LoginRequest
            {
                Email = "joao@example.com",
                Senha = "Senha@123"
            };

            var response = _service.Login(loginRequest);

            Assert.Equal("João Silva", response.Nome);
            Assert.Equal("joao@example.com", response.Email);
        }

        [Fact]
        public void Login_InvalidEmail_ShouldThrow()
        {
            var loginRequest = new LoginRequest
            {
                Email = "nonexistent@example.com",
                Senha = "Senha@123"
            };

            Assert.Throws<UnauthorizedAccessException>(() => _service.Login(loginRequest));
        }

        [Fact]
        public void Login_InvalidPassword_ShouldThrow()
        {
            var registerRequest = new RegisterRequest
            {
                Nome = "João Silva",
                Email = "joao@example.com",
                Senha = "Senha@123",
                Idade = 25,
                ConsentimentoAceito = true
            };

            _service.Register(registerRequest);

            var loginRequest = new LoginRequest
            {
                Email = "joao@example.com",
                Senha = "WrongPassword"
            };

            Assert.Throws<UnauthorizedAccessException>(() => _service.Login(loginRequest));
        }

        [Fact]
        public void Register_EmptyName_ShouldThrow()
        {
            var request = new RegisterRequest
            {
                Nome = "",
                Email = "joao@example.com",
                Senha = "Senha@123",
                Idade = 25,
                ConsentimentoAceito = true
            };

            Assert.Throws<ArgumentException>(() => _service.Register(request));
        }

        [Fact]
        public void Register_NullRequest_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _service.Register(null!));
        }

        [Fact]
        public void Register_WithoutConsentimento_ShouldThrow()
        {
            var request = new RegisterRequest
            {
                Nome = "João Silva",
                Email = "joao@example.com",
                Senha = "Senha@123",
                Idade = 25,
                ConsentimentoAceito = false
            };

            var ex = Assert.Throws<ArgumentException>(() => _service.Register(request));
            Assert.Contains("consentimento", ex.Message);
        }

        [Fact]
        public void Login_EmptyEmail_Throws()
        {
            var loginRequest = new LoginRequest { Email = "", Senha = "Senha@123" };

            Assert.Throws<ArgumentException>(() => _service.Login(loginRequest));
        }

        [Fact]
        public void Login_EmptySenha_Throws()
        {
            var loginRequest = new LoginRequest { Email = "joao@example.com", Senha = "" };

            Assert.Throws<ArgumentException>(() => _service.Login(loginRequest));
        }

        [Fact]
        public void Login_DeletedAccount_ShouldThrowUnauthorized()
        {
            var registerRequest = new RegisterRequest
            {
                Nome = "João Silva",
                Email = "joao@example.com",
                Senha = "Senha@123",
                Idade = 25,
                ConsentimentoAceito = true
            };
            _service.Register(registerRequest);

            var usuario = _usuarioRepository.GetByEmail("joao@example.com")!;
            usuario.SolicitarExclusao();

            var loginRequest = new LoginRequest
            {
                Email = "joao@example.com",
                Senha = "Senha@123"
            };

            var ex = Assert.Throws<UnauthorizedAccessException>(() => _service.Login(loginRequest));
            Assert.Contains("encerrada", ex.Message);
        }

        [Fact]
        public void Register_EmptyPassword_ShouldThrow()
        {
            var request = new RegisterRequest
            {
                Nome = "João Silva",
                Email = "joao@example.com",
                Senha = "   ",
                Idade = 25,
                ConsentimentoAceito = true
            };

            var ex = Assert.Throws<ArgumentException>(() => _service.Register(request));
            Assert.Contains("no mínimo 6 caracteres", ex.Message);
        }

        [Fact]
        public void Register_WithoutIdade_ShouldSucceed()
        {
            // Idade nula não deve disparar a validação de idade mínima.
            var request = new RegisterRequest
            {
                Nome = "Sem Idade",
                Email = "semidade@example.com",
                Senha = "Senha@123",
                Idade = null,
                ConsentimentoAceito = true
            };

            var response = _service.Register(request);

            Assert.Equal("semidade@example.com", response.Email);
        }

        [Fact]
        public void Login_UsesDefaultExpiryMinutes_WhenNotConfigured()
        {
            // Configuração sem Jwt:ExpiryMinutes deve cair no fallback "60".
            var usuarioRepository = new FakeUsuarioRepository();
            var preferenciasRepository = new FakePreferenciasRepository();
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "another-test-jwt-key-with-32-chars!!",
                    ["Jwt:Issuer"] = "BackEnd.Aventura",
                    ["Jwt:Audience"] = "BackEnd.AventuraClient",
                })
                .Build();
            var service = new AuthService(usuarioRepository, preferenciasRepository, config);

            var registerRequest = new RegisterRequest
            {
                Nome = "Sem Expiry",
                Email = "semexpiry@example.com",
                Senha = "Senha@123",
                Idade = 25,
                ConsentimentoAceito = true
            };

            var response = service.Register(registerRequest);

            Assert.False(string.IsNullOrWhiteSpace(response.Token));
        }

        [Fact]
        public void Login_UsesEnvironmentVariable_WhenJwtKeyConfiguredViaEnv()
        {
            // JWT_KEY via variável de ambiente deve ter prioridade sobre a configuração.
            var originalValue = Environment.GetEnvironmentVariable("JWT_KEY");
            try
            {
                Environment.SetEnvironmentVariable("JWT_KEY", "env-test-jwt-key-with-32-characters!!");

                var usuarioRepository = new FakeUsuarioRepository();
                var preferenciasRepository = new FakePreferenciasRepository();
                var config = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Jwt:Issuer"] = "BackEnd.Aventura",
                        ["Jwt:Audience"] = "BackEnd.AventuraClient",
                        ["Jwt:ExpiryMinutes"] = "60",
                    })
                    .Build();
                var service = new AuthService(usuarioRepository, preferenciasRepository, config);

                var registerRequest = new RegisterRequest
                {
                    Nome = "Via Env",
                    Email = "viaenv@example.com",
                    Senha = "Senha@123",
                    Idade = 25,
                    ConsentimentoAceito = true
                };

                var response = service.Register(registerRequest);

                Assert.False(string.IsNullOrWhiteSpace(response.Token));
            }
            finally
            {
                Environment.SetEnvironmentVariable("JWT_KEY", originalValue);
            }
        }
    }
}