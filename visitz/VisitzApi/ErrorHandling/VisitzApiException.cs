using VisitzApi.Extensions;
using System.Net;
using System.Text.Json;

namespace VisitzApi.ErrorHandling
{
    public class VisitzApiException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; private set; }

        public bool IsError => IsErroneousStatus(HttpStatusCode) || Message.Length > 0;

        internal VisitzApiException(HttpStatusCode statusCode, string errorMessage) : base(errorMessage)
        {
            HttpStatusCode = statusCode;
        }

        internal static bool IsErroneousStatus(HttpStatusCode statusCode)
        {
            return (int)statusCode >= 400;
        }
    }
}
