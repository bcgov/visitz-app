using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VisitzApi.Json.Converters;

public class SiebelDateTimeConverter : JsonConverter<DateTimeOffset>
{
    public static string SiebelDateFormat = "MM/dd/yyyy hh:mm:ss";

    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Parse(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(SiebelDateFormat, CultureInfo.InvariantCulture));
    }

    public static DateTimeOffset Parse(string? dateString)
    {
        if (
            DateTimeOffset.TryParseExact(
                dateString,
                SiebelDateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var date1
            )
        )
            return date1;
        else if (DateTimeOffset.TryParse(dateString, CultureInfo.InvariantCulture, out var date2))
            return date2;
        else
            throw new FormatException($"Date '{dateString}' not in a recognized format");
    }
}
