using System.Text.Json.Serialization;

namespace VisitzApi.Models.Notes;

public class ResponseNarrativeJson : INoteJson
{
    public DateTimeOffset Created { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public string CreatedByName { get; set; } = string.Empty;

    public string CreatedByOffice { get; set; } = string.Empty;

    public string Id { get; set; } = string.Empty;

    public string IncidentId { get; set; } = string.Empty;

    [JsonPropertyName("SR Id")]
    public string SrId { get; set; } = string.Empty;

    [JsonPropertyName("Response")]
    public string Text { get; set; } = string.Empty;

    public DateTimeOffset Updated { get; set; }

    public string UpdatedBy { get; set; } = string.Empty;

    public string UpdatedByName { get; set; } = string.Empty;
}
