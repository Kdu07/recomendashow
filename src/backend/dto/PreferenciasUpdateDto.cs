namespace BackEnd.Aventura.Dto
{
    public class PreferenciasUpdateDto
    {
        public IEnumerable<string>? Generos { get; set; }
        public IEnumerable<string>? Tags { get; set; }
        public string? Cep { get; set; }
        public string? Cidade { get; set; }
        public decimal? FaixaPrecoMin { get; set; }
        public decimal? FaixaPrecoMax { get; set; }
        public bool ReceberNotificacoes { get; set; } = true;
    }
}
