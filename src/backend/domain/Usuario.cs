namespace BackEnd.Aventura.Domain
{
    public class Usuario
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string SenhaHash { get; private set; }
        public int? Idade { get; private set; }
        public DateTime? DataNascimento { get; private set; }
        public DateTime DataCadastro { get; private set; }
        public string? Cep { get; private set; }
        public string? Logradouro { get; private set; }
        public string? Numero { get; private set; }
        public string? Complemento { get; private set; }
        public string? Bairro { get; private set; }
        public string? Cidade { get; private set; }
        public bool ConsentimentoAceito { get; private set; }
        public DateTime? DataConsentimento { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public bool Anonimizado { get; private set; }

        public Usuario(Guid id, string nome, string email, string senhaHash, int? idade, DateTime dataCadastro,
            string? cep = null, string? logradouro = null, string? numero = null,
            string? complemento = null, string? bairro = null, string? cidade = null,
            bool consentimentoAceito = false, DateTime? dataConsentimento = null,
            DateTime? deletedAt = null, bool anonimizado = false,
            DateTime? dataNascimento = null)
        {
            Id = id;
            Nome = nome ?? throw new ArgumentNullException(nameof(nome));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            SenhaHash = senhaHash ?? throw new ArgumentNullException(nameof(senhaHash));
            Idade = idade;
            DataNascimento = dataNascimento;
            DataCadastro = dataCadastro;
            Cep = cep;
            Logradouro = logradouro;
            Numero = numero;
            Complemento = complemento;
            Bairro = bairro;
            Cidade = cidade;
            ConsentimentoAceito = consentimentoAceito;
            DataConsentimento = dataConsentimento;
            DeletedAt = deletedAt;
            Anonimizado = anonimizado;
        }

        public static Usuario Create(string nome, string email, string senhaHash, int? idade = null,
            DateTime? dataNascimento = null,
            string? cep = null, string? logradouro = null, string? numero = null,
            string? complemento = null, string? bairro = null, string? cidade = null)
        {
            EntityGuards.NotNullOrWhiteSpace(nome, nameof(nome));
            EntityGuards.NotNullOrWhiteSpace(email, nameof(email));
            EntityGuards.NotNullOrWhiteSpace(senhaHash, nameof(senhaHash));

            var idadeCalculada = dataNascimento.HasValue ? CalcularIdade(dataNascimento.Value) : idade;

            if (idadeCalculada.HasValue && idadeCalculada < 13)
            {
                throw new ArgumentException("Idade mínima é 13 anos.", nameof(idade));
            }

            return new Usuario(
                Guid.NewGuid(),
                nome.Trim(),
                email.Trim().ToLowerInvariant(),
                senhaHash,
                idadeCalculada,
                DateTime.UtcNow,
                cep, logradouro, numero, complemento, bairro, cidade,
                dataNascimento: dataNascimento?.Date);
        }

        public void AceitarConsentimento()
        {
            ConsentimentoAceito = true;
            DataConsentimento = DateTime.UtcNow;
        }

        public void SolicitarExclusao()
        {
            DeletedAt = DateTime.UtcNow;
        }

        public void Anonimizar()
        {
            Nome = "Usuário Removido";
            Email = $"{Id}@anonimizado.invalid";
            SenhaHash = string.Empty;
            DataNascimento = null;
            Cep = null;
            Logradouro = null;
            Numero = null;
            Complemento = null;
            Bairro = null;
            Cidade = null;
            Anonimizado = true;
        }

        public void AtualizarPerfil(string? nome, int? idade, DateTime? dataNascimento = null,
            string? cep = null, string? logradouro = null, string? numero = null,
            string? complemento = null, string? bairro = null, string? cidade = null)
        {
            AtualizarPerfil(nome, null, idade, dataNascimento, cep, logradouro, numero, complemento, bairro, cidade);
        }

        public void AtualizarPerfil(string? nome, string? email, int? idade, DateTime? dataNascimento = null,
            string? cep = null, string? logradouro = null, string? numero = null,
            string? complemento = null, string? bairro = null, string? cidade = null)
        {
            if (nome is not null)
            {
                Nome = EntityGuards.NotNullOrWhiteSpace(nome, nameof(nome));
            }

            if (email is not null)
            {
                var normalizedEmail = EntityGuards.NotNullOrWhiteSpace(email, nameof(email)).Trim().ToLowerInvariant();
                if (!normalizedEmail.Contains('@'))
                {
                    throw new ArgumentException("E-mail inválido.", nameof(email));
                }

                Email = normalizedEmail;
            }

            if (dataNascimento.HasValue)
            {
                var idadeCalculada = CalcularIdade(dataNascimento.Value);
                if (idadeCalculada < 13)
                {
                    throw new ArgumentException("Idade mínima é 13 anos.", nameof(dataNascimento));
                }

                DataNascimento = dataNascimento.Value.Date;
                Idade = idadeCalculada;
            }
            else if (idade.HasValue)
            {
                if (idade < 13)
                {
                    throw new ArgumentException("Idade mínima é 13 anos.", nameof(idade));
                }

                Idade = idade;
            }

            if (cep is not null) Cep = cep;
            if (logradouro is not null) Logradouro = logradouro;
            if (numero is not null) Numero = numero;
            if (complemento is not null) Complemento = complemento;
            if (bairro is not null) Bairro = bairro;
            if (cidade is not null) Cidade = cidade;
        }

        private static int CalcularIdade(DateTime dataNascimento)
        {
            var today = DateTime.UtcNow.Date;
            var birthDate = dataNascimento.Date;
            var age = today.Year - birthDate.Year;

            if (birthDate > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
    }
}
