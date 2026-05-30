using BackEnd.Aventura.Dto;

namespace BackEnd.Aventura.Services
{
    public interface IAuthService
    {
        AuthResponse Register(RegisterRequest request);
        AuthResponse Login(LoginRequest request);
    }
}
