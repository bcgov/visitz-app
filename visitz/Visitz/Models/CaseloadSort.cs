namespace Visitz.Models;

public class CaseloadSort
{
    public static readonly string DisplayDate = nameof(CaseloadItem.DisplayDate);
    public static readonly string DisplayName = nameof(CaseloadItem.DisplayName);

    public string Id { get; set; }
    public bool Ascending { get; set; }
    public string Title { get; set; }
}
