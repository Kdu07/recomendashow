namespace BackEnd.Aventura.Dto
{
    public class TeatroDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string HorarioFuncionamento { get; set; } = string.Empty;
        public IReadOnlyList<EspetaculoDto> Espetaculos { get; set; } = Array.Empty<EspetaculoDto>();
    }
}
