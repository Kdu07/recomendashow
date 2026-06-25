using Npgsql;
using NpgsqlTypes;
using InferenceService.Models;

namespace InferenceService.Data;

public sealed class RecommendationWriteRepository : IRecommendationWriteRepository
{
    private static readonly string[] UserMappingColumns =
    [
        "ml_id",
        "user_id",
        "dataset_user_id",
        "external_user_id",
        "legacy_user_id",
    ];

    private readonly string _connectionString;
    private readonly ILogger<RecommendationWriteRepository> _logger;

    public RecommendationWriteRepository(
        IConfiguration configuration,
        ILogger<RecommendationWriteRepository> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "ConnectionStrings:DefaultConnection não configurada para persistir recomendações.");
        _logger = logger;
    }

    public async Task SaveAsync(
        int userId,
        IReadOnlyList<RecommendationResult> recommendations,
        string model,
        CancellationToken cancellationToken = default)
    {
        if (recommendations.Count == 0)
        {
            return;
        }

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        if (!await HasRecommendationTableAsync(connection, cancellationToken))
        {
            _logger.LogWarning(
                "Tabela recomendacoes ausente. Crie uma migration/script com usuario_id, espetaculo_id, score e motivo antes de persistir recomendações.");
            return;
        }

        Guid? usuarioId = await ResolveUsuarioIdAsync(connection, userId, cancellationToken);
        if (usuarioId is null)
        {
            _logger.LogWarning(
                "Não foi possível persistir recomendações: o schema atual usa usuarios.id UUID e não possui coluna numérica para mapear o userId do modelo ({UserId}). Sugestão: criar uma coluna/constraint de mapeamento entre o user_id do dataset e usuarios.id.",
                userId);
            return;
        }

        bool hasItemDataset = await HasItemDatasetColumnAsync(connection, cancellationToken);

        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        const string deleteSql = "DELETE FROM recomendacoes WHERE usuario_id = @UsuarioId";
        await using (var deleteCommand = new NpgsqlCommand(deleteSql, connection, transaction))
        {
            deleteCommand.Parameters.Add("@UsuarioId", NpgsqlDbType.Uuid).Value = usuarioId.Value;
            await deleteCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        // Caminho B: resolve o espetáculo pelo mapeamento item_dataset (nome cru do
        // show do dataset, como o modelo emite) com normalização defensiva
        // TRIM+UPPER+unaccent, priorizando-o sobre o casamento por nome "bonito".
        // Mantém o fallback por e.nome (Caminho A) para não regredir dados já
        // alinhados. Se a coluna item_dataset ainda não existe (migration não
        // aplicada), usa apenas o casamento por nome.
        string insertSql = hasItemDataset
            ? @"
INSERT INTO recomendacoes(usuario_id, espetaculo_id, score, motivo, data_geracao)
SELECT @UsuarioId, m.id, @Score, @Motivo, CURRENT_TIMESTAMP
FROM (
    SELECT e.id,
           CASE
               WHEN e.item_dataset IS NOT NULL
                    AND upper(unaccent(btrim(e.item_dataset))) = upper(unaccent(btrim(@EspetaculoNome)))
                   THEN 0
               WHEN e.nome = @EspetaculoNome THEN 1
               ELSE NULL
           END AS match_rank
    FROM espetaculos e
) m
WHERE m.match_rank IS NOT NULL
ORDER BY m.match_rank
LIMIT 1"
            : @"
INSERT INTO recomendacoes(usuario_id, espetaculo_id, score, motivo, data_geracao)
SELECT @UsuarioId, e.id, @Score, @Motivo, CURRENT_TIMESTAMP
FROM espetaculos e
WHERE e.nome = @EspetaculoNome
LIMIT 1";

        int inserted = 0;
        foreach (var recommendation in recommendations)
        {
            await using var insertCommand = new NpgsqlCommand(insertSql, connection, transaction);
            insertCommand.Parameters.Add("@UsuarioId", NpgsqlDbType.Uuid).Value = usuarioId.Value;
            insertCommand.Parameters.Add("@Score", NpgsqlDbType.Numeric).Value =
                (object?)NormalizeScore(recommendation.Score) ?? DBNull.Value;
            insertCommand.Parameters.Add("@Motivo", NpgsqlDbType.Text).Value =
                $"Recomendação calculada pelo modelo {model}.";
            insertCommand.Parameters.Add("@EspetaculoNome", NpgsqlDbType.Text).Value = recommendation.Espetaculo;

            inserted += await insertCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);

        if (inserted < recommendations.Count)
        {
            _logger.LogWarning(
                "Persistidas {Inserted}/{Total} recomendações para o usuário {UserId}. " +
                "Alguns shows retornados pelo modelo não têm espetáculo mapeado (espetaculos.item_dataset) nem nome correspondente.",
                inserted,
                recommendations.Count,
                userId);
        }
    }

    private static decimal? NormalizeScore(double? score)
    {
        if (score is null)
        {
            return null;
        }

        // O score híbrido já nasce em [0,1]; clamp defensivo (o SVD puro, removido,
        // usava escala 1–5 e era dividido por 5 aqui).
        return Math.Round((decimal)Math.Clamp(score.Value, 0.0, 1.0), 2);
    }

    private static async Task<bool> HasRecommendationTableAsync(
        NpgsqlConnection connection,
        CancellationToken cancellationToken)
    {
        const string sql = @"
SELECT EXISTS (
    SELECT 1
    FROM information_schema.tables
    WHERE table_schema = 'public'
      AND table_name = 'recomendacoes'
)";

        await using var command = new NpgsqlCommand(sql, connection);
        return await command.ExecuteScalarAsync(cancellationToken) is true;
    }

    private static async Task<bool> HasItemDatasetColumnAsync(
        NpgsqlConnection connection,
        CancellationToken cancellationToken)
    {
        const string sql = @"
SELECT EXISTS (
    SELECT 1
    FROM information_schema.columns
    WHERE table_schema = 'public'
      AND table_name = 'espetaculos'
      AND column_name = 'item_dataset'
)";

        await using var command = new NpgsqlCommand(sql, connection);
        return await command.ExecuteScalarAsync(cancellationToken) is true;
    }

    private static async Task<Guid?> ResolveUsuarioIdAsync(
        NpgsqlConnection connection,
        int userId,
        CancellationToken cancellationToken)
    {
        string? mappingColumn = await FindUserMappingColumnAsync(connection, cancellationToken);
        if (mappingColumn is null)
        {
            return null;
        }

        string sql = $"SELECT id FROM usuarios WHERE {mappingColumn} = @UserId LIMIT 1";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add("@UserId", NpgsqlDbType.Integer).Value = userId;

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return result is Guid usuarioId ? usuarioId : null;
    }

    private static async Task<string?> FindUserMappingColumnAsync(
        NpgsqlConnection connection,
        CancellationToken cancellationToken)
    {
        const string sql = @"
SELECT column_name
FROM information_schema.columns
WHERE table_schema = 'public'
  AND table_name = 'usuarios'
  AND column_name = ANY(@ColumnNames)
  AND data_type IN ('integer', 'bigint')
ORDER BY array_position(@ColumnNames, column_name)
LIMIT 1";

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add("@ColumnNames", NpgsqlDbType.Array | NpgsqlDbType.Text).Value = UserMappingColumns;

        var result = await command.ExecuteScalarAsync(cancellationToken) as string;
        return UserMappingColumns.Contains(result) ? result : null;
    }
}
