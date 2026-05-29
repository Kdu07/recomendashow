using CsvHelper.Configuration.Attributes;

namespace AventuraML.Models;

public sealed class CsvRatingRow
{
    [Name("user_id")]
    public int UserId { get; init; }

    [Name("item_id")]
    public string ItemId { get; init; } = "";

    [Name("ranking")]
    public float Ranking { get; init; }

    [Name("valor_medio_ingresso")]
    public float ValorMedioIngresso { get; init; }

    [Name("tipo_evento")]
    public string TipoEvento { get; init; } = "";
}
