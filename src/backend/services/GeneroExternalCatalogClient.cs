using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace BackEnd.Aventura.Services
{
    public class GeneroExternalCatalogClient : IGeneroExternalCatalogClient
    {
        private readonly HttpClient _httpClient;

        public GeneroExternalCatalogClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyList<string>> FilterActiveGenresAsync(IReadOnlyList<string> genres)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "/api/genre-catalog/validate",
                new GenreValidationRequest(genres));

            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadFromJsonAsync<GenreValidationResponse>();
            return payload?.Genres
                .Where(genre => genre.Active)
                .Select(genre => genre.Name)
                .ToList() ?? [];
        }

        private sealed record GenreValidationRequest(
            [property: JsonPropertyName("genres")] IReadOnlyList<string> Genres);

        private sealed record GenreValidationResponse(
            [property: JsonPropertyName("genres")] IReadOnlyList<GenreValidationItem> Genres);

        private sealed record GenreValidationItem(
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("active")] bool Active);
    }
}
