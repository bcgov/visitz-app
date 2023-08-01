namespace Visitz.Extensions;

public static class StringExtensions
{
    public static string Format(this string stringToFormat, params object[] args)
    {
        return string.Format(stringToFormat, args);
    }
}
