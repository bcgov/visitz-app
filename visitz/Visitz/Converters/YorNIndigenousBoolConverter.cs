using System.Globalization;

namespace Visitz.Converters;

public class YorNIndigenousBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return false;

        string stringValue = value.ToString().Trim().ToUpper();
        return stringValue == "Y";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return boolValue ? "Y" : "N";

        return "N";
    }
}
