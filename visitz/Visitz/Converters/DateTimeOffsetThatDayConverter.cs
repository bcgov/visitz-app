using System.Globalization;
using VisitzModel.Utilities;

namespace Visitz.Converters;

internal class DateTimeOffsetThatDayConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		var then = ((DateTimeOffset)value).LocalDateTime;
		var now = DateTimeOffset.Now.LocalDateTime;
		return value == null ? null : new ThatDay(then, now).ToString();
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null; // Not converting back, only displaying
	}
}
