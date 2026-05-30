using System.Text.Json;

namespace TrainingService.Export;

public static class WeightsExporter
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = false,
    };

    public static void Export(SvdWeightsExport weights, string outputPath)
    {
        Console.WriteLine($"\n[Export] Serializando pesos para {outputPath}...");

        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

        using var stream = File.Create(outputPath);
        JsonSerializer.Serialize(stream, weights, Options);

        var info = new FileInfo(outputPath);
        Console.WriteLine($"[Export] Concluído. Tamanho: {info.Length / 1024.0 / 1024.0:F1} MB");
    }
}
