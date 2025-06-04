using System.Text.Json;
using VisitzApi.Json;
using VisitzApi.Models.Base;

namespace VisitzApiTest.Models.Base;

public class AssignableRecordJsonTests
{
    const string availableData =
"{" + $"""
    "Id": "{BaseRecordJsonTests.ArbitraryId}",
	"Row Id": "{BaseRecordJsonTests.ArbitraryId}",
	"Assigned To": "{BaseRecordJsonTests.SomeName}",
	"Assigned To Id": "{BaseRecordJsonTests.SomeNameId}",
	"Created By": "{BaseRecordJsonTests.OtherName}",
	"Created By Id": "{BaseRecordJsonTests.OtherNameId}",
	"Created Date": "{BaseRecordJsonTests.CreatedDateValue}",
	"Updated By": "{BaseRecordJsonTests.SomeName}",
	"Updated By Id": "{BaseRecordJsonTests.SomeNameId}",
	"Updated Date": "{BaseRecordJsonTests.UpdatedDateValue}"
""" + "}";

    const string missingIdFields =
"{" + $"""
	"Assigned To": "{BaseRecordJsonTests.SomeName}",
	"Created By": "{BaseRecordJsonTests.OtherName}",
	"Created Date": "{BaseRecordJsonTests.CreatedDateValue}",
	"Updated By": "{BaseRecordJsonTests.SomeName}",
	"Updated Date": "{BaseRecordJsonTests.UpdatedDateValue}"
""" + "}";

    const string missingNameFields =
"{" + $"""
    "Id": "{BaseRecordJsonTests.ArbitraryId}",
	"Row Id": "{BaseRecordJsonTests.ArbitraryId}",
	"Assigned To Id": "{BaseRecordJsonTests.SomeNameId}",
	"Created By Id": "{BaseRecordJsonTests.OtherNameId}",
	"Created Date": "{BaseRecordJsonTests.CreatedDateValue}",
	"Updated By Id": "{BaseRecordJsonTests.SomeNameId}",
	"Updated Date": "{BaseRecordJsonTests.UpdatedDateValue}"
""" + "}";

    const string missingDateFields =
"{" + $"""
    "Id": "{BaseRecordJsonTests.ArbitraryId}",
	"Row Id": "{BaseRecordJsonTests.ArbitraryId}",
	"Assigned To": "{BaseRecordJsonTests.SomeName}",
	"Assigned To Id": "{BaseRecordJsonTests.SomeNameId}",
	"Created By": "{BaseRecordJsonTests.OtherName}",
	"Created By Id": "{BaseRecordJsonTests.OtherNameId}",
	"Updated By": "{BaseRecordJsonTests.SomeName}",
	"Updated By Id": "{BaseRecordJsonTests.SomeNameId}",
""" + "}";

    [Theory]
    [InlineData(BaseRecordJsonTests.SomeName, nameof(AssignableRecordJson.AssignedTo))]
    [InlineData(BaseRecordJsonTests.SomeNameId, nameof(AssignableRecordJson.AssignedToId))]
    public void ParsesStringFieldsCorrectly(string expectedValue, string propertyName)
    {
        BaseRecordJson baseRecord = JsonSerializer.Deserialize<AssignableRecordJson>(
            availableData, PayloadOptions.SiebelGet)!;

        string actual = (string)baseRecord.GetType().GetProperty(propertyName)!.GetValue(baseRecord)!;

        Assert.Equal(expectedValue, actual);
    }

    [Theory]
    [InlineData(missingIdFields)]
    [InlineData(missingNameFields)]
    [InlineData(missingDateFields)]
    public void ThrowWhenFieldsMissing(string json)
    {
        Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<AssignableRecordJson>(
            json, PayloadOptions.SiebelGet));
    }
}
