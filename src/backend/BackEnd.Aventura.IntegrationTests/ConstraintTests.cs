using Npgsql;
using Xunit;

namespace BackEnd.Aventura.IntegrationTests;

[Collection("Database")]
public class ConstraintTests
{
    private readonly DatabaseFixture _fixture;

    public ConstraintTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    private async Task<(Guid usuarioId, Guid espetaculoId)> InsertBaseDataAsync(NpgsqlConnection conn)
    {
        var usuarioId    = Guid.NewGuid();
        var espetaculoId = Guid.NewGuid();
        await DatabaseHelper.InsertUsuario(conn, usuarioId);
        await DatabaseHelper.InsertEspetaculo(conn, espetaculoId);
        return (usuarioId, espetaculoId);
    }

    // ────────────────────────────────────────────────────────────────
    // chk_pontuacao
    // ────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task Interacao_DeveAceitar_PontuacaoValida(decimal pontuacao)
    {
        await using var conn = new NpgsqlConnection(_fixture.ConnectionString);
        await conn.OpenAsync();
        var (usuarioId, espetaculoId) = await InsertBaseDataAsync(conn);

        const string sql = @"
INSERT INTO interacoes (usuario_id, espetaculo_id, tipo, pontuacao, data_expiracao)
VALUES (@U, @E, CAST(@T AS tipo_interacao), @P, NOW() + INTERVAL '1 day')";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@U", usuarioId);
        cmd.Parameters.AddWithValue("@E", espetaculoId);
        cmd.Parameters.AddWithValue("@T", "Clique");
        cmd.Parameters.AddWithValue("@P", pontuacao);
        await cmd.ExecuteNonQueryAsync();  // não deve lançar
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(11)]
    public async Task Interacao_DeveRejeitar_PontuacaoFora(decimal pontuacao)
    {
        await using var conn = new NpgsqlConnection(_fixture.ConnectionString);
        await conn.OpenAsync();
        var (usuarioId, espetaculoId) = await InsertBaseDataAsync(conn);

        const string sql = @"
INSERT INTO interacoes (usuario_id, espetaculo_id, tipo, pontuacao, data_expiracao)
VALUES (@U, @E, CAST(@T AS tipo_interacao), @P, NOW() + INTERVAL '1 day')";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@U", usuarioId);
        cmd.Parameters.AddWithValue("@E", espetaculoId);
        cmd.Parameters.AddWithValue("@T", "Clique");
        cmd.Parameters.AddWithValue("@P", pontuacao);

        var ex = await Assert.ThrowsAsync<PostgresException>(() => cmd.ExecuteNonQueryAsync());
        Assert.Equal("23514", ex.SqlState);  // check_violation
    }

    // ────────────────────────────────────────────────────────────────
    // chk_data_expiracao
    // ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Interacao_DeveAceitar_DataExpiracaoFutura()
    {
        await using var conn = new NpgsqlConnection(_fixture.ConnectionString);
        await conn.OpenAsync();
        var (usuarioId, espetaculoId) = await InsertBaseDataAsync(conn);

        const string sql = @"
INSERT INTO interacoes (usuario_id, espetaculo_id, tipo, data_expiracao)
VALUES (@U, @E, CAST(@T AS tipo_interacao), NOW() + INTERVAL '1 day')";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@U", usuarioId);
        cmd.Parameters.AddWithValue("@E", espetaculoId);
        cmd.Parameters.AddWithValue("@T", "Clique");
        await cmd.ExecuteNonQueryAsync();
    }

    [Theory]
    [InlineData(0)]   // igual a data_registro
    [InlineData(-60)] // anterior a data_registro (segundos de offset)
    public async Task Interacao_DeveRejeitar_DataExpiracaoInvalida(int offsetSeconds)
    {
        await using var conn = new NpgsqlConnection(_fixture.ConnectionString);
        await conn.OpenAsync();
        var (usuarioId, espetaculoId) = await InsertBaseDataAsync(conn);

        var registro  = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var expiracao = registro.AddSeconds(offsetSeconds);

        const string sql = @"
INSERT INTO interacoes (usuario_id, espetaculo_id, tipo, data_registro, data_expiracao)
VALUES (@U, @E, CAST(@T AS tipo_interacao), @DR, @DE)";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@U",  usuarioId);
        cmd.Parameters.AddWithValue("@E",  espetaculoId);
        cmd.Parameters.AddWithValue("@T",  "Clique");
        cmd.Parameters.AddWithValue("@DR", registro);
        cmd.Parameters.AddWithValue("@DE", expiracao);

        var ex = await Assert.ThrowsAsync<PostgresException>(() => cmd.ExecuteNonQueryAsync());
        Assert.Equal("23514", ex.SqlState);  // check_violation
    }

    // ────────────────────────────────────────────────────────────────
    // uq_recomendacao_usuario_espetaculo
    // ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Recomendacao_DeveRejeitar_ParDuplicado()
    {
        await using var conn = new NpgsqlConnection(_fixture.ConnectionString);
        await conn.OpenAsync();
        var (usuarioId, espetaculoId) = await InsertBaseDataAsync(conn);

        const string sql = @"
INSERT INTO recomendacoes (usuario_id, espetaculo_id, score)
VALUES (@U, @E, @S)";

        await using var cmd1 = new NpgsqlCommand(sql, conn);
        cmd1.Parameters.AddWithValue("@U", usuarioId);
        cmd1.Parameters.AddWithValue("@E", espetaculoId);
        cmd1.Parameters.AddWithValue("@S", 0.9m);
        await cmd1.ExecuteNonQueryAsync();

        await using var cmd2 = new NpgsqlCommand(sql, conn);
        cmd2.Parameters.AddWithValue("@U", usuarioId);
        cmd2.Parameters.AddWithValue("@E", espetaculoId);
        cmd2.Parameters.AddWithValue("@S", 0.8m);

        var ex = await Assert.ThrowsAsync<PostgresException>(() => cmd2.ExecuteNonQueryAsync());
        Assert.Equal("23505", ex.SqlState);  // unique_violation
    }

    [Fact]
    public async Task Recomendacao_DeveAtualizar_ViaOnConflict()
    {
        await using var conn = new NpgsqlConnection(_fixture.ConnectionString);
        await conn.OpenAsync();
        var (usuarioId, espetaculoId) = await InsertBaseDataAsync(conn);

        const string insert = @"
INSERT INTO recomendacoes (usuario_id, espetaculo_id, score)
VALUES (@U, @E, @S)";

        await using var cmd1 = new NpgsqlCommand(insert, conn);
        cmd1.Parameters.AddWithValue("@U", usuarioId);
        cmd1.Parameters.AddWithValue("@E", espetaculoId);
        cmd1.Parameters.AddWithValue("@S", 0.5m);
        await cmd1.ExecuteNonQueryAsync();

        const string upsert = @"
INSERT INTO recomendacoes (usuario_id, espetaculo_id, score)
VALUES (@U, @E, @S)
ON CONFLICT (usuario_id, espetaculo_id) DO UPDATE SET score = EXCLUDED.score";

        await using var cmd2 = new NpgsqlCommand(upsert, conn);
        cmd2.Parameters.AddWithValue("@U", usuarioId);
        cmd2.Parameters.AddWithValue("@E", espetaculoId);
        cmd2.Parameters.AddWithValue("@S", 0.95m);
        await cmd2.ExecuteNonQueryAsync();

        await using var select = new NpgsqlCommand(
            "SELECT score FROM recomendacoes WHERE usuario_id = @U AND espetaculo_id = @E", conn);
        select.Parameters.AddWithValue("@U", usuarioId);
        select.Parameters.AddWithValue("@E", espetaculoId);
        var score = (decimal?)await select.ExecuteScalarAsync();

        Assert.Equal(0.95m, score);
    }
}
