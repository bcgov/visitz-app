using System.Net;
using System.Net.Http.Json;
using VisitzApi.Json;
using VisitzApi.Models.People;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints.People;

internal class SubmitContactEducationEndpoint(
    string baseUrl,
    ApiRecordType recordType,
    string parentRowId,
    string parentContactId,
    SubmitContactEducationJson submitJson
)
    : VisitzBaseEndpoint<bool>(
        baseUrl,
        Vpi.V2,
        GetContactEducationEndpoint.MakePath(recordType, parentRowId, parentContactId)
    )
{
    public override HttpRequestMessage MakeRequest()
    {
        return new()
        {
            Content = JsonContent.Create(submitJson, options: PayloadOptions.MiddlewarePost),
            Method = HttpMethod.Post,
            RequestUri = RequestUri,
        };
    }

    public override bool HandleResponse(HttpResponseMessage response, string responseContent)
    {
        return response.StatusCode == HttpStatusCode.OK;
    }
}
