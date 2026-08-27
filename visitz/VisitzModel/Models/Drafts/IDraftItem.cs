using Realms;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Interfaces;

namespace VisitzModel.Models.Drafts;

public interface IDraftItem : IRealmObject, IRecordInfo, IComparable<IDraftItem>
{
    DateTimeOffset DraftCreated { get; set; }

    DateTimeOffset LastUpdated { get; set; }

    DateTimeOffset LastUpdatedBinding { get; set; }

    string Preview { get; }

    string DraftLocation { get; set; }
}

public static class IDraftItemExtensions
{
    public static IDraftItem InitDraftWith(this IDraftItem item, IBusinessObject businessObject)
    {
        item.DraftLocation = businessObject.DisplayName;
        (item as IRecordInfo).InitWith(businessObject);
        return item;
    }

    public static IBusinessObject? GetRelatedBusinessObjectFrom(this IDraftItem? item, Realm realm)
    {
        if (item == null)
            return null;

        // TS note: I tried using a generic function to run these queries in a nice way but Realm
        // didn't support it, so I had to make the same static function "GetByDraftItem" 4 times.
        return item.RelatedEntityType switch
        {
            EntityType.Case => CaseRecord.GetByDraftItem(realm, item),
            EntityType.Incident => IncidentRecord.GetByDraftItem(realm, item),
            EntityType.Memo => MemoRecord.GetByDraftItem(realm, item),
            EntityType.ServiceRequest => ServiceRequestRecord.GetByDraftItem(realm, item),
            _ => throw new NotImplementedException(),
        };
    }

    public static int CompareDraftItem(this IDraftItem? x, IDraftItem? y)
    {
        if (x == null)
            return y == null ? 0 : -1;
        else
        {
            if (y == null)
                return 1;

            return x.LastUpdatedBinding.CompareTo(y.LastUpdatedBinding);
        }
    }
}
