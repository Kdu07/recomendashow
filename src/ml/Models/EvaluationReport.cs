namespace AventuraML.Models;

/// <summary>Par (ranking real, rating previsto pelo SVD bruto) usado em RMSE/MAE e gráficos.</summary>
public sealed record RatingPair(float Real, float Predicted);

/// <summary>Métricas de predição de rating (SVD bruto, escala 1–5).</summary>
public sealed record RatingAccuracy(double Rmse, double Mae, int PairCount);

/// <summary>Métricas de ranking top-K para uma variante do modelo (SVD puro ou híbrido).</summary>
public sealed record RankingMetrics(
    double HitRateAtK,
    double PrecisionAtK,
    double RecallAtK,
    double CategoryPrecisionAtK);

/// <summary>
/// Resultado completo da avaliação offline (split 80/20 por usuário).
/// RMSE/MAE comparam o score SVD bruto com o ranking real; as métricas de
/// ranking comparam SVD puro × híbrido (α·SVDnorm + (1−α)·afinidade).
/// </summary>
public sealed record EvaluationReport(
    RatingAccuracy Rating,
    RankingMetrics SvdRanking,
    RankingMetrics HybridRanking,
    int UsersEvaluated,
    int TopK,
    float Alpha,
    IReadOnlyList<RatingPair> RatingPairs);
