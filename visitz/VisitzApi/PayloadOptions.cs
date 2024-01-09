using System.Text.Json;

namespace VisitzApi;

internal readonly struct PayloadOptions
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}
