using BackEnd.Aventura.Domain;

namespace BackEnd.Aventura.Repository
{
    public interface IPreferenciasRepository
    {
        Preferencias GetPreferencias(Guid usuarioId);
        bool Save(Preferencias preferencias);
    }
}
