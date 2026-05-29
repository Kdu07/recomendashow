using BackEnd.Aventura.Dto;

namespace BackEnd.Aventura.Services
{
    public interface IUsuarioService
    {
        UsuarioResponse GetById(Guid id);
        UsuarioResponse Update(Guid id, UsuarioUpdateRequest request);
    }
}
