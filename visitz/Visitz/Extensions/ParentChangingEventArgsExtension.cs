namespace Visitz.Extensions;

public static class ParentChangingEventArgsExtension
{
    public static bool AttachingToParent(this ParentChangingEventArgs e)
    {
        return e.OldParent == null && e.NewParent != null;
    }

    public static bool DetachingFromParent(this ParentChangingEventArgs e)
    {
        return e.OldParent != null && e.NewParent == null;
    }
}
