using BackEnd.Aventura.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IPreferenceService, PreferenceService>();
// RecommendationService carrega o modelo ML.NET na inicialização — deve ser singleton.
builder.Services.AddSingleton<IRecommendationService, RecommendationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
