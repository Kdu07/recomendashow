using AventuraML;
using Microsoft.Extensions.Options;
using InferenceService.Services.Interfaces;

namespace InferenceService.Services;

/// <summary>
/// Valida e carrega os artefatos do modelo híbrido na inicialização da API
/// (model.zip + cold_start.json + item_categories.json + category_affinity.json
/// + dataset.csv). Registrado como IHostedService para falhar rápido antes da
/// primeira requisição — mesmo comportamento do antigo loader de pesos SVD.
/// </summary>
public sealed class HybridModelLoader : IHybridModelLoader, IHostedService
{
    private Recommender? _recommender;

    public Recommender Recommender =>
        _recommender ?? throw new InvalidOperationException("Modelo híbrido ainda não carregado.");

    public bool IsLoaded => _recommender is not null;

    private readonly string _modelZipPath;
    private readonly string _coldStartPath;
    private readonly string _itemCategoriesPath;
    private readonly string _categoryAffinityPath;
    private readonly string _datasetPath;
    private readonly float _alpha;
    private readonly ILogger<HybridModelLoader> _logger;

    public HybridModelLoader(
        IOptions<ModelPathsOptions> paths,
        IOptions<RecommendationOptions> recommendation,
        ILogger<HybridModelLoader> logger)
    {
        _modelZipPath = ResolvePath(paths.Value.ModelZip);
        _coldStartPath = ResolvePath(paths.Value.ColdStartJson);
        _itemCategoriesPath = ResolvePath(paths.Value.ItemCategoriesJson);
        _categoryAffinityPath = ResolvePath(paths.Value.CategoryAffinityJson);
        _datasetPath = ResolvePath(paths.Value.DatasetCsv);
        _alpha = recommendation.Value.Alpha;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Carregando modelo híbrido de {ModelZip}...", _modelZipPath);

        _recommender = new Recommender(
            _modelZipPath,
            _coldStartPath,
            _datasetPath,
            _itemCategoriesPath,
            _categoryAffinityPath,
            _alpha);

        _logger.LogInformation(
            "Modelo híbrido carregado: {Users} usuários com histórico, {Items} itens, " +
            "{Factors} fatores latentes, α={Alpha}.",
            _recommender.KnownUserCount,
            _recommender.CatalogSize,
            _recommender.LatentFactors?.ToString() ?? "?",
            _recommender.Alpha);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    // ---------------------------------------------------------------------------

    private static string ResolvePath(string configured)
    {
        if (File.Exists(configured)) return configured;

        var fromExe = Path.Combine(AppContext.BaseDirectory, configured);
        if (File.Exists(fromExe)) return fromExe;

        // Sobe até encontrar a raiz do repositório (que contém data/)
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            var candidate = Path.Combine(dir.FullName, configured);
            if (File.Exists(candidate)) return candidate;
            dir = dir.Parent;
        }

        throw new FileNotFoundException(
            $"Artefato do modelo híbrido não encontrado. Caminho configurado: '{configured}'.");
    }
}
