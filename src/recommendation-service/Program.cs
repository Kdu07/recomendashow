using RecommendationService.Data;
using RecommendationService.Infrastructure.Middleware;
using RecommendationService.Services;
using RecommendationService.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Aventura Recomendações API",
        Version = "v1",
        Description = "Sistema de recomendação de espetáculos — Portal Aventura. " +
                      "Funk SVD (modelo principal) e KNN Item-Based como alternativa."
    });
});

// Carrega dataset e pesos dos modelos como singletons na inicialização
builder.Services.Configure<ModelPathsOptions>(builder.Configuration.GetSection("ModelPaths"));
builder.Services.Configure<RecommendationOptions>(builder.Configuration.GetSection("Recommendation"));

builder.Services.AddSingleton<IDatasetRepository, DatasetRepository>();
builder.Services.AddSingleton<IModelLoaderService, ModelLoaderService>();
builder.Services.AddHostedService(p => (ModelLoaderService)p.GetRequiredService<IModelLoaderService>());

builder.Services.AddSingleton<SvdRecommendationService>();
builder.Services.AddSingleton<MetricsService>();
builder.Services.AddSingleton<IChartService, ScottPlotChartService>();

// Serviço padrão de recomendação é SVD (melhor modelo: RMSE 0.2953 vs KNN 0.3402)
builder.Services.AddSingleton<IRecommendationService>(p =>
    p.GetRequiredService<SvdRecommendationService>());

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

// ---------------------------------------------------------------------------
// Opções de configuração (records para bind do appsettings.json)
// ---------------------------------------------------------------------------

public record ModelPathsOptions
{
    public string SvdWeightsJson { get; init; } = "data/models/svd_weights.json";
    public string KnnWeightsJson { get; init; } = "data/models/knn_weights.json";
    public string DatasetCsv { get; init; } = "data/dataset/dataset.csv";
}

public record RecommendationOptions
{
    public string DefaultModel { get; init; } = "svd";
    public int DefaultN { get; init; } = 10;
}
