using System.Text.Json;
using VisitzApi.Json;
using VisitzApi.Models.Base;

namespace VisitzApiTest.Models.Base;

public partial class BaseRecordJsonTests
{
    internal const string ArbitraryId = "1-1A2B3C4";
    internal const string SomeName = "SOMENAME";
    internal const string SomeNameId = "1-18F52X5";
    internal const string OtherName = "OTHERNAME";
    internal const string OtherNameId = "1-ASDUH2A";
    internal const string CreatedDateValue = "03/06/2018 03:35:39";
    internal const string UpdatedDateValue = "12/19/2024 23:14:30";

    const string availableData =
        "{"
        + $"""
    "Id": "{ArbitraryId}",
	"Created By": "{OtherName}",
	"Created By Id": "{OtherNameId}",
	"Created Date": "{CreatedDateValue}",
	"Updated By": "{SomeName}",
	"Updated By Id": "{SomeNameId}",
	"Updated Date": "{UpdatedDateValue}"
"""
        + "}";

    const string missingIdFields =
        "{"
        + $"""
	"Created By": "{OtherName}",
	"Created Date": "{CreatedDateValue}",
	"Updated By": "{SomeName}",
	"Updated Date": "{UpdatedDateValue}"
"""
        + "}";

    [Theory]
    [InlineData(ArbitraryId, nameof(BaseRecordJson.Id))]
    [InlineData(OtherName, nameof(BaseRecordJson.CreatedBy))]
    [InlineData(OtherNameId, nameof(BaseRecordJson.CreatedById))]
    [InlineData(SomeName, nameof(BaseRecordJson.UpdatedBy))]
    [InlineData(SomeNameId, nameof(BaseRecordJson.UpdatedById))]
    [InlineData(CreatedDateValue, nameof(BaseRecordJson.CreatedDate))]
    [InlineData(UpdatedDateValue, nameof(BaseRecordJson.UpdatedDate))]
    public void ParsesStringFieldsCorrectly(string expectedValue, string propertyName)
    {
        BaseRecordJson baseRecord = JsonSerializer.Deserialize<BaseRecordJson>(
            availableData,
            PayloadOptions.SiebelGet
        )!;

        string actual = (string)baseRecord.GetType().GetProperty(propertyName)!.GetValue(baseRecord)!;

        Assert.Equal(expectedValue, actual);
    }

    [Theory]
    [InlineData(missingIdFields)]
    public void ThrowWhenFieldsMissing(string json)
    {
        Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<BaseRecordJson>(json, PayloadOptions.SiebelGet));
    }
}
