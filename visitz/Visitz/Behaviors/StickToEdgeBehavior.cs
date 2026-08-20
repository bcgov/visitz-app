using System.ComponentModel;
using CommunityToolkit.Maui;
using Microsoft.Maui.Layouts;

namespace Visitz.Behaviors;

internal partial class StickToEdgeBehavior : Behavior<View>
{
    View? View { get; set; }

    [BindableProperty(PropertyChangedMethodName = nameof(StickToEdgeChanged))]
    public partial ScreenEdge EdgeRequest { get; set; }

    protected override void OnAttachedTo(View bindable)
    {
        base.OnAttachedTo(bindable);

        View = bindable;
        BindingContext = bindable.BindingContext;
        View.ParentChanged += View_ParentChanged;
        View.PropertyChanged += View_PropertyChanged;

        DeviceDisplay.Current.MainDisplayInfoChanged += Current_MainDisplayInfoChanged;
    }

    protected override void OnDetachingFrom(View bindable)
    {
        base.OnDetachingFrom(bindable);

        View?.ParentChanged -= View_ParentChanged;
        View?.PropertyChanged -= View_PropertyChanged;
        View = null;
    }

    private void Current_MainDisplayInfoChanged(object? sender, DisplayInfoChangedEventArgs e)
    {
        ApplyDeviceRotation(e.DisplayInfo.Rotation);
    }

    void ApplyDeviceRotation(DisplayRotation rotation)
    {
        // I spent too much time trying to figure out a mathy/elegant way to do this...
        // but it ended up not working and the code itself was inscrutable and way
        // less readable/resonable than just doing a menagerie of switch statements.

        ScreenEdge actualEdge = ScreenEdge.Unknown;

        if (EdgeRequest == ScreenEdge.Bottom)
        {
            actualEdge = rotation switch
            {
                DisplayRotation.Rotation0 => ScreenEdge.Bottom, // 0 -> 0
                DisplayRotation.Rotation90 => ScreenEdge.Right, // 1 -> 1
                DisplayRotation.Rotation180 => ScreenEdge.Top, // 2 -> 2
                DisplayRotation.Rotation270 => ScreenEdge.Left, // 3-> 3
                _ => ScreenEdge.Unknown,
            };
        }
        else if (EdgeRequest == ScreenEdge.Left)
        {
            actualEdge = rotation switch
            {
                DisplayRotation.Rotation0 => ScreenEdge.Left, // 0 -> 3
                DisplayRotation.Rotation90 => ScreenEdge.Bottom, // 1 -> 0
                DisplayRotation.Rotation180 => ScreenEdge.Right, // 2 -> 1
                DisplayRotation.Rotation270 => ScreenEdge.Top, // 3 -> 2
                _ => ScreenEdge.Unknown,
            };
        }
        else if (EdgeRequest == ScreenEdge.Top)
        {
            actualEdge = rotation switch
            {
                DisplayRotation.Rotation0 => ScreenEdge.Top, // 0 -> 2
                DisplayRotation.Rotation90 => ScreenEdge.Left, // 1 -> 3
                DisplayRotation.Rotation180 => ScreenEdge.Bottom, // 2 -> 0
                DisplayRotation.Rotation270 => ScreenEdge.Right, // 3 -> 1
                _ => ScreenEdge.Unknown,
            };
        }
        else if (EdgeRequest == ScreenEdge.Right)
        {
            actualEdge = rotation switch
            {
                DisplayRotation.Rotation0 => ScreenEdge.Right, // 0 -> 1
                DisplayRotation.Rotation90 => ScreenEdge.Top, // 1 -> 2
                DisplayRotation.Rotation180 => ScreenEdge.Left, // 2 -> 3
                DisplayRotation.Rotation270 => ScreenEdge.Bottom, // 3 -> 0
                _ => ScreenEdge.Unknown,
            };
        }

        if (actualEdge != ScreenEdge.Unknown)
            MoveViewToEdge(actualEdge);
    }

    private void View_ParentChanged(object? sender, EventArgs e)
    {
        if (sender is View view && view.Parent != null)
            ApplyDeviceRotation(DeviceDisplay.Current.MainDisplayInfo.Rotation);
    }

    static void StickToEdgeChanged(BindableObject bound, object _, object newValue)
    {
        StickToEdgeBehavior b = (StickToEdgeBehavior)bound;
        ScreenEdge newEdge = (ScreenEdge)newValue;

        if (b.EdgeRequest != newEdge)
            b.MoveViewToEdge(newEdge);
    }

    void MoveViewToEdge(ScreenEdge newEdge)
    {
        if (View == null || newEdge == ScreenEdge.Unknown)
            return;

        if (View.Parent is AbsoluteLayout)
        {
            AbsoluteLayout.SetLayoutFlags(View, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(View, GetEdgeRectForAbsoluteLayout(newEdge));
        }
        else
        {
            (View.HorizontalOptions, View.VerticalOptions) = GetLayoutOptionsForEdge(newEdge);
        }

        View.Rotation = GetRotation(newEdge);
        View.TranslationX = GetTranslationCorrection(newEdge);
    }

    static Rect GetEdgeRectForAbsoluteLayout(ScreenEdge edge)
    {
        return edge switch
        {
            ScreenEdge.Bottom => new(0.5, 1, -1, -1),
            ScreenEdge.Top => new(0.5, 0, -1, -1),
            ScreenEdge.Left => new Rect(0, 0.5, -1, -1),
            ScreenEdge.Right => new Rect(1, 0.5, -1, -1),
            _ => throw new InvalidOperationException($"Edge '{edge}' not supported"),
        };
    }

    static (LayoutOptions Horizontal, LayoutOptions Vertical) GetLayoutOptionsForEdge(ScreenEdge edge)
    {
        return edge switch
        {
            ScreenEdge.Bottom => (LayoutOptions.Center, LayoutOptions.End),
            ScreenEdge.Top => (LayoutOptions.Center, LayoutOptions.Start),
            ScreenEdge.Left => (LayoutOptions.Start, LayoutOptions.Center),
            ScreenEdge.Right => (LayoutOptions.End, LayoutOptions.Center),
            _ => throw new InvalidOperationException($"Edge '{edge}' not supported"),
        };
    }

    static double GetRotation(ScreenEdge edge)
    {
        return edge switch
        {
            ScreenEdge.Bottom => 0,
            ScreenEdge.Top => 180,
            ScreenEdge.Left => 90,
            ScreenEdge.Right => 270,
            _ => throw new InvalidOperationException($"Edge '{edge}' not supported"),
        };
    }

    // TODO: Add support to correct TranslationY when needed
    double GetTranslationCorrection(ScreenEdge edge)
    {
        if (View == null)
            throw new InvalidOperationException(nameof(View) + " should not be null");

        double correction = View.Height / 2;

        return edge switch
        {
            ScreenEdge.Bottom => 0,
            ScreenEdge.Top => 0,
            ScreenEdge.Left => -correction,
            ScreenEdge.Right => correction,
            _ => throw new InvalidOperationException($"Edge '{edge}' not supported"),
        };
    }

    private void View_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not View view)
            return;

        if (e.PropertyName == nameof(view.Height))
            view.TranslationX = GetTranslationCorrection(EdgeRequest);
    }
}
