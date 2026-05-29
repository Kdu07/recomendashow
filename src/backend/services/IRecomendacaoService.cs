using BackEnd.Aventura.Dto;

namespace BackEnd.Aventura.Services
{
    public interface IRecomendacaoService
    {
        IReadOnlyList<RecomendacaoItemDto> GetRecomendacoes(Guid usuarioId, int limit);
    }
}
