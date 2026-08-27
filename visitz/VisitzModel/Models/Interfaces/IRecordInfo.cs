using Realms;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;

namespace VisitzModel.Models.Interfaces;

public interface IRecordInfo : IDisposable
{
    string RelatedEntityId { get; set; }

    EntityType RelatedEntityType { get; set; }

    EntitySubtype RelatedEntitySubtype { get; set; }

    [Ignored]
    Realm? RelatedEntityRealm { get; set; }

    [Ignored]
    IQueryable<IBusinessObject>? RelatedEntitySubscriptionQuery { get; set; }

    [Ignored]
    IDisposable? RelatedEntitySubscriptionToken { get; set; }

    /// <summary>
    /// Whether or not the related entity is available for the app to interact
    /// with at all.
    /// </summary>
    [Ignored]
    bool? RelatedEntityAvailable { get; set; }

    /// <summary>
    /// Whether or not the related entity's depdendent data has been
    /// downloaded (or marked for download).
    /// </summary>
    [Ignored]
    bool? RelatedEntityDownloaded { get; set; }
}

public static class IRecordInfoExtensions
{
    public static IRecordInfo InitWith(this IRecordInfo item, IBusinessObject businessObject)
    {
        item.RelatedEntityId = businessObject.Id;
        item.RelatedEntityType = businessObject.EntityType;
        item.RelatedEntitySubtype = businessObject.EntitySubtype;

        return item;
    }

    public static void SubscribeRelatedState(this IRecordInfo recordInfo, Realm? realm)
    {
        recordInfo.RelatedEntityRealm = realm;
        recordInfo.RelatedEntitySubscriptionToken?.Dispose();
        recordInfo.RelatedEntitySubscriptionToken = null;

        if (realm != null)
        {
            recordInfo.RelatedEntitySubscriptionQuery = IBusinessObject.GetQueryableByRelaxedIdType(
                realm,
                recordInfo.RelatedEntityId,
                recordInfo.RelatedEntityType
            );

            recordInfo.RelatedEntitySubscriptionToken =
                recordInfo.RelatedEntitySubscriptionQuery.SubscribeForNotifications(
                    (items, changes) =>
                    {
                        recordInfo.RelatedEntityAvailable = items.Any();
                        recordInfo.RelatedEntityDownloaded =
                            items.FirstOrDefault() is IBusinessObject businessObject
                            && businessObject.LocalState is BoLocalState state
                            && state.ShouldDownloadDuringRefresh;
                    }
                );
        }
    }
}
