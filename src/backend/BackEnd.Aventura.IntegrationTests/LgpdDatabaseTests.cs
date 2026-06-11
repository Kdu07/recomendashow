using Npgsql;
using Xunit;

namespace BackEnd.Aventura.IntegrationTests;

public class LgpdDatabaseTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public LgpdDatabaseTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    // ────────────────────────────────────────────────────────────────
    // GRUPO 1 — sp_anonimizar_usuario
    // ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Anonimizar_DeveZerarCamposPII_QuandoDeletedAtPreenchido()
    {
        // Arrange
        await using var connection = new NpgsqlConnection(_fixture.ConnectionString);
        await connection.OpenAsync();
        await DatabaseHelper.CleanTable(connection, "usuarios");

        var id = Guid.NewGuid();
        await DatabaseHelper.InsertUsuario(connection, id,
            nome: "João Silva",
            email: "joao@teste.com",
            deletedAt: DateTime.UtcNow,
            anonimizado: false);

        // Act
        await using var cmd = new NpgsqlCommand("CALL sp_anonimizar_usuario(@Id)", connection);
        cmd.Parameters.AddWithValue("@Id", id);
        await cmd.ExecuteNonQueryAsync();

        // Assert
        await using var select = new NpgsqlCommand(@"
            SELECT nome, email, senha_hash,
                   cep, logradouro, numero, complemento, bairro, cidade,
                   anonimizado
            FROM usuarios WHERE id = @Id", connection);
        select.Parameters.AddWithValue("@Id", id);
        await using var reader = await select.ExecuteReaderAsync();

        Assert.True(await reader.ReadAsync());
        Assert.Equal("Usuário Removido",       reader.GetString(reader.GetOrdinal("nome")));
        Assert.EndsWith("@anonimizado.invalid", reader.GetString(reader.GetOrdinal("email")));
        Assert.Equal("",                        reader.GetString(reader.GetOrdinal("senha_hash")));
        Assert.True(reader.IsDBNull(reader.GetOrdinal("cep")));
        Assert.True(reader.IsDBNull(reader.GetOrdinal("logradouro")));
        Assert.True(reader.IsDBNull(reader.GetOrdinal("numero")));
        Assert.True(reader.IsDBNull(reader.GetOrdinal("complemento")));
        Assert.True(reader.IsDBNull(reader.GetOrdinal("bairro")));
        Assert.True(reader.IsDBNull(reader.GetOrdinal("cidade")));
        Assert.True(reader.GetBoolean(reader.GetOrdinal("anonimizado")));
    }

    [Fact]
    public async Task Anonimizar_NaoDeveAnonimizar_QuandoDeletedAtNulo()
    {
        // Arrange
        await using var connection = new NpgsqlConnection(_fixture.ConnectionString);
        await connection.OpenAsync();
        await DatabaseHelper.CleanTable(connection, "usuarios");

        var id = Guid.NewGuid();
        await DatabaseHelper.InsertUsuario(connection, id,
            nome: "Maria Teste",
            deletedAt: null,
            anonimizado: false);

        // Act
        await using var cmd = new NpgsqlCommand("CALL sp_anonimizar_usuario(@Id)", connection);
        cmd.Parameters.AddWithValue("@Id", id);
        await cmd.ExecuteNonQueryAsync();

        // Assert — procedure não deve ter tocado no registro
        await using var select = new NpgsqlCommand(
            "SELECT nome FROM usuarios WHERE id = @Id", connection);
        select.Parameters.AddWithValue("@Id", id);
        var nome = (string?)await select.ExecuteScalarAsync();

        Assert.Equal("Maria Teste", nome);
    }

    [Fact]
    public async Task Anonimizar_NaoDeveExecutar_QuandoJaAnonimizado()
    {
        // Arrange
        await using var connection = new NpgsqlConnection(_fixture.ConnectionString);
        await connection.OpenAsync();
        await DatabaseHelper.CleanTable(connection, "usuarios");

        var id = Guid.NewGuid();
        await DatabaseHelper.InsertUsuario(connection, id,
            nome: "Carlos Anonimizado",
            deletedAt: DateTime.UtcNow,
            anonimizado: true);

        // Act — procedure deve ser silenciosa: WHERE anonimizado = FALSE não bate
        await using var cmd = new NpgsqlCommand("CALL sp_anonimizar_usuario(@Id)", connection);
        cmd.Parameters.AddWithValue("@Id", id);
        await cmd.ExecuteNonQueryAsync();

        // Assert — nome original deve permanecer intocado
        await using var select = new NpgsqlCommand(
            "SELECT nome FROM usuarios WHERE id = @Id", connection);
        select.Parameters.AddWithValue("@Id", id);
        var nome = (string?)await select.ExecuteScalarAsync();

        Assert.Equal("Carlos Anonimizado", nome);
    }

    // ────────────────────────────────────────────────────────────────
    // GRUPO 2 — trg_bloquear_update_anonimizado
    // ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Update_DeveBloquear_QuandoUsuarioJaAnonimizado()
    {
        // Arrange
        await using var connection = new NpgsqlConnection(_fixture.ConnectionString);
        await connection.OpenAsync();
        await DatabaseHelper.CleanTable(connection, "usuarios");

        var id = Guid.NewGuid();
        await DatabaseHelper.InsertUsuario(connection, id,
            nome: "Usuário Removido",
            anonimizado: true);

        // Act & Assert
        await using var update = new NpgsqlCommand(
            "UPDATE usuarios SET nome = 'Novo Nome' WHERE id = @Id", connection);
        update.Parameters.AddWithValue("@Id", id);

        var ex = await Assert.ThrowsAsync<PostgresException>(
            () => update.ExecuteNonQueryAsync());

        Assert.Contains("já foi anonimizado", ex.Message);
    }

    [Fact]
    public async Task Update_DevePermitir_QuandoUsuarioNaoAnonimizado()
    {
        // Arrange
        await using var connection = new NpgsqlConnection(_fixture.ConnectionString);
        await connection.OpenAsync();
        await DatabaseHelper.CleanTable(connection, "usuarios");

        var id = Guid.NewGuid();
        await DatabaseHelper.InsertUsuario(connection, id,
            nome: "Nome Original",
            anonimizado: false);

        // Act
        await using var update = new NpgsqlCommand(
            "UPDATE usuarios SET nome = 'Novo Nome' WHERE id = @Id", connection);
        update.Parameters.AddWithValue("@Id", id);
        await update.ExecuteNonQueryAsync();

        // Assert
        await using var select = new NpgsqlCommand(
            "SELECT nome FROM usuarios WHERE id = @Id", connection);
        select.Parameters.AddWithValue("@Id", id);
        var nome = (string?)await select.ExecuteScalarAsync();

        Assert.Equal("Novo Nome", nome);
    }
}
