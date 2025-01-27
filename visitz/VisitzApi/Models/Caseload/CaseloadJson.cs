using System.Text.Json.Serialization;

namespace VisitzApi.Models.Caseload;

public class CaseloadJson
{
    [JsonRequired]
    [JsonPropertyName("cases")]
    public SectionJson<CaseJson> Cases { get; set; }

    [JsonRequired]
    [JsonPropertyName("incidents")]
    public SectionJson<IncidentJson> Incidents { get; set; }
}
