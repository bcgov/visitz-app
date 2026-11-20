using CommunityToolkit.Maui;

namespace Visitz.Views.Entity.ChildYouthInfo;

public partial class ChildYouthInfoDraftListItem : ContentView
{
    [BindableProperty]
    public partial string Title { get; set; } = string.Empty;

    [BindableProperty]
    public partial string Subtitle { get; set; } = string.Empty;

    [BindableProperty]
    public partial FormattedString? SubtitleFormattedText { get; set; }

    [BindableProperty]
    public partial string IconGlyph { get; set; } = string.Empty;

    [BindableProperty]
    public partial string IconFontFamily { get; set; } = string.Empty;

    public ChildYouthInfoDraftListItem()
    {
        InitializeComponent();

        TitleLabel.SetBinding(
            Label.TextProperty,
            static (ChildYouthInfoDraftListItem item) => item.Title,
            source: this
        );
        SubtitleLabel.SetBinding(
            Label.TextProperty,
            static (ChildYouthInfoDraftListItem item) => item.Subtitle,
            source: this
        );
        SubtitleLabel.SetBinding(
            Label.FormattedTextProperty,
            static (ChildYouthInfoDraftListItem item) => item.SubtitleFormattedText,
            source: this
        );
        IconLabel.SetBinding(
            Label.TextProperty,
            static (ChildYouthInfoDraftListItem item) => item.IconGlyph,
            source: this
        );
        IconLabel.SetBinding(
            Label.FontFamilyProperty,
            static (ChildYouthInfoDraftListItem item) => item.IconFontFamily,
            source: this
        );
    }
}
