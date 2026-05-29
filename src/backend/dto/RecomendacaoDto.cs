namespace BackEnd.Aventura.Dto
{
    public class RecomendacaoItemDto
    {
        public EspetaculoDto Espetaculo { get; set; } = new();
        public decimal Score { get; set; }
        public string? Motivo { get; set; }
    }
}
