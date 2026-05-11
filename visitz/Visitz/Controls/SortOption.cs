namespace Visitz.Controls;

#nullable enable

public class SortOption<TItem>(string label, IComparer<TItem> comparer) : IOption
{
    public string Text { get; } = label;

    public IComparer<TItem> Comparer { get; } = comparer;

    public bool Selected { get; set; }
}
