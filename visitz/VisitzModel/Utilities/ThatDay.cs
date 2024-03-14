using System.Globalization;

namespace VisitzModel.Utilities;

public class ThatDay(DateTimeOffset then, DateTimeOffset comparisonFrom)
{
	static readonly string TimeOnlyFormat = "hh:mm tt";
	static readonly string TimeWeekdayFormat = "hh:mm tt dddd";
	static readonly string TimeMonthDayFormat = "hh:mm tt MMM dd";
	static readonly string TimeMonthDayYearFormat = "hh:mm tt MMM dd yyyy";

	public DateTimeOffset Then { get; } = then;

	public DateTimeOffset From { get; } = comparisonFrom;

	private string GetThatDayString()
	{
		var timeDiff = From - Then;
		string output;

		if (From.Date.Equals(Then.Date))
			output = Then.ToString(TimeOnlyFormat, CultureInfo.InvariantCulture);
		else if (Math.Ceiling(timeDiff.TotalDays) < 7)
			output = Then.ToString(TimeWeekdayFormat, CultureInfo.InvariantCulture);
		else if (From.Year - Then.Year < 1)
			output = Then.ToString(TimeMonthDayFormat, CultureInfo.InvariantCulture);
		else
			output = Then.ToString(TimeMonthDayYearFormat, CultureInfo.InvariantCulture);

		return output.TrimStart('0');
	}

	public override string ToString()
	{
		return GetThatDayString();
	}
}
