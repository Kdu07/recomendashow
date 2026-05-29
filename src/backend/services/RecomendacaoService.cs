using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Mappings;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Services
{
    public class RecomendacaoService : IRecomendacaoService
    {
        private readonly IRecomendacaoRepository _recomendacaoRepository;
        private readonly IEspetaculoRepository _espetaculoRepository;

        public RecomendacaoService(
            IRecomendacaoRepository recomendacaoRepository,
            IEspetaculoRepository espetaculoRepository)
        {
            _recomendacaoRepository = recomendacaoRepository;
            _espetaculoRepository = espetaculoRepository;
        }

        public IReadOnlyList<RecomendacaoItemDto> GetRecomendacoes(Guid usuarioId, int limit)
        {
            EntityGuards.NotEmpty(usuarioId, nameof(usuarioId));
            if (limit <= 0 || limit > 50)
            {
                throw new ArgumentException("Limit deve estar entre 1 e 50.");
            }

            var stored = _recomendacaoRepository.GetByUsuario(usuarioId, limit);
            if (stored.Count > 0)
            {
                return stored.Select(r => r.ToDto()).ToList();
            }

            return BuildFallback(usuarioId, limit);
        }

        private IReadOnlyList<RecomendacaoItemDto> BuildFallback(Guid usuarioId, int limit)
        {
            return _espetaculoRepository
                .GetByGenerosFavoritos(usuarioId, limit)
                .Select((espetaculo, index) => new RecomendacaoItemDto
                {
                    Espetaculo = espetaculo.ToDto(),
                    Score = Math.Max(0.5m, 1.0m - index * 0.05m),
                    Motivo = "Baseado nos seus gêneros favoritos"
                })
                .ToList();
        }
    }
}
