using FluentAssertions;
using InferenceService.Data;
using InferenceService.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using Xunit;

namespace InferenceService.Tests;

/// <summary>
/// Cobre o elo central da feature de mapeamento: a tradução do user_id inteiro
/// do dataset para o usuarios.id (UUID) via coluna ml_id, feita no
/// RecommendationWriteRepository quando persiste em recomendacoes.
///
/// Marcado como Category=Integration: exige Docker (Testcontainers) e é excluído
/// do CI (sem Docker), como a suíte de integração do backend. Roda localmente.
/// </summary>
[Trait("Category", "Integration")]
public sealed class RecommendationWriteRepositoryTests : IClassFixture<PostgresFixture>
{
    private readonly PostgresFixture _db;

    public RecommendationWriteRepositoryTests(PostgresFixture db) => _db = db;

    private RecommendationWriteRepository BuildRepo()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _db.ConnectionString,
            })
            .Build();

        return new RecommendationWriteRepository(
            config, NullLogger<RecommendationWriteRepository>.Instance);
    }

    private async Task<Guid> InsertUsuarioAsync(int mlId)
    {
        var id = Guid.NewGuid();
        await using var conn = new NpgsqlConnection(_db.ConnectionString);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            @"INSERT INTO usuarios(id, nome, email, senha_hash, ml_id, consentimento_aceito)
              VALUES (@Id, 'Teste', @Email, 'hash', @MlId, TRUE)", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@Email", $"user_{mlId}@test.local");
        cmd.Parameters.AddWithValue("@MlId", mlId);
        await cmd.ExecuteNonQueryAsync();
        return id;
    }

    private async Task<Guid> InsertEspetaculoAsync(string nome)
    {
        var id = Guid.NewGuid();
        await using var conn = new NpgsqlConnection(_db.ConnectionString);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            @"INSERT INTO espetaculos(id, nome, data_inicio, data_fim)
              VALUES (@Id, @Nome, NOW(), NOW() + INTERVAL '1 day')", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@Nome", nome);
        await cmd.ExecuteNonQueryAsync();
        return id;
    }

    private async Task<Guid> InsertEspetaculoAsync(string nome, string? itemDataset)
    {
        var id = Guid.NewGuid();
        await using var conn = new NpgsqlConnection(_db.ConnectionString);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            @"INSERT INTO espetaculos(id, nome, item_dataset, data_inicio, data_fim)
              VALUES (@Id, @Nome, @ItemDataset, NOW(), NOW() + INTERVAL '1 day')", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@Nome", nome);
        cmd.Parameters.AddWithValue("@ItemDataset", (object?)itemDataset ?? DBNull.Value);
        await cmd.ExecuteNonQueryAsync();
        return id;
    }

    private async Task<long> CountRecomendacoesAsync(Guid espetaculoId)
    {
        await using var conn = new NpgsqlConnection(_db.ConnectionString);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            "SELECT COUNT(*) FROM recomendacoes WHERE espetaculo_id = @E", conn);
        cmd.Parameters.AddWithValue("@E", espetaculoId);
        return (long)(await cmd.ExecuteScalarAsync())!;
    }

    [Fact(DisplayName = "Com ml_id mapeado: persiste recomendação no UUID resolvido, com score normalizado")]
    public async Task SaveAsync_ComMlIdMapeado_PersisteComUuidEScoreCorretos()
    {
        const int mlId = 900001;
        var usuarioId = await InsertUsuarioAsync(mlId);
        var espId = await InsertEspetaculoAsync("Show Mapeado 900001");

        var repo = BuildRepo();
        await repo.SaveAsync(mlId, [new RecommendationResult("Show Mapeado 900001", 0.9)], "hibrido");

        await using var conn = new NpgsqlConnection(_db.ConnectionString);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            "SELECT usuario_id, espetaculo_id, score FROM recomendacoes WHERE usuario_id = @U", conn);
        cmd.Parameters.AddWithValue("@U", usuarioId);
        await using var reader = await cmd.ExecuteReaderAsync();

        reader.Read().Should().BeTrue("a recomendação deve ser persistida para o UUID resolvido via ml_id");
        reader.GetGuid(0).Should().Be(usuarioId);
        reader.GetGuid(1).Should().Be(espId);
        reader.GetDecimal(2).Should().Be(0.90m);
    }

    [Fact(DisplayName = "Sem ml_id correspondente: writer resolve null e não insere nada")]
    public async Task SaveAsync_SemMlIdMapeado_NaoPersiste()
    {
        const int mlIdInexistente = 900404; // nenhum usuário possui este ml_id
        var espId = await InsertEspetaculoAsync("Show Sem Mapeamento 900404");

        var repo = BuildRepo();
        await repo.SaveAsync(
            mlIdInexistente, [new RecommendationResult("Show Sem Mapeamento 900404", 0.8)], "hibrido");

        (await CountRecomendacoesAsync(espId)).Should().Be(0,
            "sem usuário com ml_id correspondente, o writer não tem usuario_id para gravar");
    }

    [Fact(DisplayName = "Caminho B: casa pelo item_dataset mantendo o nome bonito do espetáculo")]
    public async Task SaveAsync_ComItemDataset_CasaPeloMapeamentoNaoPeloNome()
    {
        const int mlId = 900010;
        var usuarioId = await InsertUsuarioAsync(mlId);
        // Nome "bonito" no banco difere do nome cru emitido pelo modelo.
        var espId = await InsertEspetaculoAsync("A Noviça Rebelde", itemDataset: "A NOVIÇA REBELDE");

        var repo = BuildRepo();
        await repo.SaveAsync(mlId, [new RecommendationResult("A NOVIÇA REBELDE", 0.85)], "hibrido");

        await using var conn = new NpgsqlConnection(_db.ConnectionString);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            "SELECT espetaculo_id FROM recomendacoes WHERE usuario_id = @U", conn);
        cmd.Parameters.AddWithValue("@U", usuarioId);
        var result = await cmd.ExecuteScalarAsync();

        result.Should().Be(espId,
            "a recomendação deve casar pelo item_dataset, mesmo com espetaculos.nome diferente");
    }

    [Fact(DisplayName = "Caminho B: normalização (caixa/acento) ainda casa o item_dataset")]
    public async Task SaveAsync_ItemDatasetComCaixaEAcentoDiferentes_AindaCasa()
    {
        const int mlId = 900011;
        await InsertUsuarioAsync(mlId);
        var espId = await InsertEspetaculoAsync("Elis", itemDataset: "ELIS, A MUSICAL");

        // Modelo emite com caixa baixa e sem acento -> normalização defensiva deve casar.
        var repo = BuildRepo();
        await repo.SaveAsync(mlId, [new RecommendationResult("  elis, a musical  ", 0.7)], "hibrido");

        (await CountRecomendacoesAsync(espId)).Should().Be(1,
            "TRIM+UPPER+unaccent dos dois lados deve absorver diferenças de espaço/caixa/acento");
    }

    [Fact(DisplayName = "Caminho B: espetáculo sem item_dataset e sem nome correspondente não persiste")]
    public async Task SaveAsync_EspetaculoSemMapeamentoNemNome_NaoPersiste()
    {
        const int mlId = 900012;
        await InsertUsuarioAsync(mlId);
        var espId = await InsertEspetaculoAsync("Hamlet", itemDataset: null);

        var repo = BuildRepo();
        await repo.SaveAsync(mlId, [new RecommendationResult("ALGUM SHOW DO DATASET", 0.6)], "hibrido");

        (await CountRecomendacoesAsync(espId)).Should().Be(0,
            "sem item_dataset mapeado nem nome igual, o show do modelo não tem espetáculo para gravar");
    }

    [Fact(DisplayName = "Reprocessar o mesmo usuário (DELETE+INSERT) não duplica recomendações")]
    public async Task SaveAsync_ChamadoDuasVezes_NaoDuplica()
    {
        const int mlId = 900003;
        await InsertUsuarioAsync(mlId);
        var espId = await InsertEspetaculoAsync("Show Idempotente 900003");
        RecommendationResult[] recs = [new("Show Idempotente 900003", 0.7)];

        var repo = BuildRepo();
        await repo.SaveAsync(mlId, recs, "hibrido");
        await repo.SaveAsync(mlId, recs, "hibrido");

        (await CountRecomendacoesAsync(espId)).Should().Be(1,
            "o writer faz DELETE+INSERT por usuário, então reprocessar não duplica");
    }
}
