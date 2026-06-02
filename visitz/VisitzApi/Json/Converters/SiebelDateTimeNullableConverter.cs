using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VisitzApi.Json.Converters;

internal class SiebelDateTimeNullableConverter : JsonConverter<DateTimeOffset?>
{
    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString() is string dateString && dateString.Length > 0
            ? SiebelDateTimeConverter.Parse(dateString)
            : null;
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (value is DateTimeOffset dto)
            writer.WriteStringValue(
                dto.ToString(SiebelDateTimeConverter.SiebelDateFormat, CultureInfo.InvariantCulture)
            );
        else
            writer.WriteStringValue(string.Empty);
    }
}
