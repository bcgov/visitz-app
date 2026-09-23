namespace VisitzModel.Extensions;

public static class ElementExtensions
{
    public static TargetType? FindFirstParent<TargetType>(this Element element)
        where TargetType : Element
    {
        ArgumentNullException.ThrowIfNull(element, nameof(element));

        if (element.Parent == null)
            return null;
        else if (element.Parent.GetType() == typeof(TargetType))
            return (TargetType)element.Parent;
        else
            return FindFirstParent<TargetType>(element.Parent);
    }

    public static TTargetType? FindLastParent<TTargetType>(this Element element)
        where TTargetType : Element
    {
        ArgumentNullException.ThrowIfNull(element, nameof(element));

        return FindLastParentRecursive<TTargetType>(element, null);
    }

    static TTargetType? FindLastParentRecursive<TTargetType>(this Element element, TTargetType? current)
        where TTargetType : Element
    {
        if (element.Parent == null)
            return current;
        else if (element.Parent.GetType() == typeof(TTargetType))
            return FindLastParentRecursive(element.Parent, (TTargetType)element.Parent);
        else
            return FindLastParentRecursive(element.Parent, current);
    }
}
