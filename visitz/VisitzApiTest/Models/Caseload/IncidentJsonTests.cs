using System.Text.Json;
using VisitzApi.Json;
using VisitzApi.Models.Caseload;

namespace VisitzApiTest.Models.Caseload;

public class IncidentJsonTests
{
    const string id = "Id Here";
    const string activityUID = "Id Here";
    const string afterHoursFlag = "N";
    const string assignedTo = "USER1";
    const string assignedToId = "1-1234567";
    const string contactFirstName = "First Name";
    const string contactLastName = "Last Name";
    const string contactMiddleName = "contactMiddleName";
    const string createdBy = "CREATEDBYUSER";
    const string createdById = "1-9876543";
    const string createdByLogin = "Idir Here";
    const string createdDate = "03/06/2018 03:35:39";
    const string dateClosed = "09/01/1970 00:00:00";
    const string dateCreated = "01/01/1970 00:00:00";
    const string dateOccurred = "01/01/1970 00:00:00";
    const string dateReported = "02/01/1970 00:00:00";
    const string daysOpen = "8";
    const string description = "description";
    const string display = "Calendar and Activities";
    const string iCMServiceRegion = "Region";
    const string iCMServiceRegionCode = "Region";
    const string incidentCity = "incidentCity";
    const string incidentLocation = "incidentLocation";
    const string incidentPostalCode = "incidentPostalCode";
    const string incidentSubType = "incidentSubType";
    const string incidentType = "incidentType";
    const string integrationErrorDescription = "integrationErrorDescription";
    const string integrationState = "integrationState";
    const string lastUpdated = "01/01/1970 00:00:00";
    const string lastUpdatedLogin = "Id Here";
    const string location = "location";
    const string name = "name";
    const string organization = "organization";
    const string ownedBy = "ownedBy";
    const string planned = "05/01/1970 00:00:00";
    const string primarySuspectId = "";
    const string priority = "3 - Standard";
    const string resolution = "resolution";
    const string responseTime = "5 Days";
    const string restrictedFlag = "N";
    const string rowStatusOld = "Y";
    const string serviceOffice = "Office";
    const string sourceId = "sourceId";
    const string status = "Open";
    const string subStatus = "subStatus";
    const string subSubType = "subSubType";
    const string suppressCalendar = "N";
    const string systemAsgnFlag = "N";
    const string templateFlag = "N";
    const string updatedBy = "UPDATEDBYUSER";
    const string updatedById = "1-3219875";
    const string updatedDate = "12/19/2024 23:14:30";

