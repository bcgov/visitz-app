using System.Net;

namespace VisitzApi.ErrorHandling;

public class VisitzApiException(HttpStatusCode statusCode, string errorMessage) : Exception(errorMessage)
{
    public HttpStatusCode HttpStatusCode { get; private set; } = statusCode;

    public bool IsError => IsErroneousStatus(HttpStatusCode) || Message.Length > 0;

    internal static bool IsErroneousStatus(HttpStatusCode statusCode)
    {
        return (int)statusCode >= 400;
    }
}
