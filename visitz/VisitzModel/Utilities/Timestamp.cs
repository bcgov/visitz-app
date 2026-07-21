namespace VisitzModel.Utilities;

public class Timestamp
{
    public static DateTimeOffset? ParseDateTimeOffsetNullable(string dateTime, IFormatProvider? formatProvider = null)
    {
        return string.IsNullOrWhiteSpace(dateTime) ? null : DateTimeOffset.Parse(dateTime, formatProvider);
    }

    public static string WriteDateTimeOffset(
        DateTimeOffset? dateTimeOffset,
        string format,
        IFormatProvider? formatProvider = null
    )
    {
        return dateTimeOffset is DateTimeOffset offset ? offset.ToString(format, formatProvider) : "";
    }
}
