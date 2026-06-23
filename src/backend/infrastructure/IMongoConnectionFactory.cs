using MongoDB.Driver;

namespace BackEnd.Aventura.Infrastructure
{
    public interface IMongoConnectionFactory
    {
        IMongoDatabase GetDatabase();
    }
}