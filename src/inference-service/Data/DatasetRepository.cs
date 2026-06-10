using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Options;

namespace InferenceService.Data;

/// <summary>
/// Loads dataset.csv once at startup and exposes lookup structures.
/// Replaces Python's module-level pd.read_csv + _COLD_START_ITEMS computation
/// from inference.py lines 21–36.
/// </summary>
public sealed class DatasetRepository : IDatasetRepository
{
    public IReadOnlySet<string> AllItems { get; }
    public IReadOnlyList<string> ColdStartItems { get; }
    public IReadOnlyList<InteractionRecord> AllInteractions { get; }

    // user_id → list of their interactions
    private readonly Dictionary<int, List<InteractionRecord>> _byUser;
    // (user_id, item_id) → ranking
    private readonly Dictionary<(int, string), int> _ratings;

    public DatasetRepository(IOptions<ModelPathsOptions> opts, ILogger<DatasetRepository> logger)
    {
        var path = ResolveDatasetPath(opts.Value.DatasetCsv);
        logger.LogInformation("Carregando dataset de {Path}...", path);

        var records = LoadCsv(path);
        AllInteractions = records.AsReadOnly();

        _byUser = records
            .GroupBy(r => r.UserId)
            .ToDictionary(g => g.Key, g => g.ToList());

        _ratings = records.ToDictionary(
            r => (r.UserId, r.ItemId),
            r => r.Ranking,
            EqualityComparer<(int, string)>.Default);

        AllItems = records.Select(r => r.ItemId).ToHashSet();

        // Popularidade: mesma lógica de inference.py — ordena por contagem de interações desc
        ColdStartItems = records
            .GroupBy(r => r.ItemId)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .ToList()
            .AsReadOnly();

        logger.LogInformation(
            "Dataset carregado: {Interactions} interações, {Users} usuários, {Items} itens.",
            AllInteractions.Count, _byUser.Count, AllItems.Count);
    }

    public IReadOnlyList<InteractionRecord> GetInteractions(int userId) =>
        _byUser.TryGetValue(userId, out var list) ? list.AsReadOnly() : [];

    public IReadOnlySet<string> GetConsumedItems(int userId) =>
        _byUser.TryGetValue(userId, out var list)
            ? list.Select(r => r.ItemId).ToHashSet()
            : new HashSet<string>();

    public int? GetRating(int userId, string itemId) =>
        _ratings.TryGetValue((userId, itemId), out var rank) ? rank : null;

    public bool UserExists(int userId) => _byUser.ContainsKey(userId);

    public IReadOnlyList<int> GetAllUserIds() => _byUser.Keys.ToList();

    // ---------------------------------------------------------------------------

    private static List<InteractionRecord> LoadCsv(string path)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
        };
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, config);
        return csv.GetRecords<InteractionRecord>().ToList();
    }

    private static string ResolveDatasetPath(string configured)
    {
        // Tenta relativo ao diretório de execução e também relativo ao ContentRoot
        if (File.Exists(configured)) return configured;

        var fromExe = Path.Combine(AppContext.BaseDirectory, configured);
        if (File.Exists(fromExe)) return fromExe;

        // Sobe até encontrar a raiz do repositório (contém src/)
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            var candidate = Path.Combine(dir.FullName, configured);
            if (File.Exists(candidate)) return candidate;
            dir = dir.Parent;
        }

        throw new FileNotFoundException(
            $"Dataset não encontrado. Caminho configurado: '{configured}'.");
    }
}
