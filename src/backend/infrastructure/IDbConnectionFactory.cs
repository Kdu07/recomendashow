using Npgsql;

namespace BackEnd.Aventura.Infrastructure
{
    public interface IDbConnectionFactory
    {
        NpgsqlConnection CreateConnection();
    }
}
