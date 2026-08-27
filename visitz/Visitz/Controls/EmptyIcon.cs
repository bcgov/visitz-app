using Microsoft.Maui.Layouts;
using Visitz.FontIcons;
using Visitz.Resources.Styles;

namespace Visitz.Controls;

internal partial class EmptyIcon : Label
{
    public EmptyIcon()
        : base()
    {
        FontFamily = MaterialIcons.RoundedUnfilled.FontFamily;
        FontSize = 200;
        TextColor = VisitzColors.EmptyIconView_Color;
        Text = MaterialIcons.Note_stack;

        HorizontalOptions = LayoutOptions.Center;
        VerticalOptions = LayoutOptions.Center;

        AbsoluteLayout.SetLayoutFlags(this, AbsoluteLayoutFlags.PositionProportional);
        AbsoluteLayout.SetLayoutBounds(this, new Rect(0.5, 0.5, -1, -1));
    }
}
