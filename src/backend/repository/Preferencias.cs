using System;
using System.Collections.Generic;
using System.Linq;

namespace BackEnd.Aventura.Repository
{
    /// <summary>
    /// Entidade que representa as preferências e interesses do usuário.
    /// Serve como dicionário de interesses para o motor de recomendação e filtros do app.
    /// </summary>
    public class Preferencias
    {
        public Guid Id { get; private set; }
        public Guid UsuarioId { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public DateTime DataAtualizacao { get; private set; }

        // Lista de gêneros preferidos pelo usuário (ex: "Musical", "Comédia").
        private readonly List<string> _generos = new();

        // Tags ou interesses livres (ex: "família", "infantil", "adulto").
        private readonly List<string> _tags = new();

        // Faixa de preço preferida (opcional).
        public decimal? FaixaPrecoMin { get; private set; }
        public decimal? FaixaPrecoMax { get; private set; }

        // Notificações relacionadas a novos espetáculos nas preferências do usuário.
        public bool ReceberNotificacoes { get; private set; }

        public Preferencias(Guid usuarioId)
        {
            if (usuarioId == Guid.Empty) throw new ArgumentException("Id de usuário inválido.", nameof(usuarioId));

            Id = Guid.NewGuid();
            UsuarioId = usuarioId;
            var now = DateTime.UtcNow;
            DataCriacao = now;
            DataAtualizacao = now;
            ReceberNotificacoes = true;
        }

        public IReadOnlyCollection<string> Generos => _generos.AsReadOnly();
        public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();

        public void SetGeneros(IEnumerable<string> generos)
        {
            if (generos is null) throw new ArgumentNullException(nameof(generos));

            _generos.Clear();
            foreach (var genero in generos.Where(g => !string.IsNullOrWhiteSpace(g)))
            {
                if (!_generos.Contains(genero, StringComparer.OrdinalIgnoreCase))
                {
                    _generos.Add(genero.Trim());
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
                if (!_tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                {
                    _tags.Add(tag.Trim());
                }
            }

            AtualizarData();
        }

        public void AdicionarGenero(string genero)
        {
            if (string.IsNullOrWhiteSpace(genero)) throw new ArgumentException("Gênero inválido.", nameof(genero));
            if (!_generos.Contains(genero, StringComparer.OrdinalIgnoreCase))
            {
                _generos.Add(genero);
                AtualizarData();
            }
        }

        public bool RemoverGenero(string genero)
        {
            if (string.IsNullOrWhiteSpace(genero)) throw new ArgumentException("Gênero inválido.", nameof(genero));
            var removed = _generos.RemoveAll(g => string.Equals(g, genero, StringComparison.OrdinalIgnoreCase)) > 0;
            if (removed) AtualizarData();
            return removed;
        }

        public void AdicionarTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag)) throw new ArgumentException("Tag inválida.", nameof(tag));
            if (!_tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
            {
                _tags.Add(tag);
                AtualizarData();
            }
        }

        public bool RemoverTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag)) throw new ArgumentException("Tag inválida.", nameof(tag));
            var removed = _tags.RemoveAll(t => string.Equals(t, tag, StringComparison.OrdinalIgnoreCase)) > 0;
            if (removed) AtualizarData();
            return removed;
        }

        public void AtualizarFaixaPreco(decimal? min, decimal? max)
        {
            if (min.HasValue && max.HasValue && min > max) throw new ArgumentException("Faixa de preço inválida: min maior que max.");
            FaixaPrecoMin = min;
            FaixaPrecoMax = max;
            AtualizarData();
        }

        public void SetReceberNotificacoes(bool receber)
        {
            ReceberNotificacoes = receber;
            AtualizarData();
        }

        private void AtualizarData() => DataAtualizacao = DateTime.UtcNow;
    }
}
