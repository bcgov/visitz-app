using System.Text.Json;
using VisitzApi.Extensions;

namespace VisitzApi.ErrorHandling;

#nullable enable

internal class ResponseBodyParser(string responseBody)
{
    const string StatusKey = "Status";
    const string StatusKeyLower = "status";
    const string SuccessKey = "Success";
    const string MessageKey = "Message";
    const string MessageKeyLower = "message";
    const string ErrorKey = "Error";
    const string ErrorKeyLower = "error";
    const string ErrorDetailKey = "errorDetail";
    const string ErrorMessageKey = "Error Message";

    public string ResponseBody { get; private set; } = responseBody;

    JsonElement RootElement { get; set; } = JsonDocument.Parse(responseBody).RootElement;

    public JsonElement? FindFirstMessage()
    {
        return RootElement.FindFirstByAnyName(MessageKeyLower, MessageKey);
    }

    public bool? GetSuccessStatusFromBody()
    {
        return RootElement.FindFirstByAnyName(StatusKeyLower, StatusKey)?
            .GetString()?
            .Equals(SuccessKey, StringComparison.CurrentCultureIgnoreCase);
    }

    public IList<string> GetFirstMessages()
    {
        IList<string> list = [];

        if (FindFirstMessage() is JsonElement msgElement)
        {
            if (msgElement.ValueKind == JsonValueKind.String)
            {
                if (msgElement.GetString() is string text)
                    list.Add(text);
            }
            else if (msgElement.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement element in msgElement.EnumerateArray())
                {
                    if (element.GetString() is string text)
                        list.Add(text);
                }
            }
        }

        return list;
    }

    public string? FindFirstError()
    {
        return RootElement.FindFirstByAnyName(
            ErrorDetailKey,
            ErrorMessageKey,
            ErrorKeyLower,
            ErrorKey)?.GetString();
    }
}
