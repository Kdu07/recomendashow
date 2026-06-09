using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Exceptions;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Services
{
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.IdentityModel.Tokens;
    using System.Security.Claims;
    using System.Text;

    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPreferenciasRepository _preferenciasRepository;
        private readonly IConfiguration _config;

        public AuthService(
            IUsuarioRepository usuarioRepository,
            IPreferenciasRepository preferenciasRepository,
            IConfiguration config)
        {
            _usuarioRepository = usuarioRepository;
            _preferenciasRepository = preferenciasRepository;
            _config = config;
        }

        public AuthResponse Register(RegisterRequest request)
        {
            ValidateRegisterRequest(request);

            if (_usuarioRepository.EmailExists(request.Email))
            {
                throw new ConflictException("E-mail já cadastrado.");
            }

            var senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);
            var usuario = Usuario.Create(request.Nome, request.Email, senhaHash, request.Idade,
                request.Cep, request.Logradouro, request.Numero,
                request.Complemento, request.Bairro, request.Cidade);
            usuario.AceitarConsentimento();
            _usuarioRepository.Create(usuario);
            _preferenciasRepository.GetPreferencias(usuario.Id);

            return ToAuthResponse(usuario);
        }

        public AuthResponse Login(LoginRequest request)
        {
            var email = EntityGuards.NotNullOrWhiteSpace(request.Email, nameof(request.Email));
            EntityGuards.NotNullOrWhiteSpace(request.Senha, nameof(request.Senha));

            var usuario = _usuarioRepository.GetByEmail(email);
            if (usuario is null)
            {
                throw new UnauthorizedAccessException("E-mail ou senha inválidos.");
            }

            if (usuario.DeletedAt is not null)
            {
                throw new UnauthorizedAccessException("Esta conta foi encerrada.");
            }

            return ToAuthResponse(usuario);
        }

                private AuthResponse ToAuthResponse(Usuario usuario)
        {
            var token = GenerateJwtToken(usuario);
            return new AuthResponse
            {
                UsuarioId = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Token = token,
            };
        }


        private static void ValidateRegisterRequest(RegisterRequest request)
        {
            if (!request.ConsentimentoAceito)
            {
                throw new ArgumentException("O consentimento com a política de privacidade é obrigatório.");
            }

            EntityGuards.NotNullOrWhiteSpace(request.Nome, nameof(request.Nome));
            var email = EntityGuards.NotNullOrWhiteSpace(request.Email, nameof(request.Email));
            if (!email.Contains('@'))
            {
                throw new ArgumentException("E-mail inválido.");
            }

            if (string.IsNullOrWhiteSpace(request.Senha) || request.Senha.Length < 6)
            {
                throw new ArgumentException("Senha deve ter no mínimo 6 caracteres.");
            }

            if (request.Idade.HasValue && request.Idade < 13)
            {
                throw new ArgumentException("Idade mínima é 13 anos.");
            }
        }


        private string GenerateJwtToken(Usuario usuario)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? string.Empty));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim("name", usuario.Nome)
            };

            var expiry = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpiryMinutes"] ?? "60"));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
