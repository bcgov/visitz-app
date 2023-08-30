using System.Text.RegularExpressions;

namespace Visitz.Extensions;

public static partial class StringEmojiExtensions
{
    [GeneratedRegex("\\p{Cs}")]
    private static partial Regex EmojiSurrogatesRegex();

    public static bool ContainsEmojiSurrogates(this string text)
    {
        return EmojiSurrogatesRegex().IsMatch(text);
    }
}
