using BackEnd.Aventura.Extensions;
using DotNetEnv;

LoadEnvFile();

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

        Env.Load(path);
        return;
    }
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
