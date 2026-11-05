using System.Text.Json;
using VisitzApi.Json;
using VisitzApi.Models.Activities;

namespace VisitzApiTest.Models.Activities;

public class ActivityJsonTests
{
    internal const string id = "activity1";
    internal const string activityId = "activityId1";
    internal const string activityUID = "activityUId1";
    internal const string caseId = "case123";
    internal const string caseNumber = "1-8877";
    internal const string caseOwner = "Test owner";
    internal const string caseRestricted = "N";
    internal const string caseStatus = "Open";
    internal const string caseType = "Family Services";
    internal const string incidentId = "In124";
    internal const string incidentNo = "InsNo24";
    internal const string activitySRId = "SR-12346";
    internal const string sRNumber = "SR-NUM-56";
    internal const string sROwner = "Test Owner";
    internal const string sRSubType = "Sr1234Type";
    internal const string sRType = "Sr123Type";
    internal const string iCMMemoId = "ICM-Mem-1";
    internal const string iCMMemoNo = "ICM-Mem-12";
    internal const string iCMMemoOwner = "Memo Owner";
    internal const string iCMMemoRestricted = "Y";
    internal const string iCMMemoType = "memo type";
    internal const string primaryContactId = "Test User";
    internal const string contactFirstName = "First Name Here";
    internal const string contactGivenName = "Given Name Here";
    internal const string contactMiddleName = "";
    internal const string contactLastName = "Last Name Here";
    internal const string showInContactProfile = "N";
    internal const string status = "Status Here";
    internal const string category = "General";
    internal const string type = "Type Here";
    internal const string iCMType = "ICM Type Here";
    internal const string iCMSubType = "ICM Sub Type Here";
    internal const string description = "Description Here";
    internal const string actionBy = "Action By Here";
    internal const string actionCode = "";
    internal const string alarm = "N";
    internal const string apptAlarmTimeMin = "";
    internal const string delay = "";
    internal const string display = "Display Here";
    internal const string done = "";
    internal const string doneFlag = "N";
    internal const string due = "01/01/1970 00:00:00";
    internal const string durationMinutes = "Duration Minutes Here";
    internal const string iCMServiceRegion = "Service Region Here";
    internal const string meetingLocation = "Test Location";
    internal const string ministry = "Ministry Here";
    internal const string ministryId = "Ministry Id Here";
    internal const string overdue = "Y";
    internal const string planName = "";
    internal const string planned = "01/01/1970 00:00:00";
    internal const string priority = "Priority Here";
    internal const string repeating = "N";
    internal const string repeatingExpires = "y";
    internal const string resultCode = "Success";
    internal const string sequence = "123";
    internal const string serviceOffice = "Service Office Here";
    internal const string iCMCreatedByOffice = "Created By Office Here";
    internal const string primaryOwnedBy = "Primary Owner Here";
    internal const string primaryOwnerId = "Primary Owner Id Here";
    internal const string accountLocation = "Accnt Location";
    internal const string accountName = "Account Test";
    internal const string activityTemplateId = "temp 123";
    internal const string assetNumberS_ASSET = "Asset ABC";
    internal const string assetNumber = "Asset1";
    internal const string brand = "brand123";
    internal const string c_class = "class123";
    internal const string opportunity = "opp123";
    internal const string orderNumber = "order123";
    internal const string ownedById = "test123";
    internal const string ownedBy = "Test Owner";
    internal const string previousActivityDescription = "Past desc";
    internal const string priceList = "List 123";
    internal const string property = "Prop123";
    internal const string quote = "quote123";
    internal const string rateList = "rate13";
    internal const string repeatingType = "Repeating Type Here";
    internal const string restrictedFlag = "N";
    internal const string sharedFlag = "N";
    internal const string created = "01/01/1970 00:00:00";
    internal const string createdBy = "Creator Id Here";
    internal const string createdByName = "Created By Name Here";
    internal const string updated = "01/01/1970 00:00:00";
    internal const string updatedBy = "Updater Id Here";
    internal const string updatedByName = "Updater Here";
    internal const string rowId = "Another Id Here";

