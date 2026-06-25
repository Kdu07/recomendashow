using System;
using System.Collections.Generic;
using BackEnd.Aventura.Infrastructure;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class MongoConnectionFactoryTests
    {
        [Fact]
        public void Constructor_NullConfiguration_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new MongoConnectionFactory(null!));
        }

        [Fact]
        public void Constructor_MissingSettings_ShouldThrowInvalidOperationException()
        {
            var configuration = new ConfigurationBuilder().Build();

            Assert.Throws<InvalidOperationException>(() => new MongoConnectionFactory(configuration));
        }

        [Fact]
        public void Constructor_WithIndividualSettings_ShouldSucceedAndReturnDatabase()
        {
            var myConfiguration = new Dictionary<string, string?>
            {
                {"MONGO_HOST", "localhost"},
                {"MONGO_PORT", "27017"},
                {"MONGO_USER", "admin"},
                {"MONGO_PASSWORD", "senha123"},
                {"MONGO_AUTH_SOURCE", "admin"},
                {"MONGO_DATABASE", "aventura_ml"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            var factory = new MongoConnectionFactory(configuration);
            var database = factory.GetDatabase();

            Assert.NotNull(database);
            Assert.Equal("aventura_ml", database.DatabaseNamespace.DatabaseName);
        }

        [Fact]
        public void Constructor_WithoutDatabaseName_ShouldDefaultToAventuraMl()
        {
            var myConfiguration = new Dictionary<string, string?>
            {
                {"MONGO_HOST", "localhost"},
                {"MONGO_USER", "admin"},
                {"MONGO_PASSWORD", "senha123"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            var factory = new MongoConnectionFactory(configuration);
            var database = factory.GetDatabase();

            Assert.Equal("aventura_ml", database.DatabaseNamespace.DatabaseName);
        }

        [Fact]
        public void Constructor_WithExplicitConnectionString_ShouldSucceed()
        {
            var myConfiguration = new Dictionary<string, string?>
            {
                {"ConnectionStrings:MongoConnection", "mongodb://admin:senha123@localhost:27017/?authSource=admin"},
                {"MONGO_DATABASE", "aventura_ml"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            var factory = new MongoConnectionFactory(configuration);
            var database = factory.GetDatabase();

            Assert.Equal("aventura_ml", database.DatabaseNamespace.DatabaseName);
        }

        [Fact]
        public void Constructor_MissingPassword_ShouldThrowInvalidOperationException()
        {
            var myConfiguration = new Dictionary<string, string?>
            {
                {"MONGO_HOST", "localhost"},
                {"MONGO_USER", "admin"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            Assert.Throws<InvalidOperationException>(() => new MongoConnectionFactory(configuration));
        }
    }
}