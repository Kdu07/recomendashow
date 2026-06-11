using CsvHelper.Configuration.Attributes;

namespace AventuraML.Models;

/// <summary>
/// Linha de saída de data/dataset/dataset.csv gerada pelo Preprocessor.
/// Modelo de escrita; a leitura usa CsvRatingRow.
/// </summary>
public sealed class ProcessedRecord
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
