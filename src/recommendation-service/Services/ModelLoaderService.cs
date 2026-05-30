using System.Text.Json;
using RecommendationService.Models;
using RecommendationService.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace RecommendationService.Services;

/// <summary>
/// Carrega os pesos dos modelos (svd_weights.json e knn_weights.json) na inicialização da API.
/// Registrado como IHostedService para garantir que os dados estejam prontos antes da
/// primeira requisição — espelha o comportamento de joblib.load() no nível de módulo em
/// inference.py linha 18.
/// </summary>
public sealed class ModelLoaderService : IModelLoaderService, IHostedService
{
    private SvdWeights? _svdWeights;

    public SvdWeights SvdWeights =>
        _svdWeights ?? throw new InvalidOperationException("Modelo SVD ainda não carregado.");
    public bool IsLoaded => _svdWeights is not null;

    private readonly string _svdPath;
    private readonly ILogger<ModelLoaderService> _logger;

    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public ModelLoaderService(IOptions<ModelPathsOptions> opts, ILogger<ModelLoaderService> logger)
    {
        _svdPath = ResolvePath(opts.Value.SvdWeightsJson);
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Carregando pesos do modelo SVD...");

        _svdWeights = await LoadJsonAsync<SvdWeights>(_svdPath, cancellationToken);

        _logger.LogInformation(
            "Modelo carregado. SVD: {NUsers} usuários, {NItems} itens, {NFactors} fatores.",
            _svdWeights.NUsers, _svdWeights.NItems,
            _svdWeights.Hyperparameters.NFactors);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    // ---------------------------------------------------------------------------

    private static async Task<T> LoadJsonAsync<T>(string path, CancellationToken ct)
    {
        await using var stream = File.OpenRead(path);
        var result = await JsonSerializer.DeserializeAsync<T>(stream, _jsonOpts, ct);
        return result ?? throw new InvalidDataException($"JSON inválido em '{path}'.");
    }

    private static string ResolvePath(string configured)
    {
        if (File.Exists(configured)) return configured;

        var fromExe = Path.Combine(AppContext.BaseDirectory, configured);
        if (File.Exists(fromExe)) return fromExe;

        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            var candidate = Path.Combine(dir.FullName, configured);
            if (File.Exists(candidate)) return candidate;
            dir = dir.Parent;
        }

        throw new FileNotFoundException(
            $"Arquivo de pesos não encontrado. Caminho configurado: '{configured}'.");
    }
}
