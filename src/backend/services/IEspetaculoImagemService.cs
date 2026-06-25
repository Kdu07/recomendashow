using BackEnd.Aventura.Domain;

namespace BackEnd.Aventura.Services
{
    public interface IEspetaculoImagemService
    {
        EspetaculoImagem GetImagem(Guid espetaculoId);
    }
}