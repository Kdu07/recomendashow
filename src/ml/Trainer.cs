using System.Text.Json;
using AventuraML.Models;
using Microsoft.ML;
using Microsoft.ML.Trainers;

namespace AventuraML;

public static class Trainer
{
    public static void Train(
        string datasetPath,
        string modelOutputPath,
        string coldStartOutputPath,
        string itemCategoriesOutputPath,
        string categoryAffinityOutputPath,
        string coldStartByCategoryOutputPath)
    {
        Console.WriteLine("Carregando dataset...");
        var rawRows = DataPipeline.LoadRaw(datasetPath);
        var rows = rawRows.Select(r => new RatingData
        {
            UserId = (uint)r.UserId,
            ItemId = r.ItemId,
            Rating = r.Ranking,
        }).ToList();
        Console.WriteLine($"  {rows.Count} registros carregados.");

        var popularItems = rows
            .GroupBy(r => r.ItemId)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .ToList();

        File.WriteAllText(coldStartOutputPath, JsonSerializer.Serialize(popularItems));
        Console.WriteLine($"  {popularItems.Count} itens únicos. Cold-start salvo em {coldStartOutputPath}");

        // item_id → tipo_evento
        var itemCategories = rawRows
            .GroupBy(r => r.ItemId)
            .ToDictionary(g => g.Key, g => g.First().TipoEvento);
        File.WriteAllText(itemCategoriesOutputPath, JsonSerializer.Serialize(itemCategories));
        Console.WriteLine($"  item_categories salvo em {itemCategoriesOutputPath}");

        // userId → { tipo_evento → normalized_score }
        var categoryAffinity = rawRows
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
        File.WriteAllText(categoryAffinityOutputPath, JsonSerializer.Serialize(categoryAffinity));
        Console.WriteLine($"  category_affinity salvo em {categoryAffinityOutputPath}");

        // tipo_evento → [items by popularity]
        var coldStartByCategory = rawRows
            .GroupBy(r => r.TipoEvento)
            .ToDictionary(
                g => g.Key,
                g => g.GroupBy(r => r.ItemId)
                       .OrderByDescending(item => item.Count())
                       .Select(item => item.Key)
                       .ToList());
        File.WriteAllText(coldStartByCategoryOutputPath, JsonSerializer.Serialize(coldStartByCategory));
        Console.WriteLine($"  cold_start_by_category salvo em {coldStartByCategoryOutputPath}");

        var mlContext = new MLContext(seed: 42);
        var data = mlContext.Data.LoadFromEnumerable(rows);

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

        Console.WriteLine("Treinando modelo (MatrixFactorization)...");
        var model = pipeline.Fit(data);
        Console.WriteLine("  Treinamento concluído.");

        Directory.CreateDirectory(Path.GetDirectoryName(modelOutputPath)!);
        mlContext.Model.Save(model, data.Schema, modelOutputPath);
        Console.WriteLine($"  Modelo salvo em {modelOutputPath}");
    }
}
