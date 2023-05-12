using VisitzApi.Extensions;
using System.Net;
using System.Text.Json;

namespace VisitzApi.ErrorHandling
{
    public class VisitzApiException : Exception
    {
        private static readonly string ErrorKey = "error";
        private static readonly string ErrorDetailKey = "errorDetail";

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

        private static bool TryFindFirstJsonError(HttpResponseMessage response, out string errorMessage)
        {
            var json = JsonDocument.Parse(response.Content.ReadAsStringAsync().Result);
            return TryFindError(json.RootElement.FirstProperty(), out errorMessage);
        }

        private static bool TryFindError(JsonElement element, out string errorMessage)
        {
            errorMessage = string.Empty;

            return element.TryGetProperty(JsonKey.Payload, out var payload)
                && payload.TryGetProperty(ErrorKey, out var errorArray)
                && TryGetErrorDetail(errorArray, out errorMessage);
        }

        private static bool TryGetErrorDetail(JsonElement array, out string errorMessage)
        {
            errorMessage = string.Empty;
            var errorElement = array.FirstArrayElement().FirstProperty();

            if (errorElement.TryGetProperty(ErrorDetailKey, out var error))
                errorMessage = error.GetString();

            return errorMessage.Length > 0;
        }

        internal static void ThrowIfInvalid(HttpResponseMessage response)
        {
            if (IsErroneousStatus(response.StatusCode))
                throw new VisitzApiException(response.StatusCode,
                        response.Content.ReadAsStringAsync().Result);

            else if (TryFindFirstJsonError(response, out string errorMessage))
                throw new VisitzApiException(response.StatusCode, errorMessage);
        }
    }
}
