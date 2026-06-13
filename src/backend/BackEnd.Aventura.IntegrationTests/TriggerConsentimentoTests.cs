using Npgsql;
using Xunit;

namespace BackEnd.Aventura.IntegrationTests;

[Collection("Database")]
public class TriggerConsentimentoTests
{
    private readonly DatabaseFixture _fixture;

    public TriggerConsentimentoTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Consentimento_DevePreencher_DataConsentimento_QuandoOmitidoPelaAplicacao()
    {
        // Trigger deve preencher data_consentimento = CURRENT_TIMESTAMP
        // quando a aplicação só envia consentimento_aceito = TRUE sem o timestamp.
        await using var conn = new NpgsqlConnection(_fixture.ConnectionString);
        await conn.OpenAsync();

        var id = Guid.NewGuid();
        await DatabaseHelper.InsertUsuario(conn, id, consentimentoAceito: false);

        await using var update = new NpgsqlCommand(
            "UPDATE usuarios SET consentimento_aceito = TRUE WHERE id = @Id", conn);
        update.Parameters.AddWithValue("@Id", id);
        await update.ExecuteNonQueryAsync();

        await using var select = new NpgsqlCommand(
            "SELECT data_consentimento FROM usuarios WHERE id = @Id", conn);
        select.Parameters.AddWithValue("@Id", id);
        var dt = (DateTime?)await select.ExecuteScalarAsync();

        Assert.NotNull(dt);
    }

    [Fact]
    public async Task Consentimento_NaoDeveSubstituir_DataConsentimentoExplicita()
    {
        // Quando a aplicação fornece data_consentimento explicitamente,
        // o trigger não deve sobrescrever o valor.
        await using var conn = new NpgsqlConnection(_fixture.ConnectionString);
        await conn.OpenAsync();

        var id         = Guid.NewGuid();
        var dtExplicita = new DateTime(2020, 6, 1, 10, 0, 0, DateTimeKind.Utc);
        await DatabaseHelper.InsertUsuario(conn, id, consentimentoAceito: false);

        await using var update = new NpgsqlCommand(@"
UPDATE usuarios
SET consentimento_aceito = TRUE,
    data_consentimento   = @Dt
WHERE id = @Id", conn);
        update.Parameters.AddWithValue("@Id", id);
        update.Parameters.AddWithValue("@Dt", dtExplicita);
        await update.ExecuteNonQueryAsync();

        await using var select = new NpgsqlCommand(
            "SELECT data_consentimento FROM usuarios WHERE id = @Id", conn);
        select.Parameters.AddWithValue("@Id", id);
        var dtStored = (DateTime?)await select.ExecuteScalarAsync();

        Assert.Equal(dtExplicita, dtStored!.Value.ToUniversalTime());
    }

    [Fact]
    public async Task Consentimento_NaoDeveAlterar_DataConsentimento_EmReUpdate()
    {
        // Quando consentimento_aceito já era TRUE (OLD = TRUE),
        // atualizações em outros campos não devem mudar data_consentimento.
        await using var conn = new NpgsqlConnection(_fixture.ConnectionString);
        await conn.OpenAsync();

        var id         = Guid.NewGuid();
        var dtOriginal = new DateTime(2020, 6, 1, 10, 0, 0, DateTimeKind.Utc);
        // Inserção direta com consentimento já TRUE e data conhecida
        await DatabaseHelper.InsertUsuario(conn, id,
            consentimentoAceito: true,
            dataConsentimento: dtOriginal);

        // UPDATE em outro campo
        await using var update = new NpgsqlCommand(
            "UPDATE usuarios SET cidade = 'Campinas' WHERE id = @Id", conn);
        update.Parameters.AddWithValue("@Id", id);
        await update.ExecuteNonQueryAsync();

        await using var select = new NpgsqlCommand(
            "SELECT data_consentimento FROM usuarios WHERE id = @Id", conn);
        select.Parameters.AddWithValue("@Id", id);
        var dtStored = (DateTime?)await select.ExecuteScalarAsync();

        Assert.Equal(dtOriginal, dtStored!.Value.ToUniversalTime());
    }

    [Fact]
    public async Task Consentimento_DeveManterNull_QuandoConsentimentoPermanecefalso()
    {
        // UPDATE em outros campos com consentimento_aceito = FALSE (padrão)
        // não deve tocar em data_consentimento.
        await using var conn = new NpgsqlConnection(_fixture.ConnectionString);
        await conn.OpenAsync();

        var id = Guid.NewGuid();
        await DatabaseHelper.InsertUsuario(conn, id, consentimentoAceito: false);

        await using var update = new NpgsqlCommand(
            "UPDATE usuarios SET cidade = 'Curitiba' WHERE id = @Id", conn);
        update.Parameters.AddWithValue("@Id", id);
        await update.ExecuteNonQueryAsync();

        await using var select = new NpgsqlCommand(
            "SELECT data_consentimento FROM usuarios WHERE id = @Id", conn);
        select.Parameters.AddWithValue("@Id", id);
        var raw = await select.ExecuteScalarAsync();
        var dt  = raw is DBNull ? null : (DateTime?)raw;

        Assert.Null(dt);
    }
}
