namespace BackEnd.Aventura.Services
{
    public interface IGeneroService
    {
        IReadOnlyList<string> GetAll();
        Task<IReadOnlyList<string>> GetAllValidatedAsync();
    }
}
