using System.Text.Json;
using VisitzApi.Json;
using VisitzApi.Models;

namespace VisitzApiTest.Models;

public class CaseJsonTests
{
    const string id = "1-123ABC5";
    const string assignedTo = "USER1";
    const string assignedToId = "1-1234567";
    const string caseload = "123";
    const string caseNum = "1-11111111111";
    const string closedDate = "11/18/2024 23:14:30";
    const string closeReason = "it closed";
    const string createdBy = "CREATEDBYUSER";
    const string createdById = "1-9876543";
    const string createdDate = "03/06/2018 03:35:39";
    const string earlyOpenReason = "some reason";
    const string integrationState = "Error";
    const string legacyFileNumber = "1-22222222222";
    const string middleName = "middle";
    const string myFSFlag = "Y";
    const string name = "ABCDEFCHILD, PERSON";
    const string officeName = "an office code";
    const string organization = "some organization";
    const string regionName = "Region - Large Region";
    const string renewReviewDate = "12/19/2024 23:14:30";
    const string reopenedDate = "12/19/2024 23:14:31";
    const string restrictedFlag = "N";
    const string status = "a status";
    const string subjectContactFirstName = "person";
    const string subjectContactLastName = "abcdefperson";
    const string type = "Person Services";
    const string updatedBy = "UPDATEDBYUSER";
    const string updatedById = "1-3219875";
    const string updatedDate = "12/19/2024 23:14:30";
    const string workQueue = "queue text";

    const string json =
@$"{{
	""Id"": ""{id}"",
	""Row Id"": ""{id}"",
    ""Assigned To Id"": ""{assignedToId}"",
    ""Assigned To"": ""{assignedTo}"",
    ""Case Num"": ""{caseNum}"",
    ""Caseload"": ""{caseload}"",
    ""Close Reason"": ""{closeReason}"",
    ""Closed Date"": ""{closedDate}"",
    ""Created By Id"": ""{createdById}"",
    ""Created By"": ""{createdBy}"",
    ""Created Date"": ""{createdDate}"",
    ""Early Open Reason"": ""{earlyOpenReason}"",
    ""Integration State"": ""{integrationState}"",
    ""Legacy File Number"": ""{legacyFileNumber}"",
    ""Middle Name"": ""{middleName}"",
    ""MyFS Flag"": ""{myFSFlag}"",
    ""Name"": ""{name}"",
    ""Office Name"": ""{officeName}"",
    ""Organization"": ""{organization}"",
    ""Region Name"": ""{regionName}"",
    ""Renew Review Date"": ""{renewReviewDate}"",
    ""Reopened Date"": ""{reopenedDate}"",
    ""Restricted Flag"": ""{restrictedFlag}"",
    ""Status"": ""{status}"",
    ""Subject Contact First Name"": ""{subjectContactFirstName}"",
    ""Subject Contact Last Name"": ""{subjectContactLastName}"",
    ""Type"": ""{type}"",
    ""Updated By Id"": ""{updatedById}"",
    ""Updated By"": ""{updatedBy}"",
    ""Updated Date"": ""{updatedDate}"",
    ""Work Queue"": ""{workQueue}""
}}";

    [Theory]
    [InlineData(id, nameof(CaseJson.Id))]
    [InlineData(id, nameof(CaseJson.RowId))]
    [InlineData(assignedTo, nameof(CaseJson.AssignedTo))]
    [InlineData(assignedToId, nameof(CaseJson.AssignedToId))]
    [InlineData(caseload, nameof(CaseJson.Caseload))]
    [InlineData(caseNum, nameof(CaseJson.CaseNum))]
    [InlineData(closedDate, nameof(CaseJson.ClosedDate))]
    [InlineData(closeReason, nameof(CaseJson.CloseReason))]
    [InlineData(createdBy, nameof(CaseJson.CreatedBy))]
    [InlineData(createdById, nameof(CaseJson.CreatedById))]
    [InlineData(earlyOpenReason, nameof(CaseJson.EarlyOpenReason))]
    [InlineData(integrationState, nameof(CaseJson.IntegrationState))]
    [InlineData(legacyFileNumber, nameof(CaseJson.LegacyFileNumber))]
    [InlineData(middleName, nameof(CaseJson.MiddleName))]
    [InlineData(myFSFlag, nameof(CaseJson.MyFSFlag))]
    [InlineData(name, nameof(CaseJson.Name))]
    [InlineData(officeName, nameof(CaseJson.OfficeName))]
    [InlineData(organization, nameof(CaseJson.Organization))]
    [InlineData(regionName, nameof(CaseJson.RegionName))]
    [InlineData(restrictedFlag, nameof(CaseJson.RestrictedFlag))]
    [InlineData(status, nameof(CaseJson.Status))]
    [InlineData(subjectContactFirstName, nameof(CaseJson.SubjectContactFirstName))]
    [InlineData(subjectContactLastName, nameof(CaseJson.SubjectContactLastName))]
    [InlineData(type, nameof(CaseJson.Type))]
    [InlineData(updatedBy, nameof(CaseJson.UpdatedBy))]
    [InlineData(updatedById, nameof(CaseJson.UpdatedById))]
    [InlineData(workQueue, nameof(CaseJson.WorkQueue))]
    public void ParsesStringFieldsCorrectly(string expectedValue, string propertyName)
    {
        CaseJson @case = JsonSerializer.Deserialize<CaseJson>(json, PayloadOptions.SiebelGet)!;

        string actual = (string)@case.GetType().GetProperty(propertyName)!.GetValue(@case)!;

        Assert.Equal(expectedValue, actual);
    }

    [Theory]
    [InlineData(createdDate, nameof(CaseJson.CreatedDate))]
    [InlineData(renewReviewDate, nameof(CaseJson.RenewReviewDate))]
    [InlineData(reopenedDate, nameof(CaseJson.ReopenedDate))]
    [InlineData(updatedDate, nameof(CaseJson.UpdatedDate))]
    public void ParsesDateTimeOffsetFieldsCorrectly(string expectedValue, string propertyName)
    {
        CaseJson @case = JsonSerializer.Deserialize<CaseJson>(json, PayloadOptions.SiebelGet)!;

        DateTimeOffset expected = DateTimeOffset.Parse(expectedValue);
        DateTimeOffset actual = (DateTimeOffset)@case.GetType().GetProperty(propertyName)!.GetValue(@case)!;

        Assert.Equal(expected, actual);
    }
}
