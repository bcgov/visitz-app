namespace VisitzModel.Extensions;

public static class DateTimeExtensions
{
#pragma warning disable SS002 // DateTime.Now was referenced
    public static DateTime LocalNow => DateTime.Now;
#pragma warning restore SS002 // DateTime.Now was referenced
}
