using System.Text.Json;
using VisitzApi.Json;
using VisitzApi.Models.Caseload;

#nullable enable

namespace VisitzApi.Endpoints.Caseload;

internal class GetCaseloadEndpoint(string baseUrl, DateTimeOffset? after = null)
    : VisitzBaseEndpoint<CaseloadJson>(baseUrl, Vpi.V2, CaseloadPath)
{
    static readonly string CaseloadPath = "/caseload";

    readonly DateTimeOffset? After = after;

    public override HttpRequestMessage MakeRequest()
    {
        return new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = WithQueryParams(after: After),
        };
    }

    public override CaseloadJson HandleResponse(HttpResponseMessage _, string responseContent)
    {
        return JsonSerializer.Deserialize<CaseloadJson>(responseContent, PayloadOptions.SiebelGet)
            ?? CaseloadJson.Empty;
    }
}
