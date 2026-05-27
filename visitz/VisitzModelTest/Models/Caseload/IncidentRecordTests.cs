using VisitzApi.Models.Caseload;
using VisitzModel.Extensions;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Storage;
using VisitzModel.Utilities;
using VisitzModelTest.Mocks;

namespace VisitzModelTest.Models.Caseload;

public class IncidentRecordTests
{
    const string PrimaryName = "USER";
    const string SecondaryName = "USER2";

    static IncidentJson IncidentJson =>
        new()
        {
            Id = "",
            CreatedBy = "",
            CreatedById = "",
            UpdatedBy = "",
            UpdatedById = "",
            CreatedDate = "12/10/2018 13:50:02",
            UpdatedDate = "12/10/2018 13:50:02",
            IncidentNumber = "",
            GivenNames = "",
            LastName = "",
            AssignedTo = "",
            AssignedToId = "",
            AddressComments = "",
            Address = "",
            AreAnyOfTheFamilyMembersIndigenous = "",
            CallerAddress = "",
            CallerEmail = "",
            CallerName = "",
            CallerPhone = "",
            Caseload = "",
            CellPhone = "",
            ClosedDate = "",
            CreatedByOffice = "",
            DateReported = "",
            HomePhone = "",
            MedicalExamRequired = "",
            Method = "",
            NatureOfCall = "",
            PccSummary = "",
            PoliceForce = "",
            PoliceInvestigation = "",
            PoliceNotifiedDate = "",
            PoliceReportNumber = "",
            PreferredContactMethod = "",
            ProtectionResponse = "",
            Resolution = "",
            ResponsePriority = "",
            RestrictedFlag = "N",
            ServiceOffice = "",
            Status = "",
            Type = "",
            TypeOfCaller = "",
        };

    [Fact]
    public void InstanceFromJsonIsEqualIgnoreAssignees()
    {
        IncidentRecord incident = new(IncidentJson);

        Assert.Equal(IncidentJson.Id, incident.Id);
        Assert.Equal(IncidentJson.CreatedBy, incident.CreatedBy);
        Assert.Equal(IncidentJson.CreatedById, incident.CreatedById);
        Assert.Equal(IncidentJson.UpdatedBy, incident.UpdatedBy);
        Assert.Equal(IncidentJson.UpdatedById, incident.UpdatedById);
        Assert.Equal(DateTimeOffset.Parse(IncidentJson.CreatedDate), incident.CreatedDate);
        Assert.Equal(DateTimeOffset.Parse(IncidentJson.UpdatedDate), incident.UpdatedDate);
        Assert.Equal(IncidentJson.IncidentNumber, incident.FileNumber);
        Assert.Equal(IncidentJson.GivenNames, incident.GivenNames);
        Assert.Equal(IncidentJson.LastName, incident.LastName);
        Assert.Equal(IncidentJson.AssignedTo, incident.AssignedTo);
        Assert.Equal(IncidentJson.AssignedToId, incident.AssignedToId);
        Assert.Equal(IncidentJson.AddressComments, incident.AddressComments);
        Assert.Equal(IncidentJson.Address, incident.Address);
        Assert.Equal(IncidentJson.AreAnyOfTheFamilyMembersIndigenous, incident.AreAnyOfTheFamilyMembersIndigenous);
        Assert.Equal(IncidentJson.CallerAddress, incident.CallerAddress);
        Assert.Equal(IncidentJson.CallerEmail, incident.CallerEmail);
        Assert.Equal(IncidentJson.CallerName, incident.CallerName);
        Assert.Equal(IncidentJson.CallerPhone, incident.CallerPhone);
        Assert.Equal(IncidentJson.Caseload, incident.Caseload);
        Assert.Equal(IncidentJson.CellPhone, incident.CellPhone);
        Assert.Equal(Timestamp.ParseDateTimeOffsetNullable(IncidentJson.ClosedDate), incident.ClosedDate);
        Assert.Equal(IncidentJson.CreatedByOffice, incident.CreatedByOffice);
        Assert.Equal(Timestamp.ParseDateTimeOffsetNullable(IncidentJson.DateReported), incident.DateReported);
        Assert.Equal(IncidentJson.HomePhone, incident.HomePhone);
        Assert.Equal(IncidentJson.MedicalExamRequired, incident.MedicalExamRequired);
        Assert.Equal(IncidentJson.Method, incident.Method);
        Assert.Equal(IncidentJson.NatureOfCall, incident.NatureOfCall);
        Assert.Equal(IncidentJson.PccSummary, incident.PccSummary);
        Assert.Equal(IncidentJson.PoliceForce, incident.PoliceForce);
        Assert.Equal(IncidentJson.PoliceInvestigation, incident.PoliceInvestigation);
        Assert.Equal(
            Timestamp.ParseDateTimeOffsetNullable(IncidentJson.PoliceNotifiedDate),
            incident.PoliceNotifiedDate
        );
        Assert.Equal(IncidentJson.PoliceReportNumber, incident.PoliceReportNumber);
        Assert.Equal(IncidentJson.PreferredContactMethod, incident.PreferredContactMethod);
        Assert.Equal(IncidentJson.ProtectionResponse, incident.ProtectionResponse);
        Assert.Equal(IncidentJson.Resolution, incident.Resolution);
        Assert.Equal(IncidentJson.ResponsePriority, incident.ResponsePriority);
        Assert.Equal(IncidentJson.RestrictedFlag.ParseWordTruthiness(), incident.RestrictedFlag);
        Assert.Equal(IncidentJson.ServiceOffice, incident.ServiceOffice);
        Assert.Equal(IncidentJson.Status, incident.Status);
        Assert.Equal(IncidentJson.Type?.ParseEntitySubtype() ?? EntitySubtype.Unknown, incident.EntitySubtype);
        Assert.Equal(IncidentJson.TypeOfCaller, incident.TypeOfCaller);
    }

