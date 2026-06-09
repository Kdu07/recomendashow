using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Exceptions;
using BackEnd.Aventura.Infrastructure;
using BackEnd.Aventura.Repository;
using BackEnd.Aventura.Services;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Diagnostics;
using Testcontainers.PostgreSql;
using Xunit;

namespace BackEnd.Aventura.IntegrationTests
{
    public sealed class AuthIntegrationTests : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("aventura_auth_tests")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        public async Task InitializeAsync()
        {
            await _postgres.StartAsync();
            await using var connection = new NpgsqlConnection(_postgres.GetConnectionString());
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(AuthSchemaSql, connection);
            await command.ExecuteNonQueryAsync();
        }

        public async Task DisposeAsync()
        {
            await _postgres.DisposeAsync();
        }

        [Fact]
        public void Register_ComDadosValidos_PersisteUsuarioCriaPreferenciasERetornaToken()
        {
            var service = CreateService();

            var response = service.Register(new RegisterRequest
            {
                Nome = "Nicholas Millani",
                Email = "nicholas@example.com",
                Senha = "Senha@123",
                Idade = 21,
                Cep = "01310-100",
                Logradouro = "Avenida Paulista",
                Numero = "1000",
                Bairro = "Bela Vista",
                Cidade = "Sao Paulo",
                ConsentimentoAceito = true
            });

            Assert.NotEqual(Guid.Empty, response.UsuarioId);
            Assert.Equal("nicholas@example.com", response.Email);
            Assert.False(string.IsNullOrWhiteSpace(response.Token));

            using var connection = new NpgsqlConnection(_postgres.GetConnectionString());
            connection.Open();

            using var usuarioCommand = new NpgsqlCommand(
                "SELECT nome, consentimento_aceito, cep, cidade FROM usuarios WHERE id = @Id",
                connection);
            usuarioCommand.Parameters.AddWithValue("@Id", response.UsuarioId);

            using var usuario = usuarioCommand.ExecuteReader();
            Assert.True(usuario.Read());
            Assert.Equal("Nicholas Millani", usuario.GetString(0));
            Assert.True(usuario.GetBoolean(1));
            Assert.Equal("01310-100", usuario.GetString(2));
            Assert.Equal("Sao Paulo", usuario.GetString(3));
            usuario.Close();

            using var preferenciasCommand = new NpgsqlCommand(
                "SELECT COUNT(*) FROM preferencias WHERE usuario_id = @UsuarioId",
                connection);
            preferenciasCommand.Parameters.AddWithValue("@UsuarioId", response.UsuarioId);
            Assert.Equal(1L, preferenciasCommand.ExecuteScalar());
        }

        [Fact]
        public void Register_ComEmailDuplicado_LancaConflictExceptionENaoDuplicaUsuario()
        {
            var service = CreateService();
            service.Register(CreateValidRegisterRequest("duplicado@example.com"));

            var duplicado = new RegisterRequest
            {
                Nome = "Outro Usuario",
                Email = "duplicado@example.com",
                Senha = "Senha@456",
                Idade = 24,
                ConsentimentoAceito = true
            };

            var exception = Assert.Throws<ConflictException>(() => service.Register(duplicado));
            Assert.Contains("cadastrado", exception.Message);

            using var connection = new NpgsqlConnection(_postgres.GetConnectionString());
            connection.Open();
            using var command = new NpgsqlCommand(
                "SELECT COUNT(*) FROM usuarios WHERE LOWER(email) = LOWER(@Email)",
                connection);
            command.Parameters.AddWithValue("@Email", "duplicado@example.com");

            Assert.Equal(1L, command.ExecuteScalar());
        }

