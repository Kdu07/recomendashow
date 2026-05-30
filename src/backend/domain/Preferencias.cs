namespace BackEnd.Aventura.Domain
{
    public class Preferencias
    {
        private readonly List<string> _generos = new();
        private readonly List<string> _tags = new();

        public Guid Id { get; private set; }
        public Guid UsuarioId { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public DateTime DataAtualizacao { get; private set; }
        public string? Cidade { get; private set; }
        public string? Cep { get; private set; }
        public decimal? FaixaPrecoMin { get; private set; }
        public decimal? FaixaPrecoMax { get; private set; }
        public bool ReceberNotificacoes { get; private set; }

        public IReadOnlyCollection<string> Generos => _generos.AsReadOnly();
        public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();

        public Preferencias(Guid usuarioId)
        {
            EntityGuards.NotEmpty(usuarioId, nameof(usuarioId));

            Id = Guid.NewGuid();
            UsuarioId = usuarioId;
            var now = DateTime.UtcNow;
            DataCriacao = now;
            DataAtualizacao = now;
            ReceberNotificacoes = true;
        }

        public static Preferencias FromData(
            Guid id,
            Guid usuarioId,
            string? cep,
            string? cidade,
            IEnumerable<string> generos,
            IEnumerable<string> tags,
            decimal? faixaPrecoMin,
            decimal? faixaPrecoMax,
            bool receberNotificacoes,
            DateTime dataAtualizacao)
        {
            var preferencias = new Preferencias(usuarioId)
            {
                Id = id,
                Cep = string.IsNullOrWhiteSpace(cep) ? null : cep.Trim(),
                Cidade = string.IsNullOrWhiteSpace(cidade) ? null : cidade.Trim(),
                FaixaPrecoMin = faixaPrecoMin,
                FaixaPrecoMax = faixaPrecoMax,
                ReceberNotificacoes = receberNotificacoes,
                DataAtualizacao = dataAtualizacao
            };

            preferencias.SetGeneros(generos);
            preferencias.SetTags(tags);
            return preferencias;
        }

        public void SetGeneros(IEnumerable<string> generos)
        {
            if (generos is null) throw new ArgumentNullException(nameof(generos));

            _generos.Clear();
            foreach (var genero in generos.Where(g => !string.IsNullOrWhiteSpace(g)))
            {
                var trimmed = genero.Trim();
                if (!_generos.Contains(trimmed, StringComparer.OrdinalIgnoreCase))
                {
                    _generos.Add(trimmed);
                }
            }

            AtualizarData();
        }

        public void SetTags(IEnumerable<string> tags)
        {
            if (tags is null) throw new ArgumentNullException(nameof(tags));

            _tags.Clear();
            foreach (var tag in tags.Where(t => !string.IsNullOrWhiteSpace(t)))
            {
                var trimmed = tag.Trim();
                if (!_tags.Contains(trimmed, StringComparer.OrdinalIgnoreCase))
                {
                    _tags.Add(trimmed);
                }
            }

            AtualizarData();
        }

        public void AtualizarFaixaPreco(decimal? min, decimal? max)
        {
            if (min.HasValue && max.HasValue && min > max)
            {
                throw new ArgumentException("Faixa de preço inválida: mínimo maior que máximo.");
            }

            FaixaPrecoMin = min;
            FaixaPrecoMax = max;
            AtualizarData();
        }

        public void SetReceberNotificacoes(bool receber)
        {
            ReceberNotificacoes = receber;
            AtualizarData();
        }

        public void SetLocalizacao(string? cep, string? cidade)
        {
            if (cep is not null)
            {
                Cep = string.IsNullOrWhiteSpace(cep) ? null : cep.Trim();
            }

            if (cidade is not null)
            {
                Cidade = string.IsNullOrWhiteSpace(cidade) ? null : cidade.Trim();
            }

            AtualizarData();
        }

        private void AtualizarData() => DataAtualizacao = DateTime.UtcNow;
    }
}
