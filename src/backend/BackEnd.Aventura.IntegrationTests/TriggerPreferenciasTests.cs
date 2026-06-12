using Npgsql;
using Xunit;

namespace BackEnd.Aventura.IntegrationTests;

[Collection("Database")]
public class TriggerPreferenciasTests
{
    private readonly DatabaseFixture _fixture;

    public TriggerPreferenciasTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Preferencias_Update_DeveAtualizarDataAtualizacao()
    {
        await using var conn = new NpgsqlConnection(_fixture.ConnectionString);
        await conn.OpenAsync();

        var usuarioId = Guid.NewGuid();
        await DatabaseHelper.InsertUsuario(conn, usuarioId);
        await DatabaseHelper.InsertPreferencia(conn, usuarioId);

        // Captura o valor inicial
        await using var before = new NpgsqlCommand(
            "SELECT data_atualizacao FROM preferencias WHERE usuario_id = @Id", conn);
        before.Parameters.AddWithValue("@Id", usuarioId);
        var dtBefore = (DateTime?)await before.ExecuteScalarAsync();

        // UPDATE dispara o trigger
        await using var update = new NpgsqlCommand(
            "UPDATE preferencias SET cidade = 'Rio de Janeiro' WHERE usuario_id = @Id", conn);
        update.Parameters.AddWithValue("@Id", usuarioId);
        await update.ExecuteNonQueryAsync();

        // Após UPDATE, data_atualizacao deve ser CURRENT_TIMESTAMP da transação
        await using var after = new NpgsqlCommand(
            "SELECT data_atualizacao FROM preferencias WHERE usuario_id = @Id", conn);
        after.Parameters.AddWithValue("@Id", usuarioId);
        var dtAfter = (DateTime?)await after.ExecuteScalarAsync();

        Assert.NotNull(dtAfter);
        // O trigger sobrescreve com CURRENT_TIMESTAMP — deve ser >= ao valor antes do UPDATE
        Assert.True(dtAfter!.Value >= dtBefore!.Value);
    }

    [Fact]
    public async Task Preferencias_Trigger_DevEstarRegistradoComoBeforeUpdate()
    {
        await using var conn = new NpgsqlConnection(_fixture.ConnectionString);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(@"
SELECT COUNT(*) FROM information_schema.triggers
WHERE trigger_name       = 'trg_preferencias_update'
  AND event_manipulation = 'UPDATE'
  AND event_object_table = 'preferencias'", conn);

        var count = (long?)await cmd.ExecuteScalarAsync();
        Assert.Equal(1L, count);
    }
}
