using BackEnd.Aventura.Dto;

namespace BackEnd.Aventura.Services
{
    /// <summary>
    /// Pós-processa o resultado das estratégias de recomendação usando a wishlist do usuário
    /// como sinal de afinidade:
    /// (Fase 1) remove os espetáculos já salvos pelo usuário e trunca para o limite solicitado;
    /// (Fase 3) dá um boost no score dos itens cujo gênero combina com a wishlist e reordena.
    /// </summary>
    public interface IWishlistRecommendationPostProcessor
    {
        IReadOnlyList<RecomendacaoItemDto> Apply(
            IReadOnlyList<RecomendacaoItemDto> recomendacoes,
            ISet<Guid> wishlistEspetaculoIds,
            ISet<string> wishlistGeneros,
            int limit);
    }
}
