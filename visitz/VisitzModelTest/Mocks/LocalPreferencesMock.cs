namespace VisitzModelTest.Mocks;

internal class LocalPreferencesMock : IPreferences
{
    private Dictionary<string, object> Pairs { get; } = [];

    public void Clear(string? sharedName = null)
    {
        Pairs.Clear();
    }

    public bool ContainsKey(string key, string? sharedName = null)
    {
        return Pairs.ContainsKey(key);
    }

    public T Get<T>(string key, T defaultValue, string? sharedName = null)
    {
        return ContainsKey(key) ? (T)Pairs[key] : defaultValue;
    }

    public void Remove(string key, string? sharedName = null)
    {
        Pairs.Remove(key);
    }

    public void Set<T>(string key, T value, string? sharedName = null)
    {
#pragma warning disable CS8601 // Possible null reference assignment.
        Pairs[key] = value;
#pragma warning restore CS8601 // Possible null reference assignment.
    }
}
