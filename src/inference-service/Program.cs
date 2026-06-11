using InferenceService.Data;
using InferenceService.Infrastructure.Middleware;
using InferenceService.Services;
using InferenceService.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Aventura Inferência API",
        Version = "v1",
        Description = "Host de inferência do motor de recomendação híbrido " +
                      "(Funk SVD via ML.NET + afinidade de categoria). " +
                      "As recomendações são persistidas em batch (24h) na tabela " +
                      "recomendacoes; os endpoints expõem métricas e diagnóstico do modelo."
    });
});

// Artefatos do modelo híbrido e dataset carregados como singletons na inicialização
builder.Services.Configure<ModelPathsOptions>(builder.Configuration.GetSection("ModelPaths"));
builder.Services.Configure<RecommendationOptions>(builder.Configuration.GetSection("Recommendation"));

builder.Services.AddSingleton<IDatasetRepository, DatasetRepository>();
builder.Services.AddTransient<IRecommendationWriteRepository, RecommendationWriteRepository>();
builder.Services.AddSingleton<IHybridModelLoader, HybridModelLoader>();
builder.Services.AddHostedService(p => (HybridModelLoader)p.GetRequiredService<IHybridModelLoader>());

builder.Services.AddHostedService<RecommendationBatchJob>();
builder.Services.AddSingleton<MetricsService>();
builder.Services.AddSingleton<IChartService, ScottPlotChartService>();

// Motor único de recomendação: híbrido (SVD + afinidade de categoria), fórmula em src/ml
builder.Services.AddSingleton<IRecommendationService, HybridRecommendationService>();

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


public record ModelPathsOptions
{
    public string ModelZip { get; init; } = "data/models/model.zip";
    public string ColdStartJson { get; init; } = "data/models/cold_start.json";
    public string ItemCategoriesJson { get; init; } = "data/models/item_categories.json";
    public string CategoryAffinityJson { get; init; } = "data/models/category_affinity.json";
    public string DatasetCsv { get; init; } = "data/dataset/dataset.csv";
}

public record RecommendationOptions
{
    public int DefaultN { get; init; } = 10;

    /// <summary>Peso do SVD no score híbrido (1−α = peso da afinidade de categoria).</summary>
    public float Alpha { get; init; } = 0.7f;
}
