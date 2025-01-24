using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace VisitzApi.Models.Caseload;

public class SectionJson<RecordType> where RecordType : BaseRecordJson
{
    [JsonRequired]
    [JsonPropertyName("assignedIds")]
    public List<string> AssignedIds { get; set; }

    [JsonRequired]
    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("message")]
    public JsonObject Message { get; set; }

    [JsonPropertyName("items")]
    public List<RecordType> Items { get; set; }
}
