using System.Text.RegularExpressions;

namespace VisitzModel.Extensions;

public static partial class StringUnicodeExtensions
{
    /// <summary>
    /// Surrogates (Emoji).
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"\p{Cs}")]
    private static partial Regex UnicodeSurrogatesRegex();

    [GeneratedRegex(@"\p{So}")]
    private static partial Regex UnicodeOtherSymbolsRegex();

    [GeneratedRegex(@"\p{Cs}|\p{So}")]
    private static partial Regex UnicodeSurrogatesAndOtherSymbolsRegex();

    public static bool ContainsUnicodeSurrogates(this string text)
    {
        return UnicodeSurrogatesRegex().IsMatch(text);
    }

    public static bool ContainsUnicodeOtherSymbols(this string text)
    {
        return UnicodeOtherSymbolsRegex().IsMatch(text);
    }

    public static bool ContainsUnicodeSurrogatesAndOtherSymbols(this string text)
    {
        return UnicodeSurrogatesAndOtherSymbolsRegex().IsMatch(text);
    }
}
