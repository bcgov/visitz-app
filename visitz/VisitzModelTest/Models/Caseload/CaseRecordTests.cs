using VisitzApi.Models.Caseload;
using VisitzModel.Extensions;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Models.Caseload;
using VisitzModel.Storage;
using VisitzModel.Utilities;
using VisitzModelTest.Mocks;

namespace VisitzModelTest.Models.Caseload;

public class CaseRecordTests
{
    const string PrimaryName = "USER";
    const string SecondaryName = "USER2";

    static readonly PositionSummary PrimaryPosition = new()
    {
        Id = "12345",
        IsPrimaryMvg = "Y",
        RowStatus = "Y",
        SalesRep = PrimaryName,
    };

    static readonly PositionSummary SecondaryPosition = new()
    {
        Id = "54321",
        IsPrimaryMvg = "N",
        RowStatus = "Y",
        SalesRep = SecondaryName,
    };

    static CaseJson CaseJson =>
        new()
        {
            Id = "12345",
            CreatedBy = "USER",
            CreatedById = "12345",
            UpdatedBy = "USER",
            UpdatedById = "12345",
            CreatedDate = "12/10/2018 13:50:02",
            UpdatedDate = "12/10/2018 13:50:02",
            CaseNum = "123456789ASDBCEF",
            SubjectContactFirstName = "First name",
            SubjectContactLastName = "Last name",
            AssignedTo = PrimaryPosition.SalesRep,
            AssignedToId = PrimaryPosition.Id,
            Caseload = "CASELOAD_ID",
            ClosedDate = "",
            CloseReason = "",
            EarlyOpenReason = "",
            IntegrationState = "",
            LegacyFileNumber = "",
            MiddleName = "",
            MyFSFlag = "N",
            Name = "SOME NAME HERE",
            OfficeName = "OFFICE NAME 1",
            Organization = "ORG",
            RegionName = "REG",
            RenewReviewDate = "",
            ReopenedDate = "",
            RestrictedFlag = "N",
            Position = [PrimaryPosition, SecondaryPosition],
            Status = "",
            Type = "A type",
            WorkQueue = "",
        };

    [Fact]
    public void InstanceFromJsonIsEqualIgnoreAssignees()
    {
        CaseRecord @case = new(CaseJson);

        Assert.Equal(CaseJson.Id, @case.Id);
        Assert.Equal(CaseJson.CreatedBy, @case.CreatedBy);
        Assert.Equal(CaseJson.CreatedById, @case.CreatedById);
        Assert.Equal(CaseJson.UpdatedBy, @case.UpdatedBy);
        Assert.Equal(CaseJson.UpdatedById, @case.UpdatedById);
        Assert.Equal(DateTimeOffset.Parse(CaseJson.CreatedDate), @case.CreatedDate);
        Assert.Equal(DateTimeOffset.Parse(CaseJson.UpdatedDate), @case.UpdatedDate);
        Assert.Equal(CaseJson.CaseNum, @case.FileNumber);
        Assert.Equal(CaseJson.SubjectContactFirstName, @case.GivenNames);
        Assert.Equal(CaseJson.SubjectContactLastName, @case.LastName);
        Assert.Equal(CaseJson.AssignedTo, @case.AssignedTo);
        Assert.Equal(CaseJson.AssignedToId, @case.AssignedToId);
        Assert.Equal(CaseJson.Caseload, @case.Caseload);
        Assert.Equal(Timestamp.ParseDateTimeOffsetNullable(CaseJson.ClosedDate), @case.ClosedDate);
        Assert.Equal(CaseJson.CloseReason, @case.CloseReason);
        Assert.Equal(CaseJson.EarlyOpenReason, @case.EarlyOpenReason);
        Assert.Equal(CaseJson.IntegrationState, @case.IntegrationState);
        Assert.Equal(CaseJson.LegacyFileNumber, @case.LegacyFileNumber);
        Assert.Equal(CaseJson.MiddleName, @case.MiddleName);
        Assert.Equal(CaseJson.MyFSFlag.ParseWordTruthiness(), @case.MyFSFlag);
        Assert.Equal(CaseJson.Name, @case.Name);
        Assert.Equal(CaseJson.OfficeName, @case.ServiceOffice);
        Assert.Equal(CaseJson.Organization, @case.Organization);
        Assert.Equal(CaseJson.RegionName, @case.RegionName);
        Assert.Equal(Timestamp.ParseDateTimeOffsetNullable(CaseJson.RenewReviewDate), @case.RenewReviewDate);
        Assert.Equal(Timestamp.ParseDateTimeOffsetNullable(CaseJson.ReopenedDate), @case.ReopenedDate);
        Assert.Equal(CaseJson.RestrictedFlag.ParseWordTruthiness(), @case.RestrictedFlag);
        Assert.Equal(CaseJson.Status, @case.Status);
        Assert.Equal(CaseJson.Type.ParseEntitySubtype(), @case.EntitySubtype);
        Assert.Equal(CaseJson.WorkQueue, @case.WorkQueue);
    }

