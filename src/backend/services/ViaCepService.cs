using System.Text.Json;
using BackEnd.Aventura.Dto;
using Microsoft.Extensions.Configuration;

namespace BackEnd.Aventura.Services
{
    public class ViaCepService : IViaCepService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ViaCepService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = (configuration["ViaCep:BaseUrl"] ?? "https://viacep.com.br").TrimEnd('/');
        }

        public async Task<ViaCepResponse?> BuscarAsync(string cep)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/ws/{cep}/json/");
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ViaCepResponse>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result?.Erro == true)
                    return null;

                return result;
            }
            catch
            {
                return null;
            }
        }
    }
}
