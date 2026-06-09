using BackEnd.Aventura.Dto;

namespace BackEnd.Aventura.Services
{
    public interface IUsuarioService
    {
        UsuarioResponse GetById(Guid id);
        Task<UsuarioResponse> Update(Guid id, UsuarioUpdateRequest request);
        void SolicitarExclusao(Guid id);
    }
}
