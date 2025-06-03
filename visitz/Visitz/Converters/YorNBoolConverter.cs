using System.Globalization;
using VisitzModel.Extensions;

namespace Visitz.Converters;

public class YorNBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return false;

        string stringValue = value.ToString().GetInitials();
        return stringValue == "Y";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return boolValue ? "Y" : "N";

        return "N";
    }
}
