namespace BackEnd.Aventura.Services;

public interface IPreferenceService
{
    void SetPreferences(int userId, IEnumerable<string> tiposEvento);
    IReadOnlyList<string> GetPreferences(int userId);
}
