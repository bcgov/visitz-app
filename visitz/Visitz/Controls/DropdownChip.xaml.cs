using CommunityToolkit.Maui;
using Syncfusion.Maui.Toolkit.Popup;

namespace Visitz.Controls;

#nullable enable

public partial class DropdownChip : ContentView
{
    [BindableProperty]
    public partial string Text { get; set; } = string.Empty;

    [BindableProperty]
    public partial IEnumerable<IOption> Items { get; set; } = [];

    [BindableProperty(PropertyChangedMethodName = nameof(OnSelectedItemChanged))]
    public partial IOption? SelectedOption { get; set; }

    static void OnSelectedItemChanged(BindableObject bindable, object _, object __)
    {
        ((DropdownChip)bindable).Popup.Dismiss();
    }

    public DropdownChip()
    {
        InitializeComponent();
    }

    private void SfChip_Clicked(object? sender, EventArgs e)
    {
        if (sender != null)
            Popup.ShowRelativeToView((View)sender, PopupRelativePosition.AlignBottom);
    }
}