    [Theory]
    [InlineData(PrimaryName)]
    [InlineData(SecondaryName)]
    public void IsCaseAssignedTo(string name)
    {
        CaseRecord @case = new(CaseJson);

        Assert.Contains(name, @case.Assignees);
    }

    [Theory]
    [InlineData("PrimaryName")]
    [InlineData("random name")]
    [InlineData("")]
    public void IsCaseNotAssignedTo(string name)
    {
        CaseRecord @case = new(CaseJson);

        Assert.DoesNotContain(name, @case.Assignees);
    }

    static async Task<IEnumerable<CaseRecord>> GetByAssignee(string name, bool isPersonalCaseload)
    {
        var realm = await TestingUtilities.MakeRealm<CaseRecordTests>();
        List<CaseRecord> cases = [new CaseRecord(CaseJson), new() { Id = "23456" }];
        UserIgnoredContentPrefs prefs = new(new LocalPreferencesMock());

        await realm.Write(async () =>
            await IBusinessObject.SynchronizeAsync(realm, cases, prefs, PrimaryName, isPersonalCaseload)
        );

        return CaseRecord.GetAllByAssignee(realm, name, isPersonalCaseload);
    }

    [Theory]
    [InlineData(PrimaryName)]
    [InlineData(SecondaryName)]
    public async Task PersonalCaseloadSearchableByAssignee(string name)
    {
        var cases = await GetByAssignee(name, isPersonalCaseload: true);
        foreach (var @case in cases)
            Assert.Contains(name, @case.Assignees);
    }

    [Theory]
    [InlineData(PrimaryName)]
    [InlineData(SecondaryName)]
    public async Task OfficeCaseloadMissingRecordsWithAssignee(string name)
    {
        var cases = await GetByAssignee(name, isPersonalCaseload: false);
        foreach (var @case in cases)
            Assert.DoesNotContain(name, @case.Assignees);
    }

    [Fact]
    public async Task LocalStateIsNullOnRecordCreate()
    {
        var realm = await TestingUtilities.MakeRealm<CaseRecordTests>();
        CaseRecord @case = new(CaseJson);
        realm.Write(() => realm.Add(@case));

        Assert.Null(@case.LocalState);
    }

    [Fact]
    public async Task LocalStateIsNullBeforeFirstAccess()
    {
        var realm = await TestingUtilities.MakeRealm<CaseRecordTests>();
        CaseRecord @case = new(CaseJson);
        realm.Write(() => realm.Add(@case));

        Assert.Null(realm.Find<BoLocalState>(((IBusinessObject)@case).ToIdTypeString()));
    }

    [Fact]
    public async Task LocalStateIsPersistedAfterUpsert()
    {
        var realm = await TestingUtilities.MakeRealm<CaseRecordTests>();
        CaseRecord @case = new(CaseJson);
        realm.Write(() =>
        {
            ((IBusinessObject)@case).UpsertLocalState(realm, false);
            realm.Add(@case);
        });

        Assert.NotNull(@case.LocalState);
        Assert.NotNull(realm.Find<BoLocalState>(((IBusinessObject)@case).ToIdTypeString()));
    }

    [Fact]
    public async Task LocalStateViaCaseGetterInitPersistsAfterUpsert()
    {
        var realm = await TestingUtilities.MakeRealm<CaseRecordTests>();

        CaseRecord @case = new(CaseJson);
        realm.Write(() =>
        {
            ((IBusinessObject)@case).UpsertLocalState(realm);
            @case.LocalState?.ShouldDownloadDuringRefresh = true;
            realm.Add(@case);
        });

        string closed = "Closed";
        CaseRecord upsertCase = new(CaseJson) { Status = closed };
        realm.Write(() =>
        {
            realm.Add(upsertCase, update: true);
            ((IBusinessObject)@case).UpsertLocalState(realm);
        });

        CaseRecord retrievedCase = realm.Find<CaseRecord>(CaseJson.Id)!;

        Assert.Equal(closed, retrievedCase.Status);
        Assert.True(retrievedCase.LocalState!.ShouldDownloadDuringRefresh);
    }
}
