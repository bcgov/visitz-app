#nullable enable

namespace VisitzApi.Models.CallDetails;

public class IncidentConcernsJson
{
    public string Id { get; set; }
    public string OriginalConcern { get; set; }
    public string Concern { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public string Created { get; set; }
    public string CreatedBy { get; set; }
    public string CreatedByName { get; set; }
    public string Updated { get; set; }
    public string UpdatedBy { get; set; }
    public string UpdatedByName { get; set; }
    public string IncidentId { get; set; }
}
