using System.Text.Json;
using VisitzApi.Extensions;
using VisitzApi.Json;
using VisitzApi.Models.Caseload;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints.Caseload;

internal class GetCaseloadEndpoint(string baseUrl, Pagination? pagination = null)
    : VisitzBaseEndpoint<(int TotalRecords, CaseloadJson)>(baseUrl, Vpi.V2, CaseloadPath)
{
    static readonly string CaseloadPath = "/caseload";

    readonly Pagination? Pagination = pagination;

    public override HttpRequestMessage MakeRequest()
    {
        return new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = WithQueryParams(pagination: Pagination),
        };
    }

    public override (int TotalRecords, CaseloadJson) HandleResponse(
        HttpResponseMessage response,
        string responseContent
    )
    {
        var json =
            JsonSerializer.Deserialize<CaseloadJson>(responseContent, PayloadOptions.SiebelGet) ?? new CaseloadJson();

        return (response.GetRecordCount(), json);
    }
}
