using System.Text.Json.Serialization;

namespace VisitzApi.Models.SafetyAssess;

internal class SafetyAssessmentItemJson
{
    public string Id { get; set; }

    [JsonPropertyName("ICM Incident SafetyAssessment BC")]
    public IList<SafetyAsessmentJson> IcmIncidentSafetyAssessmentBc { get; set; }
}
