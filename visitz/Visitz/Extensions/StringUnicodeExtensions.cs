using System.Text.RegularExpressions;

namespace Visitz.Extensions;

public static partial class StringUnicodeExtensions
{
    /// <summary>
    /// Surrogates (Emoji).
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex("\\p{Cs}")]
    private static partial Regex UnicodeSurrogatesRegex();

    public static bool ContainsUnicodeSurrogates(this string text)
    {
        return UnicodeSurrogatesRegex().IsMatch(text);
    }
}
