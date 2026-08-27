using System.Text.Json;

namespace VisitzApi.Extensions;

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

    public static JsonElement? FindFirstByName(this JsonElement startElement, string propertyName)
    {
        if (startElement.ValueKind == JsonValueKind.Object)
        {
            if (startElement.TryGetProperty(propertyName, out JsonElement found))
                return found;
            else if (FindFirstInObject(startElement, propertyName) is JsonElement foundDeeper)
                return foundDeeper;
        }
        else if (startElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var element in startElement.EnumerateArray())
                if (FindFirstByName(element, propertyName) is JsonElement found)
                    return found;
        }

        return null;
    }

    static JsonElement? FindFirstInObject(JsonElement startElement, string propertyName)
    {
        foreach (var property in startElement.EnumerateObject())
        {
            var element = startElement.GetProperty(property.Name);
            var kind = element.ValueKind;

            if (kind == JsonValueKind.Object || kind == JsonValueKind.Array)
                if (FindFirstByName(element, propertyName) is JsonElement foundElement)
                    return foundElement;
        }

        return null;
    }

    public static JsonElement? FindFirstByAnyName(this JsonElement startElement, params string[] propertyName)
    {
        foreach (string name in propertyName)
        {
            if (startElement.FindFirstByName(name) is JsonElement found)
                return found;
        }

        return null;
    }
}
