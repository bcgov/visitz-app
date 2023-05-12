using VisitzApi.Extensions;
using System.Net;
using System.Text.Json;

namespace VisitzApi.ErrorHandling
{
    public class VisitzApiException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; private set; }

        public bool IsError => IsErroneousStatus(HttpStatusCode) || Message.Length > 0;

        private VisitzApiException(HttpStatusCode statusCode, string errorMessage) : base(errorMessage)
        {
            HttpStatusCode = statusCode;
        }

        private static bool IsErroneousStatus(HttpStatusCode statusCode)
        {
            return (int)statusCode >= 400;
        }

        internal static void ThrowIfInvalid(HttpResponseMessage response)
        {
            if (IsErroneousStatus(response.StatusCode))
                throw new VisitzApiException(response.StatusCode,
                        response.Content.ReadAsStringAsync().Result);

            else if (WebMethodsJsonError.TryFindFirstError(response, out string errorMessage))
                throw new VisitzApiException(response.StatusCode, errorMessage);
        }
    }
}
