using Realms;
using VisitzApi.Models.Caseload;
using VisitzModel.Extensions;
using VisitzModel.Utilities;

namespace VisitzModel.Models.Caseload;

public partial class IncidentRecord : IRealmObject, IRowMetadata, IAssignedMetadata, IApiJson<IncidentJson>
{
    [PrimaryKey]
    public string Id { get; set; }

    public string CreatedBy { get; set; }

    public string CreatedById { get; set; }

    public string UpdatedBy { get; set; }

    public string UpdatedById { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset UpdatedDate { get; set; }

    public string AssignedTo { get; set; }

    public string AssignedToId { get; set; }

    public string ActivityUid { get; set; }

    public bool AfterHoursFlag { get; set; }

    public string ContactFirstName { get; set; }

    public string ContactLastName { get; set; }

    public string ContactMiddleName { get; set; }

    public DateTimeOffset? DateClosed { get; set; }

    public DateTimeOffset? DateCreated { get; set; } // TODO: not necessary since we have CreatedDate?

    public DateTimeOffset? DateOccurred { get; set; }

    public DateTimeOffset? DateReported { get; set; }

    public string DaysOpen { get; set; }

    public string Description { get; set; }

    public string Display { get; set; }

    public string IcmServiceRegion { get; set; }

    public string IcmServiceRegionCode { get; set; }

    public string IncidentCity { get; set; }

    public string IncidentLocation { get; set; }

    public string IncidentPostalCode { get; set; }

    public string IncidentSubType { get; set; }

    public string IncidentType { get; set; }

    public string IntegrationErrorDescription { get; set; }

    public string IntegrationState { get; set; }

    public string Location { get; set; }

    public string Name { get; set; }

    public string Organization { get; set; }

    public string OwnedBy { get; set; }

    public string Planned { get; set; }

    public string PrimarySuspectId { get; set; }

    public string Priority { get; set; }

    public string Resolution { get; set; }

    public string ResponseTime { get; set; }

    public bool RestrictedFlag { get; set; }

    public string RowStatusOld { get; set; }

    public string ServiceOffice { get; set; }

    public string SourceId { get; set; }

    public string Status { get; set; }

    public string SubStatus { get; set; }

    public string SubSubType { get; set; }

    public string SuppressCalendar { get; set; }

    public string SystemAsgnFlag { get; set; }

    public string TemplateFlag { get; set; }

    public IncidentRecord() { }

    public IncidentRecord(IncidentJson json)
    {
        Id = json.Id;
        CreatedBy = json.CreatedBy;
        CreatedById = json.CreatedById;
        UpdatedBy = json.UpdatedBy;
        UpdatedById = json.UpdatedById;
        CreatedDate = DateTimeOffset.Parse(json.CreatedDate);
        UpdatedDate = DateTimeOffset.Parse(json.UpdatedDate);
        AssignedTo = json.AssignedTo;
        AssignedToId = json.AssignedToId;
        ActivityUid = json.ActivityUID;
        AfterHoursFlag = json.AfterHoursFlag.ParseWordTruthiness();
        ContactFirstName = json.ContactFirstName;
        ContactLastName = json.ContactLastName;
        ContactMiddleName = json.ContactMiddleName;
        DateClosed = Timestamp.ParseDateTimeOffsetNullable(json.DateClosed);
        DateCreated = Timestamp.ParseDateTimeOffsetNullable(json.DateCreated);
        DateOccurred = Timestamp.ParseDateTimeOffsetNullable(json.DateOccurred);
        DateReported = Timestamp.ParseDateTimeOffsetNullable(json.DateReported);
        DaysOpen = json.DaysOpen;
        Description = json.Description;
        Display = json.Display;
        IcmServiceRegion = json.ICMServiceRegion;
        IcmServiceRegionCode = json.ICMServiceRegionCode;
        IncidentCity = json.IncidentCity;
        IncidentLocation = json.IncidentLocation;
        IncidentPostalCode = json.IncidentPostalCode;
        IncidentSubType = json.IncidentSubType;
        IncidentType = json.IncidentType;
        IntegrationErrorDescription = json.IntegrationErrorDescription;
        IntegrationState = json.IntegrationState;
        Location = json.Location;
        Name = json.Name;
        Organization = json.Organization;
        OwnedBy = json.OwnedBy;
        Planned = json.Planned;
        PrimarySuspectId = json.PrimarySuspectId;
        Priority = json.Priority;
        Resolution = json.Resolution;
        ResponseTime = json.ResponseTime;
        RestrictedFlag = json.RestrictedFlag.ParseWordTruthiness();
        RowStatusOld = json.RowStatusOld;
        ServiceOffice = json.ServiceOffice;
        SourceId = json.SourceId;
        Status = json.Status;
        SubStatus = json.SubStatus;
        SubSubType = json.SubSubType;
        SuppressCalendar = json.SuppressCalendar;
        SystemAsgnFlag = json.SystemAsgnFlag;
        TemplateFlag = json.TemplateFlag;
    }

    public IncidentJson ToApiJson(string dateFormat = "s")
    {
        return new()
        {
            Id = Id,
            CreatedBy = CreatedBy,
            CreatedById = CreatedById,
            UpdatedBy = UpdatedBy,
            UpdatedById = UpdatedById,
            CreatedDate = CreatedDate.ToString(dateFormat),
            UpdatedDate = UpdatedDate.ToString(dateFormat),
            AssignedTo = AssignedTo,
            AssignedToId = AssignedToId,
            ActivityUID = ActivityUid,
            AfterHoursFlag = AfterHoursFlag.AsTruthyChar(),
            ContactFirstName = ContactFirstName,
            ContactLastName = ContactLastName,
            ContactMiddleName = ContactMiddleName,
            DateClosed = Timestamp.WriteDateTimeOffset(DateClosed, dateFormat),
            DateCreated = Timestamp.WriteDateTimeOffset(DateCreated, dateFormat),
            DateOccurred = Timestamp.WriteDateTimeOffset(DateOccurred, dateFormat),
            DateReported = Timestamp.WriteDateTimeOffset(DateReported, dateFormat),
            DaysOpen = DaysOpen,
            Description = Description,
            Display = Display,
            ICMServiceRegion = IcmServiceRegion,
            ICMServiceRegionCode = IcmServiceRegionCode,
            IncidentCity = IncidentCity,
            IncidentLocation = IncidentLocation,
            IncidentPostalCode = IncidentPostalCode,
            IncidentSubType = IncidentSubType,
            IncidentType = IncidentType,
            IntegrationErrorDescription = IntegrationErrorDescription,
            IntegrationState = IntegrationState,
            Location = Location,
            Name = Name,
            Organization = Organization,
            OwnedBy = OwnedBy,
            Planned = Planned,
            PrimarySuspectId = PrimarySuspectId,
            Priority = Priority,
            Resolution = Resolution,
            ResponseTime = ResponseTime,
            RestrictedFlag = RestrictedFlag.AsTruthyChar(),
            RowStatusOld = RowStatusOld,
            ServiceOffice = ServiceOffice,
            SourceId = SourceId,
            Status = Status,
            SubStatus = SubStatus,
            SubSubType = SubSubType,
            SuppressCalendar = SuppressCalendar,
            SystemAsgnFlag = SystemAsgnFlag,
            TemplateFlag = TemplateFlag,
        };
    }

    public static List<IncidentRecord> FromApiJsonArray(IEnumerable<IncidentJson> jsonArray)
    {
        List<IncidentRecord> outList = [];

        foreach (var jsonItem in jsonArray)
            outList.Add(new IncidentRecord(jsonItem));

        return outList;
    }

    public static async Task SynchronizeAsync(Realm realm, SectionJson<IncidentJson> section)
    {
        var currentAssignedIds = realm.All<IncidentRecord>().AsEnumerable().Select(incident => incident.Id);
        var unassignedIds = currentAssignedIds.Except(section.AssignedIds);

        await RealmExtensions.CommitAsync(realm, () =>
        {
            realm.DeleteByIds<IncidentRecord>(unassignedIds);
            realm.Upsert(FromApiJsonArray(section.Items));
        });
    }
}
