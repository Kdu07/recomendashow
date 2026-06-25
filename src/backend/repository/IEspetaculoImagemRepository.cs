using BackEnd.Aventura.Domain;

namespace BackEnd.Aventura.Repository
{
    public interface IEspetaculoImagemRepository
    {
        EspetaculoImagem? GetByEspetaculoNome(string espetaculoNome);
    }
}