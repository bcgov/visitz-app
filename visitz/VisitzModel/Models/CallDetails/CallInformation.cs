using Realms;
using VisitzApi.Models.CallDetails;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Utilities;

namespace VisitzModel.Models.CallDetails;

public partial class CallInformation : IRealmObject, IApiJson<CallInformationJson>
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTimeOffset? Created { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string CreatedByName { get; set; } = string.Empty;
    public DateTimeOffset? Updated { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
    public string UpdatedByName { get; set; } = string.Empty;
    public string ParentId { get; set; } = string.Empty;
    private int ParentTypeInt { get; set; } = (int)EntityType.Unknown;
    public EntityType ParentType
    {
        get => (EntityType)ParentTypeInt;
        set => ParentTypeInt = (int)value;
    }
    public string Information { get; set; } = string.Empty;

    public string InformationBinding
    {
        get => IsValid ? Information : string.Empty;
        set
        {
            if (IsValid)
            {
                this.Commit(() => Information = value);
                RaisePropertyChanged(nameof(Information));
            }
        }
    }

    public CallInformation() { }

    public CallInformation(CallInformationJson json, EntityType type, string parentId)
    {
        Id = json.Id;
        Created = Timestamp.ParseDateTimeOffsetNullable(json.Created);
        CreatedBy = json.CreatedBy;
        CreatedByName = json.CreatedByName;
        Updated = Timestamp.ParseDateTimeOffsetNullable(json.Updated);
        UpdatedBy = json.UpdatedBy;
        UpdatedByName = json.UpdatedByName;
        ParentType = type;
        ParentId = parentId;
        if (type == EntityType.Memo)
            Information = json.Note;
        else if (type == EntityType.ServiceRequest || type == EntityType.Incident)
            Information = json.CallInformation;
        else
            throw new NotImplementedException($"'{ParentType}' not implemented");
    }

    public CallInformationJson ToApiJson(string dateFormat = "s")
    {
        var json = new CallInformationJson()
        {
            Id = Id,
            Created = Created?.ToString(dateFormat) ?? string.Empty,
            CreatedBy = CreatedBy,
            CreatedByName = CreatedByName,
            Updated = Updated?.ToString(dateFormat) ?? string.Empty,
            UpdatedBy = UpdatedBy,
            UpdatedByName = UpdatedByName,
        };

        switch (ParentType)
        {
            case EntityType.Incident:
                json.IncidentId = ParentId;
                json.CallInformation = Information;
                break;
            case EntityType.Memo:
                json.MemoId = ParentId;
                json.Note = Information;
                break;
            case EntityType.ServiceRequest:
                json.SrId = ParentId;
                json.CallInformation = Information;
                break;
            case EntityType.Case:
            case EntityType.Unknown:
            default:
                throw new NotImplementedException($"'{ParentType}' not implemented");
        }

        return json;
    }

    public static List<CallInformation> FromApiJsonArray(
        IEnumerable<CallInformationJson> jsonArray,
        EntityType type,
        string parentId
    )
    {
        List<CallInformation> outList = [];

        if (jsonArray != null)
            foreach (var jsonItem in jsonArray)
                outList.Add(new CallInformation(jsonItem, type, parentId));

        return outList;
    }

    public static async Task SynchronizeAsync(
        Realm realm,
        IEnumerable<CallInformationJson> callInformation,
        string parentId,
        EntityType type
    )
    {
        if (callInformation == null)
            return;

        var incomingCallInformation = FromApiJsonArray(callInformation, type, parentId);
        var incomingCallInformationIds = incomingCallInformation.Select(item => item.Id);

        var allCallInformation = realm
            .All<CallInformation>()
            .Where(item => item.ParentId == parentId && item.ParentTypeInt == (int)type);
        var allCallInformationIds = allCallInformation.AsEnumerable().Select(item => item.Id);

        var callInformationIdsToDelete = allCallInformationIds.Except(incomingCallInformationIds);
        var callInformationToDelete = allCallInformation
            .ToList()
            .Where(item => callInformationIdsToDelete.Contains(item.Id));

        if (!callInformationToDelete.Any() && !incomingCallInformationIds.Any())
            return;

        await RealmExtensions.CommitAsync(
            realm,
            () =>
            {
                foreach (var item in callInformationToDelete)
                {
                    if (item != null && item.IsValid)
                        realm.Remove(item);
                }

                realm.Upsert(incomingCallInformation);
            }
        );
    }

    public static void RemoveByParent(Realm realm, EntityType type, string parentId)
    {
        var callInformation = realm
            .All<CallInformation>()
            .Where(item => item.ParentId == parentId && item.ParentTypeInt == (int)type)
            .ToList();

        foreach (var item in callInformation)
        {
            realm.Remove(item);
        }
    }

    public static IQueryable<CallInformation> GetByParent(Realm realm, EntityType type, string parentId)
    {
        ArgumentNullException.ThrowIfNull(realm);
        return realm.All<CallInformation>().Where(call => call.ParentId == parentId && call.ParentTypeInt == (int)type);
    }
}
