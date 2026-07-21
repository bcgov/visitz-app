using CommunityToolkit.Maui;
using Visitz.FontIcons;

namespace Visitz.Views.Entity.CallDetails;

#nullable enable

public partial class ConcernListItemView : ContentView
{
    [BindableProperty]
    public partial bool Expanded { get; set; }

    [BindableProperty]
    public partial string ExpandedChevronGlyph { get; set; } = MaterialIcons.Keyboard_arrow_down;

    public ConcernListItemView()
    {
        InitializeComponent();
    }

    private void TapGestureRecognizer_Tapped(object? sender, TappedEventArgs e)
    {
        Expanded = !Expanded;
        ExpandedChevronGlyph = Expanded ? MaterialIcons.Keyboard_arrow_up : MaterialIcons.Keyboard_arrow_down;
    }
}
