using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Notes;

namespace VisitzModel.Models.Comparers;

public class NoteItemGroupComparer(EntityType entityType) : IComparer<NoteItemGroup>
{
    public static readonly NoteItemGroupComparer NotePeriodInstance = new(EntityType.Case);
    public static readonly NoteItemGroupComparer PageNumberInstance = new(EntityType.Incident);

    public EntityType EntityType { get; set; } = entityType;

    public int Compare(NoteItemGroup? x, NoteItemGroup? y)
    {
        if (x == null)
            return y == null ? 0 : -1;
        else
        {
            if (y == null)
                return 1;
            else
            {
                if (EntityType == EntityType.Case)
                    return x.NotePeriodDateTime.CompareTo(y.NotePeriodDateTime);
                else
                {
                    return x.PageNumber.CompareTo(y.PageNumber);
                }
            }
        }
    }
}
