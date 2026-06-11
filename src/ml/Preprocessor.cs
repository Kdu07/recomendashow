using System.Globalization;
using System.Text.RegularExpressions;
using AventuraML.Models;
using CsvHelper;
using CsvHelper.Configuration;

namespace AventuraML;

/// <summary>
/// Pré-processamento: transforma data/raw/database_sympla.csv (transações brutas
/// do Sympla) em data/dataset/dataset.csv (uma linha por par usuário-espetáculo).
/// Regras: lead time 0–180 dias, uma interação por (usuário, espetáculo, dia de
/// compra), remoção de usuários acima do percentil 99,9 de volume e agregação
/// com ranking = dias distintos de compra (máximo 5).
/// </summary>
public static class Preprocessor
{
    private static readonly string[] DateFormats =
        ["M/d/yy H:mm", "M/d/yyyy H:mm", "M/d/yy HH:mm", "M/d/yyyy HH:mm"];

    public static IReadOnlyList<ProcessedRecord> Preprocess(string rawPath, string outputPath)
    {
        Console.WriteLine($"[1/4] Lendo dados brutos: {rawPath}");
        var raw = ReadRaw(rawPath);
        Console.WriteLine($"      {raw.Count} linhas lidas.");

        Console.WriteLine("[2/4] Limpando e filtrando...");
        var cleaned = Clean(raw);
        Console.WriteLine($"      {cleaned.Count} linhas após limpeza (lead time 0–180 dias).");

        Console.WriteLine("[3/4] Removendo outliers e agregando...");
        var processed = Aggregate(cleaned);
        Console.WriteLine($"      {processed.Count} pares (usuário, espetáculo) únicos.");

        Console.WriteLine($"[4/4] Salvando dataset: {outputPath}");
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        WriteCsv(processed, outputPath);

        return processed;
    }

    // -------------------------------------------------------------------------

    private static List<SymplaRawRecord> ReadRaw(string path)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            BadDataFound = null,
        };
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, config);
        return csv.GetRecords<SymplaRawRecord>().ToList();
    }

    private record CleanedRow(int UserId, string ShowName, DateTime PurchaseDate,
        double Price, string EventType);

    private static List<CleanedRow> Clean(List<SymplaRawRecord> raw)
    {
        var result = new List<CleanedRow>(raw.Count);

        foreach (var r in raw)
        {
            if (!TryParseDate(r.PresentationDateRaw, out var presentationDate)) continue;
            if (!TryParseDate(r.PurchaseDateRaw, out var purchaseDate)) continue;

            double leadDays = (presentationDate - purchaseDate).TotalDays;
            if (leadDays < 0 || leadDays > 180) continue;

            double price = ParsePrice(r.TicketPriceRaw);

            result.Add(new CleanedRow(r.UserId, r.ShowName, purchaseDate, price, r.EventType));
        }

        return result;
    }

    private static List<ProcessedRecord> Aggregate(List<CleanedRow> cleaned)
    {
        // Deduplicar por (UserId, ShowName, PurchaseDate.Date)
        var deduped = cleaned
            .GroupBy(r => (r.UserId, r.ShowName, r.PurchaseDate.Date))
            .Select(g => g.First())
            .ToList();

        // Contar interações por usuário para cálculo do percentil 99.9
        var countByUser = deduped
            .GroupBy(r => r.UserId)
            .ToDictionary(g => g.Key, g => g.Count());

        double threshold = Percentile(countByUser.Values.Select(v => (double)v).ToList(), 99.9);
        var outlierUsers = countByUser
            .Where(kv => kv.Value > threshold)
            .Select(kv => kv.Key)
            .ToHashSet();

        Console.WriteLine($"      Outlier threshold (P99.9): {threshold:F0} interações, " +
                          $"{outlierUsers.Count} usuários removidos.");

        var filtered = deduped.Where(r => !outlierUsers.Contains(r.UserId)).ToList();

        // Agregar por (UserId, ShowName)
        return filtered
            .GroupBy(r => (r.UserId, r.ShowName))
            .Select(g => new ProcessedRecord
            {
                UserId = g.Key.UserId,
                ItemId = g.Key.ShowName,
                Ranking = Math.Min(g.Select(r => r.PurchaseDate.Date).Distinct().Count(), 5),
                ValorMedioIngresso = Math.Round(g.Average(r => r.Price), 2),
                TipoEvento = g.First().EventType,
            })
            .OrderBy(r => r.UserId)
            .ToList();
    }

    private static void WriteCsv(IReadOnlyList<ProcessedRecord> records, string path)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        };
        using var writer = new StreamWriter(path);
        using var csv = new CsvWriter(writer, config);
        csv.WriteRecords(records);
    }

    // -------------------------------------------------------------------------

    private static bool TryParseDate(string raw, out DateTime result)
    {
        return DateTime.TryParseExact(raw.Trim(), DateFormats,
            CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
    }

    private static double ParsePrice(string raw)
    {
        var cleaned = Regex.Replace(raw, @"[^0-9,.\-]", string.Empty)
                           .Replace(",", ".");
        return double.TryParse(cleaned, NumberStyles.Any,
            CultureInfo.InvariantCulture, out var val) ? val : 0.0;
    }

    private static double Percentile(List<double> sorted, double percentile)
    {
        sorted.Sort();
        if (sorted.Count == 0) return 0;
        double idx = percentile / 100.0 * (sorted.Count - 1);
        int lo = (int)idx;
        int hi = Math.Min(lo + 1, sorted.Count - 1);
        return sorted[lo] + (idx - lo) * (sorted[hi] - sorted[lo]);
    }
}
