using System.Globalization;
using VisitzModel.Formats;

namespace Visitz.Converters;

internal class DateTimeShortTimestampConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
            return null;

        var timestamp = (DateTime)value;

        return timestamp.ToString(IcmDateFormats.BasicTimestampShort, CultureInfo.InvariantCulture);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null; // Not converting back, only displaying
    }
}
