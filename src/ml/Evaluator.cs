using AventuraML.Models;
using System.Text.Json;
using Microsoft.ML;
using Microsoft.ML.Trainers;

namespace AventuraML;

public static class Evaluator
{
    /// <summary>
    /// Avaliação offline pelo console: split 80/20 por usuário, treina um SVD
    /// apenas no conjunto de treino e imprime SVD puro × híbrido.
    /// </summary>
    public static void Evaluate(
        string datasetPath,
        string itemCategoriesPath,
        int topK = 10,
        float alpha = 0.7f,
        int seed = 42)
    {
        Console.WriteLine("Carregando dataset...");
        var rawRows = DataPipeline.LoadRaw(datasetPath);

        var itemCats = JsonSerializer.Deserialize<Dictionary<string, string>>(
            File.ReadAllText(itemCategoriesPath)) ?? [];

        var (trainRows, testByUser) = SplitByUser(rawRows, seed);

        Console.WriteLine($"  Treino: {trainRows.Count} interações | Teste: {testByUser.Sum(x => x.Value.Count)} interações em {testByUser.Count} usuários");

        // Afinidade de categoria calculada sobre o treino
        var categoryAffinity = BuildCategoryAffinity(trainRows);

        // Treinar SVD apenas no conjunto de treino
        var trainingData = trainRows.Select(r => new RatingData
        {
            UserId = (uint)r.UserId,
            ItemId = r.ItemId,
            Rating = r.Ranking,
        }).ToList();

        Console.WriteLine("Treinando SVD (conjunto de treino)...");
        var mlContext = new MLContext(seed: 42);
        var data = mlContext.Data.LoadFromEnumerable(trainingData);

        var pipeline = mlContext.Transforms.Conversion
            .MapValueToKey("UserIdEncoded", nameof(RatingData.UserId))
            .Append(mlContext.Transforms.Conversion
                .MapValueToKey("ItemIdEncoded", nameof(RatingData.ItemId)))
            .Append(mlContext.Recommendation().Trainers.MatrixFactorization(
                new MatrixFactorizationTrainer.Options
                {
                    MatrixColumnIndexColumnName = "UserIdEncoded",
                    MatrixRowIndexColumnName = "ItemIdEncoded",
                    LabelColumnName = nameof(RatingData.Rating),
                    NumberOfIterations = 20,
                    ApproximationRank = 50,
                    LearningRate = 0.01f,
                    Lambda = 0.01f,
                }));

        var model = pipeline.Fit(data);
        var engine = mlContext.Model.CreatePredictionEngine<RatingData, RatingPrediction>(model);
        Console.WriteLine("  Treino concluído.\n");

        float? Predict(int userId, string itemId)
        {
            try
            {
                var pred = engine.Predict(new RatingData { UserId = (uint)userId, ItemId = itemId, Rating = 0f });
                return float.IsNaN(pred.Score) || float.IsInfinity(pred.Score) ? null : pred.Score;
            }
            catch
            {
                return null;
            }
        }

        Console.WriteLine($"Avaliando top-{topK} para {testByUser.Count} usuários...");

        var report = EvaluateWithPredictor(
            Predict, trainRows, testByUser, itemCats, categoryAffinity, topK, alpha);

        PrintReport(report);
    }

    /// <summary>
    /// Split 80/20 por usuário (mínimo 2 interações para entrar no teste),
    /// embaralhado com seed fixa para reprodutibilidade.
    /// </summary>
    public static (List<CsvRatingRow> TrainRows, Dictionary<int, List<CsvRatingRow>> TestByUser) SplitByUser(
        List<CsvRatingRow> rawRows,
        int seed = 42)
    {
        var rng = new Random(seed);
        var trainRows = new List<CsvRatingRow>();
        var testByUser = new Dictionary<int, List<CsvRatingRow>>();

        foreach (var group in rawRows.GroupBy(r => r.UserId))
        {
            var items = group.OrderBy(_ => rng.Next()).ToList();
            int nTest = items.Count < 2 ? 0 : Math.Max(1, (int)(items.Count * 0.2f));

            if (nTest == 0)
            {
                trainRows.AddRange(items);
                continue;
            }

            testByUser[group.Key] = items.Take(nTest).ToList();
            trainRows.AddRange(items.Skip(nTest));
        }

        return (trainRows, testByUser);
    }

