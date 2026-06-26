using DotNetEnv;
using Npgsql;

Env.Load("../.env");

var host = Environment.GetEnvironmentVariable("DB_HOST");
var port = Environment.GetEnvironmentVariable("DB_PORT");
var database = Environment.GetEnvironmentVariable("DB_NAME");
var user = Environment.GetEnvironmentVariable("DB_USER");
var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

var connectionString =
    $"Host={host};" +
    $"Port={port};" +
    $"Database={database};" +
    $"Username={user};" +
    $"Password={password};" +
    $"SSL Mode=Require;" +
    $"Trust Server Certificate=true";

try
{
    await using var conn = new NpgsqlConnection(connectionString);

    await conn.OpenAsync();

    Console.WriteLine("Conectado ao Supabase");

    var sql1 = File.ReadAllText(
        "../src/database/01_create_database.sql"
    );

    await new NpgsqlCommand(sql1, conn)
        .ExecuteNonQueryAsync();

    Console.WriteLine("Arquivo 1 executado");

    var sql2 = File.ReadAllText(
        "../src/database/02_sprint4_improvements.sql"
    );

    await new NpgsqlCommand(sql2, conn)
        .ExecuteNonQueryAsync();

    Console.WriteLine("Arquivo 2 executado");

    var sql3 = File.ReadAllText(
        "../src/database/03_initial_data.sql"
    );

    await new NpgsqlCommand(sql3, conn)
        .ExecuteNonQueryAsync();

    Console.WriteLine("Arquivo 3 executado");

    var sql4 = File.ReadAllText(
        "../src/database/04_espetaculo_dataset_mapping.sql"
    );

    await new NpgsqlCommand(sql4, conn)
        .ExecuteNonQueryAsync();

    Console.WriteLine("Arquivo 4 executado");

    var command = new NpgsqlCommand(
        "SELECT COUNT(*) FROM usuarios",
        conn
    );

    var count = await command.ExecuteScalarAsync();

    Console.WriteLine($"Usuários inseridos: {count}");
}
catch(Exception ex)
{
    Console.WriteLine(ex.ToString());
}
