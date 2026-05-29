namespace AventuraML.Models;

public sealed class RatingData
{
    public uint UserId { get; init; }
    public string ItemId { get; init; } = "";
    public float Rating { get; init; }
}
