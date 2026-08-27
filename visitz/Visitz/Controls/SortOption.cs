namespace Visitz.Controls;

public class SortOption<TItem>(
    string label,
    IComparer<TItem> comparer,
    string startIconGlyph = "",
    string startGlyphFontFamily = "",
    string endIconGlyph = "",
    string endGlyphFontFamily = ""
) : IOption
{
    public string Text { get; } = label;

    public IComparer<TItem> Comparer { get; } = comparer;

    public bool Selected { get; set; }

    public string StartIconGlyph { get; set; } = startIconGlyph;

    public string StartGlyphFontFamily { get; set; } = startGlyphFontFamily;

    public string EndIconGlyph { get; set; } = endIconGlyph;

    public string EndGlyphFontFamily { get; set; } = endGlyphFontFamily;
}
