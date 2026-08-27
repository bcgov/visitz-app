using VisitzApi.Models.Notes;

namespace VisitzModelTest.Models.Notes;

public partial class NoteItemTests
{
    const string CreatedValue = "2026-06-02 8:14:00";
    const string CreatedByValue = "#123";
    const string CreatedByNameValue = "Person 1";
    const string CreatedByOfficeValue = "Office 1";
    const string IdValue = "1";
    const string TextValue = "text here";
    const string UpdatedValue = "2026-06-02 8:14:00";
    const string UpdatedByValue = "#321";
    const string UpdatedByNameValue = "Person 2";
    static readonly DateTimeOffset CreatedDateValue = DateTimeOffset.Parse(CreatedValue);
    static readonly DateTimeOffset UpdatedDateValue = DateTimeOffset.Parse(UpdatedValue);
    static readonly DateTimeOffset ActualDateNotedDateValue = DateTimeOffset.Parse(ActualDateNotedValue);
    static readonly DateTimeOffset NotePeriodDateValue = DateTimeOffset.Parse(NotePeriodValue);

    const string ActualDateNotedValue = "2026-05-25 05:00:00";
    const string KeywordsValue = "key words here";
    const string NotePeriodValue = "Jun 2026";

    static CaseNoteJson NextCaseNoteJson =>
        new()
        {
            Created = CreatedDateValue,
            CreatedBy = CreatedByValue,
            CreatedByName = CreatedByNameValue,
            CreatedByOffice = CreatedByOfficeValue,
            Id = IdValue,
            Text = TextValue,
            Updated = UpdatedDateValue,
            UpdatedBy = UpdatedByValue,
            UpdatedByName = UpdatedByNameValue,
            ActualDateNoted = ActualDateNotedDateValue,
            Keywords = KeywordsValue,
            NotePeriod = NotePeriodValue,
        };

    const string ParentIdValue = "9";

    static ResponseNarrativeJson NextNarrativeNoteJson =>
        new()
        {
            Created = CreatedDateValue,
            CreatedBy = CreatedByValue,
            CreatedByName = CreatedByNameValue,
            CreatedByOffice = CreatedByOfficeValue,
            Id = IdValue,
            Text = TextValue,
            Updated = UpdatedDateValue,
            UpdatedBy = UpdatedByValue,
            UpdatedByName = UpdatedByNameValue,
            IncidentId = ParentIdValue,
            SrId = ParentIdValue,
        };
}
