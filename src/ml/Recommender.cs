using System.Text.Json;
using AventuraML.Models;
using Microsoft.ML;
using Microsoft.ML.Trainers.Recommender;

namespace AventuraML;

public sealed class Recommender
{
    private readonly MLContext _mlContext;
    private readonly PredictionEngine<RatingData, RatingPrediction> _engine;
    private readonly object _engineLock = new();
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
        LatentFactors = TryGetApproximationRank(model);

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

    /// <summary>Peso do SVD no score híbrido.</summary>
    public float Alpha => _alpha;

    /// <summary>Usuários com histórico no dataset de treino.</summary>
    public int KnownUserCount => _userHistory.Count;

    /// <summary>Itens conhecidos pelo modelo (catálogo do cold_start.json).</summary>
    public int CatalogSize => _allItems.Count;

    /// <summary>Rank da fatoração de matrizes (nº de fatores latentes), se disponível no model.zip.</summary>
    public int? LatentFactors { get; }

    /// <summary>item_id → tipo_evento (artefato item_categories.json).</summary>
    public IReadOnlyDictionary<string, string> ItemCategories => _itemCategories;

    /// <summary>userId → { tipo_evento → afinidade normalizada } (artefato category_affinity.json).</summary>
    public IReadOnlyDictionary<string, Dictionary<string, float>> CategoryAffinity => _categoryAffinity;

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
            if (PredictRating(userId, item) is float svdScore)
            {
                var cat = _itemCategories.GetValueOrDefault(item, "");
                rawScores.Add((item, svdScore, cat));
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
            .Select(x => (x.Item, (float?)x.FinalScore))
            .ToList();
    }

    /// <summary>
    /// Score SVD bruto do model.zip para o par (usuário, item), na escala de rating do treino.
    /// Retorna null para pares fora do vocabulário do modelo ou predições inválidas.
    /// O acesso ao PredictionEngine (não thread-safe) é serializado por lock.
    /// </summary>
    public float? PredictRating(int userId, string itemId)
    {
        try
        {
            RatingPrediction pred;
            lock (_engineLock)
            {
                pred = _engine.Predict(new RatingData
                {
                    UserId = (uint)userId,
                    ItemId = itemId,
                    Rating = 0f,
                });
            }

            return float.IsNaN(pred.Score) || float.IsInfinity(pred.Score) ? null : pred.Score;
        }
        catch
        {
            return null; // item fora do vocabulário do modelo
        }
    }

    private float GetCategoryAffinity(uint userId, string category)
    {
        if (string.IsNullOrEmpty(category)) return 0f;
        if (_categoryAffinity.TryGetValue(userId.ToString(), out var userAffinities))
            return userAffinities.GetValueOrDefault(category, 0f);
        return 0f;
    }

    private static int? TryGetApproximationRank(ITransformer model)
    {
        if (model is not IEnumerable<ITransformer> chain) return null;

        foreach (var transformer in chain)
        {
            if (transformer is MatrixFactorizationPredictionTransformer mf)
                return mf.Model.ApproximationRank;
        }

        return null;
    }
}
