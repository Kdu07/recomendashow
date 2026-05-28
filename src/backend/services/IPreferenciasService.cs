using System;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Services
{
    public interface IPreferenciasService
    {
        Preferencias GetPreferencias(Guid usuarioId);
        Preferencias SavePreferencias(Preferencias preferencias);
    }
}
