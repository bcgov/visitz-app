using Visitz.Views.TagViews;

namespace Visitz.Behaviors.Tags;

public abstract class TagStyleBehavior : Behavior<TagView>
{
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

    private void TagView_OnBindingContextChanged(object? sender, EventArgs e)
    {
        if (sender != null)
            ApplyTagStyle((TagView)sender);
    }

    protected abstract void ApplyTagStyle(TagView tag);
}
