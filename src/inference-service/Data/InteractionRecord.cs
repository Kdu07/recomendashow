using CsvHelper.Configuration.Attributes;

namespace InferenceService.Data;

public sealed class InteractionRecord
{
    [Name("user_id")]
    public int UserId { get; init; }

    [Name("item_id")]
    public string ItemId { get; init; } = string.Empty;

    [Name("ranking")]
    public int Ranking { get; init; }

    [Name("valor_medio_ingresso")]
    public double ValorMedioIngresso { get; init; }

    [Name("tipo_evento")]
    public string TipoEvento { get; init; } = string.Empty;
}
