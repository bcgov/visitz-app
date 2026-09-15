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

    [Fact]
    public async Task AssessmentsSynchronizeInsertionCorrectly()
    {
        var realm = await TestingUtilities.MakeRealm<NoteItemTests>();

        string parentId = "10";
        IEnumerable<NoteItem> notes = NoteItem.FromApiEntities(parentId, [NextCaseNoteJson]);
        await NoteItem.SynchronizeAsync(realm, parentId, EntityType.Case, notes);

        Assert.Equal(1, realm.All<NoteItem>().Count());
    }

    [Fact]
    public async Task AssessmentsSynchronizeDeletionCorrectly()
    {
        var realm = await TestingUtilities.MakeRealm<NoteItemTests>();

        // Add two notes with different parent IDs

        string firstParentId = "10";
        CaseNoteJson firstNote = NextCaseNoteJson;
        firstNote.Id = "1";
        await NoteItem.SynchronizeAsync(
            realm,
            firstParentId,
            EntityType.Case,
            NoteItem.FromApiEntities(firstParentId, [firstNote])
        );

        string secondParentId = "20";
        CaseNoteJson secondNote = NextCaseNoteJson;
        secondNote.Id = "2";
        await NoteItem.SynchronizeAsync(
            realm,
            secondParentId,
            EntityType.Case,
            NoteItem.FromApiEntities(secondParentId, [secondNote])
        );

        // Sync one of the parent's notes with an empty collection to delete it
        await NoteItem.SynchronizeAsync(realm, secondParentId, EntityType.Case, []);

        Assert.Equal(1, realm.All<NoteItem>().Count());
    }
}
