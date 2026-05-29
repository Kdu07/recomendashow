using BackEnd.Aventura.Repository;
using DotNetEnv;

Env.Load("../.env");

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IEspetaculoRepository, EspetaculoRepository>();
builder.Services.AddScoped<ITeatroRepository, TeatroRepository>();
builder.Services.AddScoped<IPreferenciasRepository, PreferenciasRepository>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();

builder.Services.AddScoped<IEspetaculoService, EspetaculoService>();
builder.Services.AddScoped<ITeatroService, TeatroService>();
builder.Services.AddScoped<IPreferenciasService, PreferenciasService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();