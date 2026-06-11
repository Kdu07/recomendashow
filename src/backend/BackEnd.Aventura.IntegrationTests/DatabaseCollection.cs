using Xunit;

namespace BackEnd.Aventura.IntegrationTests;

[CollectionDefinition("Database")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture> { }
