using CsvHelper.Configuration.Attributes;

namespace TrainingService.Data;

public sealed class RawRecord
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
