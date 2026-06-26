using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BackEnd.Aventura.Controller
{
    [ApiController]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [Route("api/[controller]")]
    [Tags("Auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>Cria uma nova conta de usuário.</summary>
        /// <remarks>O campo <c>consentimentoAceito</c> deve ser <c>true</c> (LGPD). Limitado a 20 req/min por IP.</remarks>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<AuthResponse> Register([FromBody] RegisterRequest request) =>
            CreatedAtAction(nameof(Register), _authService.Register(request));

        /// <summary>Autentica um usuário e retorna um token JWT.</summary>
        /// <remarks>Use o token retornado no header <c>Authorization: Bearer &lt;token&gt;</c>. Limitado a 20 req/min por IP.</remarks>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<AuthResponse> Login([FromBody] LoginRequest request)
        {
            try
            {
                return Ok(_authService.Login(request));
            }
            catch (UnauthorizedAccessException exception)
            {
                return Unauthorized(new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Não autorizado",
                    Detail = exception.Message
                });
            }
        }
    }
}
