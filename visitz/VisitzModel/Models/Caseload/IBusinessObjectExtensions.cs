using System.ComponentModel;
using Realms;
using VisitzModel.Extensions;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.People;

namespace VisitzModel.Models.Caseload;

public static partial class IBusinessObjectExtensions
{
    extension(IBusinessObject obj)
    {
        public DateTimeOffset CreatedDateBinding
        {
            get => obj.IsValid ? obj.CreatedDate : DateTimeOffset.MinValue;
            set
            {
                if (obj.IsValid)
                {
                    obj.Commit(() => obj.CreatedDate = value);
                    obj.RaisePropertyChangedEvent(nameof(obj.CreatedDate));
                }
            }
        }

        public string GivenNamesBinding
        {
            get => obj.IsValid ? obj.GivenNames : string.Empty;
            set
            {
                obj.Commit(() => obj.GivenNames = value);
                obj.RaisePropertyChangedEvent(nameof(obj.GivenNames));
            }
        }

        public string LastNameBinding
        {
            get => obj.IsValid ? obj.LastName : string.Empty;
            set
            {
                obj.Commit(() => obj.LastName = value);
                obj.RaisePropertyChangedEvent(nameof(obj.LastName));
            }
        }
    }

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
        return $"{businessObject.LastNameBinding}, {businessObject.GivenNamesBinding}";
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

    public static bool Equals(this IBusinessObject thiz, IBusinessObject? other)
    {
        return ReferenceEquals(thiz, other)
            || (thiz != null && other != null && thiz.Id == other.Id && thiz.EntityType == other.EntityType);
    }

    public static int GetHashCode(this IBusinessObject obj)
    {
        return obj.EntityType.GetHashCode() * obj.Id.GetHashCode();
    }
}
