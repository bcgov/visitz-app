using System.Globalization;
using VisitzModel.Extensions;

namespace Visitz.Converters;

internal class TruthyStringToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string str)
            return str.ParseWordTruthiness();
        else
            return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b;
        else
            return false;
    }
}
