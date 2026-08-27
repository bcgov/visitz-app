using Realms;
using VisitzApi.Models.CallDetails;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Utilities;

namespace VisitzModel.Models.CallDetails;

public partial class AdditionalInformation : IRealmObject, IApiJson<AdditionalInformationJson>
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string ParentId { get; set; } = string.Empty;
    public EntityType ParentType
    {
        get => (EntityType)ParentTypeInt;
        set => ParentTypeInt = (int)value;
    }
    private int ParentTypeInt { get; set; } = (int)EntityType.Unknown;

    public string AdditionalInformations { get; set; } = string.Empty;

    public string AdditionalInformationsBinding
    {
        get => IsValid ? AdditionalInformations : string.Empty;
        set
        {
            if (IsValid)
            {
                this.Commit(() => AdditionalInformations = value);
                RaisePropertyChanged(nameof(AdditionalInformations));
            }
        }
    }

    public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;

    public string CreatedBy { get; set; } = string.Empty;

    public string CreatedByName { get; set; } = string.Empty;

    public DateTimeOffset Updated { get; set; } = DateTimeOffset.UtcNow;

    public string UpdatedBy { get; set; } = string.Empty;

    public string UpdatedByName { get; set; } = string.Empty;

    public AdditionalInformation() { }

    public AdditionalInformation(AdditionalInformationJson json, EntityType parentType, string parentId)
    {
        Id = json.Id;
        Created = Timestamp.ParseDateTimeOffsetNullable(json.Created) ?? default;
        AdditionalInformations = json.AdditionalInformation;
        CreatedBy = json.CreatedBy;
        CreatedByName = json.CreatedByName;
        Updated = Timestamp.ParseDateTimeOffsetNullable(json.Updated) ?? default;
        UpdatedBy = json.UpdatedBy;
        UpdatedByName = json.UpdatedByName;
        ParentId = parentId;
        ParentType = parentType;
    }

    public AdditionalInformationJson ToApiJson(string dateFormat = "s")
    {
        var addInfo = new AdditionalInformationJson()
        {
            Created = Created.ToString(dateFormat),
            AdditionalInformation = AdditionalInformations,
            CreatedBy = CreatedBy,
            CreatedByName = CreatedByName,
            Updated = Updated.ToString(dateFormat),
            UpdatedBy = UpdatedBy,
            Id = Id,
            UpdatedByName = UpdatedByName,
        };
        switch (ParentType)
        {
            case EntityType.Incident:
                addInfo.IncidentId = ParentId;
                break;
            case EntityType.Memo:
                addInfo.MemoId = ParentId;
                break;
            case EntityType.ServiceRequest:
                addInfo.SRId = ParentId;
                break;
            case EntityType.Case:
            case EntityType.Unknown:
            default:
                throw new NotImplementedException($"'{ParentType}' not implemented");
        }
        return addInfo;
    }

    public static List<AdditionalInformation> FromApiJsonArray(
        IEnumerable<AdditionalInformationJson> jsonArray,
        EntityType parentType,
        string parentId
    )
    {
        List<AdditionalInformation> outList = [];

        if (jsonArray != null)
            foreach (var jsonItem in jsonArray)
                outList.Add(new AdditionalInformation(jsonItem, parentType, parentId));

        return outList;
    }

    public static async Task SynchronizeAsync(
        Realm realm,
        IEnumerable<AdditionalInformationJson> additionalInformation,
        string parentId,
        EntityType type
    )
    {
        if (additionalInformation == null)
            return;

        var incomingadditionalinformation = FromApiJsonArray(additionalInformation, type, parentId);
        var incomingIncidentConcernIds = incomingadditionalinformation.Select(item => item.Id);

        var allIncidentConcerns = realm
            .All<AdditionalInformation>()
            .Where(item => item.ParentId == parentId && item.ParentTypeInt == (int)type);
        var allIncidentConcernIds = allIncidentConcerns.AsEnumerable().Select(item => item.Id);

        var additionalInformationIdsToDelete = allIncidentConcernIds.Except(incomingIncidentConcernIds);
        var incidentConcernsToDelete = allIncidentConcerns
            .ToList()
            .Where(item => additionalInformationIdsToDelete.Contains(item.Id));

        if (!incidentConcernsToDelete.Any() && !incomingIncidentConcernIds.Any())
            return;

        await RealmExtensions.CommitAsync(
            realm,
            () =>
            {
                foreach (var item in incidentConcernsToDelete)
                {
                    if (item != null && item.IsValid)
                        realm.Remove(item);
                }

                realm.Upsert(incomingadditionalinformation);
            }
        );
    }

    public static void RemoveByParent(Realm realm, EntityType type, string parentId)
    {
        var visitItems = realm
            .All<AdditionalInformation>()
            .Where(item => item.ParentId == parentId && item.ParentTypeInt == (int)type);

        realm.RemoveRange(visitItems);
    }

    public static IQueryable<AdditionalInformation> GetByParent(Realm realm, EntityType type, string parentId)
    {
        ArgumentNullException.ThrowIfNull(realm);
        return realm
            .All<AdditionalInformation>()
            .Where(addtl => addtl.ParentId == parentId && addtl.ParentTypeInt == (int)type);
    }
}
