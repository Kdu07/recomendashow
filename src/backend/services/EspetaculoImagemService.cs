using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Exceptions;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Services
{
    public sealed class EspetaculoImagemService : IEspetaculoImagemService
    {
        private readonly IEspetaculoService _espetaculoService;
        private readonly IEspetaculoImagemRepository _imagemRepository;

        public EspetaculoImagemService(IEspetaculoService espetaculoService, IEspetaculoImagemRepository imagemRepository)
        {
            _espetaculoService = espetaculoService ?? throw new ArgumentNullException(nameof(espetaculoService));
            _imagemRepository = imagemRepository ?? throw new ArgumentNullException(nameof(imagemRepository));
        }

        public EspetaculoImagem GetImagem(Guid espetaculoId)
        {
            // GetById já lança NotFoundException se o espetáculo não existir no Postgres.
            var espetaculo = _espetaculoService.GetById(espetaculoId);

            var imagem = _imagemRepository.GetByEspetaculoNome(espetaculo.Nome);
            if (imagem is null)
            {
                throw new NotFoundException($"Imagem do espetáculo '{espetaculo.Nome}' não encontrada.");
            }

            return imagem;
        }
    }
}