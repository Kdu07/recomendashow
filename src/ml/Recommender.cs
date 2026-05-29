using System.Text.Json;
using AventuraML.Models;
using Microsoft.ML;

namespace AventuraML;

public sealed class Recommender
{
    private readonly MLContext _mlContext;
    private readonly PredictionEngine<RatingData, RatingPrediction> _engine;
    private readonly Dictionary<uint, HashSet<string>> _userHistory;
    private readonly List<string> _popularItems;
    private readonly List<string> _allItems;
    private readonly Dictionary<string, string> _itemCategories;
    private readonly Dictionary<string, Dictionary<string, float>> _categoryAffinity;
    private readonly float _alpha;

    public Recommender(
        string modelPath,
        string coldStartPath,
        string datasetPath,
        string itemCategoriesPath,
        string categoryAffinityPath,
        float alpha = 0.7f)
    {
        _alpha = alpha;
        _mlContext = new MLContext(seed: 42);

        var model = _mlContext.Model.Load(modelPath, out _);
        _engine = _mlContext.Model.CreatePredictionEngine<RatingData, RatingPrediction>(model);

        var rows = DataPipeline.Load(datasetPath);

        _userHistory = rows
            .GroupBy(r => r.UserId)
            .ToDictionary(g => g.Key, g => g.Select(r => r.ItemId).ToHashSet());

        _popularItems = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(coldStartPath))
            ?? throw new InvalidOperationException("Falha ao desserializar cold_start.json.");

        _allItems = _popularItems;

        _itemCategories = JsonSerializer.Deserialize<Dictionary<string, string>>(
            File.ReadAllText(itemCategoriesPath))
            ?? throw new InvalidOperationException("Falha ao carregar item_categories.json.");

        _categoryAffinity = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, float>>>(
            File.ReadAllText(categoryAffinityPath))
            ?? throw new InvalidOperationException("Falha ao carregar category_affinity.json.");
    }

    public List<(string Show, float? Score)> Recommend(int userId, int n = 10)
    {
        if (n <= 0) throw new ArgumentOutOfRangeException(nameof(n), "n deve ser maior que zero.");

        var uid = (uint)userId;
        var consumed = _userHistory.GetValueOrDefault(uid) ?? [];

        if (consumed.Count == 0)
            return _popularItems.Take(n).Select(item => (item, (float?)null)).ToList();

        var rawScores = new List<(string Item, float SvdScore, string Category)>();

        foreach (var item in _allItems.Where(i => !consumed.Contains(i)))
        {
            try
            {
                var pred = _engine.Predict(new RatingData
                {
                    UserId = uid,
                    ItemId = item,
                    Rating = 0f,
                });

                if (!float.IsNaN(pred.Score) && !float.IsInfinity(pred.Score))
                {
                    var cat = _itemCategories.GetValueOrDefault(item, "");
                    rawScores.Add((item, pred.Score, cat));
                }
            }
            catch { /* item fora do vocabulário do modelo, ignora */ }
        }

        if (rawScores.Count == 0)
            return [];

        var minSvd = rawScores.Min(x => x.SvdScore);
        var maxSvd = rawScores.Max(x => x.SvdScore);
        var svdRange = maxSvd - minSvd;

        return rawScores
            .Select(x =>
            {
                var normSvd = svdRange > 1e-6f ? (x.SvdScore - minSvd) / svdRange : 0.5f;
                var affinity = GetCategoryAffinity(uid, x.Category);
                return (Item: x.Item, FinalScore: _alpha * normSvd + (1f - _alpha) * affinity);
            })
            .OrderByDescending(x => x.FinalScore)
            .Take(n)
            .Select(x => (x.Item, (float?)x.FinalScore))
            .ToList();
    }

    private float GetCategoryAffinity(uint userId, string category)
    {
        if (string.IsNullOrEmpty(category)) return 0f;
        if (_categoryAffinity.TryGetValue(userId.ToString(), out var userAffinities))
            return userAffinities.GetValueOrDefault(category, 0f);
        return 0f;
    }
}
