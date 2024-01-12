using System.Text.Json;
using System.Text.Json.Nodes;
using VisitzApi.Models.SafetyAssess;

namespace VisitzApi.Requests;

internal class SubmitSafetyAssessmentEndpoint(string baseUrl, SafetyAssessmentEntity safetyAssessment)
    : VisitzBaseEndpoint<(bool success, string status)>(baseUrl, SubmitSafetyAssessmentPath)
{
    public static readonly string SubmitSafetyAssessmentPath = "/v1/622";

    public static readonly string SafetyAssessmentKey = "safetyAssessment";

    public readonly SafetyAssessmentEntity SafetyAssessment = safetyAssessment;

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

    public override (bool success, string status) HandleResponse(string responseContent)
    {
        var payload = JsonDocument.Parse(responseContent)
                .RootElement
                .GetProperty(JsonKey.StatusResponse)
                .GetProperty(JsonKey.Payload);

        var status = payload.GetProperty(JsonKey.Status).GetString();

        return (status.Equals(JsonKey.Success), status);
    }
}
