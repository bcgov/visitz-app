using Realms;
using VisitzApi.Models.CallDetails;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models.EntityTypes;

namespace VisitzModel.Models.CallDetails;

public partial class AdditionalInformation :
    IRealmObject,
    IApiJson<AdditionalInformationJson>
{
    [PrimaryKey]
    public string Id { get; set; }

    public string ParentId { get; set; }

    private int ParentTypeInt { get; set; } = (int)EntityType.Case;
    public string AdditionalInformations { get; set; }
    
    public string Created { get; set; }

    public string CreatedBy { get; set; }

    public string CreatedByName { get; set; }

    public string Updated { get; set; }

    public string UpdatedBy { get; set; }

    public string UpdatedByName { get; set; }

    public string IncidentId { get; set; }

    public AdditionalInformation() { }

    public AdditionalInformation(AdditionalInformationJson json)
    {
        Id = json.Id;
        Created = json.Created;
        AdditionalInformations = json.AdditionalInformation;
        CreatedBy = json.CreatedBy;
        CreatedByName = json.CreatedByName;
        Updated = json.Updated;
        UpdatedBy = json.UpdatedBy;
        UpdatedByName = json.UpdatedByName;
        IncidentId = json.IncidentId;
    }

    public AdditionalInformationJson ToApiJson(string dateFormat = "s")
    {
        return new()
        {
            Created = Created,
            AdditionalInformation= AdditionalInformations,
            CreatedBy = CreatedBy,
            CreatedByName = CreatedByName,
            Updated = Updated,
            UpdatedBy = UpdatedBy,
            IncidentId = IncidentId,
            Id = Id,
            UpdatedByName = UpdatedByName
        };
    }

    public static List<AdditionalInformation> FromApiJsonArray(
        IEnumerable<AdditionalInformationJson> jsonArray)
    {
        List<AdditionalInformation> outList = [];

        if (jsonArray != null)
            foreach (var jsonItem in jsonArray)
                outList.Add(new AdditionalInformation(jsonItem));

        return outList;
    }

    public static async Task SynchronizeAsync(
        Realm realm,
        IEnumerable<AdditionalInformationJson> additionalInformation, string parentId,
        EntityType type)
    {
        if (additionalInformation == null)
            return;

        var incomingadditionalinformation = FromApiJsonArray(additionalInformation);
        var incomingIncidentConcernIds = incomingadditionalinformation.Select(item => item.Id);

        var allIncidentConcerns = realm.All<AdditionalInformation>();
        var allIncidentConcernIds = allIncidentConcerns.AsEnumerable().Select(item => item.Id);

        var additionalInformationIdsToDelete = allIncidentConcernIds.Except(incomingIncidentConcernIds);
        var incidentConcernsToDelete = allIncidentConcerns.ToList().Where(item => additionalInformationIdsToDelete.Contains(item.Id));

        if (!incidentConcernsToDelete.Any() && !incomingIncidentConcernIds.Any())
            return;

        await RealmExtensions.CommitAsync(realm, () =>
        {
            foreach (var item in incidentConcernsToDelete)
            {
                if (item != null && item.IsValid)
                    realm.Remove(item);
            }

            realm.Upsert(incomingadditionalinformation);
        });
    }
    public static void RemoveByParent(Realm realm, EntityType type, string parentId)
    {
        var visitItems = realm.All<AdditionalInformation>()
            .Where(item => item.ParentId == parentId && item.ParentTypeInt == (int)type);

        realm.RemoveRange(visitItems);
    }
}
