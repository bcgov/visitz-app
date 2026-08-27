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
}
