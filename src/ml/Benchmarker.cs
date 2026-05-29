using System.Diagnostics;
using AventuraML.Models;
using Microsoft.ML;

namespace AventuraML;

public static class Benchmarker
{
    public static void Run(
        string modelPath,
        string coldStartPath,
        string datasetPath,
        string itemCategoriesPath,
        string categoryAffinityPath,
        int n = 500,
        int topK = 10)
    {
        Console.WriteLine($"=== Benchmark C# — ML.NET MatrixFactorization (n={n}, top-{topK}) ===\n");

        // 1. Startup
        var swLoad = Stopwatch.StartNew();
        var recommender = new Recommender(modelPath, coldStartPath, datasetPath, itemCategoriesPath, categoryAffinityPath);
        swLoad.Stop();
        Console.WriteLine($"  Startup (model load + CSV): {swLoad.ElapsedMilliseconds} ms");

        // Usuários reais com histórico (para evitar cold-start na medição)
        var rows = DataPipeline.Load(datasetPath);
        var usersWithHistory = rows
            .GroupBy(r => (int)r.UserId)
            .Where(g => g.Count() >= 2)
            .Select(g => g.Key)
            .ToArray();

        var rng = new Random(42);

        // 2. Warmup
        for (int i = 0; i < 30; i++)
            recommender.Recommend(usersWithHistory[rng.Next(usersWithHistory.Length)], topK);

        // 3. Benchmark
        Console.WriteLine($"  Warmup concluído. Rodando {n} requests...\n");
        var latenciesUs = new double[n];
        var totalSw = Stopwatch.StartNew();

        for (int i = 0; i < n; i++)
        {
            var uid = usersWithHistory[rng.Next(usersWithHistory.Length)];
            var sw = Stopwatch.StartNew();
            recommender.Recommend(uid, topK);
            sw.Stop();
            latenciesUs[i] = sw.Elapsed.TotalMicroseconds;
        }

        totalSw.Stop();

        // 4. Resultados
        Array.Sort(latenciesUs);
        PrintResults(latenciesUs, n, totalSw.Elapsed.TotalSeconds);
    }

    private static void PrintResults(double[] sortedUs, int n, double totalSec)
    {
        double mean  = sortedUs.Average();
        double p50   = sortedUs[n / 2];
        double p95   = sortedUs[(int)(n * 0.95)];
        double p99   = sortedUs[(int)(n * 0.99)];
        double tps   = n / totalSec;

        Console.WriteLine("  Latência por request:");
        Console.WriteLine($"    Média  : {mean / 1000.0:F2} ms  ({mean:F0} µs)");
        Console.WriteLine($"    P50    : {p50  / 1000.0:F2} ms  ({p50:F0} µs)");
        Console.WriteLine($"    P95    : {p95  / 1000.0:F2} ms  ({p95:F0} µs)");
        Console.WriteLine($"    P99    : {p99  / 1000.0:F2} ms  ({p99:F0} µs)");
        Console.WriteLine($"  Throughput (single-thread): {tps:F1} req/s");
        Console.WriteLine($"  Tempo total ({n} reqs):    {totalSec * 1000:F0} ms");
    }
}
