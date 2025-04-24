using System.Net;
using System.Web;
using VisitzApi.ErrorHandling;

namespace VisitzApi.Requests
{
    internal abstract class VisitzBaseEndpoint<ResponseType>(string baseUrl, string version, string requestPath)
    {
        protected static KeyValuePair<string, string> FormDataPair(string key, string value)
        {
            return new KeyValuePair<string, string>(key, value);
        }

        protected static IEnumerable<KeyValuePair<string, string>> FormDataCollection(string key, string value)
        {
            return new List<KeyValuePair<string, string>>()
            {
                FormDataPair(key, value)
            };
        }

        public string BaseUrl { get; } = baseUrl;
        public string Version { get; } = version;
        public string RequestPath { get; } = requestPath;

        public string RequestUrl => BaseUrl.TrimEnd('/')
            + "/" + Version.Trim('/')
            + "/" + RequestPath.TrimStart('/');
        public Uri RequestUri => new(RequestUrl);

        public abstract HttpRequestMessage MakeRequest();

        public virtual void ThrowOnHttpErrors(HttpResponseMessage response, string content)
        {
            if (VisitzApiException.IsErroneousStatus(response.StatusCode))
            {
                if (!KongJsonMessage.TryFindMessage(content, out string message))
                    message = content;

                throw new VisitzApiException(response.StatusCode, BuildMessage(response.StatusCode, message));
            }
        }

        public virtual void ThrowOnWebMethodsErrors(HttpResponseMessage response, string content)
        {
            if (WebMethodsJsonError.TryFindFirstError(content, out string errorMessage))
                throw new VisitzApiException(response.StatusCode, BuildMessage(response.StatusCode, errorMessage));
        }

        public abstract ResponseType HandleResponse(HttpResponseMessage response, string responseContent);

        static string BuildMessage(HttpStatusCode code, string message)
        {
            return $"HTTP {(int)code} {code} {message}";
        }

        protected Uri WithQueryParams(Pagination pagination = null, string format = "s")
        {
            return WithQueryParams(
                pagination?.RowOffset,
                pagination?.PageSize,
                recordCountNeeded: pagination != null,
                pagination?.After,
                format);
        }

        protected Uri WithQueryParams(
            int? rowOffset = null,
            int? pageSize = null,
            bool? recordCountNeeded = null,
            DateTimeOffset? after = null,
            string format = "s")
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
                query[RequestParam.RecordCountNeeded] = getCount.ToString();

            if (after is DateTimeOffset timestamp)
                query[RequestParam.Since] = timestamp.ToString(format);

            var urlWithoutQuery = RequestUri.ToString().Split('?')[0];
            string queryString = query.ToString();

            return new Uri(urlWithoutQuery + "?" + queryString);
        }
    }
}
