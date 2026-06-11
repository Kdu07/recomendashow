using CsvHelper.Configuration.Attributes;

namespace AventuraML.Models;

/// <summary>
/// Linha bruta de data/raw/database_sympla.csv (uma transação de compra de ingresso).
/// Datas e valor ficam como string crua: o parse acontece no Preprocessor.
/// </summary>
public sealed class SymplaRawRecord
{
    [Name("ID COMPRADOR.1.ID USUÁRIO")]
    public int UserId { get; init; }

    [Name("NOME ESPETÁCULO")]
    public string ShowName { get; init; } = string.Empty;

    [Name("DATA DA APRESENTAÇÃO")]
    public string PresentationDateRaw { get; init; } = string.Empty;

    [Name("DATA DA COMPRA")]
    public string PurchaseDateRaw { get; init; } = string.Empty;

    [Name("VALOR DO INGRESSO")]
    public string TicketPriceRaw { get; init; } = string.Empty;

    [Name("TIPO")]
    public string EventType { get; init; } = string.Empty;
}
