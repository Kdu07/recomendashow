using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Tests.Fakes
{
    /// <summary>
    /// In-memory fake implementation of IRecomendacaoRepository for testing purposes.
    /// </summary>
    public class FakeRecomendacaoRepository : IRecomendacaoRepository
    {
        private readonly Dictionary<Guid, List<Recomendacao>> _recomendacoes = new();

        public IReadOnlyList<Recomendacao> GetByUsuario(Guid usuarioId, int limit)
        {
            if (!_recomendacoes.TryGetValue(usuarioId, out var recomendacoes))
                return new List<Recomendacao>();

            return recomendacoes.Take(limit).ToList();
        }

        public void AddRecomendacao(Guid usuarioId, Recomendacao recomendacao)
        {
            if (!_recomendacoes.ContainsKey(usuarioId))
                _recomendacoes[usuarioId] = new List<Recomendacao>();

            _recomendacoes[usuarioId].Add(recomendacao);
        }

        public void Clear() => _recomendacoes.Clear();
    }
}
