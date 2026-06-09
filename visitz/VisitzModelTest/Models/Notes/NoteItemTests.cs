using VisitzApi.Models.Notes;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Notes;

namespace VisitzModelTest.Models.Notes;

public partial class NoteItemTests
{
    [Fact]
    public void FromApiEntity_ParseCaseNoteJsonCorrectly()
    {
        // Arrange
        CaseNoteJson json = NextCaseNoteJson;

        // Act
        NoteItem caseNote = NoteItem.FromApiEntity(ParentIdValue, EntityType.Case, json, 0);

        // Assert
        Assert.Equal(CreatedDateValue, caseNote.CreatedDate);
        Assert.Equal(CreatedByValue, caseNote.CreatedBy);
        Assert.Equal(CreatedByNameValue, caseNote.CreatedByName);
        Assert.Equal(CreatedByOfficeValue, caseNote.CreatedByOffice);
        Assert.Equal(IdValue, caseNote.FullID);
        Assert.Equal(TextValue, caseNote.Content);
        Assert.Equal(UpdatedDateValue, caseNote.UpdatedDate);
        Assert.Equal(UpdatedByValue, caseNote.UpdatedBy);
        Assert.Equal(UpdatedByNameValue, caseNote.UpdatedByName);
        Assert.Equal(NotePeriodValue, caseNote.NotePeriod);
        Assert.Equal(NotePeriodDateValue, caseNote.NotePeriodDateTime);
    }

    public static IEnumerable<TheoryDataRow<EntityType>> NarrativeTheoryRows =
    [
        EntityType.Incident,
        EntityType.ServiceRequest,
    ];

    [Theory]
    [MemberData(nameof(NarrativeTheoryRows))]
    public void FromApiEntity_ParseNarrativeNoteJsonCorrectly(EntityType parentType)
    {
        // Arrange
        ResponseNarrativeJson json = NextNarrativeNoteJson;

        // Act
        NoteItem narrativeNote = NoteItem.FromApiEntity(ParentIdValue, parentType, json, 0);

        // Assert
        Assert.Equal(CreatedDateValue, narrativeNote.CreatedDate);
        Assert.Equal(CreatedByValue, narrativeNote.CreatedBy);
        Assert.Equal(CreatedByNameValue, narrativeNote.CreatedByName);
        Assert.Equal(CreatedByOfficeValue, narrativeNote.CreatedByOffice);
        Assert.Equal(IdValue, narrativeNote.FullID);
        Assert.Equal(TextValue, narrativeNote.Content);
        Assert.Equal(UpdatedDateValue, narrativeNote.UpdatedDate);
        Assert.Equal(UpdatedByValue, narrativeNote.UpdatedBy);
        Assert.Equal(UpdatedByNameValue, narrativeNote.UpdatedByName);
        Assert.Equal(ParentIdValue, narrativeNote.ParentId);
    }
}
