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

        // TODO: set up listeners to keep View stuck to edge based on device rotation
    }

    protected override void OnDetachingFrom(View bindable)
    {
        base.OnDetachingFrom(bindable);

        View?.ParentChanged -= View_ParentChanged;
        View?.PropertyChanged -= View_PropertyChanged;
        View = null;
    }

    private void View_ParentChanged(object? sender, EventArgs e)
    {
        if (sender is View view && view.Parent != null)
            MoveViewToEdge(EdgeRequest);
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
