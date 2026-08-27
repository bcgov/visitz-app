using System.Globalization;
using VisitzModel.Formats;

namespace Visitz.Converters;

internal class DateTimeOffsetShortTimestampConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            return null;

        var timestamp = (DateTimeOffset)value;

        return timestamp.ToString(IcmDateFormats.BasicTimestampShort, CultureInfo.InvariantCulture);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null; // Not converting back, only displaying
    }
}
