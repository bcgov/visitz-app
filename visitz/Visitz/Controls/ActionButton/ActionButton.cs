using CommunityToolkit.Maui;
using Syncfusion.Maui.Toolkit.Popup;
using Visitz.Resources.Styles;
using Visitz.Resources.Templates;

namespace Visitz.Controls.ActionButton;

public partial class ActionButton : Button
{
    readonly SfPopup _popup;

    [BindableProperty(PropertyChangedMethodName = nameof(OnMenuOptionsChanged))]
    public partial IEnumerable<ActionButtonOption> MenuOptions { get; set; } = [];

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

        Clicked += FloatingActionButton_Clicked;
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
}
