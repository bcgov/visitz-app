using System.Net.Http.Json;
using VisitzApi.Models.AppLogs;

namespace VisitzApi.Endpoints.AppLogs;

internal class SendAppLogsEndpoint(string baseUrl, IList<AppLogJson> logs)
    : VisitzBaseEndpoint<HttpResponseMessage>(baseUrl, Vpi.V2, AppLogsPath)
{
    const string AppLogsPath = "/app-logs";

    readonly IList<AppLogJson> _logs = logs;

    public override HttpRequestMessage MakeRequest()
    {
        return new() { Content = JsonContent.Create(_logs), Method = HttpMethod.Post };
    }

    public override HttpResponseMessage HandleResponse(HttpResponseMessage response, string _)
    {
        return response;
    }
}
