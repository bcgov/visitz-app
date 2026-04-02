using System.Text.Json.Serialization;

namespace VisitzApi.Models.SafetyAssess;

internal class GetSafetyAssessmentItemsJson
{
    [JsonPropertyName("items")]
    public IList<GetSafetyAssessmentItemJson> Items { get; set; } = [];
}
