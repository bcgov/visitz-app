using System.Globalization;
using VisitzModel.Utilities;

namespace Visitz.Converters;

internal class DateTimeOffsetThatDayConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
            return null;

        var then = (DateTimeOffset)value;
        if (then == DateTimeOffset.MinValue)
            return "N/A";

        var now = DateTimeOffset.Now;
        return value == null ? null : new ThatDay(then.LocalDateTime, now.LocalDateTime).ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null; // Not converting back, only displaying
    }
}
