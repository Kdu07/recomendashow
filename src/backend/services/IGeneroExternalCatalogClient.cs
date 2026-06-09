namespace BackEnd.Aventura.Services
{
    public interface IGeneroExternalCatalogClient
    {
        Task<IReadOnlyList<string>> FilterActiveGenresAsync(IReadOnlyList<string> genres);
    }
}