    const string json =
$@"{{
    ""Id"": ""{id}"",
    ""Row Id"": ""{id}"",
    ""Activity UID"": ""{activityUID}"",
    ""After Hours Flag"": ""{afterHoursFlag}"",
    ""Assigned To Id"": ""{assignedToId}"",
    ""Assigned To"": ""{assignedTo}"",
    ""Contact First Name"": ""{contactFirstName}"",
    ""Contact Last Name"": ""{contactLastName}"",
    ""Contact Middle Name"": ""{contactMiddleName}"",
    ""Created By Id"": ""{createdById}"",
    ""Created By Login"": ""{createdByLogin}"",
    ""Created By"": ""{createdBy}"",
    ""Created Date"": ""{createdDate}"",
    ""Date Closed"": ""{dateClosed}"",
    ""Date Created"": ""{dateCreated}"",
    ""Date Occurred"": ""{dateOccurred}"",
    ""Date Reported"": ""{dateReported}"",
    ""Days Open"": ""{daysOpen}"",
    ""Description"": ""{description}"",
    ""Display"": ""{display}"",
    ""ICM Service Region Code"": ""{iCMServiceRegionCode}"",
    ""ICM Service Region"": ""{iCMServiceRegion}"",
    ""Incident City"": ""{incidentCity}"",
    ""Incident Location"": ""{incidentLocation}"",
    ""Incident Postal Code"": ""{incidentPostalCode}"",
    ""Incident Sub Type"": ""{incidentSubType}"",
    ""Incident Type"": ""{incidentType}"",
    ""Integration Error Description"": ""{integrationErrorDescription}"",
    ""Integration State"": ""{integrationState}"",
    ""Last Updated Login"": ""{lastUpdatedLogin}"",
    ""Last Updated"": ""{lastUpdated}"",
    ""Location"": ""{location}"",
    ""Name"": ""{name}"",
    ""Organization"": ""{organization}"",
    ""Owned By"": ""{ownedBy}"",
    ""Planned"": ""{planned}"",
    ""Primary Suspect Id"": ""{primarySuspectId}"",
    ""Priority"": ""{priority}"",
    ""Resolution"": ""{resolution}"",
    ""Response Time"": ""{responseTime}"",
    ""Restricted Flag"": ""{restrictedFlag}"",
    ""Row Status Old"": ""{rowStatusOld}"",
    ""Service Office"": ""{serviceOffice}"",
    ""Source Id"": ""{sourceId}"",
    ""Status"": ""{status}"",
    ""Sub Status"": ""{subStatus}"",
    ""Sub Sub Type"": ""{subSubType}"",
    ""Suppress Calendar"": ""{suppressCalendar}"",
    ""System Asgn Flag"": ""{systemAsgnFlag}"",
    ""Template Flag"": ""{templateFlag}"",
    ""Updated By Id"": ""{updatedById}"",
    ""Updated By"": ""{updatedBy}"",
    ""Updated Date"": ""{updatedDate}""
}}";

    [Theory]
    [InlineData(activityUID, nameof(IncidentJson.ActivityUID))]
    [InlineData(afterHoursFlag, nameof(IncidentJson.AfterHoursFlag))]
    [InlineData(assignedTo, nameof(IncidentJson.AssignedTo))]
    [InlineData(assignedToId, nameof(IncidentJson.AssignedToId))]
    [InlineData(contactFirstName, nameof(IncidentJson.ContactFirstName))]
    [InlineData(contactLastName, nameof(IncidentJson.ContactLastName))]
    [InlineData(contactMiddleName, nameof(IncidentJson.ContactMiddleName))]
    [InlineData(createdBy, nameof(IncidentJson.CreatedBy))]
    [InlineData(createdById, nameof(IncidentJson.CreatedById))]
    [InlineData(createdDate, nameof(IncidentJson.CreatedDate))]
    [InlineData(dateClosed, nameof(IncidentJson.DateClosed))]
    [InlineData(dateCreated, nameof(IncidentJson.DateCreated))]
    [InlineData(dateOccurred, nameof(IncidentJson.DateOccurred))]
    [InlineData(dateReported, nameof(IncidentJson.DateReported))]
    [InlineData(daysOpen, nameof(IncidentJson.DaysOpen))]
    [InlineData(description, nameof(IncidentJson.Description))]
    [InlineData(display, nameof(IncidentJson.Display))]
    [InlineData(iCMServiceRegion, nameof(IncidentJson.ICMServiceRegion))]
    [InlineData(iCMServiceRegionCode, nameof(IncidentJson.ICMServiceRegionCode))]
    [InlineData(id, nameof(IncidentJson.RowId))]
    [InlineData(incidentCity, nameof(IncidentJson.IncidentCity))]
    [InlineData(incidentLocation, nameof(IncidentJson.IncidentLocation))]
    [InlineData(incidentPostalCode, nameof(IncidentJson.IncidentPostalCode))]
    [InlineData(incidentSubType, nameof(IncidentJson.IncidentSubType))]
    [InlineData(incidentType, nameof(IncidentJson.IncidentType))]
    [InlineData(integrationErrorDescription, nameof(IncidentJson.IntegrationErrorDescription))]
    [InlineData(integrationState, nameof(IncidentJson.IntegrationState))]
    [InlineData(location, nameof(IncidentJson.Location))]
    [InlineData(name, nameof(IncidentJson.Name))]
    [InlineData(organization, nameof(IncidentJson.Organization))]
    [InlineData(ownedBy, nameof(IncidentJson.OwnedBy))]
    [InlineData(planned, nameof(IncidentJson.Planned))]
    [InlineData(primarySuspectId, nameof(IncidentJson.PrimarySuspectId))]
    [InlineData(priority, nameof(IncidentJson.Priority))]
    [InlineData(resolution, nameof(IncidentJson.Resolution))]
    [InlineData(responseTime, nameof(IncidentJson.ResponseTime))]
    [InlineData(restrictedFlag, nameof(IncidentJson.RestrictedFlag))]
    [InlineData(rowStatusOld, nameof(IncidentJson.RowStatusOld))]
    [InlineData(serviceOffice, nameof(IncidentJson.ServiceOffice))]
    [InlineData(sourceId, nameof(IncidentJson.SourceId))]
    [InlineData(status, nameof(IncidentJson.Status))]
    [InlineData(subStatus, nameof(IncidentJson.SubStatus))]
    [InlineData(subSubType, nameof(IncidentJson.SubSubType))]
    [InlineData(suppressCalendar, nameof(IncidentJson.SuppressCalendar))]
    [InlineData(systemAsgnFlag, nameof(IncidentJson.SystemAsgnFlag))]
    [InlineData(templateFlag, nameof(IncidentJson.TemplateFlag))]
    [InlineData(updatedBy, nameof(IncidentJson.UpdatedBy))]
    [InlineData(updatedById, nameof(IncidentJson.UpdatedById))]
    [InlineData(updatedDate, nameof(IncidentJson.UpdatedDate))]
    public void ParsesFields(string expectedValue, string propertyName)
    {
        IncidentJson incident = JsonSerializer.Deserialize<IncidentJson>(json, PayloadOptions.SiebelGet)!;

        string actual = (string)incident.GetType().GetProperty(propertyName)!.GetValue(incident)!;

        Assert.Equal(expectedValue, actual);
    }
}
