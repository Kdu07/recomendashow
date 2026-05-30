using System;
using System.Collections.Generic;
using BackEnd.Aventura.Infrastructure;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class DbConnectionFactoryTests
    {
        [Fact]
        public void Constructor_NullConfiguration_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new DbConnectionFactory(null!));
        }

        [Fact]
        public void Constructor_MissingSettings_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new DbConnectionFactory(configuration));
        }

        [Fact]
        public void Constructor_WithDefaultConnection_ShouldSucceed()
        {
            // Arrange
            var myConfiguration = new Dictionary<string, string?>
            {
                {"ConnectionStrings:DefaultConnection", "Host=localhost;Database=test;Username=postgres;Password=pwd"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            // Act
            var factory = new DbConnectionFactory(configuration);
            var connection = factory.CreateConnection();

            // Assert
            Assert.NotNull(connection);
            Assert.Contains("localhost", connection.ConnectionString);
            Assert.Contains("test", connection.ConnectionString);
        }

        [Fact]
        public void Constructor_WithIndividualSettings_ShouldSucceed()
        {
            // Arrange
            var myConfiguration = new Dictionary<string, string?>
            {
                {"DB_HOST", "localhost"},
                {"DB_PORT", "5432"},
                {"DB_NAME", "testdb"},
                {"DB_USER", "testuser"},
                {"DB_PASSWORD", "testpwd"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            // Act
            var factory = new DbConnectionFactory(configuration);
            var connection = factory.CreateConnection();

            // Assert
            Assert.NotNull(connection);
            Assert.Contains("localhost", connection.ConnectionString);
            Assert.Contains("testdb", connection.ConnectionString);
        }
    }
}
