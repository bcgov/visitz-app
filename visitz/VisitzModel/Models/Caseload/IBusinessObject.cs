using Realms;
using System.ComponentModel;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Formats;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.People;

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

    public string ServiceOffice { get; set; }

    public BoLocalState LocalState { get; set; }

    public string DisplayDate { get; }

    public string DisplayName { get; }

    public string FullType { get; }

    public IQueryable<IcmContact> Contacts { get; }

    public bool IsAssigned(string username);
}

public static class IBusinessObjectExtensions
{
    public static string ToIdTypeString(this IBusinessObject businessObject)
    {
        return $"{businessObject.Id}||{(int)businessObject.EntityType}";
    }

    public static DateTime DisplayDateTransform(this IBusinessObject businessObject)
    {
        return businessObject.DisplayDate?.Length > 0
            ? DateTime.Parse(businessObject.DisplayDate)
            : DateTime.MinValue;
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

    public static IcmContact GetKeyPlayer(this IBusinessObject businessObject, Realm? realm = null)
    {
        return IcmContact.GetKeyPlayerFor(realm ?? businessObject.Realm, businessObject);
    }

    public static IQueryable<IcmContact> GetContacts(this IBusinessObject businessObject, Realm? realm = null)
    {
        return IcmContact.GetByParentObject(realm ?? businessObject.Realm, businessObject);
    }

    public static void SubscribePropertyChanged(
        this IBusinessObject business,
        PropertyChangedEventHandler handler)
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

    public static void UnsubscribePropertyChanged(
        this IBusinessObject business,
        PropertyChangedEventHandler handler)
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

    public static void UpsertLocalState(
        this IBusinessObject item,
        Realm realm,
        bool markForDownload)
    {
        if (realm.Find<BoLocalState>(item.ToIdTypeString()) is BoLocalState local)
        {
            item.LocalState = local;
            realm.Add(local, update: true);
        }
        else
        {
            item.LocalState = new(item) { ShouldDownloadDuringRefresh = markForDownload };
            realm.Add(item.LocalState);
        }
    }
}
