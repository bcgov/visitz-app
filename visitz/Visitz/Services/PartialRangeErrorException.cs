using System.Text;

namespace Visitz.Services;

internal class PartialRangeErrorException(string serviceName, List<string> successIds, List<string> errors)
    : Exception(MakeMessage(serviceName, errors))
{
    public List<string> SuccessIds { get; set; } = successIds;

    public List<string> ErrorIds { get; set; } = errors;

    public static string MakeMessage(string serviceName, List<string> errors)
    {
        StringBuilder sb = new($"{serviceName} errors:\n\n");

        foreach (var error in errors.Order())
            sb.AppendLine($"• {error}");

        return sb.ToString();
    }
}
