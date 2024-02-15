using System.Text.Json;

namespace VisitzApi.ErrorHandling
{
    internal class WebMethodsJsonError
    {
        private static readonly string StatusKey = "status";
        private static readonly string ErrorKey = "error";
        private static readonly string ErrorsKey = "errors";
        private static readonly string ErrorDetailKey = "errorDetail";

        internal static bool TryFindFirstError(string content, out string errorMessage)
        {
            errorMessage = string.Empty;
            var json = JsonDocument.Parse(content);

            var jsonPayload = FindPayload(json.RootElement);

            return jsonPayload is JsonElement payload
                && (TryFindStatusResponse(payload, out errorMessage) || TryFindError(payload, out errorMessage));
        }

        private static JsonElement? FindPayload(JsonElement element)
        {
            if (element.TryGetProperty(JsonKey.PayLoad, out JsonElement payload))
                return payload;
            else if (element.TryGetProperty(JsonKey.Payload, out payload))
                return payload;
            else
                foreach (var e in element.EnumerateObject())
                    return FindPayload(element.GetProperty(e.Name));

            return null;
        }

        private static bool TryFindStatusResponse(JsonElement payload, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (payload.TryGetProperty(StatusKey, out var status) && IsStatusError(status))
                if (payload.TryGetProperty(ErrorsKey, out var errors))
                    if (errors.GetArrayLength() > 0 && errors[0].TryGetProperty(ErrorKey, out var error))
                        errorMessage = error.GetString();

            return errorMessage?.Length > 0;
        }

        private static bool IsStatusError(JsonElement status)
        {
            return status.GetString().Equals(ErrorKey, StringComparison.CurrentCultureIgnoreCase);
        }

        private static bool TryFindError(JsonElement error, out string errorMessage)
        {
            errorMessage = string.Empty;

            return error.TryGetProperty(ErrorKey, out var errorJson)
                ? TryFindErrorDetail(errorJson, out errorMessage)
                : errorMessage?.Length > 0;
        }

        private static bool TryFindErrorDetail(JsonElement error, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (error.ValueKind == JsonValueKind.Object)
            {
                if (error.TryGetProperty(ErrorDetailKey, out var errorDetail))
                    errorMessage = errorDetail.GetString();
                else
                    foreach (var p in error.EnumerateObject())
                        return TryFindErrorDetail(error.GetProperty(p.Name), out errorMessage);
            }
            else if (error.ValueKind == JsonValueKind.Array)
                foreach (var e in error.EnumerateArray())
                    return TryFindErrorDetail(e, out errorMessage);

            return errorMessage?.Length > 0;
        }
    }
}
