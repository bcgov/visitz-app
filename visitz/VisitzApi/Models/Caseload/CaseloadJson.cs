using System.Text.Json.Serialization;

namespace VisitzApi.Models.Caseload;

public class CaseloadJson
{
    [JsonRequired]
    [JsonPropertyName("cases")]
    public SectionJson<CaseJson> Cases { get; set; }

    // TODO: Incidents, Memos, SRs
}
