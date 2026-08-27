using CommunityToolkit.Mvvm.ComponentModel;

namespace Visitz.Views.FormControls;

public partial class InfoItem : ObservableObject
{
    [ObservableProperty]
    public partial string IconGlyph { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string FontFamily { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Label { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Value { get; set; } = string.Empty;

    [ObservableProperty]
    public partial Color? ValueColor { get; set; }

    [ObservableProperty]
    public partial TextDecorations ValueTextDecorations { get; set; } = TextDecorations.None;

    public Action? TapAction { get; set; }
}
