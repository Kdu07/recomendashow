using System.Globalization;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.ML;

namespace BackEnd.Aventura.Services;

internal sealed class RatingInput
{
    public uint UserId { get; init; }
    public string ItemId { get; init; } = "";
    public float Rating { get; init; }
}

internal sealed class RatingOutput
{
    public float Score { get; set; }
}

public sealed class RecommendationService : IRecommendationService
{
    private readonly PredictionEngine<RatingInput, RatingOutput> _engine;
    private readonly Dictionary<uint, HashSet<string>> _userHistory;
    private readonly List<string> _popularItems;
    private readonly List<string> _allItems;
    private readonly Dictionary<string, string> _itemCategories;
    private readonly Dictionary<string, Dictionary<string, float>> _categoryAffinity;
    private readonly Dictionary<string, List<string>> _coldStartByCategory;
    private readonly IPreferenceService _preferenceService;
    private readonly float _alpha;
    private readonly Lock _lock = new();

    public RecommendationService(IConfiguration config, IWebHostEnvironment env, IPreferenceService preferenceService)
    {
        _preferenceService = preferenceService;
        var root = env.ContentRootPath;

        var modelPath               = Path.GetFullPath(config["ML:ModelPath"]               ?? "../models/model.zip",                    root);
        var coldStartPath           = Path.GetFullPath(config["ML:ColdStartPath"]           ?? "../models/cold_start.json",              root);
        var datasetPath             = Path.GetFullPath(config["ML:DatasetPath"]             ?? "../dataset/dataset.csv",                 root);
        var itemCategoriesPath      = Path.GetFullPath(config["ML:ItemCategoriesPath"]      ?? "../models/item_categories.json",         root);
        var categoryAffinityPath    = Path.GetFullPath(config["ML:CategoryAffinityPath"]    ?? "../models/category_affinity.json",       root);
        var coldStartByCategoryPath = Path.GetFullPath(config["ML:ColdStartByCategoryPath"] ?? "../models/cold_start_by_category.json",  root);
        _alpha = config.GetValue<float>("ML:Alpha", 0.7f);

        var mlContext = new MLContext(seed: 42);
        var model = mlContext.Model.Load(modelPath, out _);
        _engine = mlContext.Model.CreatePredictionEngine<RatingInput, RatingOutput>(model);

        var rows = LoadCsv(datasetPath);
        _userHistory = rows
            .GroupBy(r => r.UserId)
            .ToDictionary(g => g.Key, g => g.Select(r => r.ItemId).ToHashSet());

        _popularItems = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(coldStartPath))
            ?? throw new InvalidOperationException("Falha ao carregar cold_start.json.");

        _allItems = _popularItems;

        _itemCategories = JsonSerializer.Deserialize<Dictionary<string, string>>(
            File.ReadAllText(itemCategoriesPath))
            ?? throw new InvalidOperationException("Falha ao carregar item_categories.json.");

        _categoryAffinity = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, float>>>(
            File.ReadAllText(categoryAffinityPath))
            ?? throw new InvalidOperationException("Falha ao carregar category_affinity.json.");

        _coldStartByCategory = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(
            File.ReadAllText(coldStartByCategoryPath))
            ?? throw new InvalidOperationException("Falha ao carregar cold_start_by_category.json.");
    }

    public List<RecommendationResult> Recommend(int userId, int n = 10)
    {
        if (n <= 0) throw new ArgumentOutOfRangeException(nameof(n));

        var uid = (uint)userId;
        var consumed = _userHistory.GetValueOrDefault(uid) ?? [];

        if (consumed.Count == 0)
        {
            var prefs = _preferenceService.GetPreferences(userId);
            if (prefs.Count > 0)
            {
                var seen = new HashSet<string>();
                var categoryItems = prefs
                    .Where(_coldStartByCategory.ContainsKey)
                    .SelectMany(tipo => _coldStartByCategory[tipo])
                    .Where(seen.Add)
                    .Take(n)
                    .Select(item => new RecommendationResult(item, null))
                    .ToList();
                if (categoryItems.Count > 0)
                    return categoryItems;
            }
            return _popularItems.Take(n)
                .Select(item => new RecommendationResult(item, null))
                .ToList();
        }

        var rawScores = new List<(string Item, float SvdScore, string Category)>();

        lock (_lock)
        {
            foreach (var item in _allItems.Where(i => !consumed.Contains(i)))
            {
                try
                {
                    var pred = _engine.Predict(new RatingInput
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
                catch { }
            }
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
            .Select(x => new RecommendationResult(x.Item, x.FinalScore))
            .ToList();
    }

    private float GetCategoryAffinity(uint userId, string category)
    {
        if (string.IsNullOrEmpty(category)) return 0f;
        if (_categoryAffinity.TryGetValue(userId.ToString(), out var userAffinities))
            return userAffinities.GetValueOrDefault(category, 0f);
        return 0f;
    }

    private static List<RatingInput> LoadCsv(string path)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            BadDataFound = null,
        };

        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, config);

        return csv.GetRecords<dynamic>()
            .Select(row => new RatingInput
            {
                UserId = (uint)int.Parse(row.user_id),
                ItemId = ((string)row.item_id).Trim(),
                Rating = float.Parse(row.ranking, CultureInfo.InvariantCulture),
            })
            .ToList();
    }
}
