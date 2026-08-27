using System.Text.Json.Serialization;
using VisitzApi.Models.Base;

namespace VisitzApi.Models.Caseload;

public class CaseJson : AssignableRecordJson
{
    public string Caseload { get; set; } = string.Empty;

    public string CaseNum { get; set; } = string.Empty;

    public string ClosedDate { get; set; } = string.Empty;

    public string CloseReason { get; set; } = string.Empty;

    public string EarlyOpenReason { get; set; } = string.Empty;

    public string IntegrationState { get; set; } = string.Empty;

    public string LegacyFileNumber { get; set; } = string.Empty;

    public string MiddleName { get; set; } = string.Empty;

    [JsonPropertyName("MyFS Flag")]
    public string MyFSFlag { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string OfficeName { get; set; } = string.Empty;

    public string Organization { get; set; } = string.Empty;

    public string RegionName { get; set; } = string.Empty;

    public string RenewReviewDate { get; set; } = string.Empty;

    public string ReopenedDate { get; set; } = string.Empty;

    public string RestrictedFlag { get; set; } = string.Empty;

    public List<PositionSummary> Position { get; set; } = [];

    public string Status { get; set; } = string.Empty;

    public string SubjectContactFirstName { get; set; } = string.Empty;

    public string SubjectContactLastName { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string WorkQueue { get; set; } = string.Empty;
}
