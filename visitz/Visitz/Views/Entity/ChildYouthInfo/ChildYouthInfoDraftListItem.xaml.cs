using System.Windows.Input;
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

    [BindableProperty]
    public partial ICommand? PrimaryCommand { get; set; }

    [BindableProperty]
    public partial ICommand? EditCommand { get; set; }

    [BindableProperty]
    public partial ICommand? DiscardCommand { get; set; }

    [BindableProperty]
    public partial ICommand? PublishCommand { get; set; }

    [BindableProperty]
    public partial object CommandParameter { get; set; }

    [BindableProperty]
    public partial bool CanPublish { get; set; }

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

        BorderTapRecognizer.SetBinding(
            TapGestureRecognizer.CommandProperty,
            static (ChildYouthInfoDraftListItem item) => item.PrimaryCommand,
            source: this
        );
        BorderTapRecognizer.SetBinding(
            TapGestureRecognizer.CommandParameterProperty,
            static (ChildYouthInfoDraftListItem item) => item.CommandParameter,
            source: this
        );

        DiscardButton.SetBinding(
            Button.CommandProperty,
            static (ChildYouthInfoDraftListItem item) => item.DiscardCommand,
            source: this
        );
        DiscardButton.SetBinding(
            Button.CommandParameterProperty,
            static (ChildYouthInfoDraftListItem item) => item.CommandParameter,
            source: this
        );

        EditButton.SetBinding(
            Button.CommandProperty,
            static (ChildYouthInfoDraftListItem item) => item.EditCommand,
            source: this
        );
        EditButton.SetBinding(
            Button.CommandParameterProperty,
            static (ChildYouthInfoDraftListItem item) => item.CommandParameter,
            source: this
        );

        PublishButton.SetBinding(
            Button.CommandProperty,
            static (ChildYouthInfoDraftListItem item) => item.PublishCommand,
            source: this
        );
        PublishButton.SetBinding(
            Button.CommandParameterProperty,
            static (ChildYouthInfoDraftListItem item) => item.CommandParameter,
            source: this
        );
        PublishButton.SetBinding(
            IsEnabledProperty,
            static (ChildYouthInfoDraftListItem item) => item.CanPublish,
            source: this
        );
    }
}
