using VisitzModel.Events;
using VisitzModel.Extensions;

namespace VisitzModel.Storage;

public class LastUpdatedPrefs(IPreferences prefs)
{
    static readonly string KeyPrefix = "LastUpdatedTimestamp.";

    IPreferences Preferences { get; set; } = prefs;

    public event EventHandler<LastUpdatedChangedEventArgs>? LastUpdatedChanged;

    public void Set(string key, DateTime value)
    {
        var fullKey = KeyPrefix + key;

        Preferences.Set(fullKey, value);
        LastUpdatedChanged?.Invoke(this, new LastUpdatedChangedEventArgs(key, value));
    }

    public void SetLocalNow(string key)
    {
        Set(key, DateTimeExtensions.LocalNow);
    }

    public void SetUtcNow(string key)
    {
        Set(key, DateTime.UtcNow);
    }

    public DateTime? Get(string key)
    {
        var fullKey = KeyPrefix + key;
        return Preferences.ContainsKey(fullKey) ? Get(key, default) : null;
    }

    public DateTime Get(string key, DateTime defaultValue)
    {
        var fullKey = KeyPrefix + key;
        return Preferences.Get(fullKey, defaultValue);
    }
}
