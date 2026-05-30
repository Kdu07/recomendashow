namespace BackEnd.Aventura.Services;

public sealed class PreferenceService : IPreferenceService
{
    private readonly Dictionary<int, List<string>> _preferences = new();
    private readonly Lock _lock = new();

    public void SetPreferences(int userId, IEnumerable<string> tiposEvento)
    {
        lock (_lock)
            _preferences[userId] = tiposEvento.ToList();
    }

    public IReadOnlyList<string> GetPreferences(int userId)
    {
        lock (_lock)
            return _preferences.TryGetValue(userId, out var prefs) ? prefs : [];
    }
}
