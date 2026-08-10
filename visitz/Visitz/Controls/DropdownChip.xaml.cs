using CommunityToolkit.Maui;
using Syncfusion.Maui.Toolkit.Popup;
using VisitzModel.Extensions;

namespace Visitz.Controls;

public partial class DropdownChip : ContentView
{
    const int MaxTextLength = 15;

    [BindableProperty]
    public partial string Text { get; set; } = string.Empty;

    [BindableProperty(PropertyChangedMethodName = nameof(OnPlaceholderChanged))]
    public partial string Placeholder { get; set; } = string.Empty;

    [BindableProperty]
    public partial IEnumerable<IOption> Items { get; set; } = [];

    [BindableProperty(
        DefaultBindingMode = BindingMode.TwoWay,
        PropertyChangedMethodName = nameof(OnSelectedItemChanged)
    )]
    public partial IOption? SelectedOption { get; set; }

    [BindableProperty]
    public partial bool StickySelection { get; set; }

    static void OnPlaceholderChanged(BindableObject bindable, object _, object __)
    {
        if (bindable is DropdownChip view)
            view.ApplySelectionText();
    }

    static void OnSelectedItemChanged(BindableObject bindable, object _, object __)
    {
        if (bindable is DropdownChip view)
        {
            view.ApplySelectionText();
            view.Popup.Dismiss();
        }
    }

    public DropdownChip()
    {
        InitializeComponent();

        HorizontalOptions = LayoutOptions.Start;

        Popup.Opened += Popup_Opened;
        Popup.Closed += Popup_Closed;
    }

    void ApplySelectionText()
    {
        Text = SelectedOption?.Text.TruncateEnd(MaxTextLength, true) ?? Placeholder;
    }

    private void TapGestureRecognizer_Tapped(object? sender, TappedEventArgs e)
    {
        if (sender != null)
            Popup.ShowRelativeToView((View)sender, PopupRelativePosition.AlignBottom);
    }

    private void Popup_Opened(object? sender, EventArgs e)
    {
        Chip.IsOpen = true;
    }

    private void Popup_Closed(object? sender, EventArgs e)
    {
        Chip.IsOpen = false;
    }
}
