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
        bool anonimizado = false)
    {
        email ??= $"{id}@teste.com";

        const string sql = @"
INSERT INTO usuarios(id, nome, email, senha_hash, idade,
                     consentimento_aceito, data_consentimento,
                     deleted_at, anonimizado)
VALUES (@Id, @Nome, @Email, @SenhaHash, @Idade,
        TRUE, NOW(),
        @DeletedAt, @Anonimizado)";

        await using var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@Nome", nome);
        cmd.Parameters.AddWithValue("@Email", email);
        cmd.Parameters.AddWithValue("@SenhaHash", senhaHash);
        cmd.Parameters.AddWithValue("@Idade", (object?)idade ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DeletedAt", (object?)deletedAt ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Anonimizado", anonimizado);
        await cmd.ExecuteNonQueryAsync();
    }

    // tableName deve ser uma constante confiável — não passe entrada do usuário.
    public static async Task CleanTable(NpgsqlConnection connection, string tableName)
    {
        await using var cmd = new NpgsqlCommand($"DELETE FROM {tableName}", connection);
        await cmd.ExecuteNonQueryAsync();
    }
}
