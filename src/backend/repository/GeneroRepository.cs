using BackEnd.Aventura.Infrastructure;
using Npgsql;

namespace BackEnd.Aventura.Repository
{
    public class GeneroRepository : IGeneroRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public GeneroRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public IEnumerable<string> GetAllNomes()
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            const string sql = @"SELECT nome FROM generos ORDER BY nome";

            using var command = new NpgsqlCommand(sql, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                yield return reader.GetString(reader.GetOrdinal("nome"));
            }
        }
    }
}
