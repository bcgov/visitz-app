using System.Text.Json;

namespace VisitzApi.ErrorHandling
{
    internal class KongJsonMessage
    {
        private static readonly string MessageKey = "message";

        internal static bool TryFindMessage(string content, out string message)
        {
            try
            {
                var json = JsonDocument.Parse(content).RootElement;

                if (json.TryGetProperty(MessageKey, out var element))
                {
                    message = element.GetString();
                    return true;
                }

                message = "";
                return false;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }
    }
}
