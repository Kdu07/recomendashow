using System;
using System.Collections.Generic;
using System.Linq;

namespace BackEnd.Aventura.Repository
{
    public class Wishlist
    {
        public Guid Id { get; private set; }
        public Guid UsuarioId { get; private set; }
        public DateTime DataAtualizacao { get; private set; }

        private readonly HashSet<Guid> _espetaculos = new();

        public Wishlist(Guid usuarioId)
        {
            if (usuarioId == Guid.Empty)
            {
                throw new ArgumentException("Id de usuário inválido.", nameof(usuarioId));
            }

            Id = Guid.NewGuid();
            UsuarioId = usuarioId;
            DataAtualizacao = DateTime.UtcNow;
        }

        public IReadOnlyCollection<Guid> Espetaculos => _espetaculos.ToList().AsReadOnly();

        public bool AdicionarEspetaculo(Guid espetaculoId)
        {
            if (espetaculoId == Guid.Empty)
            {
                throw new ArgumentException("Id de espetáculo inválido.", nameof(espetaculoId));
            }

            var adicionou = _espetaculos.Add(espetaculoId);
            if (adicionou)
            {
                AtualizarData();
            }

            return adicionou;
        }

        public bool RemoverEspetaculo(Guid espetaculoId)
        {
            if (espetaculoId == Guid.Empty)
            {
                throw new ArgumentException("Id de espetáculo inválido.", nameof(espetaculoId));
            }

            var removeu = _espetaculos.Remove(espetaculoId);
            if (removeu)
            {
                AtualizarData();
            }

            return removeu;
        }

        public bool ContemEspetaculo(Guid espetaculoId)
        {
            if (espetaculoId == Guid.Empty)
            {
                throw new ArgumentException("Id de espetáculo inválido.", nameof(espetaculoId));
            }

            return _espetaculos.Contains(espetaculoId);
        }

        public bool EstaVazia() => !_espetaculos.Any();

        private void AtualizarData() => DataAtualizacao = DateTime.UtcNow;
    }
}
