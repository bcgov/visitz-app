using Realms;
using VisitzApi.Models.CallDetails;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Utilities;

namespace VisitzModel.Models.CallDetails;

public partial class IncidentConcerns : IRealmObject, IApiJson<IncidentConcernsJson>
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string OriginalConcern { get; set; } = string.Empty;

    public string Concern { get; set; } = string.Empty;

    public DateTimeOffset? StartDate { get; set; }

    public DateTimeOffset? EndDate { get; set; }

    public string Created { get; set; } = string.Empty;

    public string CreatedBy { get; set; } = string.Empty;

    public string CreatedByName { get; set; } = string.Empty;

    public string Updated { get; set; } = string.Empty;

    public string UpdatedBy { get; set; } = string.Empty;

    public string UpdatedByName { get; set; } = string.Empty;

    public string IncidentId { get; set; } = string.Empty;

    public IncidentConcerns() { }

    public IncidentConcerns(IncidentConcernsJson json)
    {
        Id = json.Id;
        OriginalConcern = json.OriginalConcern;
        Concern = json.Concern;
        StartDate = Timestamp.ParseDateTimeOffsetNullable(json.StartDate);
        EndDate = Timestamp.ParseDateTimeOffsetNullable(json.EndDate);
        Created = json.Created;
        CreatedBy = json.CreatedBy;
        CreatedByName = json.CreatedByName;
        Updated = json.Updated;
        UpdatedBy = json.UpdatedBy;
        UpdatedByName = json.UpdatedByName;
        IncidentId = json.IncidentId;
    }

    public IncidentConcernsJson ToApiJson(string dateFormat = "s")
    {
        return new()
        {
            Concern = Concern,
            Created = Created,
            CreatedBy = CreatedBy,
            CreatedByName = CreatedByName,
            Updated = Updated,
            UpdatedBy = UpdatedBy,
            IncidentId = IncidentId,
            EndDate = EndDate?.ToString(dateFormat) ?? string.Empty,
            Id = Id,
            OriginalConcern = OriginalConcern,
            StartDate = StartDate?.ToString(dateFormat) ?? string.Empty,
            UpdatedByName = UpdatedByName,
        };
    }

    public static List<IncidentConcerns> FromApiJsonArray(IEnumerable<IncidentConcernsJson> jsonArray)
    {
        List<IncidentConcerns> outList = [];

        if (jsonArray != null)
            foreach (var jsonItem in jsonArray)
                outList.Add(new IncidentConcerns(jsonItem));

        return outList;
    }

    public static async Task SynchronizeAsync(
        Realm realm,
        IEnumerable<IncidentConcernsJson> newIncidentConcerns,
        string parentId
    )
    {
        if (newIncidentConcerns == null)
            return;

        var incomingIncidentConcerns = FromApiJsonArray(newIncidentConcerns);
        var incomingIncidentConcernIds = incomingIncidentConcerns.Select(item => item.Id);

        var allIncidentConcerns = realm.All<IncidentConcerns>().Where(item => item.IncidentId == parentId);
        var allIncidentConcernIds = allIncidentConcerns.AsEnumerable().Select(item => item.Id);

        var incidentConcernIdsToDelete = allIncidentConcernIds.Except(incomingIncidentConcernIds);
        var incidentConcernsToDelete = allIncidentConcerns
            .ToList()
            .Where(item => incidentConcernIdsToDelete.Contains(item.Id));

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

                realm.Upsert(incomingIncidentConcerns);
            }
        );
    }

    public static void RemoveByParent(Realm realm, string parentIncidentId)
    {
        var incidentConcerns = realm
            .All<IncidentConcerns>()
            .Where(item => item.IncidentId == parentIncidentId)
            .ToList();

        foreach (var item in incidentConcerns)
        {
            realm.Remove(item);
        }
    }
}
