using System;
using System.Collections.Generic;
using System.Linq;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Tests.Fakes
{
    public class FakeWishlistRepository : IWishlistRepository
    {
        private readonly List<(Guid UsuarioId, Guid EspetaculoId)> _items = new();
        private readonly Dictionary<Guid, List<string>> _generosPorUsuario = new();

        public bool AddItem(Guid usuarioId, Guid espetaculoId)
        {
            if (ContainsItem(usuarioId, espetaculoId))
            {
                return false;
            }

            _items.Add((usuarioId, espetaculoId));
            return true;
        }

        public bool RemoveItem(Guid usuarioId, Guid espetaculoId)
        {
            var item = _items.FirstOrDefault(x => x.UsuarioId == usuarioId && x.EspetaculoId == espetaculoId);
            if (item == default)
            {
                return false;
            }

            _items.Remove(item);
            return true;
        }

        public bool ContainsItem(Guid usuarioId, Guid espetaculoId)
        {
            return _items.Any(x => x.UsuarioId == usuarioId && x.EspetaculoId == espetaculoId);
        }

        public IReadOnlyList<Guid> GetItemIds(Guid usuarioId)
        {
            return _items
                .Where(x => x.UsuarioId == usuarioId)
                .Select(x => x.EspetaculoId)
                .ToList();
        }

        public IReadOnlyList<string> GetGenerosDaWishlist(Guid usuarioId)
        {
            return _generosPorUsuario.TryGetValue(usuarioId, out var generos)
                ? generos.ToList()
                : new List<string>();
        }

        public void SetGenerosDaWishlist(Guid usuarioId, params string[] generos)
        {
            _generosPorUsuario[usuarioId] = generos.ToList();
        }

        public void Clear()
        {
            _items.Clear();
            _generosPorUsuario.Clear();
        }
    }
}
