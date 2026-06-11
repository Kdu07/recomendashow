using Npgsql;
using Xunit;

namespace BackEnd.Aventura.IntegrationTests;

[Collection("Database")]
public class SchemaTests
{
    private readonly DatabaseFixture _fixture;

    public SchemaTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [InlineData("usuarios",      "anonimizado_em",  "timestamp with time zone")]
    [InlineData("recomendacoes", "visualizada_em",  "timestamp with time zone")]
    public async Task Schema_NovasColunas_DevemExistirComTipoCorreto(
        string tabela, string coluna, string tipoEsperado)
    {
        await using var conn = new NpgsqlConnection(_fixture.ConnectionString);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(@"
SELECT data_type FROM information_schema.columns
WHERE table_name = @T AND column_name = @C", conn);
        cmd.Parameters.AddWithValue("@T", tabela);
        cmd.Parameters.AddWithValue("@C", coluna);

        var tipo = (string?)await cmd.ExecuteScalarAsync();
        Assert.Equal(tipoEsperado, tipo);
    }

    [Theory]
    [InlineData("idx_interacoes_usuario_expiracao")]
    [InlineData("idx_teatros_cidade")]
    [InlineData("idx_espetaculos_datas")]
    [InlineData("idx_recomendacoes_usuario_score")]
    [InlineData("idx_wishlist_usuario")]
    [InlineData("idx_usuarios_ml_id")]
    public async Task Schema_IndicesSprint4_DevemExistir(string nomeIndice)
    {
        await using var conn = new NpgsqlConnection(_fixture.ConnectionString);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "SELECT COUNT(*) FROM pg_indexes WHERE indexname = @N", conn);
        cmd.Parameters.AddWithValue("@N", nomeIndice);

        var count = (long?)await cmd.ExecuteScalarAsync();
        Assert.Equal(1L, count);
    }

    [Theory]
    [InlineData("idx_interacoes_usuario")]
    [InlineData("idx_recomendacao_usuario")]
    public async Task Schema_IndicesAntigos_NaoDevemExistir(string nomeIndice)
    {
        await using var conn = new NpgsqlConnection(_fixture.ConnectionString);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "SELECT COUNT(*) FROM pg_indexes WHERE indexname = @N", conn);
        cmd.Parameters.AddWithValue("@N", nomeIndice);

        var count = (long?)await cmd.ExecuteScalarAsync();
        Assert.Equal(0L, count);
    }
}
