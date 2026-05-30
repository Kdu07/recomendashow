using System.Text.Json.Serialization;

namespace TrainingService.Export;

// Espelha exatamente SvdWeights.cs da recommendation-service (mesmo contrato JSON)
public sealed class SvdWeightsExport
{
    [JsonPropertyName("model_type")]
    public string ModelType { get; init; } = "funk_svd";

    [JsonPropertyName("hyperparameters")]
    public SvdHyperparametersExport Hyperparameters { get; init; } = new();

    [JsonPropertyName("rating_scale")]
    public double[] RatingScale { get; init; } = [1.0, 5.0];

    [JsonPropertyName("global_mean")]
    public double GlobalMean { get; init; }

    [JsonPropertyName("n_users")]
    public int NUsers { get; init; }

    [JsonPropertyName("n_items")]
    public int NItems { get; init; }

    [JsonPropertyName("user_id_to_inner")]
    public Dictionary<string, int> UserIdToInner { get; init; } = [];

    [JsonPropertyName("item_id_to_inner")]
    public Dictionary<string, int> ItemIdToInner { get; init; } = [];

    [JsonPropertyName("inner_to_item_id")]
    public Dictionary<string, string> InnerToItemId { get; init; } = [];

    [JsonPropertyName("bu")]
    public double[] Bu { get; init; } = [];

    [JsonPropertyName("bi")]
    public double[] Bi { get; init; } = [];

    [JsonPropertyName("pu")]
    public double[][] Pu { get; init; } = [];

    [JsonPropertyName("qi")]
    public double[][] Qi { get; init; } = [];
}

public sealed class SvdHyperparametersExport
{
    [JsonPropertyName("n_factors")]
    public int NFactors { get; init; }

    [JsonPropertyName("lr_all")]
    public double LrAll { get; init; }

    [JsonPropertyName("reg_all")]
    public double RegAll { get; init; }

    [JsonPropertyName("n_epochs")]
    public int NEpochs { get; init; }
}
