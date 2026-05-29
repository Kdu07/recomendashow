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

            const string sql = @"
SELECT id, nome, email, senha_hash, idade, data_cadastro
FROM usuarios
WHERE id = @Id";

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

            const string sql = @"
SELECT id, nome, email, senha_hash, idade, data_cadastro
FROM usuarios
WHERE LOWER(email) = LOWER(@Email)";

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

            const string sql = @"
INSERT INTO usuarios(id, nome, email, senha_hash, idade, data_cadastro)
VALUES (@Id, @Nome, @Email, @SenhaHash, @Idade, @DataCadastro)";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", usuario.Id);
            command.Parameters.AddWithValue("@Nome", usuario.Nome);
            command.Parameters.AddWithValue("@Email", usuario.Email);
            command.Parameters.AddWithValue("@SenhaHash", usuario.SenhaHash);
            command.Parameters.AddWithValue("@Idade", (object?)usuario.Idade ?? DBNull.Value);
            command.Parameters.AddWithValue("@DataCadastro", usuario.DataCadastro);
            command.ExecuteNonQuery();

            return usuario;
        }

        public bool Update(Usuario usuario)
        {
            if (usuario is null) throw new ArgumentNullException(nameof(usuario));

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            const string sql = @"
UPDATE usuarios
SET nome = @Nome, idade = @Idade
WHERE id = @Id";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", usuario.Id);
            command.Parameters.AddWithValue("@Nome", usuario.Nome);
            command.Parameters.AddWithValue("@Idade", (object?)usuario.Idade ?? DBNull.Value);

            return command.ExecuteNonQuery() > 0;
        }

        private static Usuario MapUsuario(NpgsqlDataReader reader)
        {
            return new Usuario(
                reader.GetGuid(reader.GetOrdinal("id")),
                reader.GetString(reader.GetOrdinal("nome")),
                reader.GetString(reader.GetOrdinal("email")),
                reader.GetString(reader.GetOrdinal("senha_hash")),
                reader.IsDBNull(reader.GetOrdinal("idade")) ? null : reader.GetInt32(reader.GetOrdinal("idade")),
                reader.GetDateTime(reader.GetOrdinal("data_cadastro")));
        }
    }
}
