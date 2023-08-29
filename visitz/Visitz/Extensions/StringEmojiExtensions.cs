using System.Text.RegularExpressions;

namespace Visitz.Extensions;

public static partial class StringEmojiExtensions
{
    [GeneratedRegex("\\p{Cs}")]
    private static partial Regex EmojiRegex();

    public static bool ContainsEmoji(this string text)
    {
        return EmojiRegex().IsMatch(text);
    }
}
