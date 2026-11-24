using CommunityToolkit.Maui;
using Syncfusion.Maui.Toolkit.Popup;
using Visitz.FontIcons;
using Visitz.Resources.Styles;
using Visitz.Resources.Templates;

namespace Visitz.Controls.ActionButton;

public partial class ActionButton : Button
{
    static readonly ImageSource s_closeIconSource = new FontImageSource()
    {
        FontFamily = MaterialIcons.RoundedUnfilled.FontFamily,
        Glyph = MaterialIcons.Close,
        Size = VisitzDimensions.ButtonFontImageSourceSize,
        Color = Colors.White,
    };

    readonly SfPopup _popup;

    public new string? Text
    {
        get => base.Text;
        protected set => base.Text = value;
    }

    public new ImageSource? ImageSource
    {
        get => base.ImageSource;
        protected set => base.ImageSource = value;
    }

    [BindableProperty(PropertyChangedMethodName = nameof(OnMenuOptionsChanged))]
    public partial IEnumerable<ActionButtonOption> MenuOptions { get; set; } = [];

    [BindableProperty(PropertyChangedMethodName = nameof(OnRequestedTextChanged))]
    public partial string RequestedText { get; set; } = string.Empty;

    [BindableProperty(PropertyChangedMethodName = nameof(OnRequestedImageSourceChanged))]
    public partial ImageSource? RequestedImageSource { get; set; }

    public ActionButton()
    {
        _popup = new()
        {
            AnimationDuration = 50,
            AutoSizeMode = PopupAutoSizeMode.Both,
            BindingContext = this,
            ContentTemplate = VisitzDataTemplates.ActionButtonMenuDataTemplate,
            Margin = new Thickness(0, VisitzDimensions.DefaultMargin),
            ShowHeader = false,
            ShowOverlayAlways = false,
            StaysOpen = true,
            PopupStyle = new() { PopupBackground = Colors.Transparent, CornerRadius = 0 },
        };

        _popup.Opened += Menu_Opened;
        _popup.Closed += Menu_Closed;

        Clicked += FloatingActionButton_Clicked;
    }

    private void Menu_Opened(object? sender, EventArgs e)
    {
        Text = string.Empty;
        ImageSource = s_closeIconSource;
    }

    private void Menu_Closed(object? sender, EventArgs e)
    {
        Text = RequestedText;
        ImageSource = RequestedImageSource;
    }

    void FloatingActionButton_Clicked(object? sender, EventArgs e)
    {
        if (_popup.IsOpen)
            _popup.Dismiss();
        else
            _popup.ShowRelativeToView(
                relativeView: this,
                PopupRelativePosition.AlignTop,
                absoluteY: -VisitzDimensions.DefaultMargin
            );
    }

    void Option_Selected(object? sender, EventArgs e)
    {
        _popup.Dismiss();
    }

    static void OnMenuOptionsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not ActionButton fab)
            return;

        if (oldValue is IEnumerable<ActionButtonOption> oldOptions)
        {
            foreach (ActionButtonOption option in oldOptions)
                option.Selected -= fab.Option_Selected;
        }

        if (newValue is IEnumerable<ActionButtonOption> newOptions)
        {
            foreach (ActionButtonOption option in newOptions)
                option.Selected += fab.Option_Selected;
        }
    }

    static void OnRequestedTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not ActionButton fab)
            return;

        if (!fab._popup.IsOpen)
            fab.Text = newValue as string;
    }

    static void OnRequestedImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not ActionButton fab)
            return;

        if (!fab._popup.IsOpen)
            fab.ImageSource = newValue as ImageSource;
    }
}
