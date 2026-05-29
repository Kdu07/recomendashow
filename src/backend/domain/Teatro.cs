namespace BackEnd.Aventura.Domain
{
    public class Teatro
    {
        private readonly List<Espetaculo> _espetaculos = new();

        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Endereco { get; private set; }
        public string Cidade { get; private set; }
        public string HorarioFuncionamento { get; private set; }

        public IReadOnlyCollection<Espetaculo> Espetaculos => _espetaculos.AsReadOnly();

        public Teatro(Guid id, string nome, string endereco, string cidade, string horarioFuncionamento)
        {
            Id = id;
            Nome = nome ?? throw new ArgumentNullException(nameof(nome));
            Endereco = endereco ?? throw new ArgumentNullException(nameof(endereco));
            Cidade = cidade ?? throw new ArgumentNullException(nameof(cidade));
            HorarioFuncionamento = horarioFuncionamento ?? throw new ArgumentNullException(nameof(horarioFuncionamento));
        }

        public void AdicionarEspetaculo(Espetaculo espetaculo)
        {
            if (espetaculo is null)
            {
                throw new ArgumentNullException(nameof(espetaculo));
            }

            if (_espetaculos.Any(e => e.Id == espetaculo.Id))
            {
                throw new InvalidOperationException("Espetáculo já cadastrado neste teatro.");
            }

            _espetaculos.Add(espetaculo);
        }
    }
}
