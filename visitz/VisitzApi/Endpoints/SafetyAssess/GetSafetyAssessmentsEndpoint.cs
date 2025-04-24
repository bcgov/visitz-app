using System.Net;
using System.Text.Json;
using VisitzApi.Json;
using VisitzApi.Models.SafetyAssess;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints.SafetyAssess;

#nullable enable

internal class GetSafetyAssessmentsEndpoint(
    string baseUrl,
    string incidentId,
    Pagination? pagination = null)
    : VisitzBaseEndpoint<IEnumerable<SafetyAsessmentJson>>(
        baseUrl,
        Vpi.V2,
        MakePath(incidentId))
{
    static readonly string AssessmentsPath = "/incident/{0}/safety-assessments";

    readonly Pagination? Pagination = pagination;

    static string MakePath(string incidentId)
    {
        return string.Format(AssessmentsPath, incidentId);
    }

    public override HttpRequestMessage MakeRequest()
    {
        return new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = WithQueryParams(Pagination),
        };
    }

    public override IEnumerable<SafetyAsessmentJson> HandleResponse(
        HttpResponseMessage response,
        string responseContent)
    {
        if (response.StatusCode == HttpStatusCode.NoContent)
            return [];

        var json = JsonSerializer.Deserialize<SafetyAssessmentItemsJson>
            (responseContent, PayloadOptions.SiebelGet);

        return json?.Items?.First().IcmIncidentSafetyAssessmentBc ?? [];
    }
}
