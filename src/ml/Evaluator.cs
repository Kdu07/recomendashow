using System.Text.Json;
using AventuraML.Models;
using Microsoft.ML;
using Microsoft.ML.Trainers;

namespace AventuraML;

public static class Evaluator
{
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

        // 80/20 split por usuário (mínimo 2 interações para entrar no teste)
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

        var popularItems = trainRows
            .GroupBy(r => r.ItemId)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .ToList();

        var userHistory = trainRows
            .GroupBy(r => r.UserId)
            .ToDictionary(g => g.Key, g => g.Select(r => r.ItemId).ToHashSet());

        // Acumuladores de métricas
        double svdRmse = 0, svdMae = 0;
        int ratingCount = 0;

        int svdHits = 0, hybridHits = 0;
        double svdPrecSum = 0, hybridPrecSum = 0;
        double svdRecSum = 0, hybridRecSum = 0;
        double svdCatAvg = 0, hybridCatAvg = 0;
        int evaluated = 0;

        Console.WriteLine($"Avaliando top-{topK} para {testByUser.Count} usuários...");

        foreach (var (userId, testItems) in testByUser)
        {
            var uid = (uint)userId;
            var consumed = userHistory.GetValueOrDefault(userId) ?? [];
            var testItemSet = testItems.Select(r => r.ItemId).ToHashSet();

            // RMSE / MAE apenas sobre predições de rating (SVD puro)
            foreach (var ti in testItems)
            {
                try
                {
                    var pred = engine.Predict(new RatingData { UserId = uid, ItemId = ti.ItemId, Rating = 0f });
                    if (!float.IsNaN(pred.Score) && !float.IsInfinity(pred.Score))
                    {
                        var err = ti.Ranking - pred.Score;
                        svdRmse += err * err;
                        svdMae += Math.Abs(err);
                        ratingCount++;
                    }
                }
                catch { }
            }

            // Candidatos: itens não consumidos no treino
            var rawScores = new List<(string Item, float SvdScore, string Category)>();
            foreach (var item in popularItems.Where(i => !consumed.Contains(i)))
            {
                try
                {
                    var pred = engine.Predict(new RatingData { UserId = uid, ItemId = item, Rating = 0f });
                    if (!float.IsNaN(pred.Score) && !float.IsInfinity(pred.Score))
                        rawScores.Add((item, pred.Score, itemCats.GetValueOrDefault(item, "")));
                }
                catch { }
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

            var hybridTopK = rawScores
                .Select(x =>
                {
                    var normSvd = svdRange > 1e-6f ? (x.SvdScore - minSvd) / svdRange : 0.5f;
                    var aff = x.Category != "" ? userAff.GetValueOrDefault(x.Category, 0f) : 0f;
                    return (Item: x.Item, Category: x.Category, Score: alpha * normSvd + (1f - alpha) * aff);
                })
                .OrderByDescending(x => x.Score)
                .Take(topK)
                .ToList();

            var hybridTopKItems = hybridTopK.Select(x => x.Item).ToHashSet();

            // Hit Rate
            if (svdTopK.Any(testItemSet.Contains)) svdHits++;
            if (hybridTopKItems.Any(testItemSet.Contains)) hybridHits++;

            // Precision e Recall
            svdPrecSum += (double)svdTopK.Count(testItemSet.Contains) / topK;
            hybridPrecSum += (double)hybridTopKItems.Count(testItemSet.Contains) / topK;
            svdRecSum += (double)svdTopK.Count(testItemSet.Contains) / testItemSet.Count;
            hybridRecSum += (double)hybridTopKItems.Count(testItemSet.Contains) / testItemSet.Count;

            // Category match: % dos top-K que pertencem às categorias preferidas do usuário
            var userTopCats = trainRows
                .Where(r => r.UserId == userId)
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

        // --- Impressão dos resultados ---
        Console.WriteLine("\n══════════════════════════════════════════════════════");
        Console.WriteLine("  MÉTRICAS DE AVALIAÇÃO (split 80/20 por usuário)");
        Console.WriteLine("══════════════════════════════════════════════════════\n");

        if (ratingCount > 0)
        {
            Console.WriteLine($"  Predição de rating (SVD puro, {ratingCount} pares):");
            Console.WriteLine($"    RMSE : {Math.Sqrt(svdRmse / ratingCount):F4}");
            Console.WriteLine($"    MAE  : {svdMae / ratingCount:F4}");
        }

        Console.WriteLine($"\n  Ranking (top-{topK}, {evaluated} usuários avaliados):\n");
        Console.WriteLine($"  {"Métrica",-28} {"SVD puro",10} {"Híbrido",10}  {"Δ",8}");
        Console.WriteLine($"  {new string('─', 60)}");

        void Row(string label, double svd, double hybrid)
        {
            var delta = hybrid - svd;
            var sign = delta >= 0 ? "+" : "";
            Console.WriteLine($"  {label,-28} {svd,10:F4} {hybrid,10:F4}  {sign}{delta,7:F4}");
        }

        Row($"Hit Rate@{topK}",       (double)svdHits / evaluated,    (double)hybridHits / evaluated);
        Row($"Precision@{topK}",      svdPrecSum / evaluated,          hybridPrecSum / evaluated);
        Row($"Recall@{topK}",         svdRecSum / evaluated,           hybridRecSum / evaluated);
        Row($"Category Precision@{topK}", svdCatAvg / evaluated,       hybridCatAvg / evaluated);

        Console.WriteLine($"\n  α = {alpha:F1}  (SVD weight)");
        Console.WriteLine("══════════════════════════════════════════════════════");
    }

    private static Dictionary<string, Dictionary<string, float>> BuildCategoryAffinity(
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
}
