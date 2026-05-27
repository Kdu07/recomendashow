using BackEnd.Aventura.Repository;
using BackEnd.Aventura.Services;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// TODO: Register your repository implementations after connecting to your database
// Example:
// builder.Services.AddScoped<ITeatroRepository, YourTeatroRepository>();
// builder.Services.AddScoped<ITeatroService, TeatroService>();
// builder.Services.AddScoped<IEspetaculoRepository, YourEspetaculoRepository>();
// builder.Services.AddScoped<IEspetaculoService, EspetaculoService>();
// builder.Services.AddScoped<IWishlistRepository, YourWishlistRepository>();
// builder.Services.AddScoped<IWishlistService, WishlistService>();

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