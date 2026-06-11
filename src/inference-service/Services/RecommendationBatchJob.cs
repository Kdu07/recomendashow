// Services/RecommendationBatchJob.cs
using InferenceService.Data;
using InferenceService.Services.Interfaces;

namespace InferenceService.Services;

public sealed class RecommendationBatchJob : BackgroundService
{
    private readonly IRecommendationService _recommendationService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IDatasetRepository _dataset;
    private readonly ILogger<RecommendationBatchJob> _logger;

    public RecommendationBatchJob(
        IRecommendationService recommendationService,
        IServiceProvider serviceProvider,
        IDatasetRepository dataset,
        ILogger<RecommendationBatchJob> logger)
    {
        _recommendationService = recommendationService;
        _serviceProvider = serviceProvider;
        _dataset = dataset;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Iniciando batch de recomendações...");

            foreach (var userId in _dataset.GetAllUserIds())
            {
                if (stoppingToken.IsCancellationRequested) break;

                try
                {
                    var recs = await _recommendationService.RecommendAsync(userId, n: 10);

                    await using var scope = _serviceProvider.CreateAsyncScope();
                    var writeRepo = scope.ServiceProvider.GetRequiredService<IRecommendationWriteRepository>();
                    await writeRepo.SaveAsync(userId, recs, _recommendationService.ModelType);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Falha ao processar userId={UserId}.", userId);
                }
            }

            _logger.LogInformation("Batch concluído. Próxima execução em 24h.");
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}