using Visitz.Extensions;
using Visitz.Resources.Styles;
using Visitz.Views;

namespace Visitz.Behaviors;

public abstract class TagStyleBehavior : Behavior<TagView>
{
    public Color BackgroundColor { get; set; } = Colors.Transparent;

    public Color BorderColor { get; set; } = Colors.Black;

    public string IconName { get; set; }

    public Color TextColor { get; set; } = VisitzColors.BC_TextColor;

    protected TagStyleBehavior() { }

    protected override void OnAttachedTo(TagView tag)
    {
        base.OnAttachedTo(tag);
        tag.BindingContextChanged += TagView_OnBindingContextChanged;
        ApplyTagStyle(tag);
    }

    protected override void OnDetachingFrom(TagView tag)
    {
        base.OnDetachingFrom(tag);
        tag.BindingContextChanged -= TagView_OnBindingContextChanged;
    }

    private void TagView_OnBindingContextChanged(object sender, EventArgs e)
    {
        ApplyTagStyle(sender as TagView);
    }

    protected abstract void ApplyTagStyle(TagView tag);
}
