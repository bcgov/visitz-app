using System.Text.Json;
using System.Text.Json.Nodes;
using VisitzApi.Models.SafetyAssess;

namespace VisitzApi.Requests;

internal class SubmitSafetyAssessmentEndpoint(string baseUrl, SafetyAssessmentEntity safetyAssessment)
    : VisitzBaseEndpoint<bool>(baseUrl, SubmitSafetyAssessmentPath)
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

    public override bool HandleResponse(string responseContent)
    {
        throw new NotImplementedException();
    }
}
