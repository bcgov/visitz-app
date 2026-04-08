using System.Net.Http.Json;
using System.Text.Json;
using VisitzApi.Json;
using VisitzApi.Models.SafetyAssess;

namespace VisitzApi.Endpoints.SafetyAssess;

internal class SubmitSafetyAssessmentEndpoint(string baseUrl, SubmitSafetyAssessmentJson safetyAssessment)
    : VisitzBaseEndpoint<(bool success, string status)>(baseUrl, Vpi.V2, SubmitSafetyAssessmentPath)
{
    public static readonly string SubmitSafetyAssessmentPath = "/wf/submit-safety-assessment";

    public static readonly string SafetyAssessmentKey = "safetyAssessment";

    public readonly SubmitSafetyAssessmentJson SafetyAssessment = safetyAssessment;

    public override HttpRequestMessage MakeRequest()
    {
        return new HttpRequestMessage()
        {
            Content = JsonContent.Create(SafetyAssessment),
            Method = HttpMethod.Post,
            RequestUri = RequestUri,
        };
    }

    public override (bool success, string status) HandleResponse(HttpResponseMessage _, string responseContent)
    {
        var json = JsonDocument.Parse(responseContent).RootElement;

        string statusText = json.GetProperty(JsonKey.Status).GetString() ?? "";
        bool status = statusText == JsonKey.Success;

        return (status, statusText);
    }
}
