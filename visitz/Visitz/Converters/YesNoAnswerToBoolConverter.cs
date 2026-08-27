using System.Globalization;
using Visitz.Views.FormControls;

namespace Visitz.Converters;

public class YesNoAnswerToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value != null)
            return ((bool)value) ? YesNoAnswer.Yes : YesNoAnswer.No;

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value == null ? null : ((YesNoAnswer)value) == YesNoAnswer.Yes;
    }
}
