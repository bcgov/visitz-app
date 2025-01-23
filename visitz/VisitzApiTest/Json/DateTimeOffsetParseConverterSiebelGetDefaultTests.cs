using System.Text.Json;
using VisitzApi.Json;

namespace VisitzApiTest.Json;

public class DateTimeOffsetParseConverterSiebelGetDefaultTests
{
    static readonly string date = @"12/19/2024 23:14:30";
    static readonly string dateJson = @"""" + date + @"""";

    [Fact]
    public void ParsesSiebelDefaultDateFormat()
    {
        DateTimeOffset.Parse(date);
    }

    [Fact]
    public void ParsesSiebelDefaultDateFormatFromJson()
    {
        JsonSerializer.Deserialize<DateTimeOffset>(dateJson, PayloadOptions.SiebelGet);
    }

    [Fact]
    public void ParsesAndWritesSiebelDefaultDateFormatWithJson()
    {
        DateTimeOffset dto = JsonSerializer.Deserialize<DateTimeOffset>(dateJson, PayloadOptions.SiebelGet);
        string serializedDate = JsonSerializer.Serialize(dto, PayloadOptions.SiebelGet);

        Assert.Equal(dateJson, serializedDate);
    }
}
