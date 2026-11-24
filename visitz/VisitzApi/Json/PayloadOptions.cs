using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using VisitzApi.Json.Converters;

namespace VisitzApi.Json;

public static class PayloadOptions
{
    public static readonly JsonSerializerOptions Default = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public static readonly JsonSerializerOptions SiebelGet = new()
    {
        PropertyNamingPolicy = new PascalWhitespaceNamingPolicy(),
    };

    public static readonly JsonSerializerOptions MiddlewarePost = new()
    {
        PropertyNamingPolicy = new PascalWhitespaceNamingPolicy(),
    };

    public static readonly JsonSerializerOptions MiddlewarePostOmitNulls = new()
    {
        PropertyNamingPolicy = new PascalWhitespaceNamingPolicy(),
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    static PayloadOptions()
    {
        SiebelGet.Converters.Add(new SiebelDateTimeConverter());
        SiebelGet.Converters.Add(new SiebelDateTimeNullableConverter());

        DefaultJsonTypeInfoResolver omitNullStringsResolver = new();
        omitNullStringsResolver.Modifiers.Add(typeInfo =>
        {
            foreach (var prop in typeInfo.Properties)
            {
                if (prop.PropertyType == typeof(string))
                    prop.ShouldSerialize = (_, value) => value is string { Length: > 0 };
            }
        });
        MiddlewarePostOmitNulls.Converters.Add(new EmptyStringToNullConverter());
        MiddlewarePostOmitNulls.TypeInfoResolver = omitNullStringsResolver;
    }
}
