namespace BackEnd.Aventura.Services;

public record RecommendationResult(string Show, float? Score);

public interface IRecommendationService
{
    List<RecommendationResult> Recommend(int userId, int n = 10);
}
