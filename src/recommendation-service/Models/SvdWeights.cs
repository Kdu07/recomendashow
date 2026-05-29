using System.Text.Json.Serialization;

namespace RecommendationService.Models;

/// <summary>
/// Pesos do modelo Funk SVD desserializados de svd_weights.json.
/// Produzido por export_weights.py a partir do svd_model.joblib treinado.
/// </summary>
public sealed class SvdWeights
{
    [JsonPropertyName("model_type")]
    public string ModelType { get; init; } = "funk_svd";

    [JsonPropertyName("hyperparameters")]
    public SvdHyperparameters Hyperparameters { get; init; } = new();

    [JsonPropertyName("rating_scale")]
    public double[] RatingScale { get; init; } = [1.0, 5.0];

    [JsonPropertyName("global_mean")]
    public double GlobalMean { get; init; }

    [JsonPropertyName("n_users")]
    public int NUsers { get; init; }

    [JsonPropertyName("n_items")]
    public int NItems { get; init; }

    /// <summary>raw user_id (string) → inner index</summary>
    [JsonPropertyName("user_id_to_inner")]
    public Dictionary<string, int> UserIdToInner { get; init; } = [];

    /// <summary>raw item name → inner index</summary>
    [JsonPropertyName("item_id_to_inner")]
    public Dictionary<string, int> ItemIdToInner { get; init; } = [];

    /// <summary>inner index (string) → raw item name</summary>
    [JsonPropertyName("inner_to_item_id")]
    public Dictionary<string, string> InnerToItemId { get; init; } = [];

    /// <summary>Viés de usuário b_u, shape (n_users,)</summary>
    [JsonPropertyName("bu")]
    public double[] Bu { get; init; } = [];

    /// <summary>Viés de item b_i, shape (n_items,)</summary>
    [JsonPropertyName("bi")]
    public double[] Bi { get; init; } = [];

    /// <summary>Fatores latentes de usuário p_u, shape (n_users, n_factors)</summary>
    [JsonPropertyName("pu")]
    public double[][] Pu { get; init; } = [];

    /// <summary>Fatores latentes de item q_i, shape (n_items, n_factors)</summary>
    [JsonPropertyName("qi")]
    public double[][] Qi { get; init; } = [];
}

public sealed class SvdHyperparameters
{
    [JsonPropertyName("n_factors")]
    public int NFactors { get; init; } = 10;

    [JsonPropertyName("lr_all")]
    public double LrAll { get; init; } = 0.001;

    [JsonPropertyName("reg_all")]
    public double RegAll { get; init; } = 0.2;

    [JsonPropertyName("n_epochs")]
    public int NEpochs { get; init; } = 50;
}
