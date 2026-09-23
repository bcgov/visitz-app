using CommunityToolkit.Maui;
using Visitz.FontIcons;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;

namespace Visitz.Views;

public partial class Expander : BaseContentView
{
    [BindableProperty(PropertyChangedMethodName = nameof(IsExpanded_PropertyChanged))]
    public partial bool IsExpanded { get; set; }

    [BindableProperty]
    public partial View HeaderView { get; set; }

    [BindableProperty]
    public partial View ExpandedView { get; set; }

    [BindableProperty]
    public partial string ExpandedChevronGlyph { get; set; } = MaterialIcons.Keyboard_arrow_down;

    public event EventHandler<bool>? ExpandedChanged;

    public Expander()
    {
        InitializeComponent();
    }

    static void IsExpanded_PropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is Expander expander)
        {
            expander.ToggleGlyph();
            expander.ExpandedChanged?.Invoke(expander, expander.IsExpanded);
            _ = expander.TrySrollIntoView();
        }
    }

    void ToggleGlyph()
    {
        ExpandedChevronGlyph = IsExpanded ? MaterialIcons.Keyboard_arrow_up : MaterialIcons.Keyboard_arrow_down;
    }

    void Header_Tapped(object? sender, TappedEventArgs e)
    {
        IsExpanded = !IsExpanded;
    }

    async Task TrySrollIntoView()
    {
        try
        {
            if (!IsExpanded)
                return;

            await Task.Delay(10); // not a fan but it's the easiest way to let the layout settle

            // Scrolling both first and last parent as a low-effort way to try to scroll this
            // into view in scenarios where scrollable views are nested (e.g. CollectionView
            // inside ScrollView)
            await ScrollFirstParent();
            await ScrollLastParent();
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
        }
    }

    async Task ScrollFirstParent()
    {
        if (Parent.FindFirstParent<ScrollView>() is ScrollView sv)
        {
            await sv.ScrollToAsync(this, ScrollToPosition.Start, true);
        }
        else if (BindingContext != null && Parent.FindFirstParent<CollectionView>() is CollectionView cv)
        {
            // Because UI items in a collection inherit BindingContext, we can reasonably assume we
            // can use it directory to scroll the parent.

            cv.ScrollTo(BindingContext, position: ScrollToPosition.Start);
        }
    }

    async Task ScrollLastParent()
    {
        if (this.FindLastParent<ScrollView>() is ScrollView lastSv)
        {
            await lastSv.ScrollToAsync(this, ScrollToPosition.Start, true);
        }
        else if (BindingContext != null && this.FindLastParent<CollectionView>() is CollectionView lastCv)
        {
            // Because UI items in a collection inherit BindingContext, we can reasonably assume we
            // can use it directory to scroll the parent.
            lastCv.ScrollTo(BindingContext, position: ScrollToPosition.Start);
        }
    }
}
