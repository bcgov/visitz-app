using System.Globalization;
using Visitz.Views.FormControls;

namespace Visitz.Converters;

public class YesNoAnswerToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return null;
        else
            return ((bool)value) ? YesNoAnswer.Yes : YesNoAnswer.No;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return null;
        else
            return ((YesNoAnswer)value) == YesNoAnswer.Yes;
    }
}
