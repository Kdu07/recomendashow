namespace RecommendationService.Models;

/// <summary>
/// Métricas de avaliação do modelo de recomendação.
/// Espelha as métricas calculadas em 04_avaliacao_inferencia.ipynb.
/// </summary>
public sealed class EvaluationMetrics
{
    public string ModelType { get; init; } = string.Empty;

    // Métricas de regressão (comparação direta de scores preditos vs. reais)
    public double Rmse { get; init; }
    public double Mae { get; init; }

    // Métricas de ranking (threshold: ranking >= 2 = relevante)
    public double PrecisionAtK { get; init; }
    public double RecallAtK { get; init; }
    public double F1AtK { get; init; }
    public int K { get; init; }

    // Métricas de classificação binária
    public double Accuracy { get; init; }
    public double Precision { get; init; }
    public double Recall { get; init; }
    public double F1Score { get; init; }

    public int TestSetSize { get; init; }
}
