using AventuraML;

// Caminhos relativos ao diretório de trabalho (use `dotnet run` a partir de src/ml/).
// Diretório canônico de artefatos: data/models/ na raiz do repositório.
var rawPath               = ResolveRelative(args, "--raw",                    "../../data/raw/database_sympla.csv");
var datasetPath           = ResolveRelative(args, "--dataset",                "../../data/dataset/dataset.csv");
var modelPath             = ResolveRelative(args, "--model",                  "../../data/models/model.zip");
var coldStart             = ResolveRelative(args, "--cold-start",             "../../data/models/cold_start.json");
var itemCategories        = ResolveRelative(args, "--item-categories",        "../../data/models/item_categories.json");
var categoryAffinity      = ResolveRelative(args, "--category-affinity",      "../../data/models/category_affinity.json");
var coldStartByCategory   = ResolveRelative(args, "--cold-start-by-category", "../../data/models/cold_start_by_category.json");

var command = args.FirstOrDefault(a => !a.StartsWith("--")) ?? "train";

switch (command)
{
    case "preprocess":
        Preprocessor.Preprocess(rawPath, datasetPath);
        Console.WriteLine("\nPré-processamento concluído. Execute 'dotnet run train' para treinar o modelo.");
        break;

    case "train":
        Trainer.Train(datasetPath, modelPath, coldStart, itemCategories, categoryAffinity, coldStartByCategory);
        Console.WriteLine("\nPipeline concluído. Execute 'dotnet run recommend <userId>' para testar.");
        break;

    case "recommend":
        var userIdArg = args.SkipWhile(a => a != "recommend").Skip(1).FirstOrDefault();
        var nArg      = GetFlag(args, "--n") ?? "10";

        if (!int.TryParse(userIdArg, out var userId))
        {
            Console.Error.WriteLine("Uso: dotnet run recommend <userId> [--n <quantidade>]");
            return 1;
        }

        var recommender = new Recommender(modelPath, coldStart, datasetPath, itemCategories, categoryAffinity);
        var results = recommender.Recommend(userId, int.Parse(nArg));

        Console.WriteLine($"\nTop {results.Count} recomendações para usuário {userId}:");
        foreach (var (show, score) in results)
            Console.WriteLine($"  {show,-60} score: {score?.ToString("F4") ?? "cold-start"}");
        break;

    case "benchmark":
        var benchN     = int.Parse(GetFlag(args, "--n")    ?? "500");
        var benchTopK  = int.Parse(GetFlag(args, "--k")    ?? "10");
        Benchmarker.Run(modelPath, coldStart, datasetPath, itemCategories, categoryAffinity,
            n: benchN, topK: benchTopK);
        break;

    case "evaluate":
        var kArg      = GetFlag(args, "--k")     ?? "10";
        var alphaArg  = GetFlag(args, "--alpha")  ?? "0.7";
        Evaluator.Evaluate(
            datasetPath,
            itemCategories,
            topK: int.Parse(kArg),
            alpha: float.Parse(alphaArg, System.Globalization.CultureInfo.InvariantCulture));
        break;

    default:
        Console.Error.WriteLine($"Comando desconhecido: '{command}'. Use: preprocess | train | recommend <userId> | evaluate");
        return 1;
}

return 0;

static string ResolveRelative(string[] args, string flag, string defaultRelative)
{
    var flagValue = GetFlag(args, flag);
    return Path.GetFullPath(flagValue ?? defaultRelative);
}

static string? GetFlag(string[] args, string flag)
{
    var idx = Array.IndexOf(args, flag);
    return idx >= 0 && idx + 1 < args.Length ? args[idx + 1] : null;
}
