using System.Text.Json;

namespace visitzApi.Extensions
{
    internal static class JsonElementFirstsExtensions
    {
        public static JsonElement FirstProperty(this JsonElement element)
        {
            return element.GetProperty(element.EnumerateObject().First().Name);
        }

        public static JsonElement FirstArrayElement(this JsonElement element)
        {
            return element.EnumerateArray().First();
        }
    }
}
