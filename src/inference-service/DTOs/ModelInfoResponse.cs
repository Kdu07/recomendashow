namespace InferenceService.DTOs;

public sealed class ModelInfoResponse
{
    public required HybridModelInfoDto Model { get; init; }
    public required DatasetInfoDto Dataset { get; init; }
}

public sealed class HybridModelInfoDto
{
    public string ModelType { get; init; } = "hibrido";

    /// <summary>Peso do SVD no score híbrido (1−α = afinidade de categoria).</summary>
    public float Alpha { get; init; }

    /// <summary>Usuários com histórico conhecidos pelo modelo.</summary>
    public int KnownUsers { get; init; }

    /// <summary>Itens do catálogo (cold_start.json).</summary>
    public int CatalogItems { get; init; }

    /// <summary>Rank da fatoração de matrizes, se disponível no model.zip.</summary>
    public int? LatentFactors { get; init; }
}

public sealed class DatasetInfoDto
{
    public int TotalInteractions { get; init; }
    public int TotalItems { get; init; }
    public int ColdStartItemsCount { get; init; }
}
