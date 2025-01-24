using System.Text.Json;

namespace VisitzApi.Json;

public static class PayloadOptions
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    static readonly JsonSerializerOptions _siebelGet = new()
    {
        PropertyNamingPolicy = new PascalWhitespaceNamingPolicy(),
    };

    static readonly DateTimeOffsetParseConverter _parseConverter = new(SiebelFormats.SiebelDateFormat);

    public static JsonSerializerOptions SiebelGet
    {
        get
        {
            if (!_siebelGet.Converters.Contains(_parseConverter))
                _siebelGet.Converters.Add(_parseConverter);

            return _siebelGet;
        }
    }
}
