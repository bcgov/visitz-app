using System.Net;
using System.Text.Json.Serialization;

namespace VisitzApi.Models.Caseload;

public class CaseloadJson
{
    public static readonly CaseloadJson Empty = new()
    {
        Cases = new() { Status = (int)HttpStatusCode.NoContent, Items = [] },
        Incidents = new() { Status = (int)HttpStatusCode.NoContent, Items = [] },
        ServiceRequests = new() { Status = (int)HttpStatusCode.NoContent, Items = [] },
        Memos = new() { Status = (int)HttpStatusCode.NoContent, Items = [] },
    };

    [JsonRequired]
    [JsonPropertyName("cases")]
    public SectionJson<CaseJson> Cases { get; set; }

    [JsonRequired]
    [JsonPropertyName("incidents")]
    public SectionJson<IncidentJson> Incidents { get; set; }

    [JsonRequired]
    [JsonPropertyName("srs")]
    public SectionJson<ServiceRequestJson> ServiceRequests { get; set; }

    [JsonRequired]
    [JsonPropertyName("memos")]
    public SectionJson<MemoJson> Memos { get; set; }
}
