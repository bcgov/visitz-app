using VisitzModel.Models.Notes;

namespace VisitzModel.Models.Comparers;

public class NoteItemComparer : IComparer<NoteItem>
{
    public static readonly NoteItemComparer Instance = new();

    public int Compare(NoteItem? x, NoteItem? y)
    {
        if (x == null)
            return y == null ? 0 : -1;
        else
        {
            if (y == null)
                return 1;
            else
            {
                if (x.NotePeriod.CompareTo(y.NotePeriod) is int periodCompare && periodCompare != 0)
                    return periodCompare;
                else if (x.PageNumber.CompareTo(y.PageNumber) is int pageCompare && pageCompare != 0)
                    return pageCompare;
                else
                    return x.CreatedDate.CompareTo(y.CreatedDate);
            }
        }
    }
}