    [Theory]
    [InlineData(PrimaryName)]
    [InlineData(SecondaryName)]
    public void IsIncidentAssignedToForcedAssignee(string name)
    {
        IncidentRecord incident = new(IncidentJson, name);

        Assert.Contains(name, incident.Assignees);
    }

    [Theory]
    [InlineData("PrimaryName")]
    [InlineData("random name")]
    [InlineData("")]
    public void IsIncidentNotAssignedTo(string name)
    {
        IncidentRecord incident = new(IncidentJson);

        Assert.DoesNotContain(name, incident.Assignees);
    }

    static async Task<IEnumerable<IncidentRecord>> GetByAssignee(string name, bool isPersonalCaseload)
    {
        var realm = await TestingUtilities.MakeRealm<IncidentRecordTests>();
        List<IncidentRecord> incidents = [new IncidentRecord(IncidentJson), new() { Id = "23456" }];
        UserIgnoredContentPrefs prefs = new(new LocalPreferencesMock());

        await realm.Write(async () =>
            await IBusinessObject.SynchronizeAsync(realm, incidents, prefs, PrimaryName, isPersonalCaseload)
        );

        return IncidentRecord.GetAllByAssignee(realm, name, isPersonalCaseload);
    }

    [Theory]
    [InlineData(PrimaryName)]
    [InlineData(SecondaryName)]
    public async Task PersonalCaseloadSearchableByAssignee(string name)
    {
        var incidents = await GetByAssignee(name, isPersonalCaseload: true);
        foreach (var incident in incidents)
            Assert.Contains(name, incident.Assignees);
    }

    [Theory]
    [InlineData(PrimaryName)]
    [InlineData(SecondaryName)]
    public async Task OfficeCaseloadMissingRecordsWithAssignee(string name)
    {
        var incidents = await GetByAssignee(name, isPersonalCaseload: false);
        foreach (var incident in incidents)
            Assert.DoesNotContain(name, incident.Assignees);
    }

    [Fact]
    public async Task LocalStateIsNullOnRecordCreate()
    {
        var realm = await TestingUtilities.MakeRealm<IncidentRecordTests>();
        IncidentRecord incident = new(IncidentJson);
        realm.Write(() => realm.Add(incident));

        Assert.Null(incident.LocalState);
    }

    [Fact]
    public async Task LocalStateIsNullBeforeFirstAccess()
    {
        var realm = await TestingUtilities.MakeRealm<IncidentRecordTests>();
        IncidentRecord incident = new(IncidentJson);
        realm.Write(() => realm.Add(incident));

        Assert.Null(realm.Find<BoLocalState>(((IBusinessObject)incident).ToIdTypeString()));
    }

    [Fact]
    public async Task LocalStateIsPersistedAfterFirstAccess()
    {
        var realm = await TestingUtilities.MakeRealm<IncidentRecordTests>();
        IncidentRecord incident = new(IncidentJson);
        realm.Write(() =>
        {
            ((IBusinessObject)incident).UpsertLocalState(realm);
            realm.Add(incident);
        });

        Assert.NotNull(incident.LocalState);
        Assert.NotNull(realm.Find<BoLocalState>(((IBusinessObject)incident).ToIdTypeString()));
    }

    [Fact]
    public async Task LocalStateViaCaseGetterInitPersistsAfterUpsert()
    {
        var realm = await TestingUtilities.MakeRealm<IncidentRecordTests>();

        IncidentRecord incident = new(IncidentJson);
        realm.Write(() =>
        {
            ((IBusinessObject)incident).UpsertLocalState(realm);
            incident.LocalState?.ShouldDownloadDuringRefresh = true;
            realm.Add(incident);
        });

        string closed = "Closed";
        IncidentRecord upsertCase = new(IncidentJson) { Status = closed };
        realm.Write(() =>
        {
            realm.Add(upsertCase, update: true);
            ((IBusinessObject)incident).UpsertLocalState(realm);
        });

        IncidentRecord retrievedCase = realm.Find<IncidentRecord>(IncidentJson.Id)!;

        Assert.Equal(closed, retrievedCase.Status);
        Assert.True(retrievedCase.LocalState!.ShouldDownloadDuringRefresh);
    }
}
