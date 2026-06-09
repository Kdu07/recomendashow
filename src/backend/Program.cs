using BackEnd.Aventura.Extensions;
using BackEnd.Aventura.Services;
using DotNetEnv;


var builder = WebApplication.CreateBuilder(args);

static void LoadEnvFile()
{
    var candidates = new[]
    {
        Path.Combine(Directory.GetCurrentDirectory(), ".env"),
        Path.Combine(Directory.GetCurrentDirectory(), "..", ".env"),
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".env"),
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", ".env"),
    };

    foreach (var path in candidates.Select(Path.GetFullPath).Distinct())
    {
        if (!File.Exists(path))
        {
            continue;
        }

        try
        {
            Env.Load(path);
        }
        catch
        {
            LoadEnvFileFallback(path);
        }

        return;
    }
}

static void LoadEnvFileFallback(string path)
{
    foreach (var line in File.ReadAllLines(path)
        .Select(line => line.Trim())
        .Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith("#")))
    {
        var separatorIndex = line.IndexOf('=');
        if (separatorIndex <= 0)
        {
            continue;
        }

        var key = line[..separatorIndex].Trim();
        var value = line[(separatorIndex + 1)..].Trim();

        if ((value.StartsWith("'") && value.EndsWith("'")) ||
            (value.StartsWith("\"") && value.EndsWith("\"")))
        {
            value = value[1..^1];
        }

        Environment.SetEnvironmentVariable(key, value);
    }
}

LoadEnvFile();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// HttpClient para o recommendation-service
builder.Services.AddHttpClient("RecommendationService", client =>
{
    var url = builder.Configuration["RecommendationService:BaseUrl"]
        ?? Environment.GetEnvironmentVariable("RECOMMENDATION_SERVICE_URL")
        ?? "http://localhost:5000";

    client.BaseAddress = new Uri(url);
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddSingleton<IPreferenceService, PreferenceService>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services
    .AddAventuraInfrastructure()
    .AddAventuraApplication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseCors();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
