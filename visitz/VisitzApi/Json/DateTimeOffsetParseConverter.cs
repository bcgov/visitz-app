using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VisitzApi.Json;

public class DateTimeOffsetParseConverter(string dateFormat) : JsonConverter<DateTimeOffset>
{
    public string DateFormat { get; } = dateFormat;

    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTimeOffset.Parse(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
    }
}
