namespace Visitz.Controls;

public interface IOption
{
    bool Selected { get; set; }

    string Text { get; }

    string StartIconGlyph { get; set; }

    string StartGlyphFontFamily { get; set; }

    string EndIconGlyph { get; set; }

    string EndGlyphFontFamily { get; set; }
}
