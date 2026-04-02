using System.Text.Json.Serialization;

namespace VisitzApi.Models.Caseload;

public class CaseloadJson
{
    [JsonRequired]
    [JsonPropertyName("cases")]
    public SectionJson<CaseJson> Cases { get; set; } = new SectionJson<CaseJson>();

    [JsonRequired]
    [JsonPropertyName("incidents")]
    public SectionJson<IncidentJson> Incidents { get; set; } = new SectionJson<IncidentJson>();

    [JsonRequired]
    [JsonPropertyName("srs")]
    public SectionJson<ServiceRequestJson> ServiceRequests { get; set; } = new SectionJson<ServiceRequestJson>();

    [JsonRequired]
    [JsonPropertyName("memos")]
    public SectionJson<MemoJson> Memos { get; set; } = new SectionJson<MemoJson>();
}
