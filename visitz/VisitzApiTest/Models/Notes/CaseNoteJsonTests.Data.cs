namespace VisitzApiTest.Models.Notes;

public partial class CaseNoteJsonTests
{
    const string ActualDateNotedValue = "2026-05-25T05:00:00";
    const string CreatedByNameValue = "Person 1";
    const string CreatedByOfficeValue = "Office 1";
    const string CreatedByValue = "#123";
    const string CreatedValue = "04/08/2026 08:03:17";
    const string IdValue = "1";
    const string KeywordsValue = "key words here";
    const string NotePeriodValue = "Jun 2026";
    const string NoteValue = "text here";
    const string UpdatedByNameValue = "Person 2";
    const string UpdatedByValue = "#321";
    const string UpdatedValue = "2026-06-02T08:30:00";

    static readonly DateTimeOffset ActualDateNotedDateValue = DateTimeOffset.Parse(ActualDateNotedValue);
    static readonly DateTimeOffset CreatedDateValue = DateTimeOffset.Parse(CreatedValue);
    static readonly DateTimeOffset UpdatedDateValue = DateTimeOffset.Parse(UpdatedValue);

    static string AllFieldsJson =>
        $$"""
{
    "Actual Date Noted": "{{ActualDateNotedValue}}",
    "Created By Name": "{{CreatedByNameValue}}",
    "Created By Office Name": "{{CreatedByOfficeValue}}",
    "Created By": "{{CreatedByValue}}",
    "Created": "{{CreatedValue}}",
    "Id": "{{IdValue}}",
    "Keywords": "{{KeywordsValue}}",
    "Note Period": "{{NotePeriodValue}}",
    "Note": "{{NoteValue}}",
    "Last Updated By Name": "{{UpdatedByNameValue}}",
    "Updated By": "{{UpdatedByValue}}",
    "Updated": "{{UpdatedValue}}"
}
""";

    static string EmptyDateJson =>
        $$"""
{
    "Actual Date Noted": "",
    "Created By Name": "{{CreatedByNameValue}}",
    "Created By Office Name": "{{CreatedByOfficeValue}}",
    "Created By": "{{CreatedByValue}}",
    "Created": "{{CreatedValue}}",
    "Id": "{{IdValue}}",
    "Keywords": "{{KeywordsValue}}",
    "Note Period": "{{NotePeriodValue}}",
    "Note": "{{NoteValue}}",
    "Last Updated By Name": "{{UpdatedByNameValue}}",
    "Updated By": "{{UpdatedByValue}}",
    "Updated": "{{UpdatedValue}}"
}
""";
}
