namespace Visitz.Views.Caseload;

#nullable enable

public class SortOption<TItem>(string label, IComparer<TItem> comparer)
{
    public string Label { get; set; } = label;

    public IComparer<TItem> Comparer { get; private set; } = comparer;
}
