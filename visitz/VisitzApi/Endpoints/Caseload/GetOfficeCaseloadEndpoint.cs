using System.Text.Json;
using VisitzApi.Extensions;
using VisitzApi.Json;
using VisitzApi.Models.Caseload;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints.Caseload;

internal class GetOfficeCaseloadEndpoint(string baseUrl, Pagination? pagination = null)
    : VisitzBaseEndpoint<(int TotalRecords, OfficeCaseloadJson)>(baseUrl, Vpi.V2, OfficeCaseloadPath)
{
    static readonly string OfficeCaseloadPath = "/office-caseload";

    readonly Pagination? Pagination = pagination;

    public override HttpRequestMessage MakeRequest()
    {
        return new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = WithQueryParams(pagination: Pagination),
        };
    }

    public override (int TotalRecords, OfficeCaseloadJson) HandleResponse(
        HttpResponseMessage response,
        string responseContent
    )
    {
        var json =
            JsonSerializer.Deserialize<OfficeCaseloadJson>(responseContent, PayloadOptions.SiebelGet)
            ?? new OfficeCaseloadJson();

        return (response.GetRecordCount(), json);
    }
}
