namespace InferenceService.Data;

public interface IDatasetRepository
{
    /// <summary>All known item_ids from dataset.csv.</summary>
    IReadOnlySet<string> AllItems { get; }

    /// <summary>
    /// Items sorted descending by interaction count — used for cold-start fallback.
    /// Mirrors Python's _COLD_START_ITEMS computed once at module load.
    /// </summary>
    IReadOnlyList<string> ColdStartItems { get; }

    /// <summary>All interactions for the given user.</summary>
    IReadOnlyList<InteractionRecord> GetInteractions(int userId);

    /// <summary>Item IDs the user has already interacted with.</summary>
    IReadOnlySet<string> GetConsumedItems(int userId);

    /// <summary>Returns the ranking the user gave to an item, or null if not found.</summary>
    int? GetRating(int userId, string itemId);

    /// <summary>True if the userId appears in dataset.csv.</summary>
    bool UserExists(int userId);

    /// <summary>All interactions (for evaluation / metrics).</summary>
    IReadOnlyList<InteractionRecord> AllInteractions { get; }

    /// <summary>All user IDs present in dataset.csv.</summary>
    IReadOnlyList<int> GetAllUserIds();
}
