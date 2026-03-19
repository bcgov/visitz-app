using VisitzModel.Utilities;

namespace VisitzModelTest.Utilities;

public class ThatDayTests
{
    static readonly string ArbitraryNowDateTime = "2024-03-14 11:53 AM";

    static readonly string ArbitrarySameDayTime = "8:56 AM";
    static readonly string ArbitrarySameDayDateTime = "2024-03-14 " + ArbitrarySameDayTime;

    [Fact]
    public void ThrowsWhenComparingFutureDateTime()
    {
        var then = DateTime.Parse(ArbitraryNowDateTime);
        var now = DateTime.Parse(ArbitrarySameDayDateTime);

        Assert.Throws<ArgumentException>(() => new ThatDay(then, now));
    }

    [Fact]
    public void DateTimeIsToday()
    {
        var then = DateTime.Parse(ArbitrarySameDayDateTime);
        var now = DateTime.Parse(ArbitraryNowDateTime);

        var thatDay = new ThatDay(then, now);

        Assert.Equal(ArbitrarySameDayTime, thatDay.ToString());
    }

    static readonly string ArbitraryLastNightTime = "8:56 PM";
    static readonly string ArbitraryLastNightDateTime = "2024-03-13 " + ArbitraryLastNightTime;

    [Fact]
    public void DateTimeLastNightIsYesterday()
    {
        var then = DateTime.Parse(ArbitraryLastNightDateTime);
        var now = DateTime.Parse(ArbitraryNowDateTime);

        var thatDay = new ThatDay(then, now);
        string todayTime = ArbitraryLastNightTime + " Yesterday";

        Assert.Equal(todayTime, thatDay.ToString());
    }

    static readonly string ArbitrarySameWeekTime = "6:21 AM";
    static readonly string ArbitrarySameWeekDateTime = "2024-03-12 " + ArbitrarySameWeekTime;

    [Fact]
    public void DateTimeIsTuesdayThisWeek()
    {
        var then = DateTime.Parse(ArbitrarySameWeekDateTime);
        var now = DateTime.Parse(ArbitraryNowDateTime);

        var thatDay = new ThatDay(then, now);
        string tuesdayTime = ArbitrarySameWeekTime + " Tuesday";

        Assert.Equal(tuesdayTime, thatDay.ToString());
    }

    static readonly string Arbitrary6DaysAgoTime = "1:33 PM";
    static readonly string Arbitrary6DaysAgoDateTime = "2024-03-8 " + Arbitrary6DaysAgoTime;

    [Fact]
    public void DateTimeIsSixDaysAgoLastWeek()
    {
        var then = DateTime.Parse(Arbitrary6DaysAgoDateTime);
        var now = DateTime.Parse(ArbitraryNowDateTime);

        var thatDay = new ThatDay(then, now);
        string lastWeekDateTime = Arbitrary6DaysAgoTime + " Last Friday";

        Assert.Equal(lastWeekDateTime, thatDay.ToString());
    }

    static readonly string ArbitraryLastWeekTime = "1:33 PM";
    static readonly string ArbitraryLastWeekDateTime = "2024-03-7 " + ArbitraryLastWeekTime;

    [Fact]
    public void DateTimeIsLastWeek()
    {
        var then = DateTime.Parse(ArbitraryLastWeekDateTime);
        var now = DateTime.Parse(ArbitraryNowDateTime);

        var thatDay = new ThatDay(then, now);
        string lastWeekDateTime = ArbitraryLastWeekTime + " Mar 07";

        Assert.Equal(lastWeekDateTime, thatDay.ToString());
    }

    static readonly string ArbitraryLastMonthTime = "12:17 PM";
    static readonly string ArbitraryLastMonthDateTime = "2024-02-2 " + ArbitraryLastMonthTime;

    [Fact]
    public void DateTimeIsLastMonth()
    {
        var then = DateTime.Parse(ArbitraryLastMonthDateTime);
        var now = DateTime.Parse(ArbitraryNowDateTime);

        var thatDay = new ThatDay(then, now);
        string lastMonthDateTime = ArbitraryLastMonthTime + " Feb 02";

        Assert.Equal(lastMonthDateTime, thatDay.ToString());
    }

    static readonly string ArbitraryLastYearTime = "12:17 PM";
    static readonly string ArbitraryLastYearDateTime = "2023-08-28 " + ArbitraryLastYearTime;

    [Fact]
    public void DateTimeIsLastYear()
    {
        var then = DateTime.Parse(ArbitraryLastYearDateTime);
        var now = DateTime.Parse(ArbitraryNowDateTime);

        var thatDay = new ThatDay(then, now);
        string lastYearDateTime = ArbitraryLastYearTime + " Aug 28 2023";

        Assert.Equal(lastYearDateTime, thatDay.ToString());
    }

    static readonly string ArbitrarySeveralYearsAgoTime = "9:01 PM";
    static readonly string ArbitrarySeveralYearsAgoDateTime = "2015-08-28 " + ArbitrarySeveralYearsAgoTime;

    [Fact]
    public void DateTimeIsSeveralYearsAgo()
    {
        var then = DateTime.Parse(ArbitrarySeveralYearsAgoDateTime);
        var now = DateTime.Parse(ArbitraryNowDateTime);

        var thatDay = new ThatDay(then, now);
        string severalYearsAgoDateTime = ArbitrarySeveralYearsAgoTime + " Aug 28 2015";

        Assert.Equal(severalYearsAgoDateTime, thatDay.ToString());
    }
}
