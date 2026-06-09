using BackEnd.Aventura.Dto;
using BackEnd.Aventura.Exceptions;
using BackEnd.Aventura.Repository;

namespace BackEnd.Aventura.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;
        private readonly IViaCepService _viaCepService;

        public UsuarioService(IUsuarioRepository repository, IViaCepService viaCepService)
        {
            _repository = repository;
            _viaCepService = viaCepService;
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

        public async Task<UsuarioResponse> Update(Guid id, UsuarioUpdateRequest request)
        {
            var usuario = _repository.GetById(id);
            if (usuario is null)
            {
                throw new NotFoundException($"Usuário com ID {id} não encontrado.");
            }

            var logradouro = request.Logradouro;
            var bairro = request.Bairro;
            var cidade = request.Cidade;

            if (!string.IsNullOrWhiteSpace(request.Cep))
            {
                var viaCep = await _viaCepService.BuscarAsync(request.Cep);
                if (viaCep is null)
                    throw new ArgumentException("CEP não encontrado.");

                logradouro = viaCep.Logradouro;
                bairro = viaCep.Bairro;
                cidade = viaCep.Localidade;
            }

            usuario.AtualizarPerfil(request.Nome, request.Idade,
                request.Cep, logradouro, request.Numero,
                request.Complemento, bairro, cidade);
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
