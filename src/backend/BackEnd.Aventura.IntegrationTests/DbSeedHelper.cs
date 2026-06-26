using Npgsql;

namespace BackEnd.Aventura.ApiTests.Helpers;

public static class DbSeedHelper
{
    public static async Task InsertUsuarioAsync(
        NpgsqlConnection conn,
        Guid    id,
        string  nome          = "Usuário Teste",
        string? email         = null,
        int?    idade         = 25,
        bool    consentimento = true)
    {
        email ??= $"test_{id:N}@aventura.test";
        const string sql = @"
INSERT INTO usuarios (id, nome, email, senha_hash, idade,
                      consentimento_aceito, data_consentimento, data_cadastro)
VALUES (@Id, @Nome, @Email, 'hash_placeholder', @Idade,
        @Consentimento, NOW(), NOW())
ON CONFLICT (id) DO NOTHING";
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Id",            id);
        cmd.Parameters.AddWithValue("@Nome",          nome);
        cmd.Parameters.AddWithValue("@Email",         email);
        cmd.Parameters.AddWithValue("@Idade",         (object?)idade ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Consentimento", consentimento);
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task InsertPreferenciasAsync(NpgsqlConnection conn, Guid usuarioId)
    {
        const string sql = @"
INSERT INTO preferencias (usuario_id)
VALUES (@UsuarioId)
ON CONFLICT (usuario_id) DO NOTHING";
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
        await cmd.ExecuteNonQueryAsync();
    }

    // espetaculos no schema real: id, nome, tipo_evento, descricao,
    // url_bilheteria, data_inicio, data_fim, valor_ingresso
    // sem coluna genero, sem compra_url
    public static async Task<Guid> InsertEspetaculoAsync(
        NpgsqlConnection conn,
        string? nome          = null,
        string  urlBilheteria = "https://ticketera.com/show",
        decimal preco         = 80m)
    {
        var id = Guid.NewGuid();
        nome ??= $"Espetáculo_{id}";
        const string sql = @"
INSERT INTO espetaculos (id, nome, tipo_evento, descricao, url_bilheteria,
                         data_inicio, data_fim, valor_ingresso)
VALUES (@Id, @Nome, 'Teatro', 'Descrição de teste', @Url,
        NOW() - INTERVAL '1 day', NOW() + INTERVAL '30 days', @Preco)";
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Id",    id);
        cmd.Parameters.AddWithValue("@Nome",  nome);
        cmd.Parameters.AddWithValue("@Url",   urlBilheteria);
        cmd.Parameters.AddWithValue("@Preco", preco);
        await cmd.ExecuteNonQueryAsync();
        return id;
    }

    public static async Task<Guid> InsertGeneroAsync(NpgsqlConnection conn, string nome)
    {
        var id = Guid.NewGuid();
        const string sql = @"
INSERT INTO generos (id, nome) VALUES (@Id, @Nome)
ON CONFLICT (nome) DO UPDATE SET nome = EXCLUDED.nome
RETURNING id";
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@Nome", nome);
        return (Guid)(await cmd.ExecuteScalarAsync())!;
    }

    public static async Task LinkEspetaculoGeneroAsync(
        NpgsqlConnection conn, Guid espetaculoId, Guid generoId)
    {
        const string sql = @"
INSERT INTO espetaculo_genero (espetaculo_id, genero_id) VALUES (@E, @G)
ON CONFLICT (espetaculo_id, genero_id) DO NOTHING";
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@E", espetaculoId);
        cmd.Parameters.AddWithValue("@G", generoId);
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task InsertWishlistAsync(
        NpgsqlConnection conn, Guid usuarioId, Guid espetaculoId)
    {
        const string sql = @"
INSERT INTO wishlist_curtidas (usuario_id, espetaculo_id, tipo)
VALUES (@U, @E, 'wishlist')
ON CONFLICT (usuario_id, espetaculo_id, tipo) DO NOTHING";
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@U", usuarioId);
        cmd.Parameters.AddWithValue("@E", espetaculoId);
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task<long> CountRowsAsync(
        NpgsqlConnection conn, string tableName, string? where = null)
    {
        var sql = $"SELECT COUNT(*) FROM {tableName}";
        if (where is not null) sql += $" WHERE {where}";
        await using var cmd = new NpgsqlCommand(sql, conn);
        return (long)(await cmd.ExecuteScalarAsync())!;
    }
}