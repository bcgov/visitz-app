using System.Text.Json;
using VisitzApi.Json;
using VisitzApi.Models.Caseload;

namespace VisitzApiTest.Models.Caseload;

public class CaseJsonTests
{
    internal const string id = "1-123ABC5";
    internal const string assignedTo = "USER1";
    internal const string assignedToId = "1-1234567";
    internal const string caseload = "123";
    internal const string caseNum = "1-11111111111";
    internal const string closedDate = "11/18/2024 23:14:30";
    internal const string closeReason = "it closed";
    internal const string createdBy = "CREATEDBYUSER";
    internal const string createdById = "1-9876543";
    internal const string createdDate = "03/06/2018 03:35:39";
    internal const string earlyOpenReason = "some reason";
    internal const string integrationState = "Error";
    internal const string legacyFileNumber = "1-22222222222";
    internal const string middleName = "middle";
    internal const string myFSFlag = "Y";
    internal const string name = "ABCDEFCHILD, PERSON";
    internal const string officeName = "an office code";
    internal const string organization = "some organization";
    internal const string regionName = "Region - Large Region";
    internal const string renewReviewDate = "12/19/2024 23:14:30";
    internal const string reopenedDate = "12/19/2024 23:14:31";
    internal const string restrictedFlag = "N";
    internal const string status = "a status";
    internal const string subjectContactFirstName = "person";
    internal const string subjectContactLastName = "abcdefperson";
    internal const string type = "Person Services";
    internal const string updatedBy = "UPDATEDBYUSER";
    internal const string updatedById = "1-3219875";
    internal const string updatedDate = "12/19/2024 23:14:30";
    internal const string workQueue = "queue text";

    internal const string json =
        @$"{{
	""Id"": ""{id}"",
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
    [InlineData(assignedTo, nameof(CaseJson.AssignedTo))]
    [InlineData(assignedToId, nameof(CaseJson.AssignedToId))]
    [InlineData(caseload, nameof(CaseJson.Caseload))]
    [InlineData(caseNum, nameof(CaseJson.CaseNum))]
    [InlineData(closedDate, nameof(CaseJson.ClosedDate))]
    [InlineData(closeReason, nameof(CaseJson.CloseReason))]
    [InlineData(createdBy, nameof(CaseJson.CreatedBy))]
    [InlineData(createdById, nameof(CaseJson.CreatedById))]
    [InlineData(createdDate, nameof(CaseJson.CreatedDate))]
    [InlineData(earlyOpenReason, nameof(CaseJson.EarlyOpenReason))]
    [InlineData(integrationState, nameof(CaseJson.IntegrationState))]
    [InlineData(legacyFileNumber, nameof(CaseJson.LegacyFileNumber))]
    [InlineData(middleName, nameof(CaseJson.MiddleName))]
    [InlineData(myFSFlag, nameof(CaseJson.MyFSFlag))]
    [InlineData(name, nameof(CaseJson.Name))]
    [InlineData(officeName, nameof(CaseJson.OfficeName))]
    [InlineData(organization, nameof(CaseJson.Organization))]
    [InlineData(regionName, nameof(CaseJson.RegionName))]
    [InlineData(renewReviewDate, nameof(CaseJson.RenewReviewDate))]
    [InlineData(reopenedDate, nameof(CaseJson.ReopenedDate))]
    [InlineData(restrictedFlag, nameof(CaseJson.RestrictedFlag))]
    [InlineData(status, nameof(CaseJson.Status))]
    [InlineData(subjectContactFirstName, nameof(CaseJson.SubjectContactFirstName))]
    [InlineData(subjectContactLastName, nameof(CaseJson.SubjectContactLastName))]
    [InlineData(type, nameof(CaseJson.Type))]
    [InlineData(updatedBy, nameof(CaseJson.UpdatedBy))]
    [InlineData(updatedById, nameof(CaseJson.UpdatedById))]
    [InlineData(updatedDate, nameof(CaseJson.UpdatedDate))]
    [InlineData(workQueue, nameof(CaseJson.WorkQueue))]
    public void ParsesStringFieldsCorrectly(string expectedValue, string propertyName)
    {
        CaseJson @case = JsonSerializer.Deserialize<CaseJson>(json, PayloadOptions.SiebelGet)!;

        string actual = (string)@case.GetType().GetProperty(propertyName)!.GetValue(@case)!;

        Assert.Equal(expectedValue, actual);
    }
}
