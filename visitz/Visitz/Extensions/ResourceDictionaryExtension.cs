using System;
namespace Visitz.Extensions
{
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
    }
}

