namespace VisitzModel.Utilities;

public class Timestamp
{
    public static DateTimeOffset? ParseDateTimeOffsetNullable(string dateTime)
    {
        return string.IsNullOrWhiteSpace(dateTime) ? null : DateTimeOffset.Parse(dateTime);
    }

    public static string WriteDateTimeOffset(DateTimeOffset? dateTimeOffset, string format)
    {
        return dateTimeOffset is DateTimeOffset offset ? offset.ToString(format) : "";
    }
}
