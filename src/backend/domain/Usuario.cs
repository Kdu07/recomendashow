namespace BackEnd.Aventura.Domain
{
    public class Usuario
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string SenhaHash { get; private set; }
        public int? Idade { get; private set; }
        public DateTime DataCadastro { get; private set; }
        public string? Cep { get; private set; }
        public string? Logradouro { get; private set; }
        public string? Numero { get; private set; }
        public string? Complemento { get; private set; }
        public string? Bairro { get; private set; }
        public string? Cidade { get; private set; }

        public Usuario(Guid id, string nome, string email, string senhaHash, int? idade, DateTime dataCadastro,
            string? cep = null, string? logradouro = null, string? numero = null,
            string? complemento = null, string? bairro = null, string? cidade = null)
        {
            Id = id;
            Nome = nome ?? throw new ArgumentNullException(nameof(nome));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            SenhaHash = senhaHash ?? throw new ArgumentNullException(nameof(senhaHash));
            Idade = idade;
            DataCadastro = dataCadastro;
            Cep = cep;
            Logradouro = logradouro;
            Numero = numero;
            Complemento = complemento;
            Bairro = bairro;
            Cidade = cidade;
        }

        public static Usuario Create(string nome, string email, string senhaHash, int? idade = null,
            string? cep = null, string? logradouro = null, string? numero = null,
            string? complemento = null, string? bairro = null, string? cidade = null)
        {
            EntityGuards.NotNullOrWhiteSpace(nome, nameof(nome));
            EntityGuards.NotNullOrWhiteSpace(email, nameof(email));
            EntityGuards.NotNullOrWhiteSpace(senhaHash, nameof(senhaHash));

            if (idade.HasValue && idade < 13)
            {
                throw new ArgumentException("Idade mínima é 13 anos.", nameof(idade));
            }

            return new Usuario(
                Guid.NewGuid(),
                nome.Trim(),
                email.Trim().ToLowerInvariant(),
                senhaHash,
                idade,
                DateTime.UtcNow,
                cep, logradouro, numero, complemento, bairro, cidade);
        }

        public void AtualizarPerfil(string? nome, int? idade)
        {
            if (nome is not null)
            {
                Nome = EntityGuards.NotNullOrWhiteSpace(nome, nameof(nome));
            }

            if (idade.HasValue)
            {
                if (idade < 13)
                {
                    throw new ArgumentException("Idade mínima é 13 anos.", nameof(idade));
                }

                Idade = idade;
            }
        }
    }
}
