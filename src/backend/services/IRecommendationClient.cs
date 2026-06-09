namespace BackEnd.Aventura.Services
{
    public interface IRecommendationClient
    {
        Task<bool> IsEspetaculoAtivoAsync(Guid espetaculoId);
    }
}
