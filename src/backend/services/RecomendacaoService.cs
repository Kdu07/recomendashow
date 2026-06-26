using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Services
{
    public class RecomendacaoService : IRecomendacaoService
    {
        private readonly IReadOnlyList<IRecommendationStrategy> _strategies;
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IWishlistRecommendationPostProcessor _postProcessor;

        public RecomendacaoService(
            IEnumerable<IRecommendationStrategy> strategies,
            IWishlistRepository wishlistRepository,
            IWishlistRecommendationPostProcessor postProcessor)
        {
            _strategies = strategies?.ToList() ?? throw new ArgumentNullException(nameof(strategies));
            if (_strategies.Count == 0)
            {
                throw new InvalidOperationException("Ao menos uma estrategia de recomendacao deve ser registrada.");
            }

            _wishlistRepository = wishlistRepository ?? throw new ArgumentNullException(nameof(wishlistRepository));
            _postProcessor = postProcessor ?? throw new ArgumentNullException(nameof(postProcessor));
        }

        public IReadOnlyList<RecomendacaoItemDto> GetRecomendacoes(Guid usuarioId, int limit)
        {
            EntityGuards.NotEmpty(usuarioId, nameof(usuarioId));
            if (limit <= 0 || limit > 50)
            {
                throw new ArgumentException("Limit deve estar entre 1 e 50.");
            }

            var wishlistIds = new HashSet<Guid>(_wishlistRepository.GetItemIds(usuarioId));

            // Gêneros da wishlist só importam para o boost e só existem se houver itens salvos;
            // evitamos a query quando a wishlist está vazia.
            var wishlistGeneros = wishlistIds.Count > 0
                ? new HashSet<string>(_wishlistRepository.GetGenerosDaWishlist(usuarioId))
                : new HashSet<string>();

            // Buffer: a exclusão dos itens já salvos pode encolher a lista, então pedimos
            // itens extras às estratégias para ainda conseguir preencher o limite solicitado.
            var fetchLimit = limit + wishlistIds.Count;

            foreach (var strategy in _strategies)
            {
                var recommendations = strategy.GetRecommendations(usuarioId, fetchLimit);
                if (recommendations.Count > 0)
                {
                    return _postProcessor.Apply(recommendations, wishlistIds, wishlistGeneros, limit);
                }
            }

            return Array.Empty<RecomendacaoItemDto>();
        }
    }
}
