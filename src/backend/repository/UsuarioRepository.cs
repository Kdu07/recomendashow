using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Infrastructure;
using Npgsql;

namespace BackEnd.Aventura.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UsuarioRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public Usuario? GetById(Guid id)
        {
            EntityGuards.NotEmpty(id, nameof(id));

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            EnsureDataNascimentoColumn(connection);

            const string sql = @"
SELECT id, nome, email, senha_hash, idade, data_nascimento, data_cadastro,
       cep, logradouro, numero, complemento, bairro, cidade,
       consentimento_aceito, data_consentimento, deleted_at, anonimizado
FROM usuarios
WHERE id = @Id
  AND deleted_at IS NULL";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            return reader.Read() ? MapUsuario(reader) : null;
        }

        public Usuario? GetByEmail(string email)
        {
            var normalizedEmail = EntityGuards.NotNullOrWhiteSpace(email, nameof(email));

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            EnsureDataNascimentoColumn(connection);

            const string sql = @"
SELECT id, nome, email, senha_hash, idade, data_nascimento, data_cadastro,
       cep, logradouro, numero, complemento, bairro, cidade,
       consentimento_aceito, data_consentimento, deleted_at, anonimizado
FROM usuarios
WHERE LOWER(email) = LOWER(@Email)
  AND deleted_at IS NULL";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", normalizedEmail);

            using var reader = command.ExecuteReader();
            return reader.Read() ? MapUsuario(reader) : null;
        }

        public bool EmailExists(string email)
        {
            var normalizedEmail = EntityGuards.NotNullOrWhiteSpace(email, nameof(email));

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            const string sql = @"SELECT EXISTS(SELECT 1 FROM usuarios WHERE LOWER(email) = LOWER(@Email))";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", normalizedEmail);

            return command.ExecuteScalar() is bool exists && exists;
        }

        public Usuario Create(Usuario usuario)
        {
            if (usuario is null) throw new ArgumentNullException(nameof(usuario));

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            EnsureDataNascimentoColumn(connection);

            const string sql = @"
INSERT INTO usuarios(id, nome, email, senha_hash, idade, data_nascimento, data_cadastro,
                     cep, logradouro, numero, complemento, bairro, cidade,
                     consentimento_aceito, data_consentimento)
VALUES (@Id, @Nome, @Email, @SenhaHash, @Idade, @DataNascimento, @DataCadastro,
        @Cep, @Logradouro, @Numero, @Complemento, @Bairro, @Cidade,
        @ConsentimentoAceito, @DataConsentimento)";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", usuario.Id);
            command.Parameters.AddWithValue("@Nome", usuario.Nome);
            command.Parameters.AddWithValue("@Email", usuario.Email);
            command.Parameters.AddWithValue("@SenhaHash", usuario.SenhaHash);
            command.Parameters.AddWithValue("@Idade", (object?)usuario.Idade ?? DBNull.Value);
            command.Parameters.AddWithValue("@DataNascimento", (object?)usuario.DataNascimento?.Date ?? DBNull.Value);
            command.Parameters.AddWithValue("@DataCadastro", usuario.DataCadastro);
            command.Parameters.AddWithValue("@Cep", (object?)usuario.Cep ?? DBNull.Value);
            command.Parameters.AddWithValue("@Logradouro", (object?)usuario.Logradouro ?? DBNull.Value);
            command.Parameters.AddWithValue("@Numero", (object?)usuario.Numero ?? DBNull.Value);
            command.Parameters.AddWithValue("@Complemento", (object?)usuario.Complemento ?? DBNull.Value);
            command.Parameters.AddWithValue("@Bairro", (object?)usuario.Bairro ?? DBNull.Value);
            command.Parameters.AddWithValue("@Cidade", (object?)usuario.Cidade ?? DBNull.Value);
            command.Parameters.AddWithValue("@ConsentimentoAceito", usuario.ConsentimentoAceito);
            command.Parameters.AddWithValue("@DataConsentimento", (object?)usuario.DataConsentimento ?? DBNull.Value);
            command.ExecuteNonQuery();

            return usuario;
        }

        public bool Update(Usuario usuario)
        {
            if (usuario is null) throw new ArgumentNullException(nameof(usuario));

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            EnsureDataNascimentoColumn(connection);

            const string sql = @"
UPDATE usuarios
SET nome = @Nome, email = @Email, idade = @Idade, data_nascimento = @DataNascimento,
    cep = @Cep, logradouro = @Logradouro, numero = @Numero,
    complemento = @Complemento, bairro = @Bairro, cidade = @Cidade
WHERE id = @Id";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", usuario.Id);
            command.Parameters.AddWithValue("@Nome", usuario.Nome);
            command.Parameters.AddWithValue("@Email", usuario.Email);
            command.Parameters.AddWithValue("@Idade", (object?)usuario.Idade ?? DBNull.Value);
            command.Parameters.AddWithValue("@DataNascimento", (object?)usuario.DataNascimento?.Date ?? DBNull.Value);
            command.Parameters.AddWithValue("@Cep", (object?)usuario.Cep ?? DBNull.Value);
            command.Parameters.AddWithValue("@Logradouro", (object?)usuario.Logradouro ?? DBNull.Value);
            command.Parameters.AddWithValue("@Numero", (object?)usuario.Numero ?? DBNull.Value);
            command.Parameters.AddWithValue("@Complemento", (object?)usuario.Complemento ?? DBNull.Value);
            command.Parameters.AddWithValue("@Bairro", (object?)usuario.Bairro ?? DBNull.Value);
            command.Parameters.AddWithValue("@Cidade", (object?)usuario.Cidade ?? DBNull.Value);

            return command.ExecuteNonQuery() > 0;
        }

        public bool SolicitarExclusao(Guid id)
        {
            EntityGuards.NotEmpty(id, nameof(id));

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            EnsureDataNascimentoColumn(connection);

            const string sql = @"
UPDATE usuarios
SET deleted_at = @DeletedAt
WHERE id = @Id
  AND deleted_at IS NULL";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@DeletedAt", DateTime.UtcNow);

            return command.ExecuteNonQuery() > 0;
        }

        public async Task<int?> ObterMlIdAsync(Guid usuarioId)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string sql = "SELECT ml_id FROM usuarios WHERE id = @Id";

            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", usuarioId);

            var result = await command.ExecuteScalarAsync();
            return result is int mlId ? mlId : null;
        }

        private static void EnsureDataNascimentoColumn(NpgsqlConnection connection)
        {
            const string sql = "ALTER TABLE usuarios ADD COLUMN IF NOT EXISTS data_nascimento DATE";
            using var command = new NpgsqlCommand(sql, connection);
            command.ExecuteNonQuery();
        }

        private static Usuario MapUsuario(NpgsqlDataReader reader)
        {
            return new Usuario(
                reader.GetGuid(reader.GetOrdinal("id")),
                reader.GetString(reader.GetOrdinal("nome")),
                reader.GetString(reader.GetOrdinal("email")),
                reader.GetString(reader.GetOrdinal("senha_hash")),
                reader.IsDBNull(reader.GetOrdinal("idade")) ? null : reader.GetInt32(reader.GetOrdinal("idade")),
                reader.GetDateTime(reader.GetOrdinal("data_cadastro")),
                reader.IsDBNull(reader.GetOrdinal("cep")) ? null : reader.GetString(reader.GetOrdinal("cep")),
                reader.IsDBNull(reader.GetOrdinal("logradouro")) ? null : reader.GetString(reader.GetOrdinal("logradouro")),
                reader.IsDBNull(reader.GetOrdinal("numero")) ? null : reader.GetString(reader.GetOrdinal("numero")),
                reader.IsDBNull(reader.GetOrdinal("complemento")) ? null : reader.GetString(reader.GetOrdinal("complemento")),
                reader.IsDBNull(reader.GetOrdinal("bairro")) ? null : reader.GetString(reader.GetOrdinal("bairro")),
                reader.IsDBNull(reader.GetOrdinal("cidade")) ? null : reader.GetString(reader.GetOrdinal("cidade")),
                reader.GetBoolean(reader.GetOrdinal("consentimento_aceito")),
                reader.IsDBNull(reader.GetOrdinal("data_consentimento")) ? null : reader.GetDateTime(reader.GetOrdinal("data_consentimento")),
                reader.IsDBNull(reader.GetOrdinal("deleted_at")) ? null : reader.GetDateTime(reader.GetOrdinal("deleted_at")),
                reader.GetBoolean(reader.GetOrdinal("anonimizado")),
                reader.IsDBNull(reader.GetOrdinal("data_nascimento")) ? null : reader.GetDateTime(reader.GetOrdinal("data_nascimento")));
        }
    }
}
