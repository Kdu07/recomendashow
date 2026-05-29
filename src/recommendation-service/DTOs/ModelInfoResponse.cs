namespace RecommendationService.DTOs;

public sealed class ModelInfoResponse
{
    public SvdInfoDto Svd { get; init; } = new();
    public DatasetInfoDto Dataset { get; init; } = new();
}

public sealed class SvdInfoDto
{
    public int NUsers { get; init; }
    public int NItems { get; init; }
    public int NFactors { get; init; }
    public double LrAll { get; init; }
    public double RegAll { get; init; }
    public int NEpochs { get; init; }
    public double GlobalMean { get; init; }
    public double[] RatingScale { get; init; } = [];
}

public sealed class DatasetInfoDto
{
    public int TotalInteractions { get; init; }
    public int TotalItems { get; init; }
    public int ColdStartItemsCount { get; init; }
}
