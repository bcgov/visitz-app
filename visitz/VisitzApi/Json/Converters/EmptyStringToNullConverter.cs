using System.Text.Json;
using System.Text.Json.Serialization;

namespace VisitzApi.Json.Converters;

internal class EmptyStringToNullConverter : JsonConverter<string>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType == JsonTokenType.Null ? null : reader.GetString();
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        if (string.IsNullOrEmpty(value))
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value);
    }
}
