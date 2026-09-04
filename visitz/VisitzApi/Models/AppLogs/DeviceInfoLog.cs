using System.Text.Json.Serialization;

namespace VisitzApi.Models.AppLogs;

public class DeviceInfoLog
{
    public required string Model { get; set; }

    public required string Manufacturer { get; set; }

    [JsonPropertyName("os-version")]
    public required string OSVersion { get; set; }

    public required string Idiom { get; set; }

    public required string Platform { get; set; }
}
