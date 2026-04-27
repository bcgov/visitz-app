using System.ComponentModel;
using Realms;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Formats;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.People;
using VisitzModel.Storage;

namespace VisitzModel.Models.Caseload;

#nullable enable

public interface IBusinessObject : IRealmObject
{
    public static readonly string DisplayDateFormat = IcmDateFormats.BasicTimestampShort;

    public string Id { get; set; }

    public string FileNumber { get; set; }

    public string GivenNames { get; set; }

    public string LastName { get; set; }

    public string AssignedTo { get; set; }

    public string AssignedToId { get; set; }

    public string DisplayAssignees { get; }

    public EntityType EntityType { get; }

    public EntitySubtype EntitySubtype { get; set; }

    public string EntitySubtypeInitials { get; }

    public string ServiceOffice { get; set; }

    public BoLocalState? LocalState { get; set; }

    public string DisplayDate { get; }

    public string DisplayName { get; }

    public string FullType { get; }

    public IQueryable<IcmContact> Contacts { get; }

    public bool IsAssigned(string username);

    /// <summary>
    /// Deletes most of the dependent data for a BusinessObject.
    /// </summary>
    /// <param name="userIgnoredPrefs"></param>
    /// <param name="fromRealm">A Realm reference to delete from or leave null to use the private reference.</param>
    /// <param name="deleteLocalState">true to delete LocalState as well. false to keep it.</param>
    public void DeleteDependentData(
        UserIgnoredContentPrefs userIgnoredPrefs,
        Realm? fromRealm = null,
        bool deleteLocalState = false
    );

    /// <summary>
    /// Deletes a BusinessObject and all its dependent data.
    /// </summary>
    /// <param name="userIgnoredPrefs"></param>
    /// <param name="fromRealm">A Realm reference to delete from or leave null
    /// to use the private reference.</param>
    /// <param name="cascade">Delete all dependent data for this BusinessObject.
    /// Defaults to true.</param>
    /// <param name="deleteLocalState">Delete LocalState for this BusinessObject.
    /// Defaults to true. Independent from cascade.</param>
    public void Delete(
        UserIgnoredContentPrefs userIgnoredPrefs,
        Realm? fromRealm = null,
        bool cascade = true,
        bool deleteLocalState = true
    );
}

public static class IBusinessObjectExtensions
{
    public static string ToIdTypeString(this IBusinessObject businessObject)
    {
        return $"{businessObject.Id}||{(int)businessObject.EntityType}";
    }

    public static DateTime DisplayDateTransform(this IBusinessObject businessObject)
    {
        return businessObject.DisplayDate?.Length > 0 ? DateTime.Parse(businessObject.DisplayDate) : DateTime.MinValue;
    }

    public static string GetDisplayName(this IBusinessObject businessObject)
    {
        return $"{businessObject.LastName}, {businessObject.GivenNames}";
    }

    public static string GetFullType(this IBusinessObject businessObject)
    {
        string subtype = businessObject.EntitySubtype.GetDisplayString();
        string type = businessObject.EntityType.GetDisplayString();
        return $"{subtype} {type}";
    }

    public static IcmContact? GetKeyPlayer(this IBusinessObject businessObject, Realm? realm = null)
    {
        realm ??= businessObject.Realm ?? throw new InvalidOperationException("Attached Realm is null");
        return IcmContact.GetKeyPlayerFor(realm, businessObject);
    }

    public static IQueryable<IcmContact> GetContacts(this IBusinessObject businessObject, Realm? realm = null)
    {
        realm ??= businessObject.Realm;
        ArgumentNullException.ThrowIfNull(realm);
        return IcmContact.GetByParentObject(realm, businessObject);
    }

    public static void SubscribePropertyChanged(this IBusinessObject business, PropertyChangedEventHandler handler)
    {
        if (business is CaseRecord @case)
            @case.PropertyChanged += handler;
        else if (business is IncidentRecord incident)
            incident.PropertyChanged += handler;
        else if (business is MemoRecord memo)
            memo.PropertyChanged += handler;
        else if (business is ServiceRequestRecord sr)
            sr.PropertyChanged += handler;
        else
            throw new NotImplementedException($"Type '{business.GetType()}' not implemented for subscription");
    }

    public static void UnsubscribePropertyChanged(this IBusinessObject business, PropertyChangedEventHandler handler)
    {
        if (business is CaseRecord @case)
            @case.PropertyChanged -= handler;
        else if (business is IncidentRecord incident)
            incident.PropertyChanged -= handler;
        else if (business is MemoRecord memo)
            memo.PropertyChanged -= handler;
        else if (business is ServiceRequestRecord sr)
            sr.PropertyChanged -= handler;
        else
            throw new NotImplementedException($"Type '{business.GetType()}' not implemented for unsubscription");
    }

    public static void UpsertLocalState(this IBusinessObject item, Realm realm, bool? markForDownload = null)
    {
        if (realm.Find<BoLocalState>(item.ToIdTypeString()) is BoLocalState local)
        {
            if (!local.ShouldDownloadDuringRefresh && markForDownload is bool mark)
                local.ShouldDownloadDuringRefresh = mark;

            item.LocalState = local;
            realm.Add(local, update: true);
        }
        else
        {
            item.LocalState = new(item) { ShouldDownloadDuringRefresh = markForDownload ?? false };
            realm.Add(item.LocalState);
        }
    }

    public static IQueryable<IBusinessObject> GetQueryableByRelaxedIdType(Realm realm, string id, EntityType type)
    {
        return type switch
        {
            EntityType.Case => realm.All<CaseRecord>().Where(rec => rec.Id == id || rec.FileNumber == id),
            EntityType.Incident => realm.All<IncidentRecord>().Where(rec => rec.Id == id || rec.FileNumber == id),
            EntityType.Memo => realm.All<MemoRecord>().Where(rec => rec.Id == id || rec.FileNumber == id),
            EntityType.ServiceRequest => realm
                .All<ServiceRequestRecord>()
                .Where(rec => rec.Id == id || rec.FileNumber == id),
            _ => throw new InvalidOperationException($"'{type}' not supported"),
        };
    }

    public static IBusinessObject? GetByIdType(Realm realm, string id, EntityType type)
    {
        return type switch
        {
            EntityType.Case => realm.Find<CaseRecord>(id),
            EntityType.Incident => realm.Find<IncidentRecord>(id),
            EntityType.Memo => realm.Find<MemoRecord>(id),
            EntityType.ServiceRequest => realm.Find<ServiceRequestRecord>(id),
            _ => throw new InvalidOperationException($"'{type}' not supported"),
        };
    }
}
