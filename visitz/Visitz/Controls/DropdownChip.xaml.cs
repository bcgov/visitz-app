using CommunityToolkit.Maui;
using Syncfusion.Maui.Toolkit.Popup;
using VisitzModel.Extensions;

namespace Visitz.Controls;

#nullable enable

public partial class DropdownChip : ContentView
{
    const int MaxTextLength = 15;

    [BindableProperty]
    public partial string Text { get; set; } = string.Empty;

    [BindableProperty(PropertyChangedMethodName = nameof(OnPlaceholderChanged))]
    public partial string Placeholder { get; set; } = string.Empty;

    [BindableProperty]
    public partial IEnumerable<IOption> Items { get; set; } = [];

    [BindableProperty(PropertyChangedMethodName = nameof(OnSelectedItemChanged))]
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
    }

    void SfChip_Clicked(object? sender, EventArgs e)
    {
        if (sender != null)
            Popup.ShowRelativeToView((View)sender, PopupRelativePosition.AlignBottom);
    }

    void ApplySelectionText()
    {
        Text = SelectedOption?.Text.TruncateEnd(MaxTextLength, true) ?? Placeholder;
    }
}
