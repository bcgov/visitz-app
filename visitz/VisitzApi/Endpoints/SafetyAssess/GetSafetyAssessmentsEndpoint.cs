using System.Net;
using System.Text.Json;
using VisitzApi.Extensions;
using VisitzApi.Json;
using VisitzApi.Models.SafetyAssess;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints.SafetyAssess;

internal class GetSafetyAssessmentsEndpoint(string baseUrl, string incidentId, Pagination? pagination = null)
    : VisitzBaseEndpoint<(int TotalRecords, IEnumerable<GetSafetyAsessmentJson>)>(baseUrl, Vpi.V2, MakePath(incidentId))
{
    static readonly string AssessmentsPath = "/incident/{0}/safety-assessments";

    readonly Pagination? Pagination = pagination;

    static string MakePath(string incidentId)
    {
        return string.Format(AssessmentsPath, incidentId);
    }

    public override HttpRequestMessage MakeRequest()
    {
        return new HttpRequestMessage() { Method = HttpMethod.Get, RequestUri = WithQueryParams(Pagination) };
    }

    public override (int TotalRecords, IEnumerable<GetSafetyAsessmentJson>) HandleResponse(
        HttpResponseMessage response,
        string responseContent
    )
    {
        if (response.StatusCode == HttpStatusCode.NoContent)
            return (-1, []);

        var json = JsonSerializer.Deserialize<GetSafetyAssessmentItemsJson>(responseContent, PayloadOptions.SiebelGet);

        return (response.GetRecordCount(), json?.Items?.First().IcmIncidentSafetyAssessmentBc ?? []);
    }
}
