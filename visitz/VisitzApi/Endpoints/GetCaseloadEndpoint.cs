using System.Text.Json;
using VisitzApi.Json;
using VisitzApi.Models.Caseload;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints;

internal class GetCaseloadEndpoint(string baseUrl, DateTimeOffset? after = null)
    : VisitzBaseEndpoint<CaseloadJson>(baseUrl, Vpi.V2, CaseloadPath)
{
    static readonly string CaseloadPath = "/caseload";

    readonly DateTimeOffset? After = after;

    public override HttpRequestMessage MakeRequest()
    {
        Uri uri = RequestUri;

        if (After is DateTimeOffset after)
            uri = new Uri(uri, $"?{ParamNames.Since}=" + after.ToString("s"));

        return new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = uri
        };
    }

    public override CaseloadJson HandleResponse(string responseContent)
    {
        return JsonSerializer.Deserialize<CaseloadJson>(responseContent, PayloadOptions.SiebelGet);
    }    
}
