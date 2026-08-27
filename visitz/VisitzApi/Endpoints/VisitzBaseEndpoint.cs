using System.Net;
using System.Text;
using System.Text.Json;
using System.Web;
using VisitzApi.ErrorHandling;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints;

internal abstract class VisitzBaseEndpoint<ResponseType>(string baseUrl, string version, string requestPath)
{
    protected static KeyValuePair<string, string> FormDataPair(string key, string value)
    {
        return new KeyValuePair<string, string>(key, value);
    }

    protected static IEnumerable<KeyValuePair<string, string>> FormDataCollection(string key, string value)
    {
        return [FormDataPair(key, value)];
    }

    public string BaseUrl { get; } = baseUrl;
    public string Version { get; } = version;
    public string RequestPath { get; } = requestPath;

    public string RequestUrl => BaseUrl.TrimEnd('/') + "/" + Version.Trim('/') + "/" + RequestPath.TrimStart('/');
    public Uri RequestUri => new(RequestUrl);

    public abstract HttpRequestMessage MakeRequest();

    public virtual void ThrowOnHttpErrors(HttpResponseMessage response, ResponseBodyParser bodyParser)
    {
        if (VisitzApiException.IsErroneousStatus(response.StatusCode))
        {
            string? message = null;

            if (bodyParser.FindFirstMessage() is JsonElement msgElement)
            {
                if (msgElement.ValueKind == JsonValueKind.String)
                    message = msgElement.GetString();
                else if (msgElement.ValueKind == JsonValueKind.Array)
                {
                    StringBuilder stringBuilder = new();
                    foreach (JsonElement element in msgElement.EnumerateArray())
                    {
                        if (element.GetString() is string text)
                            stringBuilder.Append("•" + text + Environment.NewLine);
                    }

                    if (stringBuilder.Length > 0)
                        message = stringBuilder.ToString();
                }
            }

            throw new VisitzApiException(
                response.StatusCode,
                BuildMessage(response.StatusCode, message ?? bodyParser.ResponseBody)
            );
        }
    }

    public virtual void ThrowOnErrorsInBody(HttpResponseMessage response, ResponseBodyParser bodyParser)
    {
        if (bodyParser.GetSuccessStatusFromBody() ?? false)
            return;

        string? error = bodyParser.FindFirstError();
        if (error == null)
            return;

        HttpStatusCode code =
            (int)response.StatusCode >= 200 && (int)response.StatusCode < 300
                ? HttpStatusCode.BadRequest
                : response.StatusCode;

        string message = BuildMessage(code, error);
        throw new VisitzApiException(code, message);
    }

    public abstract ResponseType HandleResponse(HttpResponseMessage response, string responseContent);

    static string BuildMessage(HttpStatusCode code, string message)
    {
        return $"HTTP {(int)code} -> {code}" + Environment.NewLine + Environment.NewLine + message;
    }

    protected Uri WithQueryParams(
        Pagination? pagination = null,
        string format = "s",
        bool excludeEmptyFields = true,
        params (string Name, string Value)[] @params
    )
    {
        return WithQueryParams(
            pagination?.RowOffset,
            pagination?.PageSize,
            recordCountNeeded: pagination != null,
            pagination?.After,
            format,
            excludeEmptyFields,
            @params
        );
    }

    protected Uri WithQueryParams(
        int? rowOffset = null,
        int? pageSize = null,
        bool? recordCountNeeded = null,
        DateTimeOffset? after = null,
        string format = "s",
        bool excludeEmptyFields = true,
        params (string Name, string Value)[] extraParams
    )
    {
        var query = HttpUtility.ParseQueryString(RequestUri.Query);

        if (rowOffset is int offset)
        {
            query[RequestParam.StartRowNum] = offset.ToString();
            recordCountNeeded ??= true;
        }

        if (pageSize is int size)
        {
            query[RequestParam.PageSize] = size.ToString();
            recordCountNeeded ??= true;
        }

        if (recordCountNeeded is bool getCount)
            query[RequestParam.RecordCountNeeded] = getCount.ToString().ToLowerInvariant();

        if (after is DateTimeOffset timestamp)
            query[RequestParam.Since] = timestamp.ToString(format);

        query[RequestParam.ExcludeEmptyFields] = excludeEmptyFields ? "Y" : "N";

        foreach (var (name, value) in extraParams)
            query[name] = value;

        var urlWithoutQuery = RequestUri.ToString().Split('?')[0];
        string queryString = query.ToString() ?? string.Empty;

        return new Uri(urlWithoutQuery + "?" + queryString);
    }
}
