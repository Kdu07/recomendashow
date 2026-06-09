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

        /// <summary>
        /// Retorna o ml_id numérico do usuário — identificador usado pelo modelo SVD.
        /// Retorna null se o usuário não existir ou ainda não tiver ml_id atribuído.
        /// </summary>
        Task<int?> ObterMlIdAsync(Guid usuarioId);
    }
}
