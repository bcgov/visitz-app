namespace VisitzModel.Extensions;

public static class StringExtensions
{
    public static string Format(this string stringToFormat, params object[] args)
    {
        return string.Format(stringToFormat, args);
    }

    //from https://stackoverflow.com/questions/63760445/c-sharp-get-initials-of-displayname

    private static readonly char[] separator = [' ', ','];
    public static string GetInitials(this string text)
    {
        return string.Concat(text
            .Split(separator, StringSplitOptions.RemoveEmptyEntries)
            .Where(split => split.Length >= 1 && char.IsLetter(split[0]))
            .Select(split => char.ToUpper(split[0])));
    }

    public static string FormatAddressPart(this string addressPart, string separator)
    {
        return addressPart?.Length > 0 ? addressPart + separator : string.Empty;
    }

    public static string TruncateEnd(this string text, int length)
    {
        return text[..Math.Min(text.Length, length)];
    }

    public static bool ParseWordTruthiness(this string text)
    {
        return text != null && text.Trim().StartsWith("Y", StringComparison.CurrentCultureIgnoreCase);
    }

	public static bool? ParseEmptyWordTruthiness(this string text)
	{
		if (text == null || text.Trim().Length == 0)
			return null;
		else
			return ParseWordTruthiness(text);
	}
}
