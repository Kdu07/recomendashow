using Npgsql;
using Xunit;

namespace BackEnd.Aventura.IntegrationTests;

/// <summary>
/// Verifica o mapeamento usuário do sistema -> user_id do dataset (coluna ml_id),
/// definido pelos UPDATEs no fim de 02_sprint4_improvements.sql.
/// A fixture aplica só 01 + 02_sprint4 (sem seed); aqui aplicamos 02_initial_data
/// (cria alex/maria/joao) e reaplicamos o script de melhorias (idempotente) para
/// exercitar os UPDATEs de mapeamento sobre os usuários reais do seed.
/// </summary>
[Collection("Database")]
public class MlIdMappingTests
{
    private readonly DatabaseFixture _fixture;

    public MlIdMappingTests(DatabaseFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task UsuariosDoSeed_RecebemMlIdDoUserIdDoDataset()
    {
        await _fixture.ApplyScriptAsync("02_initial_data.sql");
        await _fixture.ApplyScriptAsync("02_sprint4_improvements.sql");

        var esperado = new Dictionary<string, int>
        {
            ["alex@email.com"]  = 63261,
            ["maria@email.com"] = 14888,
            ["joao@email.com"]  = 55251,
        };

        await using var conn = new NpgsqlConnection(_fixture.ConnectionString);
        await conn.OpenAsync();

        foreach (var (email, mlIdEsperado) in esperado)
        {
            await using var cmd = new NpgsqlCommand(
                "SELECT ml_id FROM usuarios WHERE email = @Email", conn);
            cmd.Parameters.AddWithValue("@Email", email);

            var mlId = (int?)await cmd.ExecuteScalarAsync();
            Assert.Equal(mlIdEsperado, mlId);
        }
    }
}
