using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Dto;

namespace BackEnd.Aventura.Mappings
{
    public static class DtoMapper
    {
        public static EspetaculoDto ToDto(this Espetaculo espetaculo) => new()
        {
            Id = espetaculo.Id,
            Nome = espetaculo.Nome,
            Genero = espetaculo.Genero,
            TipoEvento = espetaculo.TipoEvento,
            Descricao = espetaculo.Descricao,
            UrlBilheteria = espetaculo.UrlBilheteria,
            DataInicio = espetaculo.DataInicio,
            DataFim = espetaculo.DataFim,
            ValorIngresso = espetaculo.ValorIngresso,
            EmCartaz = espetaculo.EstaEmCartaz()
        };

        public static TeatroDto ToDto(this Teatro teatro) => new()
        {
            Id = teatro.Id,
            Nome = teatro.Nome,
            Endereco = teatro.Endereco,
            Cidade = teatro.Cidade,
            HorarioFuncionamento = teatro.HorarioFuncionamento,
            Espetaculos = teatro.Espetaculos.Select(e => e.ToDto()).ToList()
        };

        public static PreferenciasDto ToDto(this Preferencias preferencias) => new()
        {
            UsuarioId = preferencias.UsuarioId,
            Cep = preferencias.Cep,
            Cidade = preferencias.Cidade,
            Generos = preferencias.Generos.ToList(),
            Tags = preferencias.Tags.ToList(),
            FaixaPrecoMin = preferencias.FaixaPrecoMin,
            FaixaPrecoMax = preferencias.FaixaPrecoMax,
            ReceberNotificacoes = preferencias.ReceberNotificacoes,
            DataAtualizacao = preferencias.DataAtualizacao
        };

        public static RecomendacaoItemDto ToDto(this Recomendacao recomendacao) => new()
        {
            Espetaculo = recomendacao.Espetaculo.ToDto(),
            Score = recomendacao.Score,
            Motivo = recomendacao.Motivo
        };
    }
}
