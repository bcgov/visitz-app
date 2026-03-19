using System.Text.Json;

namespace VisitzApi.Json;

public static class PayloadOptions
{
    public static readonly JsonSerializerOptions Default = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    static readonly JsonSerializerOptions _siebelGet = new()
    {
        PropertyNamingPolicy = new PascalWhitespaceNamingPolicy(),
    };

    public static JsonSerializerOptions SiebelGet => _siebelGet;

    static readonly JsonSerializerOptions _middlewarePost = new()
    {
        PropertyNamingPolicy = new PascalWhitespaceNamingPolicy(),
    };

    public static JsonSerializerOptions MiddlewarePost => _middlewarePost;
}