    /// <summary>
    /// Núcleo da avaliação — fonte única das métricas, usado pelo console e pelo
    /// host de inferência. Recebe um preditor de rating (score SVD bruto) e calcula:
    /// RMSE/MAE sobre os pares de teste, e métricas de ranking top-K (SVD puro ×
    /// híbrido). <paramref name="maxRankingUsers"/> permite amostrar os usuários da
    /// avaliação de ranking (cara: ~1 predict por item candidato por usuário);
    /// RMSE/MAE sempre usam todos os pares de teste.
    /// </summary>
    public static EvaluationReport EvaluateWithPredictor(
        Func<int, string, float?> predictRating,
        List<CsvRatingRow> trainRows,
        Dictionary<int, List<CsvRatingRow>> testByUser,
        IReadOnlyDictionary<string, string> itemCats,
        IReadOnlyDictionary<string, Dictionary<string, float>> categoryAffinity,
        int topK = 10,
        float alpha = 0.7f,
        int? maxRankingUsers = null,
        int seed = 42)
    {
        var popularItems = trainRows
            .GroupBy(r => r.ItemId)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .ToList();

        var trainByUser = trainRows
            .GroupBy(r => r.UserId)
            .ToDictionary(g => g.Key, g => g.ToList());

        // RMSE / MAE sobre todos os pares de teste (predição de rating, SVD bruto)
        double rmseSum = 0, maeSum = 0;
        var ratingPairs = new List<RatingPair>();

        foreach (var (userId, testItems) in testByUser)
        {
            foreach (var ti in testItems)
            {
                if (predictRating(userId, ti.ItemId) is float predicted)
                {
                    var err = ti.Ranking - predicted;
                    rmseSum += err * err;
                    maeSum += Math.Abs(err);
                    ratingPairs.Add(new RatingPair(ti.Ranking, predicted));
                }
            }
        }

        var rating = new RatingAccuracy(
            Rmse: ratingPairs.Count > 0 ? Math.Sqrt(rmseSum / ratingPairs.Count) : 0,
            Mae: ratingPairs.Count > 0 ? maeSum / ratingPairs.Count : 0,
            PairCount: ratingPairs.Count);

        // Ranking top-K — opcionalmente amostrado (custo: 1 predict por candidato)
        var rankingUserIds = testByUser.Keys.ToList();
        if (maxRankingUsers is int max && max < rankingUserIds.Count)
        {
            var rng = new Random(seed);
            rankingUserIds = rankingUserIds.OrderBy(_ => rng.Next()).Take(max).ToList();
        }

        int svdHits = 0, hybridHits = 0;
        double svdPrecSum = 0, hybridPrecSum = 0;
        double svdRecSum = 0, hybridRecSum = 0;
        double svdCatAvg = 0, hybridCatAvg = 0;
        int evaluated = 0;

        foreach (var userId in rankingUserIds)
        {
            var testItems = testByUser[userId];
            var userTrainRows = trainByUser.GetValueOrDefault(userId) ?? [];
            var consumed = userTrainRows.Select(r => r.ItemId).ToHashSet();
            var testItemSet = testItems.Select(r => r.ItemId).ToHashSet();

            // Candidatos: itens não consumidos no treino
            var rawScores = new List<(string Item, float SvdScore, string Category)>();
            foreach (var item in popularItems.Where(i => !consumed.Contains(i)))
            {
                if (predictRating(userId, item) is float score)
                    rawScores.Add((item, score, itemCats.GetValueOrDefault(item, "")));
            }

            if (rawScores.Count == 0) continue;
            evaluated++;

            // SVD puro: top-K por score bruto
            var svdTopK = rawScores
                .OrderByDescending(x => x.SvdScore)
                .Take(topK)
                .Select(x => x.Item)
                .ToHashSet();

            // Híbrido: normaliza SVD → [0,1] depois combina com afinidade
            var minSvd = rawScores.Min(x => x.SvdScore);
            var maxSvd = rawScores.Max(x => x.SvdScore);
            var svdRange = maxSvd - minSvd;
            var userAff = categoryAffinity.GetValueOrDefault(userId.ToString()) ?? [];

            var hybridTopKItems = rawScores
                .Select(x =>
                {
                    var normSvd = svdRange > 1e-6f ? (x.SvdScore - minSvd) / svdRange : 0.5f;
                    var aff = x.Category != "" ? userAff.GetValueOrDefault(x.Category, 0f) : 0f;
                    return (Item: x.Item, Score: alpha * normSvd + (1f - alpha) * aff);
                })
                .OrderByDescending(x => x.Score)
                .Take(topK)
                .Select(x => x.Item)
                .ToHashSet();

            // Hit Rate
            if (svdTopK.Any(testItemSet.Contains)) svdHits++;
            if (hybridTopKItems.Any(testItemSet.Contains)) hybridHits++;

            // Precision e Recall
            svdPrecSum += (double)svdTopK.Count(testItemSet.Contains) / topK;
            hybridPrecSum += (double)hybridTopKItems.Count(testItemSet.Contains) / topK;
            svdRecSum += (double)svdTopK.Count(testItemSet.Contains) / testItemSet.Count;
            hybridRecSum += (double)hybridTopKItems.Count(testItemSet.Contains) / testItemSet.Count;

            // Category match: % dos top-K que pertencem às categorias preferidas do usuário
            var userTopCats = userTrainRows
                .GroupBy(r => r.TipoEvento)
                .OrderByDescending(g => g.Sum(r => r.Ranking))
                .Take(2)
                .Select(g => g.Key)
                .ToHashSet();

            svdCatAvg += userTopCats.Count > 0
                ? (double)svdTopK.Count(i => userTopCats.Contains(itemCats.GetValueOrDefault(i, ""))) / topK
                : 0;
            hybridCatAvg += userTopCats.Count > 0
                ? (double)hybridTopKItems.Count(i => userTopCats.Contains(itemCats.GetValueOrDefault(i, ""))) / topK
                : 0;
        }

        var svdRanking = evaluated > 0
            ? new RankingMetrics(
                HitRateAtK: (double)svdHits / evaluated,
                PrecisionAtK: svdPrecSum / evaluated,
                RecallAtK: svdRecSum / evaluated,
                CategoryPrecisionAtK: svdCatAvg / evaluated)
            : new RankingMetrics(0, 0, 0, 0);

        var hybridRanking = evaluated > 0
            ? new RankingMetrics(
                HitRateAtK: (double)hybridHits / evaluated,
                PrecisionAtK: hybridPrecSum / evaluated,
                RecallAtK: hybridRecSum / evaluated,
                CategoryPrecisionAtK: hybridCatAvg / evaluated)
            : new RankingMetrics(0, 0, 0, 0);

        return new EvaluationReport(
            rating, svdRanking, hybridRanking, evaluated, topK, alpha, ratingPairs);
    }

