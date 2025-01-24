using System.Text.Json.Serialization;

namespace VisitzApi.Models;

public class CaseJson : BaseRecordJson
{
    public string Caseload { get; set; }

    public string CaseNum { get; set; }

    public string ClosedDate { get; set; }

    public string CloseReason { get; set; }

    public string EarlyOpenReason { get; set; }

    public string IntegrationState { get; set; }

    public string LegacyFileNumber { get; set; }

    public string MiddleName { get; set; }

    [JsonPropertyName("MyFS Flag")]
    public string MyFSFlag { get; set; }

    public string Name { get; set; }

    public string OfficeName { get; set; }

    public string Organization { get; set; }

    public string RegionName { get; set; }

    public DateTimeOffset RenewReviewDate { get; set; }

    public DateTimeOffset ReopenedDate { get; set; }

    public string RestrictedFlag { get; set; }

    public string Status { get; set; }

    public string SubjectContactFirstName { get; set; }

    public string SubjectContactLastName { get; set; }

    public string Type { get; set; }

    public string WorkQueue { get; set; }
}
