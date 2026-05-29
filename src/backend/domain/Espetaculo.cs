namespace BackEnd.Aventura.Domain
{
    public class Espetaculo
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Genero { get; private set; }
        public string TipoEvento { get; private set; }
        public string Descricao { get; private set; }
        public string UrlBilheteria { get; private set; }
        public DateTime DataInicio { get; private set; }
        public DateTime DataFim { get; private set; }
        public decimal ValorIngresso { get; private set; }

        public Espetaculo(
            Guid id,
            string nome,
            string genero,
            string tipoEvento,
            string descricao,
            string urlBilheteria,
            DateTime dataInicio,
            DateTime dataFim,
            decimal valorIngresso)
        {
            Id = id;
            Nome = nome ?? throw new ArgumentNullException(nameof(nome));
            Genero = genero ?? throw new ArgumentNullException(nameof(genero));
            TipoEvento = tipoEvento ?? throw new ArgumentNullException(nameof(tipoEvento));
            Descricao = descricao ?? throw new ArgumentNullException(nameof(descricao));
            UrlBilheteria = urlBilheteria ?? throw new ArgumentNullException(nameof(urlBilheteria));
            DataInicio = dataInicio;
            DataFim = dataFim;
            ValorIngresso = valorIngresso;
        }

        public bool PertenceAoGenero(string genero)
        {
            EntityGuards.NotNullOrWhiteSpace(genero, nameof(genero));
            return string.Equals(Genero, genero, StringComparison.OrdinalIgnoreCase);
        }

        public string ObterUrlCompra() => UrlBilheteria;

        public bool EstaEmCartaz() =>
            DateTime.UtcNow.Date >= DataInicio.Date && DateTime.UtcNow.Date <= DataFim.Date;
    }
}
