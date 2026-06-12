using BackEnd.Aventura.Extensions;
using BackEnd.Aventura.Services;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;


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

builder.Configuration.AddEnvironmentVariables();


builder.Services.AddControllers(options =>
{
    var authenticatedPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(authenticatedPolicy));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT configuration
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")
    ?? builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT key is not configured.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "BackEnd.Aventura";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "BackEnd.AventuraClient";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
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
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program
{
}