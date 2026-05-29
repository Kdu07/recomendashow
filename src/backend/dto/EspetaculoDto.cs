namespace BackEnd.Aventura.Dto
{
    public class EspetaculoDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public string TipoEvento { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string UrlBilheteria { get; set; } = string.Empty;
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public decimal ValorIngresso { get; set; }
        public bool EmCartaz { get; set; }
    }
}
