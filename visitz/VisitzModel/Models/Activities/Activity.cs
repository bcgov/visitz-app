using RealmBindings;
using Realms;
using VisitzApi.Models.Activities;
using VisitzModel.Extensions;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Interfaces;
using VisitzModel.Utilities;

namespace VisitzModel.Models.Activities;

[GenerateRealmBindings]
public partial class Activity : IRealmObject, IParentRecord
{
    [PrimaryKey]
    public string LocalId { get; set; } = Guid.NewGuid().ToString();
    public string Id { get; set; } = string.Empty;
    public string AccountLocation { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string ActionBy { get; set; } = string.Empty;
    public string ActionCode { get; set; } = string.Empty;
    public string ActivityId { get; set; } = string.Empty;
    public string ActivitySRId { get; set; } = string.Empty;
    public string ActivityTemplateId { get; set; } = string.Empty;
    public string ActivityUID { get; set; } = string.Empty;
    public string Alarm { get; set; } = string.Empty;
    public string ApptAlarmTimeMin { get; set; } = string.Empty;
    public string AssetNumber { get; set; } = string.Empty;
    public string AssetNumberS_ASSET { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string CaseId { get; set; } = string.Empty;
    public string CaseNumber { get; set; } = string.Empty;
    public string CaseOwner { get; set; } = string.Empty;
    public string CaseRestricted { get; set; } = string.Empty;
    public string CaseStatus { get; set; } = string.Empty;
    public string CaseType { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public string ContactFirstName { get; set; } = string.Empty;
    public string ContactGivenName { get; set; } = string.Empty;
    public string ContactLastName { get; set; } = string.Empty;
    public string ContactMiddleName { get; set; } = string.Empty;
    public DateTimeOffset? Created { get; set; } = DateTimeOffset.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public string CreatedByName { get; set; } = string.Empty;
    public string Delay { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Display { get; set; } = string.Empty;
    public string Done { get; set; } = string.Empty;
    public string DoneFlag { get; set; } = string.Empty;
    public DateTimeOffset? Due { get; set; } = DateTimeOffset.UtcNow;
    public string DurationMinutes { get; set; } = string.Empty;
    public string ICMCreatedByOffice { get; set; } = string.Empty;
    public string ICMMemoNo { get; set; } = string.Empty;
    public string ICMMemoId { get; set; } = string.Empty;
    public string ICMMemoOwner { get; set; } = string.Empty;
    public string ICMMemoRestricted { get; set; } = string.Empty;
    public string ICMMemoType { get; set; } = string.Empty;
    public string ICMServiceRegion { get; set; } = string.Empty;
    public string ICMSubType { get; set; } = string.Empty;
    public string ICMType { get; set; } = string.Empty;
    public string IncidentNo { get; set; } = string.Empty;
    public string IncidentId { get; set; } = string.Empty;
    public string MeetingLocation { get; set; } = string.Empty;
    public string Ministry { get; set; } = string.Empty;
    public string MinistryId { get; set; } = string.Empty;
    public string Opportunity { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public string Overdue { get; set; } = string.Empty;
    public string OwnedBy { get; set; } = string.Empty;
    public string OwnedById { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;

    public DateTimeOffset? Planned { get; set; } = DateTimeOffset.UtcNow;
    public string PreviousActivityDescription { get; set; } = string.Empty;
    public string PriceList { get; set; } = string.Empty;
    public string PrimaryContactId { get; set; } = string.Empty;
    public string PrimaryOwnedBy { get; set; } = string.Empty;
    public string PrimaryOwnerId { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Property { get; set; } = string.Empty;
    public string Quote { get; set; } = string.Empty;
    public string RateList { get; set; } = string.Empty;
    public string Repeating { get; set; } = string.Empty;
    public string RepeatingExpires { get; set; } = string.Empty;
    public string RepeatingType { get; set; } = string.Empty;
    public string RestrictedFlag { get; set; } = string.Empty;
    public string ResultCode { get; set; } = string.Empty;
    public string RowId { get; set; } = string.Empty;
    public string Sequence { get; set; } = string.Empty;
    public string ServiceOffice { get; set; } = string.Empty;
    public string SharedFlag { get; set; } = string.Empty;
    public string ShowInContactProfile { get; set; } = string.Empty;
    public string SRNumber { get; set; } = string.Empty;
    public string SROwner { get; set; } = string.Empty;
    public string SRSubType { get; set; } = string.Empty;
    public string SRType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTimeOffset? Updated { get; set; } = DateTimeOffset.UtcNow;
    public string UpdatedBy { get; set; } = string.Empty;
    public string UpdatedByName { get; set; } = string.Empty;
    public string ParentId { get; set; } = string.Empty;

    internal int ParentTypeInt { get; set; } = (int)EntityType.Case;

    public EntityType ParentType
    {
        get => (EntityType)ParentTypeInt;
        set => ParentTypeInt = (int)value;
    }

    public Activity(ActivityJson json)
    {
        LocalId = MakeLocalId(json.Id, json.CaseId, json.IncidentId, json.ActivitySRId);
        Id = json.Id;
        AccountLocation = json.AccountLocation;
        AccountName = json.AccountName;
        ActionBy = json.ActionBy;
        ActionCode = json.ActionCode;
        ActivityId = json.ActivityId;
        ActivitySRId = json.ActivitySRId;
        ActivityTemplateId = json.ActivityTemplateId;
        ActivityUID = json.ActivityUID;
        Alarm = json.Alarm;
        ApptAlarmTimeMin = json.ApptAlarmTimeMin;
        AssetNumber = json.AssetNumber;
        AssetNumberS_ASSET = json.AssetNumberS_ASSET;
        Brand = json.Brand;
        CaseId = json.CaseId;
        CaseNumber = json.CaseNumber;
        CaseOwner = json.CaseOwner;
        CaseRestricted = json.CaseRestricted;
        CaseStatus = json.CaseStatus;
        CaseType = json.CaseType;
        Category = json.Category;
        Class = json.Class;
        ContactFirstName = json.ContactFirstName;
        ContactGivenName = json.ContactGivenName;
        ContactLastName = json.ContactLastName;
        ContactMiddleName = json.ContactMiddleName;
        Created = Timestamp.ParseDateTimeOffsetNullable(json.Created);
        CreatedBy = json.CreatedBy;
        CreatedByName = json.CreatedByName;
        Delay = json.Delay;
        Description = json.Description;
        Display = json.Display;
        Done = json.Done;
        DoneFlag = json.DoneFlag;
        Due = Timestamp.ParseDateTimeOffsetNullable(json.Due);
        DurationMinutes = json.DurationMinutes;
        ICMCreatedByOffice = json.ICMCreatedByOffice;
        ICMMemoNo = json.ICMMemoNo;
        ICMMemoId = json.ICMMemoId;
        ICMMemoOwner = json.ICMMemoOwner;
        ICMMemoRestricted = json.ICMMemoRestricted;
        ICMMemoType = json.ICMMemoType;
        ICMServiceRegion = json.ICMServiceRegion;
        ICMSubType = json.ICMSubType;
        ICMType = json.ICMType;
        IncidentNo = json.IncidentNo;
        IncidentId = json.IncidentId;
        MeetingLocation = json.MeetingLocation;
        Ministry = json.Ministry;
        MinistryId = json.MinistryId;
        Opportunity = json.Opportunity;
        OrderNumber = json.OrderNumber;
        Overdue = json.Overdue;
        OwnedBy = json.OwnedBy;
        OwnedById = json.OwnedById;
        PlanName = json.PlanName;
        Planned = Timestamp.ParseDateTimeOffsetNullable(json.Planned);
        PreviousActivityDescription = json.PreviousActivityDescription;
        PriceList = json.PriceList;
        PrimaryContactId = json.PrimaryContactId;
        PrimaryOwnedBy = json.PrimaryOwnedBy;
        PrimaryOwnerId = json.PrimaryOwnerId;
        Priority = json.Priority;
        Property = json.Property;
        Quote = json.Quote;
        RateList = json.RateList;
        Repeating = json.Repeating;
        RepeatingExpires = json.RepeatingExpires;
        RepeatingType = json.RepeatingType;
        RestrictedFlag = json.RestrictedFlag;
        ResultCode = json.ResultCode;
        RowId = json.RowId;
        Sequence = json.Sequence;

        SharedFlag = json.SharedFlag;
        ShowInContactProfile = json.ShowInContactProfile;
        SRNumber = json.SRNumber;
        SROwner = json.SROwner;
        SRSubType = json.SRSubType;
        SRType = json.SRType;
        Status = json.Status;
        Type = json.Type;
        Updated = Timestamp.ParseDateTimeOffsetNullable(json.Updated);
        UpdatedBy = json.UpdatedBy;
        UpdatedByName = json.UpdatedByName;
    }

    public static string MakeLocalId(params string?[] ids)
    {
        return string.Join("|", ids.Where(id => !string.IsNullOrWhiteSpace(id)));
    }

    public static List<Activity> FromApiJsonArray(IEnumerable<ActivityJson> jsonArray)
    {
        List<Activity> outList = [];

        if (jsonArray != null)
            foreach (var jsonItem in jsonArray)
                outList.Add(new Activity(jsonItem));

        return outList;
    }

    public static async Task SynchronizeAsync(
        Realm realm,
        IEnumerable<ActivityJson> activities,
        string parentId,
        EntityType parentType
    )
    {
        await realm.SynchronizeByQueryAsync(
            incomingItems: FromApiJsonArray(activities),
            existingQuery: GetActivities(realm, parentId, parentType)
        );
    }

    public static IQueryable<Activity> GetActivities(Realm realm, string parentId, EntityType parentType)
    {
        IQueryable<Activity> query = realm.All<Activity>();

        return parentType switch
        {
            EntityType.Case => query.Where(activity => activity.CaseId == parentId),

            EntityType.Incident => query.Where(activity => activity.IncidentId == parentId),

            EntityType.ServiceRequest => query.Where(activity => activity.ActivitySRId == parentId),

            EntityType.Memo => query.Where(activity => activity.ICMMemoId == parentId),

            _ => throw new ArgumentOutOfRangeException(nameof(parentType)),
        };
    }

    public static void RemoveByParent(Realm realm, EntityType entityType, string parentId)
    {
        var removeableActivities = GetActivities(realm, parentId, entityType)
            .AsEnumerable()
            .Where(item =>
                (entityType == EntityType.Case || RecordDoesNotExist<CaseRecord>(realm, item.CaseId))
                && (entityType == EntityType.Incident || RecordDoesNotExist<IncidentRecord>(realm, item.IncidentId))
                && (entityType == EntityType.Memo || RecordDoesNotExist<MemoRecord>(realm, item.ICMMemoId))
                && (
                    entityType == EntityType.ServiceRequest
                    || RecordDoesNotExist<ServiceRequestRecord>(realm, item.ActivitySRId)
                )
            );

        foreach (var activity in removeableActivities)
        {
            realm.Remove(activity);
        }
    }

    static bool RecordDoesNotExist<T>(Realm realm, string? id)
        where T : IRealmObject
    {
        return string.IsNullOrEmpty(id) || realm.Find<T>(id) == null;
    }
}
