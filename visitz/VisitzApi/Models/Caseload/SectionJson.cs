using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using VisitzApi.Models.Base;

namespace VisitzApi.Models.Caseload;

public class SectionJson<RecordType>
    where RecordType : BaseRecordJson
{
    const string messageFieldName = "message";
    const string errorFieldName = "error";

    [JsonRequired]
    [JsonPropertyName("assignedIds")]
    public List<string> AssignedIds { get; set; } = [];

    [JsonRequired]
    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("message")] // Not using const 'messageFieldName' so it is decoupled
    public JsonObject Message { get; set; } = [];

    [JsonPropertyName("items")]
    public List<RecordType> Items { get; set; } = [];

    public string GetFirstMessage()
    {
        return FindFirstStringByFieldName(messageFieldName, Message);
    }

    public string GetFirstError()
    {
        return FindFirstStringByFieldName(errorFieldName, Message);
    }

    public string GetFullDisplayError()
    {
        return GetFirstMessage() + " -> " + GetFirstError();
    }

    static string FindFirstStringByFieldName(string fieldName, JsonObject obj)
    {
        foreach (KeyValuePair<string, JsonNode?> fieldPair in obj)
        {
            if (fieldPair.Value == null)
                continue;

            if (
                fieldPair.Value.GetValueKind() == JsonValueKind.String
                && string.Equals(fieldPair.Key, fieldName, StringComparison.InvariantCultureIgnoreCase)
            )
                return fieldPair.Value.ToString();
            else if (fieldPair.Value is JsonObject nestedObj)
                return FindFirstStringByFieldName(fieldName, nestedObj);
        }

        return string.Empty;
    }
}