    public static Dictionary<string, Dictionary<string, float>> BuildCategoryAffinity(
        List<CsvRatingRow> rows)
    {
        return rows
            .GroupBy(r => r.UserId)
            .ToDictionary(
                g => g.Key.ToString(),
                g =>
                {
                    var byCategory = g
                        .GroupBy(r => r.TipoEvento)
                        .ToDictionary(cat => cat.Key, cat => cat.Sum(r => r.Ranking));
                    var total = byCategory.Values.Sum();
                    return byCategory.ToDictionary(
                        kvp => kvp.Key,
                        kvp => total > 0f ? kvp.Value / total : 0f);
                });
    }

    private static void PrintReport(EvaluationReport report)
    {
        Console.WriteLine("\n══════════════════════════════════════════════════════");
        Console.WriteLine("  MÉTRICAS DE AVALIAÇÃO (split 80/20 por usuário)");
        Console.WriteLine("══════════════════════════════════════════════════════\n");

        if (report.Rating.PairCount > 0)
        {
            Console.WriteLine($"  Predição de rating (SVD puro, {report.Rating.PairCount} pares):");
            Console.WriteLine($"    RMSE : {report.Rating.Rmse:F4}");
            Console.WriteLine($"    MAE  : {report.Rating.Mae:F4}");
        }

        Console.WriteLine($"\n  Ranking (top-{report.TopK}, {report.UsersEvaluated} usuários avaliados):\n");
        Console.WriteLine($"  {"Métrica",-28} {"SVD puro",10} {"Híbrido",10}  {"Δ",8}");
        Console.WriteLine($"  {new string('─', 60)}");

        void Row(string label, double svd, double hybrid)
        {
            var delta = hybrid - svd;
            var sign = delta >= 0 ? "+" : "";
            Console.WriteLine($"  {label,-28} {svd,10:F4} {hybrid,10:F4}  {sign}{delta,7:F4}");
        }

        var k = report.TopK;
        Row($"Hit Rate@{k}", report.SvdRanking.HitRateAtK, report.HybridRanking.HitRateAtK);
        Row($"Precision@{k}", report.SvdRanking.PrecisionAtK, report.HybridRanking.PrecisionAtK);
        Row($"Recall@{k}", report.SvdRanking.RecallAtK, report.HybridRanking.RecallAtK);
        Row($"Category Precision@{k}", report.SvdRanking.CategoryPrecisionAtK, report.HybridRanking.CategoryPrecisionAtK);

        Console.WriteLine($"\n  α = {report.Alpha:F1}  (SVD weight)");
        Console.WriteLine("══════════════════════════════════════════════════════");
    }
}
