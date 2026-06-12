using Npgsql;
using Xunit;

namespace BackEnd.Aventura.IntegrationTests;

[Collection("Database")]
public class TimestamptzTests
{
    private readonly DatabaseFixture _fixture;

    public TimestamptzTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [InlineData("data_inicio")]
    [InlineData("data_fim")]
    public async Task Espetaculos_ColunasDeData_DevemSerTimestamptz(string coluna)
    {
        await using var conn = new NpgsqlConnection(_fixture.ConnectionString);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(@"
SELECT data_type FROM information_schema.columns
WHERE table_name = 'espetaculos' AND column_name = @Col", conn);
        cmd.Parameters.AddWithValue("@Col", coluna);

        var tipo = (string?)await cmd.ExecuteScalarAsync();
        Assert.Equal("timestamp with time zone", tipo);
    }

    [Fact]
    public async Task Espetaculos_InsercaoComOffset_DeveArmazenarEmUtc()
    {
        // 20h00 BRT (UTC-3) = 23h00 UTC
        await using var conn = new NpgsqlConnection(_fixture.ConnectionString);
        await conn.OpenAsync();

        var id = Guid.NewGuid();
        const string sql = @"
INSERT INTO espetaculos (id, nome, data_inicio, data_fim, valor_ingresso)
VALUES (@Id, @Nome,
        '2025-10-01 20:00:00-03'::TIMESTAMPTZ,
        '2025-10-02 00:00:00-03'::TIMESTAMPTZ,
        0)";

        await using var insert = new NpgsqlCommand(sql, conn);
        insert.Parameters.AddWithValue("@Id",   id);
        insert.Parameters.AddWithValue("@Nome", $"_tz_{id}");
        await insert.ExecuteNonQueryAsync();

        await using var select = new NpgsqlCommand(
            "SELECT data_inicio AT TIME ZONE 'UTC' FROM espetaculos WHERE id = @Id", conn);
        select.Parameters.AddWithValue("@Id", id);
        var storedUtc = (DateTime?)await select.ExecuteScalarAsync();

        Assert.NotNull(storedUtc);
        Assert.Equal(new DateTime(2025, 10, 1, 23, 0, 0, DateTimeKind.Unspecified), storedUtc!.Value);
    }
}
