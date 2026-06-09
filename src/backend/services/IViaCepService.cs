using BackEnd.Aventura.Dto;

namespace BackEnd.Aventura.Services
{
    public interface IViaCepService
    {
        Task<ViaCepResponse?> BuscarAsync(string cep);
    }
}
