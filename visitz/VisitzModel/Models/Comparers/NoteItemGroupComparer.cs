using VisitzModel.Models;

namespace VisitzModel.Models.Comparers;

public class NoteItemGroupComparer(string entityType) : IComparer<NoteItemGroup>
{
    public static readonly NoteItemGroupComparer NotePeriodInstance = new(IcmEntity.Case);
    public static readonly NoteItemGroupComparer PageNumberInstance = new(IcmEntity.Incident);

    public string EntityType { get; set; } = entityType;

    public int Compare(NoteItemGroup x, NoteItemGroup y)
    {
        if (x == null)
            return y == null ? 0 : -1;
        else
        {
            if (y == null)
                return 1;
            else
            {
                if (EntityType == IcmEntity.Case)
                    return x.NotePeriodDateTime.CompareTo(y.NotePeriodDateTime);
                else
                {
                    return x.PageNumber.CompareTo(y.PageNumber);
                }
            }
        }
    }
}
