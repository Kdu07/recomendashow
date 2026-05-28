using System;

namespace BackEnd.Aventura.Repository
{
    public interface IPreferenciasRepository
    {
        Preferencias GetPreferencias(Guid usuarioId);
        Preferencias? GetPreferenciasOrNull(Guid usuarioId);
        bool Save(Preferencias preferencias);
        bool Delete(Guid usuarioId);
    }
}
