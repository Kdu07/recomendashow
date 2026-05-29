namespace BackEnd.Aventura.Dto
{
    public class PreferenciasDto
    {
        public Guid UsuarioId { get; set; }
        public string? Cep { get; set; }
        public string? Cidade { get; set; }
        public IReadOnlyList<string> Generos { get; set; } = Array.Empty<string>();
        public IReadOnlyList<string> Tags { get; set; } = Array.Empty<string>();
        public decimal? FaixaPrecoMin { get; set; }
        public decimal? FaixaPrecoMax { get; set; }
        public bool ReceberNotificacoes { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }
}
