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
            if (BindingContext == null || !IsExpanded)
                return;

            await Task.Delay(10); // not a fan but it's the easiest way to let the layout settle

            if (Parent.FindFirstParent<CollectionView>() is CollectionView cv)
            {
                // Because UI items in a collection inherit BindingContext, we can reasonably assume we
                // can use it directory to scroll the parent.

                cv.ScrollTo(BindingContext, position: ScrollToPosition.Start);
            }
            else if (Parent.FindFirstParent<ScrollView>() is ScrollView sv)
            {
                await sv.ScrollToAsync(this, ScrollToPosition.Start, true);
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
        }
    }
}
