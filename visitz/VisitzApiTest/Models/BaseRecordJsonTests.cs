using System.Text.Json;
using VisitzApi.Json;
using VisitzApi.Models;

namespace VisitzApiTest.Models;
public partial class BaseRecordJsonTests
{
    const string ArbitraryId = "1-1A2B3C4";
    const string SomeName = "SOMENAME";
    const string SomeNameId = "1-18F52X5";
    const string OtherName = "OTHERNAME";
    const string OtherNameId = "1-ASDUH2A";
    const string CreatedDateValue = "03/06/2018 03:35:39";
    const string UpdatedDateValue = "12/19/2024 23:14:30";

    const string availableData =
"{" + $"""
    "Id": "{ArbitraryId}",
	"Row Id": "{ArbitraryId}",
	"Assigned To": "{SomeName}",
	"Assigned To Id": "{SomeNameId}",
	"Created By": "{OtherName}",
	"Created By Id": "{OtherNameId}",
	"Created Date": "{CreatedDateValue}",
	"Updated By": "{SomeName}",
	"Updated By Id": "{SomeNameId}",
	"Updated Date": "{UpdatedDateValue}"
""" + "}";

    const string missingIdFields =
"{" + $"""
	"Assigned To": "{SomeName}",
	"Created By": "{OtherName}",
	"Created Date": "{CreatedDateValue}",
	"Updated By": "{SomeName}",
	"Updated Date": "{UpdatedDateValue}"
""" + "}";

    const string missingNameFields =
"{" + $"""
    "Id": "{ArbitraryId}",
	"Row Id": "{ArbitraryId}",
	"Assigned To Id": "{SomeNameId}",
	"Created By Id": "{OtherNameId}",
	"Created Date": "{CreatedDateValue}",
	"Updated By Id": "{SomeNameId}",
	"Updated Date": "{UpdatedDateValue}"
""" + "}";

    const string missingDateFields =
"{" + $"""
    "Id": "{ArbitraryId}",
	"Row Id": "{ArbitraryId}",
	"Assigned To": "{SomeName}",
	"Assigned To Id": "{SomeNameId}",
	"Created By": "{OtherName}",
	"Created By Id": "{OtherNameId}",
	"Updated By": "{SomeName}",
	"Updated By Id": "{SomeNameId}",
""" + "}";

    [Fact]
    public void ParsesAvailableDataCorrectly()
    {
        BaseRecordJson baseRecord = JsonSerializer.Deserialize<BaseRecordJson>(
            availableData, PayloadOptions.SiebelGet);

        Assert.Equal(ArbitraryId, baseRecord.Id);
        Assert.Equal(ArbitraryId, baseRecord.RowId);
        Assert.Equal(SomeName, baseRecord.AssignedTo);
        Assert.Equal(SomeNameId, baseRecord.AssignedToId);
        Assert.Equal(OtherName, baseRecord.CreatedBy);
        Assert.Equal(OtherNameId, baseRecord.CreatedById);
        Assert.Equal(DateTimeOffset.Parse(CreatedDateValue), baseRecord.CreatedDate);
        Assert.Equal(SomeName, baseRecord.UpdatedBy);
        Assert.Equal(SomeNameId, baseRecord.UpdatedById);
        Assert.Equal(DateTimeOffset.Parse(UpdatedDateValue), baseRecord.UpdatedDate);
    }

    [Theory]
    [InlineData(missingIdFields)]
    [InlineData(missingNameFields)]
    [InlineData(missingDateFields)]
    public void ThrowWhenFieldsMissing(string json)
    {
        Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<BaseRecordJson>(
            json, PayloadOptions.SiebelGet));
    }
}