        [Fact]
        public void Register_SemConsentimento_LancaArgumentExceptionENaoPersisteUsuario()
        {
            var service = CreateService();
            var request = CreateValidRegisterRequest("sem-consentimento@example.com");
            request.ConsentimentoAceito = false;

            var exception = Assert.Throws<ArgumentException>(() => service.Register(request));

            Assert.Contains("consentimento", exception.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(0L, CountUsuariosByEmail("sem-consentimento@example.com"));
        }

        [Fact]
        public void Register_ComEmailInvalido_LancaArgumentExceptionENaoPersisteUsuario()
        {
            var service = CreateService();
            var request = CreateValidRegisterRequest("email-invalido");

            var exception = Assert.Throws<ArgumentException>(() => service.Register(request));

            Assert.Contains("E-mail", exception.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(0L, CountUsuariosByEmail("email-invalido"));
        }

        [Fact]
        public void Register_ComSenhaCurta_LancaArgumentExceptionENaoPersisteUsuario()
        {
            var service = CreateService();
            var request = CreateValidRegisterRequest("senha-curta@example.com");
            request.Senha = "123";

            var exception = Assert.Throws<ArgumentException>(() => service.Register(request));

            Assert.Contains("6 caracteres", exception.Message);
            Assert.Equal(0L, CountUsuariosByEmail("senha-curta@example.com"));
        }

        [Fact]
        public void Register_ComUsuarioMenorDeIdade_LancaArgumentExceptionENaoPersisteUsuario()
        {
            var service = CreateService();
            var request = CreateValidRegisterRequest("menor@example.com");
            request.Idade = 12;

            var exception = Assert.Throws<ArgumentException>(() => service.Register(request));

            Assert.Contains("13 anos", exception.Message);
            Assert.Equal(0L, CountUsuariosByEmail("menor@example.com"));
        }

        [Fact]
        public void Login_ComUsuarioCadastrado_RetornaDadosDoUsuarioEToken()
        {
            var service = CreateService();
            service.Register(CreateValidRegisterRequest("login@example.com"));

            var response = service.Login(new LoginRequest
            {
                Email = "login@example.com",
                Senha = "Senha@123"
            });

            Assert.Equal("Usuario Teste", response.Nome);
            Assert.Equal("login@example.com", response.Email);
            Assert.False(string.IsNullOrWhiteSpace(response.Token));
        }

        [Fact]
        public void Login_ComEmailNaoCadastrado_LancaUnauthorizedAccessException()
        {
            var service = CreateService();

            var exception = Assert.Throws<UnauthorizedAccessException>(() => service.Login(new LoginRequest
            {
                Email = "nao-existe@example.com",
                Senha = "Senha@123"
            }));

            Assert.Contains("inv", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Register_ComDadosValidos_DeveResponderDentroDoLimiteDeLatencia()
        {
            var service = CreateService();
            var stopwatch = Stopwatch.StartNew();

            service.Register(CreateValidRegisterRequest("latencia-cadastro@example.com"));

            stopwatch.Stop();
            Assert.True(
                stopwatch.ElapsedMilliseconds <= 1500,
                $"Cadastro excedeu o limite de latencia: {stopwatch.ElapsedMilliseconds} ms.");
        }

        [Fact]
        public void Login_ComUsuarioCadastrado_DeveResponderDentroDoLimiteDeLatencia()
        {
            var service = CreateService();
            service.Register(CreateValidRegisterRequest("latencia-login@example.com"));

            var stopwatch = Stopwatch.StartNew();
            service.Login(new LoginRequest
            {
                Email = "latencia-login@example.com",
                Senha = "Senha@123"
            });
            stopwatch.Stop();

            Assert.True(
                stopwatch.ElapsedMilliseconds <= 500,
                $"Login excedeu o limite de latencia: {stopwatch.ElapsedMilliseconds} ms.");
        }

        private AuthService CreateService()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = _postgres.GetConnectionString(),
                    ["Jwt:Key"] = "test-jwt-key-with-at-least-32-chars!",
                    ["Jwt:Issuer"] = "BackEnd.Aventura",
                    ["Jwt:Audience"] = "BackEnd.AventuraClient",
                    ["Jwt:ExpiryMinutes"] = "60",
                })
                .Build();

            var connectionFactory = new DbConnectionFactory(configuration);
            return new AuthService(
                new UsuarioRepository(connectionFactory),
                new PreferenciasRepository(connectionFactory),
                configuration);
        }

        private long CountUsuariosByEmail(string email)
        {
            using var connection = new NpgsqlConnection(_postgres.GetConnectionString());
            connection.Open();
            using var command = new NpgsqlCommand(
                "SELECT COUNT(*) FROM usuarios WHERE LOWER(email) = LOWER(@Email)",
                connection);
            command.Parameters.AddWithValue("@Email", email);

            return (long)(command.ExecuteScalar() ?? 0L);
        }

        private static RegisterRequest CreateValidRegisterRequest(string email) => new()
        {
            Nome = "Usuario Teste",
            Email = email,
            Senha = "Senha@123",
            Idade = 22,
            ConsentimentoAceito = true
        };

        private const string AuthSchemaSql = """
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

CREATE TABLE IF NOT EXISTS usuarios(
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nome VARCHAR(100) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    senha_hash TEXT NOT NULL,
    idade INT CHECK(idade >= 13),
    data_cadastro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    cep VARCHAR(9),
    logradouro VARCHAR(255),
    numero VARCHAR(10),
    complemento VARCHAR(255),
    bairro VARCHAR(100),
    cidade VARCHAR(100),
    consentimento_aceito BOOLEAN NOT NULL DEFAULT FALSE,
    data_consentimento TIMESTAMP,
    deleted_at TIMESTAMP,
    anonimizado BOOLEAN NOT NULL DEFAULT FALSE,
    ml_id SERIAL
);

CREATE INDEX IF NOT EXISTS idx_usuario_email ON usuarios(email);
CREATE UNIQUE INDEX IF NOT EXISTS idx_usuarios_ml_id ON usuarios(ml_id);

CREATE TABLE IF NOT EXISTS generos(
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nome VARCHAR(50) UNIQUE NOT NULL
);

CREATE TABLE IF NOT EXISTS preferencias(
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    usuario_id UUID UNIQUE,
    cidade VARCHAR(100),
    cep VARCHAR(9),
    tags TEXT[],
    faixa_preco_min DECIMAL(10,2),
    faixa_preco_max DECIMAL(10,2),
    receber_notificacoes BOOLEAN DEFAULT TRUE,
    data_atualizacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_preferencia_usuario
        FOREIGN KEY(usuario_id)
        REFERENCES usuarios(id)
        ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS usuario_genero_favorito(
    usuario_id UUID,
    genero_id UUID,
    PRIMARY KEY(usuario_id, genero_id),
    FOREIGN KEY(usuario_id) REFERENCES usuarios(id) ON DELETE CASCADE,
    FOREIGN KEY(genero_id) REFERENCES generos(id) ON DELETE CASCADE
);
""";
    }
}
