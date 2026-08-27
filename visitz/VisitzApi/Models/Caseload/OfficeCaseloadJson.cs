using System.Text.Json.Serialization;

namespace VisitzApi.Models.Caseload;

public class OfficeCaseloadJson : CaseloadJson
{
    [JsonPropertyName("officeNames")]
    public List<string> OfficeNames { get; set; } = [];
}
