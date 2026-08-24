using System.Text.Json.Serialization;

namespace VisitzApi.Models.AppLogs;

/// <summary>
/// <para>Model class to hold information for logs send upstream to the API.</para>
///
/// <para>Assign the intended message to <see cref="Message"/>—everything else is metadata.</para>
/// </summary>
public class AppLogJson
{
    public required AppLogLevel Level { get; set; }

    [JsonPropertyName("app-timestamp")]
    public required ulong AppTimestamp { get; set; }

    [JsonPropertyName("dotnet-runtime")]
    public required string DotnetRuntime { get; set; }

    [JsonPropertyName("app-version")]
    public required string AppVersion { get; set; }

    [JsonPropertyName("source-name")]
    public required string SourceName { get; set; }

    public required DeviceInfoLog Device { get; set; }

    public required object Message { get; set; }
}
