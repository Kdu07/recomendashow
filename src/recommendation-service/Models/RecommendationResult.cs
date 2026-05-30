namespace RecommendationService.Models;

/// <summary>
/// Um item recomendado para um usuário.
/// Espelha o dict Python {"espetaculo": str, "score": float | None} de inference.py.
/// score == null indica fallback por popularidade (cold-start).
/// </summary>
public sealed record RecommendationResult(string Espetaculo, double? Score);
