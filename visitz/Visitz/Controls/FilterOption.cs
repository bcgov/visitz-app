namespace Visitz.Controls;

#nullable enable

public class FilterOption<TItem>(string label, Func<TItem, bool> wherePredicate)
    : IOption,
        IEquatable<FilterOption<TItem>>
{
    public Func<TItem, bool> WherePredicate { get; set; } = wherePredicate;

    public bool Selected { get; set; }

    public string Text { get; set; } = label;

    public bool Equals(FilterOption<TItem>? other)
    {
        return ReferenceEquals(this, other)
            || (
                Text.Equals(other?.Text, StringComparison.InvariantCultureIgnoreCase)
                && WherePredicate == other?.WherePredicate
            );
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(WherePredicate, Text);
    }

    public override bool Equals(object? obj)
    {
        return obj is FilterOption<TItem> filter ? Equals(filter) : Equals(this, obj);
    }
}
