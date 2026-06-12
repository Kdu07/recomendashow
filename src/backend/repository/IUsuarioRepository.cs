using BackEnd.Aventura.Domain;

namespace BackEnd.Aventura.Repository
{
    public interface IUsuarioRepository
    {
        Usuario? GetById(Guid id);
        Usuario? GetByEmail(string email);
        bool EmailExists(string email);
        Usuario Create(Usuario usuario);
        bool Update(Usuario usuario);
        bool SolicitarExclusao(Guid id);
        Task<int?> ObterMlIdAsync(Guid usuarioId);
    }
}
