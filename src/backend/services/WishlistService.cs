using System.Net.Http;
using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Exceptions;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IEspetaculoRepository _espetaculoRepository;
        private readonly IRecommendationClient _recommendationClient;

        public WishlistService(IWishlistRepository wishlistRepository, IEspetaculoRepository espetaculoRepository)
            : this(wishlistRepository, espetaculoRepository, new AlwaysActiveRecommendationClient())
        {
        }

        public WishlistService(
            IWishlistRepository wishlistRepository,
            IEspetaculoRepository espetaculoRepository,
            IRecommendationClient recommendationClient)
        {
            _wishlistRepository = wishlistRepository;
            _espetaculoRepository = espetaculoRepository;
            _recommendationClient = recommendationClient;
        }

        public IReadOnlyList<Espetaculo> GetWishlist(Guid usuarioId)
        {
            EntityGuards.NotEmpty(usuarioId, nameof(usuarioId));
            var ids = _wishlistRepository.GetItemIds(usuarioId);
            return _espetaculoRepository.GetByIds(ids);
        }

        public Task<IReadOnlyList<Espetaculo>> ObterPorUsuarioAsync(Guid usuarioId) =>
            Task.FromResult(GetWishlist(usuarioId));

        public void Add(Guid usuarioId, Guid espetaculoId)
        {
            AdicionarAsync(usuarioId, espetaculoId)
                .GetAwaiter()
                .GetResult();
        }

        public async Task AdicionarAsync(Guid usuarioId, Guid espetaculoId)
        {
            EntityGuards.NotEmpty(usuarioId, nameof(usuarioId));
            EntityGuards.NotEmpty(espetaculoId, nameof(espetaculoId));

            if (!_espetaculoRepository.Exists(espetaculoId))
            {
                throw new NotFoundException($"Espetaculo com ID {espetaculoId} nao encontrado.");
            }

            await EnsureEspetaculoAtivoAsync(espetaculoId);

            if (!_wishlistRepository.AddItem(usuarioId, espetaculoId))
            {
                throw new ConflictException("Espetaculo ja esta na wishlist.");
            }
        }

        public void Remove(Guid usuarioId, Guid espetaculoId)
        {
            EntityGuards.NotEmpty(usuarioId, nameof(usuarioId));
            EntityGuards.NotEmpty(espetaculoId, nameof(espetaculoId));

            if (!_wishlistRepository.RemoveItem(usuarioId, espetaculoId))
            {
                throw new NotFoundException("Espetaculo nao encontrado na wishlist.");
            }
        }

        public bool Contains(Guid usuarioId, Guid espetaculoId)
        {
            EntityGuards.NotEmpty(usuarioId, nameof(usuarioId));
            EntityGuards.NotEmpty(espetaculoId, nameof(espetaculoId));
            return _wishlistRepository.ContainsItem(usuarioId, espetaculoId);
        }

        private async Task EnsureEspetaculoAtivoAsync(Guid espetaculoId)
        {
            bool isAtivo;

            try
            {
                isAtivo = await _recommendationClient.IsEspetaculoAtivoAsync(espetaculoId);
            }
            catch (HttpRequestException exception)
            {
                throw new InvalidOperationException("Nao foi possivel validar o espetaculo no servico de recomendacao.", exception);
            }

            if (!isAtivo)
            {
                throw new InvalidOperationException("Espetaculo inativo no servico de recomendacao.");
            }
        }

        private sealed class AlwaysActiveRecommendationClient : IRecommendationClient
        {
            public Task<bool> IsEspetaculoAtivoAsync(Guid espetaculoId) => Task.FromResult(true);
        }
    }
}
