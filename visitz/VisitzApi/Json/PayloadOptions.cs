using System.Text.Json;
using VisitzApi.Json.Converters;

namespace VisitzApi.Json;

public static class PayloadOptions
{
    public static readonly JsonSerializerOptions Default = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public static readonly JsonSerializerOptions SiebelGet = new()
    {
        PropertyNamingPolicy = new PascalWhitespaceNamingPolicy(),
    };

    static readonly JsonSerializerOptions _middlewarePost = new()
    {
        PropertyNamingPolicy = new PascalWhitespaceNamingPolicy(),
    };

    public static JsonSerializerOptions MiddlewarePost => _middlewarePost;

    static PayloadOptions()
    {
        SiebelGet.Converters.Add(new SiebelDateTimeConverter());
        SiebelGet.Converters.Add(new SiebelDateTimeNullableConverter());
    }
}
