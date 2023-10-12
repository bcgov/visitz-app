namespace Visitz.Behaviors;

partial class SoftPageKeyboardBehavior : Behavior<Page> 
{
    Page Page { get; set; }

    protected override void OnAttachedTo(Page bindable)
    {
        base.OnAttachedTo(bindable);
        Page = bindable;

        Attach();
    }

    protected override void OnDetachingFrom(Page bindable)
    {
        Detach();

        Page = null;
        base.OnDetachingFrom(bindable);
    }

    partial void Attach();

    partial void Detach();
}
