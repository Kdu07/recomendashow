using BackEnd.Aventura.Infrastructure;
using BackEnd.Aventura.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using Xunit;

namespace BackEnd.Aventura.IntegrationTests;

/// <summary>
/// Exercita o AnonimizacaoJob (BackgroundService) de ponta a ponta contra um Postgres real.
/// A classe depende diretamente de Npgsql/NpgsqlCommand, por isso não é testável com mocks
/// puros — é coberta aqui via integração, e excluída do gate de cobertura unitário
/// (ver Exclude em BackEnd.Aventura.Tests.csproj).
/// </summary>
[Collection("Database")]
public class AnonimizacaoJobTests
{
    private readonly DatabaseFixture _fixture;

    public AnonimizacaoJobTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    private AnonimizacaoJob CreateJob()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IDbConnectionFactory>(new TestConnectionFactory(_fixture.ConnectionString));
        var provider = services.BuildServiceProvider();

        var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
        return new AnonimizacaoJob(scopeFactory, NullLogger<AnonimizacaoJob>.Instance);
    }

    [Fact]
    public async Task ExecuteAsync_DeveAnonimizarUsuarioComDeletedAtAntigo()
    {
        await using var connection = new NpgsqlConnection(_fixture.ConnectionString);
        await connection.OpenAsync();

        var id = Guid.NewGuid();
        await DatabaseHelper.InsertUsuario(connection, id,
            nome: "Usuário Pendente Job",
            deletedAt: DateTime.UtcNow.AddDays(-31),
            anonimizado: false);

        var job = CreateJob();

        using var cts = new CancellationTokenSource();
        await job.StartAsync(cts.Token);

        var anonimizado = await AguardarAnonimizacaoAsync(connection, id, TimeSpan.FromSeconds(10));

        cts.Cancel();
        await job.StopAsync(CancellationToken.None);

        Assert.True(anonimizado, "O AnonimizacaoJob deveria ter anonimizado o usuário pendente.");

        await using var select = new NpgsqlCommand(
            "SELECT nome, anonimizado FROM usuarios WHERE id = @Id", connection);
        select.Parameters.AddWithValue("@Id", id);
        await using var reader = await select.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal("Usuário Removido", reader.GetString(reader.GetOrdinal("nome")));
        Assert.True(reader.GetBoolean(reader.GetOrdinal("anonimizado")));
    }

    [Fact]
    public async Task ExecuteAsync_PrimeiraIteracao_NaoDeveLancarExcecao()
    {
        // Não cria pendência própria; o objetivo é garantir que o laço de ExecuteAsync
        // roda (com ou sem pendências de outros testes da mesma collection) sem lançar
        // exceção não tratada, e que StartAsync/StopAsync funcionam corretamente.
        var job = CreateJob();

        using var cts = new CancellationTokenSource();
        await job.StartAsync(cts.Token);

        await Task.Delay(TimeSpan.FromSeconds(2));

        cts.Cancel();

        await job.StopAsync(CancellationToken.None);
    }

    private static async Task<bool> AguardarAnonimizacaoAsync(NpgsqlConnection connection, Guid id, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            await using var cmd = new NpgsqlCommand(
                "SELECT anonimizado FROM usuarios WHERE id = @Id", connection);
            cmd.Parameters.AddWithValue("@Id", id);
            var result = await cmd.ExecuteScalarAsync();

            if (result is bool anonimizado && anonimizado)
            {
                return true;
            }

            await Task.Delay(200);
        }

        return false;
    }

    private sealed class TestConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;
        public TestConnectionFactory(string connectionString) => _connectionString = connectionString;
        public NpgsqlConnection CreateConnection() => new(_connectionString);
    }
}