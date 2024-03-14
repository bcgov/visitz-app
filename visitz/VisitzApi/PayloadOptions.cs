using System.Text.Json;

namespace VisitzApi;

public readonly struct PayloadOptions
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}
