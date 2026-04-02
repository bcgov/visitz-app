using System.Text.Json.Serialization;

namespace VisitzApi.Models.SafetyAssess;

internal class GetSafetyAssessmentItemJson
{
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("ICM Incident SafetyAssessment BC")]
    public IList<GetSafetyAsessmentJson> IcmIncidentSafetyAssessmentBc { get; set; } = [];
}
