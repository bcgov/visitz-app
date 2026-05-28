namespace Visitz.Extensions;

public static class ResourceDictionaryExtension
{
    /// <summary>
    /// Access the custom colors defined in the ResourceDictionary.
    /// </summary>
    public static Color TryGetColor(this ResourceDictionary resources, string key, Color fallback)
    {
        resources.TryGetValue(key, out var color);
        return color as Color ?? fallback;
    }

    public static double? TryGetDimension(this ResourceDictionary resources, string key, double? fallback = null)
    {
        resources.TryGetValue(key, out var dimension);
        return dimension is double dim ? dim : fallback ?? null;
    }

    public static Shadow TryGetShadow(this ResourceDictionary resources, string key, Shadow fallback = null)
    {
        resources.TryGetValue(key, out var shadow);
        return shadow as Shadow ?? fallback;
    }
}
