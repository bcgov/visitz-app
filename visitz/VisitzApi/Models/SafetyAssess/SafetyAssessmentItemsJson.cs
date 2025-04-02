using System.Text.Json.Serialization;

namespace VisitzApi.Models.SafetyAssess;

internal class SafetyAssessmentItemsJson
{
    [JsonPropertyName("items")]
    public IList<SafetyAssessmentItemJson> Items { get; set; }
}
