namespace Visitz.Controls;

#nullable enable

public class SortOption<TItem>(string label, IComparer<TItem> comparer) : IOption
{
    public string Text { get; set; } = label;

    public IComparer<TItem> Comparer { get; private set; } = comparer;

    public bool Selected { get; set; }
}
