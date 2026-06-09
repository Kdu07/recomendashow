using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Exceptions;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPreferenciasRepository _preferenciasRepository;

        public AuthService(
            IUsuarioRepository usuarioRepository,
            IPreferenciasRepository preferenciasRepository)
        {
            _usuarioRepository = usuarioRepository;
            _preferenciasRepository = preferenciasRepository;
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
            if (usuario is null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
            {
                throw new UnauthorizedAccessException("E-mail ou senha inválidos.");
            }

            if (usuario.DeletedAt is not null)
            {
                throw new UnauthorizedAccessException("Esta conta foi encerrada.");
            }

            return ToAuthResponse(usuario);
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

        private static AuthResponse ToAuthResponse(Usuario usuario) => new()
        {
            UsuarioId = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email
        };
    }
}