    internal const string json =
        @$"{{
      ""Id"": ""{id}"",
      ""Activity Id"": ""{activityId}"",
      ""Activity UID"": ""{activityUID}"",
      ""Case Id"": ""{caseId}"",
      ""Case Number"": ""{caseNumber}"",
      ""Case Owner"": ""{caseOwner}"",
      ""Case Restricted"": ""{caseRestricted}"",
      ""Case Status"": ""{caseStatus}"",
      ""Case Type"": ""{caseType}"",
      ""Incident Id"": ""{incidentId}"",
      ""Incident #"": ""{incidentNo}"",
      ""Activity SR Id"": ""{activitySRId}"",
      ""SR Number"": ""{sRNumber}"",
      ""SR Owner"": ""{sROwner}"",
      ""SR Sub Type"": ""{sRSubType}"",
      ""SR Type"": ""{sRType}"",
      ""ICM Memo Id"": ""{iCMMemoId}"",
      ""ICM Memo #"": ""{iCMMemoNo}"",
      ""ICM Memo Owner"": ""{iCMMemoOwner}"",
      ""ICM Memo Restricted"": ""{iCMMemoRestricted}"",
      ""ICM Memo Type"": ""{iCMMemoType}"",
      ""Primary Contact Id"": ""{primaryContactId}"",
      ""Contact First Name"": ""{contactFirstName}"",
      ""Contact Given Name"": ""{contactGivenName}"",
      ""Contact Middle Name"": ""{contactMiddleName}"",
      ""Contact Last Name"": ""{contactLastName}"",
      ""Show In Contact Profile"": ""{showInContactProfile}"",
      ""Status"": ""{status}"",
      ""Category"": ""{category}"",
      ""Type"": ""{type}"",
      ""ICM Type"": ""{iCMType}"",
      ""ICM Sub Type"": ""{iCMSubType}"",
      ""Description"": ""{description}"",
      ""Action By"": ""{actionBy}"",
      ""Action Code"": ""{actionCode}"",
      ""Alarm"": ""{alarm}"",
      ""Appt Alarm Time Min"": ""{apptAlarmTimeMin}"",
      ""Delay"": ""{delay}"",
      ""Display"": ""{display}"",
      ""Done"": ""{done}"",
      ""Done Flag"": ""{doneFlag}"",
      ""Due"": ""{due}"",
      ""Duration Minutes"": ""{durationMinutes}"",
      ""ICM Service Region"": ""{iCMServiceRegion}"",
      ""MeetingLocation"": ""{meetingLocation}"",
      ""Ministry"": ""{ministry}"",
      ""Ministry Id"": ""{ministryId}"",
      ""Overdue"": ""{overdue}"",
      ""Plan Name"": ""{planName}"",
      ""Planned"": ""{planned}"",
      ""Priority"": ""{priority}"",
      ""Repeating"": ""{repeating}"",
      ""Repeating Expires"": ""{repeatingExpires}"",
      ""Result Code"": ""{resultCode}"",
      ""Sequence"": ""{sequence}"",
      ""Service Office"": ""{serviceOffice}"",
      ""ICM Created By Office"": ""{iCMCreatedByOffice}"",
      ""Primary Owned By"": ""{primaryOwnedBy}"",
      ""Primary Owner Id"": ""{primaryOwnerId}"",
      ""Account Location"": ""{accountLocation}"",
      ""Account Name"": ""{accountName}"",
      ""Activity Template Id"": ""{activityTemplateId}"",
      ""Asset Number S_ASSET"": ""{assetNumberS_ASSET}"",
      ""Asset Number"": ""{assetNumber}"",
      ""Brand"": ""{brand}"",
      ""Class"": ""{c_class}"",
      ""Opportunity"": ""{opportunity}"",
      ""Order Number"": ""{orderNumber}"",
      ""Owned By Id"": ""{ownedById}"",
      ""Owned By"": ""{ownedBy}"",
      ""Previous Activity Description"": ""{previousActivityDescription}"",
      ""Price List"": ""{priceList}"",
      ""Property"": ""{property}"",
      ""Quote"": ""{quote}"",
      ""Rate List"": ""{rateList}"",
      ""Repeating Type"": ""{repeatingType}"",
      ""Restricted Flag"": ""{restrictedFlag}"",
      ""Shared Flag"": ""{sharedFlag}"",
      ""Created"": ""{created}"",
      ""Created By"": ""{createdBy}"",
      ""Created By Name"": ""{createdByName}"",
      ""Updated"": ""{updated}"",
      ""Updated By"": ""{updatedBy}"",
      ""Updated By Name"": ""{updatedByName}"",
      ""Row Id"": ""{rowId}""
    }}";

    [Theory]
    [InlineData(id, nameof(ActivityJson.Id))]
    [InlineData(activityId, nameof(ActivityJson.ActivityId))]
    [InlineData(activityUID, nameof(ActivityJson.ActivityUID))]
    [InlineData(caseId, nameof(ActivityJson.CaseId))]
    [InlineData(caseNumber, nameof(ActivityJson.CaseNumber))]
    [InlineData(caseOwner, nameof(ActivityJson.CaseOwner))]
    [InlineData(caseRestricted, nameof(ActivityJson.CaseRestricted))]
    [InlineData(caseStatus, nameof(ActivityJson.CaseStatus))]
    [InlineData(caseType, nameof(ActivityJson.CaseType))]
    [InlineData(incidentId, nameof(ActivityJson.IncidentId))]
    [InlineData(incidentNo, nameof(ActivityJson.IncidentNo))]
    [InlineData(activitySRId, nameof(ActivityJson.ActivitySRId))]
    [InlineData(sRNumber, nameof(ActivityJson.SRNumber))]
    [InlineData(sROwner, nameof(ActivityJson.SROwner))]
    [InlineData(sRSubType, nameof(ActivityJson.SRSubType))]
    [InlineData(sRType, nameof(ActivityJson.SRType))]
    [InlineData(iCMMemoId, nameof(ActivityJson.ICMMemoId))]
    [InlineData(iCMMemoNo, nameof(ActivityJson.ICMMemoNo))]
    [InlineData(iCMMemoOwner, nameof(ActivityJson.ICMMemoOwner))]
    [InlineData(iCMMemoRestricted, nameof(ActivityJson.ICMMemoRestricted))]
    [InlineData(iCMMemoType, nameof(ActivityJson.ICMMemoType))]
    [InlineData(primaryContactId, nameof(ActivityJson.PrimaryContactId))]
    [InlineData(contactFirstName, nameof(ActivityJson.ContactFirstName))]
    [InlineData(contactGivenName, nameof(ActivityJson.ContactGivenName))]
    [InlineData(contactMiddleName, nameof(ActivityJson.ContactMiddleName))]
    [InlineData(contactLastName, nameof(ActivityJson.ContactLastName))]
    [InlineData(showInContactProfile, nameof(ActivityJson.ShowInContactProfile))]
    [InlineData(status, nameof(ActivityJson.Status))]
    [InlineData(category, nameof(ActivityJson.Category))]
    [InlineData(type, nameof(ActivityJson.Type))]
    [InlineData(iCMType, nameof(ActivityJson.ICMType))]
    [InlineData(iCMSubType, nameof(ActivityJson.ICMSubType))]
    [InlineData(description, nameof(ActivityJson.Description))]
    [InlineData(actionBy, nameof(ActivityJson.ActionBy))]
    [InlineData(actionCode, nameof(ActivityJson.ActionCode))]
    [InlineData(alarm, nameof(ActivityJson.Alarm))]
    [InlineData(apptAlarmTimeMin, nameof(ActivityJson.ApptAlarmTimeMin))]
    [InlineData(delay, nameof(ActivityJson.Delay))]
    [InlineData(display, nameof(ActivityJson.Display))]
    [InlineData(done, nameof(ActivityJson.Done))]
    [InlineData(doneFlag, nameof(ActivityJson.DoneFlag))]
    [InlineData(due, nameof(ActivityJson.Due))]
    [InlineData(durationMinutes, nameof(ActivityJson.DurationMinutes))]
    [InlineData(iCMServiceRegion, nameof(ActivityJson.ICMServiceRegion))]
    [InlineData(meetingLocation, nameof(ActivityJson.MeetingLocation))]
    [InlineData(ministry, nameof(ActivityJson.Ministry))]
    [InlineData(ministryId, nameof(ActivityJson.MinistryId))]
    [InlineData(overdue, nameof(ActivityJson.Overdue))]
    [InlineData(planName, nameof(ActivityJson.PlanName))]
    [InlineData(planned, nameof(ActivityJson.Planned))]
    [InlineData(priority, nameof(ActivityJson.Priority))]
    [InlineData(repeating, nameof(ActivityJson.Repeating))]
    [InlineData(repeatingExpires, nameof(ActivityJson.RepeatingExpires))]
    [InlineData(resultCode, nameof(ActivityJson.ResultCode))]
    [InlineData(sequence, nameof(ActivityJson.Sequence))]
    [InlineData(serviceOffice, nameof(ActivityJson.ServiceOffice))]
    [InlineData(iCMCreatedByOffice, nameof(ActivityJson.ICMCreatedByOffice))]
    [InlineData(primaryOwnedBy, nameof(ActivityJson.PrimaryOwnedBy))]
    [InlineData(primaryOwnerId, nameof(ActivityJson.PrimaryOwnerId))]
    [InlineData(accountLocation, nameof(ActivityJson.AccountLocation))]
    [InlineData(accountName, nameof(ActivityJson.AccountName))]
    [InlineData(activityTemplateId, nameof(ActivityJson.ActivityTemplateId))]
    [InlineData(assetNumberS_ASSET, nameof(ActivityJson.AssetNumberS_ASSET))]
    [InlineData(assetNumber, nameof(ActivityJson.AssetNumber))]
    [InlineData(brand, nameof(ActivityJson.Brand))]
    [InlineData(c_class, nameof(ActivityJson.Class))]
    [InlineData(opportunity, nameof(ActivityJson.Opportunity))]
    [InlineData(orderNumber, nameof(ActivityJson.OrderNumber))]
    [InlineData(ownedById, nameof(ActivityJson.OwnedById))]
    [InlineData(ownedBy, nameof(ActivityJson.OwnedBy))]
    [InlineData(previousActivityDescription, nameof(ActivityJson.PreviousActivityDescription))]
    [InlineData(priceList, nameof(ActivityJson.PriceList))]
    [InlineData(property, nameof(ActivityJson.Property))]
    [InlineData(quote, nameof(ActivityJson.Quote))]
    [InlineData(rateList, nameof(ActivityJson.RateList))]
    [InlineData(repeatingType, nameof(ActivityJson.RepeatingType))]
    [InlineData(restrictedFlag, nameof(ActivityJson.RestrictedFlag))]
    [InlineData(sharedFlag, nameof(ActivityJson.SharedFlag))]
    [InlineData(created, nameof(ActivityJson.Created))]
    [InlineData(createdBy, nameof(ActivityJson.CreatedBy))]
    [InlineData(createdByName, nameof(ActivityJson.CreatedByName))]
    [InlineData(updated, nameof(ActivityJson.Updated))]
    [InlineData(updatedBy, nameof(ActivityJson.UpdatedBy))]
    [InlineData(updatedByName, nameof(ActivityJson.UpdatedByName))]
    [InlineData(rowId, nameof(ActivityJson.RowId))]
    public void ParsesStringFieldsCorrectly(string expectedValue, string propertyName)
    {
        ActivityJson activity = JsonSerializer.Deserialize<ActivityJson>(json, PayloadOptions.SiebelGet)!;

        string actual = (string)activity.GetType().GetProperty(propertyName)!.GetValue(activity)!;

        Assert.Equal(expectedValue, actual);
    }
}
