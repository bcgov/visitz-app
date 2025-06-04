namespace VisitzModel.Storage;

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

