using Npgsql;

namespace BackEnd.Aventura.IntegrationTests;

public static class DatabaseHelper
{
    public static async Task InsertUsuario(
        NpgsqlConnection connection,
        Guid id,
        string nome = "Usuário Teste",
        string? email = null,
        string senhaHash = "hash_placeholder",
        int? idade = 25,
        DateTime? deletedAt = null,
        bool anonimizado = false,
        bool consentimentoAceito = false,
        DateTime? dataConsentimento = null)
    {
        email ??= $"{id}@teste.invalid";

        const string sql = @"
INSERT INTO usuarios (id, nome, email, senha_hash, idade,
                      consentimento_aceito, data_consentimento,
                      deleted_at, anonimizado)
VALUES (@Id, @Nome, @Email, @SenhaHash, @Idade,
        @ConsentimentoAceito, @DataConsentimento,
        @DeletedAt, @Anonimizado)";

        await using var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@Id",                id);
        cmd.Parameters.AddWithValue("@Nome",              nome);
        cmd.Parameters.AddWithValue("@Email",             email);
        cmd.Parameters.AddWithValue("@SenhaHash",         senhaHash);
        cmd.Parameters.AddWithValue("@Idade",             (object?)idade ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ConsentimentoAceito", consentimentoAceito);
        cmd.Parameters.AddWithValue("@DataConsentimento", (object?)dataConsentimento ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DeletedAt",         (object?)deletedAt ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Anonimizado",       anonimizado);
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task InsertEspetaculo(
        NpgsqlConnection connection,
        Guid id,
        string? nome = null)
    {
        nome ??= $"_test_{id}";

        const string sql = @"
INSERT INTO espetaculos (id, nome, data_inicio, data_fim, valor_ingresso)
VALUES (@Id, @Nome, NOW(), NOW() + INTERVAL '30 days', 50.00)";

        await using var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@Id",   id);
        cmd.Parameters.AddWithValue("@Nome", nome);
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task InsertPreferencia(NpgsqlConnection connection, Guid usuarioId)
    {
        const string sql = "INSERT INTO preferencias (usuario_id) VALUES (@UsuarioId)";
        await using var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
        await cmd.ExecuteNonQueryAsync();
    }

    // tableName deve ser uma constante confiável — não passe entrada do usuário.
    public static async Task CleanTable(NpgsqlConnection connection, string tableName)
    {
        await using var cmd = new NpgsqlCommand($"DELETE FROM {tableName}", connection);
        await cmd.ExecuteNonQueryAsync();
    }
}
