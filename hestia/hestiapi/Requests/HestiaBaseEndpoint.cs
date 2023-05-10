using System.Collections;

namespace hestiapi.Requests
{
    internal abstract class HestiaBaseEndpoint<ResponseType>
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

        public string BaseUrl { get; }
        public string RequestPath { get; }

        public string RequestUrl => BaseUrl.TrimEnd('/') + "/" + RequestPath.TrimStart('/');
        public Uri RequestUri => new(RequestUrl);

        public HestiaBaseEndpoint(string baseUrl, string requestPath)
        {
            BaseUrl = baseUrl;
            RequestPath = requestPath;
        }

        public abstract HttpRequestMessage MakeRequest();

        public abstract ResponseType HandleResponse(HttpResponseMessage response);
    }
}
