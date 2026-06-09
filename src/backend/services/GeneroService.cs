using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Services
{
    public class GeneroService : IGeneroService
    {
        private readonly IGeneroRepository _repository;
        private readonly IGeneroExternalCatalogClient? _externalCatalogClient;

        public GeneroService(
            IGeneroRepository repository,
            IGeneroExternalCatalogClient? externalCatalogClient = null)
        {
            _repository = repository;
            _externalCatalogClient = externalCatalogClient;
        }

        public IReadOnlyList<string> GetAll() => _repository.GetAllNomes().ToList();

        public async Task<IReadOnlyList<string>> GetAllValidatedAsync()
        {
            var generos = _repository.GetAllNomes().ToList();
            if (generos.Count == 0)
            {
                return [];
            }

            if (_externalCatalogClient is null)
            {
                throw new InvalidOperationException("Cliente externo de catálogo de gêneros não configurado.");
            }

            return await _externalCatalogClient.FilterActiveGenresAsync(generos);
        }
    }
}
