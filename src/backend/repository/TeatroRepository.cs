using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Infrastructure;
using BackEnd.Aventura.Infrastructure.Persistence;
using Npgsql;

namespace BackEnd.Aventura.Repository
{
    public class TeatroRepository : ITeatroRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public TeatroRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public IReadOnlyList<Teatro> GetAll()
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            const string sql = @"
SELECT id, nome, endereco, cidade, horario_funcionamento
FROM teatros
ORDER BY nome";

            using var command = new NpgsqlCommand(sql, connection);
            using var reader = command.ExecuteReader();

            var teatros = new List<Teatro>();
            while (reader.Read())
            {
                teatros.Add(MapTeatro(reader));
            }

            return teatros;
        }

        public Teatro? GetById(Guid id)
        {
            EntityGuards.NotEmpty(id, nameof(id));

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            const string sql = @"
SELECT id, nome, endereco, cidade, horario_funcionamento
FROM teatros
WHERE id = @Id";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            var teatro = MapTeatro(reader);
            reader.Close();
            LoadEspetaculos(teatro, connection);
            return teatro;
        }

        private static Teatro MapTeatro(NpgsqlDataReader reader) =>
            new(
                reader.GetGuid(reader.GetOrdinal("id")),
                reader.GetString(reader.GetOrdinal("nome")),
                reader.GetString(reader.GetOrdinal("endereco")),
                reader.GetString(reader.GetOrdinal("cidade")),
                reader.GetString(reader.GetOrdinal("horario_funcionamento")));

        private static void LoadEspetaculos(Teatro teatro, NpgsqlConnection connection)
        {
            var sql = $@"
{EspetaculoQueries.SelectColumns}
FROM espetaculos e
JOIN teatro_espetaculo te ON te.espetaculo_id = e.id
WHERE te.teatro_id = @TeatroId";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@TeatroId", teatro.Id);

            using var reader = command.ExecuteReader();
            foreach (var espetaculo in EspetaculoMapper.MapAll(reader))
            {
                teatro.AdicionarEspetaculo(espetaculo);
            }
        }
    }
}
