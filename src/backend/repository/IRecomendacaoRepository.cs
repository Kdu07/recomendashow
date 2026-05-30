using BackEnd.Aventura.Domain;

namespace BackEnd.Aventura.Repository
{
    public interface IRecomendacaoRepository
    {
        IReadOnlyList<Recomendacao> GetByUsuario(Guid usuarioId, int limit);
    }
}
