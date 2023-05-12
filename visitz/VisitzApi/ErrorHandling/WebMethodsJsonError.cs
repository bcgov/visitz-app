using System.Text.Json;
using VisitzApi.Extensions;

namespace VisitzApi.ErrorHandling
{
    internal class WebMethodsJsonError
    {
        private static readonly string ErrorKey = "error";
        private static readonly string ErrorDetailKey = "errorDetail";

        internal static bool TryFindFirstError(HttpResponseMessage response, out string errorMessage)
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
    }
}
