using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Dto;

namespace BackEnd.Aventura.Services
{
    public interface IPreferenciasService
    {
        Preferencias GetByUsuario(Guid usuarioId);
        Preferencias Update(Guid usuarioId, PreferenciasUpdateDto dto);
    }
}
