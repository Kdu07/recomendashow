using System.Globalization;
using AventuraML.Models;
using CsvHelper;
using CsvHelper.Configuration;

namespace AventuraML;

public static class DataPipeline
{
    public static List<CsvRatingRow> LoadRaw(string csvPath)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            BadDataFound = null,
        };

        using var reader = new StreamReader(csvPath);
        using var csv = new CsvReader(reader, config);

        return csv.GetRecords<CsvRatingRow>()
            .Select(r => new CsvRatingRow
            {
                UserId = r.UserId,
                ItemId = r.ItemId.Trim(),
                Ranking = r.Ranking,
                ValorMedioIngresso = r.ValorMedioIngresso,
                TipoEvento = r.TipoEvento.Trim(),
            })
            .ToList();
    }

    public static List<RatingData> Load(string csvPath) =>
        LoadRaw(csvPath).Select(r => new RatingData
        {
            UserId = (uint)r.UserId,
            ItemId = r.ItemId,
            Rating = r.Ranking,
        }).ToList();
}
