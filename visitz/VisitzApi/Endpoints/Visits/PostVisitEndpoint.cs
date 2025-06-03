using System.Net;
using System.Net.Http.Json;
using VisitzApi.Json;
using VisitzApi.Models.Visits;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints.Visits;

internal class PostVisitEndpoint(string baseUrl, string caseId, PostVisitJson visitToSend)
    : VisitzBaseEndpoint<bool>(baseUrl, Vpi.V2, MakePath(caseId))
{
    static readonly string PostVisitPath = "/case/{0}/visits";

    PostVisitJson VisitToSend => visitToSend;

    static string MakePath(string caseId)
    {
        return string.Format(PostVisitPath, caseId);
    }

    public override HttpRequestMessage MakeRequest()
    {
        RemoveTimeFromDateOfVisit(VisitToSend);

        return new()
        {
            Content = JsonContent.Create(VisitToSend, options: PayloadOptions.MiddlewarePost),
            Method = HttpMethod.Post,
            RequestUri = RequestUri,
        };
    }

    public override bool HandleResponse(HttpResponseMessage response, string _)
    {
        return response.StatusCode == HttpStatusCode.OK;
    }

    static void RemoveTimeFromDateOfVisit(PostVisitJson visit)
    {
        visit.DateOfVisit = visit.DateOfVisit.Date;
    }
}
