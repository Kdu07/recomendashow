using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Services
{
    public class PreferenciasService : IPreferenciasService
    {
        private readonly IPreferenciasRepository _repository;

        public PreferenciasService(IPreferenciasRepository repository)
        {
            _repository = repository;
        }

        public Preferencias GetByUsuario(Guid usuarioId) =>
            _repository.GetPreferencias(usuarioId);

        public Preferencias Update(Guid usuarioId, PreferenciasUpdateDto dto)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var preferencias = _repository.GetPreferencias(usuarioId);

            if (dto.Generos is not null)
            {
                preferencias.SetGeneros(dto.Generos);
            }

            if (dto.Tags is not null)
            {
                preferencias.SetTags(dto.Tags);
            }

            if (dto.Cidade is not null || dto.Cep is not null)
            {
                preferencias.SetLocalizacao(dto.Cep, dto.Cidade);
            }

            preferencias.AtualizarFaixaPreco(dto.FaixaPrecoMin, dto.FaixaPrecoMax);

            if (dto.ReceberNotificacoes.HasValue)
            {
                preferencias.SetReceberNotificacoes(dto.ReceberNotificacoes.Value);
            }

            _repository.Save(preferencias);
            return _repository.GetPreferencias(usuarioId);
        }
    }
}
