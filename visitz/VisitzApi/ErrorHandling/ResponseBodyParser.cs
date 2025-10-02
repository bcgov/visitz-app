using System.Text.Json;
using VisitzApi.Extensions;

namespace VisitzApi.ErrorHandling;

#nullable enable

internal class ResponseBodyParser
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

    public string ResponseBody { get; private set; }

    /// <summary>
    /// Exception thrown when trying to parse input responseBody text.
    /// </summary>
    public Exception? ParseException { get; private set; }

    JsonElement? RootElement { get; set; }

    public ResponseBodyParser(string responseBody)
    {
        ResponseBody = responseBody;
        try
        {
            RootElement = JsonDocument.Parse(responseBody).RootElement;
        }
        catch (Exception ex)
        {
            ParseException = ex;
        }
    }

    public JsonElement? FindFirstMessage()
    {
        return RootElement?.FindFirstByAnyName(MessageKeyLower, MessageKey);
    }

    public bool? GetSuccessStatusFromBody()
    {
        return RootElement?.FindFirstByAnyName(StatusKeyLower, StatusKey)?
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
        return RootElement?.FindFirstByAnyName(
            ErrorDetailKey,
            ErrorMessageKey,
            ErrorKeyLower,
            ErrorKey)?.GetString();
    }
}
