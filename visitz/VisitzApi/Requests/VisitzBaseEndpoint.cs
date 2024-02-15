using VisitzApi.ErrorHandling;

namespace VisitzApi.Requests
{
    internal abstract class VisitzBaseEndpoint<ResponseType>(string baseUrl, string requestPath)
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
        public string RequestPath { get; } = requestPath;

        public string RequestUrl => BaseUrl.TrimEnd('/') + "/" + RequestPath.TrimStart('/');
        public Uri RequestUri => new(RequestUrl);

        public abstract HttpRequestMessage MakeRequest();

        public virtual void ThrowOnHttpErrors(HttpResponseMessage response, string content)
        {
            if (VisitzApiException.IsErroneousStatus(response.StatusCode))
            {
                if (!KongJsonMessage.TryFindMessage(content, out string message))
                    message = content;

                throw new VisitzApiException(response.StatusCode, message);
            }
        }

        public virtual void ThrowOnWebMethodsErrors(HttpResponseMessage response, string content)
        {
            if (WebMethodsJsonError.TryFindFirstError(content, out string errorMessage))
                throw new VisitzApiException(response.StatusCode, errorMessage);
        }

        public abstract ResponseType HandleResponse(string responseContent);
    }
}
