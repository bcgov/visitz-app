namespace Visitz.Controls;

#nullable enable

public class FilterOption<TItem>(string label, Func<TItem, bool> wherePredicate) : IOption
{
    public Func<TItem, bool> WherePredicate { get; set; } = wherePredicate;

    public bool Selected { get; set; }

    public string Text { get; set; } = label;
}
