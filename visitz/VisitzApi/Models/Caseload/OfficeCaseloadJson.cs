using System.Text.Json.Serialization;

namespace VisitzApi.Models.Caseload;

public class OfficeCaseloadJson : CaseloadJson
{
    public static readonly new OfficeCaseloadJson Empty = new()
    {
        Cases = CaseloadJson.Empty.Cases,
        Incidents = CaseloadJson.Empty.Incidents,
        ServiceRequests = CaseloadJson.Empty.ServiceRequests,
        Memos = CaseloadJson.Empty.Memos,
        OfficeNames = [],
    };

    [JsonPropertyName("officeNames")]
    public List<string> OfficeNames;
}
