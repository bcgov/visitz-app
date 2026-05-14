namespace Visitz.Controls;

#nullable enable

public class FilterOption<TItem>(
    string label,
    Func<TItem, bool> wherePredicate,
    string startIconGlyph = "",
    string startGlyphFontFamily = "",
    string endIconGlyph = "",
    string endGlyphFontFamily = ""
) : IOption, IEquatable<FilterOption<TItem>>, IComparable<FilterOption<TItem>>
{
    public Func<TItem, bool> WherePredicate { get; } = wherePredicate;

    public bool Selected { get; set; }

    public string Text { get; } = label;

    public string StartIconGlyph { get; set; } = startIconGlyph;

    public string StartGlyphFontFamily { get; set; } = startGlyphFontFamily;

    public string EndIconGlyph { get; set; } = endIconGlyph;

    public string EndGlyphFontFamily { get; set; } = endGlyphFontFamily;

    public bool Equals(FilterOption<TItem>? other)
    {
        return ReferenceEquals(this, other)
            || (
                Text.Equals(other?.Text, StringComparison.InvariantCultureIgnoreCase)
                && WherePredicate == other?.WherePredicate
            );
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(WherePredicate, Text);
    }

    public override bool Equals(object? obj)
    {
        return obj is FilterOption<TItem> filter ? Equals(filter) : Equals(this, obj);
    }

    public int CompareTo(FilterOption<TItem>? other)
    {
        return Text.CompareTo(other?.Text);
    }
}
