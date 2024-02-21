using System.Globalization;
using Visitz.Views.FormControls;

namespace Visitz.Converters;

public class YesNoAnswerToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value == null
            ? null
            : ((bool)value) ? YesNoAnswer.Yes : YesNoAnswer.No;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value == null
            ? null
            : ((YesNoAnswer)value) == YesNoAnswer.Yes;
    }
}
