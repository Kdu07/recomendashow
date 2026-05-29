namespace BackEnd.Aventura.Domain
{
    public sealed class Recomendacao
    {
        public Recomendacao(Espetaculo espetaculo, decimal score, string? motivo)
        {
            Espetaculo = espetaculo ?? throw new ArgumentNullException(nameof(espetaculo));
            Score = score;
            Motivo = motivo;
        }

        public Espetaculo Espetaculo { get; }
        public decimal Score { get; }
        public string? Motivo { get; }
    }
}
