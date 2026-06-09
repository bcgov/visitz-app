using System.Text.Json;
using VisitzApi.Json;
using VisitzApi.Models.Notes;

namespace VisitzApiTest.Models.Notes;

public partial class CaseNoteJsonTests
{
    public static IEnumerable<TheoryDataRow<string, string>> CaseNoteStringTheories =
    [
        new(CreatedByNameValue, nameof(CaseNoteJson.CreatedByName)),
        new(CreatedByOfficeValue, nameof(CaseNoteJson.CreatedByOffice)),
        new(CreatedByValue, nameof(CaseNoteJson.CreatedBy)),
        new(IdValue, nameof(CaseNoteJson.Id)),
        new(KeywordsValue, nameof(CaseNoteJson.Keywords)),
        new(NotePeriodValue, nameof(CaseNoteJson.NotePeriod)),
        new(NoteValue, nameof(CaseNoteJson.Text)),
        new(UpdatedByNameValue, nameof(CaseNoteJson.UpdatedByName)),
        new(UpdatedByValue, nameof(CaseNoteJson.UpdatedBy)),
    ];

    [Theory]
    [MemberData(nameof(CaseNoteStringTheories))]
    public void CaseNote_StringsParseCorrectly(string expected, string propertyName)
    {
        // Arrange
        string noteJson = AllFieldsJson;

        // Act
        CaseNoteJson caseNoteJson = JsonSerializer.Deserialize<CaseNoteJson>(noteJson, PayloadOptions.SiebelGet)!;
        object? actual = caseNoteJson.GetType().GetProperty(propertyName)?.GetValue(caseNoteJson);

        // Assert
        Assert.Equal(expected, actual);
    }

    public static IEnumerable<TheoryDataRow<string, string>> CaseNoteDateTheories =
    [
        new(ActualDateNotedValue, nameof(CaseNoteJson.ActualDateNoted)),
        new(CreatedValue, nameof(CaseNoteJson.Created)),
        new(UpdatedValue, nameof(CaseNoteJson.Updated)),
    ];

    [Theory]
    [MemberData(nameof(CaseNoteDateTheories))]
    public void CaseNote_DatesParseCorrectly(string expected, string propertyName)
    {
        // Arrange
        string noteJson = AllFieldsJson;
        DateTimeOffset expectedDate = DateTimeOffset.Parse(expected);

        // Act
        CaseNoteJson caseNoteJson = JsonSerializer.Deserialize<CaseNoteJson>(noteJson, PayloadOptions.SiebelGet)!;
        DateTimeOffset actual = (DateTimeOffset)
            caseNoteJson.GetType().GetProperty(propertyName)?.GetValue(caseNoteJson)!;

        // Assert
        Assert.Equal(expectedDate, actual);
    }

    [Fact]
    public void CaseNote_EmptyStringDatesBecomeNull()
    {
        // Arrange
        string noteJson = EmptyDateJson;

        // Act
        CaseNoteJson caseNoteJson = JsonSerializer.Deserialize<CaseNoteJson>(noteJson, PayloadOptions.SiebelGet)!;

        // Assert
        Assert.Null(caseNoteJson.ActualDateNoted);
    }
}
