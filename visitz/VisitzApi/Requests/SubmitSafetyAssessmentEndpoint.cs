using System.Text.Json;
using System.Text.Json.Nodes;
using VisitzApi.Json;
using VisitzApi.Models.SafetyAssess;

namespace VisitzApi.Requests;

internal class SubmitSafetyAssessmentEndpoint(string baseUrl, SubmitSafetyAssessmentJson safetyAssessment)
    : VisitzBaseEndpoint<(bool success, string status)>(baseUrl, Vpi.V1, SubmitSafetyAssessmentPath)
{
    public static readonly string SubmitSafetyAssessmentPath = "/622";

    public static readonly string SafetyAssessmentKey = "safetyAssessment";

    public readonly SubmitSafetyAssessmentJson SafetyAssessment = safetyAssessment;

    private string RequestPayload
    {
        get
        {
            var assessmentJson = JsonSerializer.Serialize(SafetyAssessment, PayloadOptions.Default);

            return new JsonObject
            {
                [SafetyAssessmentKey] = new JsonObject
                {
                    [JsonKey.Payload] = JsonNode.Parse(assessmentJson)
                }
            }.ToString();
        }
    }

    public override HttpRequestMessage MakeRequest()
    {
        return new HttpRequestMessage()
        {
            Content = new FormUrlEncodedContent(FormDataCollection(JsonKey.DocRequest, RequestPayload)),
            Method = HttpMethod.Post,
            RequestUri = RequestUri,
        };
    }

    public override (bool success, string status) HandleResponse(HttpResponseMessage _, string responseContent)
    {
        var payload = JsonDocument.Parse(responseContent)
                .RootElement
                .GetProperty(JsonKey.StatusResponse)
                .GetProperty(JsonKey.Payload);

        var status = payload.GetProperty(JsonKey.Status).GetString();

        return (status.Equals(JsonKey.Success), status);
    }
}
