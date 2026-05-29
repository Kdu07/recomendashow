using DotNetEnv;
using Npgsql;
using System.IO;

// Procura um arquivo .env subindo a hierarquia a partir do diretório atual
string? FindEnvFile()
{
    var dir = Directory.GetCurrentDirectory();
    for (int i = 0; i < 6; i++)
    {
        var candidate = Path.Combine(dir, ".env");
        if (File.Exists(candidate)) return candidate;
        var parent = Directory.GetParent(dir);
        if (parent == null) break;
        dir = parent.FullName;
    }
    return null;
}

var envPath = FindEnvFile();
if (!string.IsNullOrEmpty(envPath)) Env.Load(envPath);
else Env.Load();

var user = Environment.GetEnvironmentVariable("POSTGRES_USER");
var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
var database = Environment.GetEnvironmentVariable("POSTGRES_DB");
var port = Environment.GetEnvironmentVariable("POSTGRES_PORT");
var host = Environment.GetEnvironmentVariable("POSTGRES_HOST");

var connectionString = $"Host={host};Port={port};Username={user};Password={password};Database={database}";

try{
    await using var conn = new NpgsqlConnection(connectionString);
    await conn.OpenAsync();
    Console.WriteLine("conexão feita com sucesso!!!!!!");
}
catch (Exception ex)
{
    Console.WriteLine($"Erro ao conectar ao banco de dados: {ex.Message}");
}
