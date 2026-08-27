namespace VisitzModel.Storage;

/// <summary>
/// <para>Stores which attachments the user wants the app to ignore when
/// refreshing data.</para>
///
/// <para>FIXME: When feasible, this class should be replaced by using
/// BoLocalState instead—or when a new DB system is implemented.</para>
/// </summary>
/// <param name="prefs"></param>
public class UserIgnoredContentPrefs(IPreferences prefs)
{
    private const string IgnoredContentKeyPrefix = "UserIgnoredContent";

    IPreferences Preferences { get; } = prefs;

    public void SetUserIgnoredContent(string key, bool value)
    {
        var fullKey = IgnoredContentKeyPrefix + key;

        Preferences.Set(fullKey, value);
    }

    public bool? GetUserIgnoredContent(string key)
    {
        var fullKey = IgnoredContentKeyPrefix + key;
        return Preferences.ContainsKey(fullKey) ? Preferences.Get(fullKey, false) : null;
    }

    public void RemoveUserIgnoredContent(string key)
    {
        var fullKey = IgnoredContentKeyPrefix + key;
        Preferences.Remove(fullKey);
    }
}
