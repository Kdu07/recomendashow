using System.Text.Json;

namespace BackEnd.Aventura.Services
{
    public class RecommendationHttpClient : IRecommendationClient
    {
        private readonly HttpClient _httpClient;

        public RecommendationHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> IsEspetaculoAtivoAsync(Guid espetaculoId)
        {
            using var response = await _httpClient.GetAsync($"/api/recommendations/espetaculos/{espetaculoId}/ativo");
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            using var payload = await JsonDocument.ParseAsync(stream);

            if (payload.RootElement.ValueKind is JsonValueKind.True or JsonValueKind.False)
            {
                return payload.RootElement.GetBoolean();
            }

            if (payload.RootElement.ValueKind == JsonValueKind.Object)
            {
                if (TryGetBoolean(payload.RootElement, "ativo", out var ativo) ||
                    TryGetBoolean(payload.RootElement, "isActive", out ativo) ||
                    TryGetBoolean(payload.RootElement, "active", out ativo))
                {
                    return ativo;
                }
            }

            throw new InvalidOperationException("Resposta invalida do servico de recomendacao.");
        }

        private static bool TryGetBoolean(JsonElement element, string propertyName, out bool value)
        {
            if (element.TryGetProperty(propertyName, out var property) &&
                property.ValueKind is JsonValueKind.True or JsonValueKind.False)
            {
                value = property.GetBoolean();
                return true;
            }

            value = false;
            return false;
        }
    }
}
