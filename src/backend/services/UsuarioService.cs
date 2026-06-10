using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Exceptions;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;

        public UsuarioService(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        public UsuarioResponse GetById(Guid id)
        {
            var usuario = _repository.GetById(id);
            if (usuario is null)
            {
                throw new NotFoundException($"Usuário com ID {id} não encontrado.");
            }

            return MapResponse(usuario);
        }

        public UsuarioResponse Update(Guid id, UsuarioUpdateRequest request)
        {
            var usuario = _repository.GetById(id);
            if (usuario is null)
            {
                throw new NotFoundException($"Usuário com ID {id} não encontrado.");
            }

            usuario.AtualizarPerfil(request.Nome, request.Idade,
                request.Cep, request.Logradouro, request.Numero,
                request.Complemento, request.Bairro, request.Cidade);
            _repository.Update(usuario);
            return MapResponse(usuario);
        }

        public void SolicitarExclusao(Guid id)
        {
            var usuario = _repository.GetById(id);
            if (usuario is null)
            {
                throw new NotFoundException($"Usuário com ID {id} não encontrado.");
            }

            _repository.SolicitarExclusao(id);
        }

        private static UsuarioResponse MapResponse(Domain.Usuario usuario) => new()
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Idade = usuario.Idade,
            DataCadastro = usuario.DataCadastro,
            Cep = usuario.Cep,
            Logradouro = usuario.Logradouro,
            Numero = usuario.Numero,
            Complemento = usuario.Complemento,
            Bairro = usuario.Bairro,
            Cidade = usuario.Cidade
        };
    }
}
