using CommunityToolkit.Maui;

namespace Visitz.Controls.VisitzChips;

public partial class VChip : ContentView
{
    [BindableProperty]
    public partial string Text { get; set; }

    [BindableProperty(PropertyChangedMethodName = nameof(OnOpenedChanged))]
    public partial bool IsOpen { get; set; }

    static void OnOpenedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is VChip chip)
        {
            bool isOpened = (bool)newValue;
            chip.Arrow.RotationX = isOpened ? 180 : 0;
        }
    }

    public VChip()
    {
        InitializeComponent();
    }
}
