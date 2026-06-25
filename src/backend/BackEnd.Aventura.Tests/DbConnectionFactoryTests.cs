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
            var configuration = new ConfigurationBuilder().Build();

            Assert.Throws<InvalidOperationException>(() => new DbConnectionFactory(configuration));
        }

        [Fact]
        public void Constructor_WithDefaultConnection_ShouldSucceed()
        {
            var myConfiguration = new Dictionary<string, string?>
            {
                {"ConnectionStrings:DefaultConnection", "Host=localhost;Database=test;Username=postgres;Password=pwd"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            var factory = new DbConnectionFactory(configuration);
            var connection = factory.CreateConnection();

            Assert.NotNull(connection);
            Assert.Contains("localhost", connection.ConnectionString);
            Assert.Contains("test", connection.ConnectionString);
        }

        [Fact]
        public void Constructor_WithIndividualSettings_ShouldSucceed()
        {
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

            var factory = new DbConnectionFactory(configuration);
            var connection = factory.CreateConnection();

            Assert.NotNull(connection);
            Assert.Contains("localhost", connection.ConnectionString);
            Assert.Contains("testdb", connection.ConnectionString);
        }

        [Fact]
        public void Constructor_WithPostgresPrefixedSettings_ShouldSucceed()
        {
            var myConfiguration = new Dictionary<string, string?>
            {
                {"POSTGRES_HOST", "db.internal"},
                {"POSTGRES_PORT", "5433"},
                {"POSTGRES_DB", "aventuradb"},
                {"POSTGRES_USER", "aventurauser"},
                {"POSTGRES_PASSWORD", "aventurapwd"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            var factory = new DbConnectionFactory(configuration);
            var connection = factory.CreateConnection();

            Assert.NotNull(connection);
            Assert.Contains("db.internal", connection.ConnectionString);
            Assert.Contains("aventuradb", connection.ConnectionString);
            Assert.Contains("5433", connection.ConnectionString);
        }

        [Fact]
        public void Constructor_WithInvalidPort_ShouldFallbackToDefaultPort()
        {
            var myConfiguration = new Dictionary<string, string?>
            {
                {"DB_HOST", "localhost"},
                {"DB_PORT", "nao-e-um-numero"},
                {"DB_NAME", "testdb"},
                {"DB_USER", "testuser"},
                {"DB_PASSWORD", "testpwd"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            var factory = new DbConnectionFactory(configuration);
            var connection = factory.CreateConnection();

            Assert.NotNull(connection);
            Assert.Contains("5432", connection.ConnectionString);
        }

        [Fact]
        public void Constructor_MissingPassword_ShouldThrowInvalidOperationException()
        {
            var myConfiguration = new Dictionary<string, string?>
            {
                {"DB_HOST", "localhost"},
                {"DB_NAME", "testdb"},
                {"DB_USER", "testuser"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            Assert.Throws<InvalidOperationException>(() => new DbConnectionFactory(configuration));
        }
    }
}