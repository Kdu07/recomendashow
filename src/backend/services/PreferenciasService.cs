using System;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Services
{
    public class PreferenciasService : IPreferenciasService
    {
        private readonly IPreferenciasRepository _preferenciasRepository;

        public PreferenciasService(IPreferenciasRepository preferenciasRepository)
        {
            _preferenciasRepository = preferenciasRepository ?? throw new ArgumentNullException(nameof(preferenciasRepository));
        }

        public Preferencias GetPreferencias(Guid usuarioId)
        {
            if (usuarioId == Guid.Empty)
            {
                throw new ArgumentException("Id de usuário inválido.", nameof(usuarioId));
            }

            return _preferenciasRepository.GetPreferencias(usuarioId);
        }

        public Preferencias SavePreferencias(Preferencias preferencias)
        {
            if (preferencias is null)
            {
                throw new ArgumentNullException(nameof(preferencias));
            }

            if (preferencias.UsuarioId == Guid.Empty)
            {
                throw new ArgumentException("Id de usuário inválido.", nameof(preferencias.UsuarioId));
            }

            _preferenciasRepository.Save(preferencias);
            return preferencias;
        }
    }
}
