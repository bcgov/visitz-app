using Realms;
using VisitzApi.Models.Caseload;
using VisitzModel.Extensions;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Interfaces;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Interfaces;
using VisitzModel.Utilities;

namespace VisitzModel.Models.Caseload;

public partial class IncidentRecord :
    IRealmObject,
    IRowMetadata,
    IBusinessObject,
    IAssignedMetadata,
    IApiJson<IncidentJson>
{
    [PrimaryKey]
    public string Id { get; set; }

    public string CreatedBy { get; set; }

    public string CreatedById { get; set; }

    public string UpdatedBy { get; set; }

    public string UpdatedById { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset UpdatedDate { get; set; }

    public string FileNumber { get; set; }

    public string GivenNames { get; set; }

    public string LastName { get; set; }

    public string AssignedTo { get; set; }

    public string AssignedToId { get; set; }

    public string AddressComments { get; set; }

    public string Address { get; set; }

    public string AreAnyOfTheFamilyMembersIndigenous { get; set; }

    public string CallerAddress { get; set; }

    public string CallerEmail { get; set; }

    public string CallerName { get; set; }

    public string CallerPhone { get; set; }

    public string Caseload { get; set; }

    public string CellPhone { get; set; }

    public DateTimeOffset? ClosedDate { get; set; }

    public string CreatedByOffice { get; set; }

    public DateTimeOffset? DateReported { get; set; }

    public string HomePhone { get; set; }

    public string MedicalExamRequired { get; set; }

    public string Method { get; set; }

    public string NatureOfCall { get; set; }

    public string PccSummary { get; set; }

    public string PoliceForce { get; set; }

    public string PoliceInvestigation { get; set; }

    public DateTimeOffset? PoliceNotifiedDate { get; set; }

    public string PoliceReportNumber { get; set; }

    public string PreferredContactMethod { get; set; }

    public string ProtectionResponse { get; set; }

    public string Resolution { get; set; }

    public string ResponsePriority { get; set; }

    public bool RestrictedFlag { get; set; }

    public string ServiceOffice { get; set; }

    public string Status { get; set; }

    private int TypeInt { get; set; }
    public EntitySubtype Type
    {
        get => (EntitySubtype)TypeInt;
        set => TypeInt = (int)value;
    }

    public string TypeOfCaller { get; set; }

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
        FileNumber = json.IncidentNumber;
        GivenNames = json.GivenNames;
        LastName = json.LastName;
        AssignedTo = json.AssignedTo;
        AssignedToId = json.AssignedToId;
        AddressComments = json.AddressComments;
        Address = json.Address;
        AreAnyOfTheFamilyMembersIndigenous = json.AreAnyOfTheFamilyMembersIndigenous;
        CallerAddress = json.CallerAddress;
        CallerEmail = json.CallerEmail;
        CallerName = json.CallerName;
        CallerPhone = json.CallerPhone;
        Caseload = json.Caseload;
        CellPhone = json.CellPhone;
        ClosedDate = Timestamp.ParseDateTimeOffsetNullable(json.ClosedDate);
        CreatedByOffice = json.CreatedByOffice;
        DateReported = Timestamp.ParseDateTimeOffsetNullable(json.DateReported);
        HomePhone = json.HomePhone;
        MedicalExamRequired = json.MedicalExamRequired;
        Method = json.Method;
        NatureOfCall = json.NatureOfCall;
        PccSummary = json.PccSummary;
        PoliceForce = json.PoliceForce;
        PoliceInvestigation = json.PoliceInvestigation;
        PoliceNotifiedDate = Timestamp.ParseDateTimeOffsetNullable(json.PoliceNotifiedDate);
        PoliceReportNumber = json.PoliceReportNumber;
        PreferredContactMethod = json.PreferredContactMethod;
        ProtectionResponse = json.ProtectionResponse;
        Resolution = json.Resolution;
        ResponsePriority = json.ResponsePriority;
        RestrictedFlag = json.RestrictedFlag.ParseWordTruthiness();
        ServiceOffice = json.ServiceOffice;
        Status = json.Status;
        Type = json.Type?.ParseEntitySubtype() ?? EntitySubtype.Unknown;
        TypeOfCaller = json.TypeOfCaller;
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
            IncidentNumber = FileNumber,
            GivenNames = GivenNames,
            LastName = LastName,
            AssignedTo = AssignedTo,
            AssignedToId = AssignedToId,
            AddressComments = AddressComments,
            Address = Address,
            AreAnyOfTheFamilyMembersIndigenous = AreAnyOfTheFamilyMembersIndigenous,
            CallerAddress = CallerAddress,
            CallerEmail = CallerEmail,
            CallerName = CallerName,
            CallerPhone = CallerPhone,
            Caseload = Caseload,
            CellPhone = CellPhone,
            ClosedDate = ClosedDate?.ToString(dateFormat),
            CreatedByOffice = CreatedByOffice,
            DateReported = DateReported?.ToString(dateFormat),
            HomePhone = HomePhone,
            MedicalExamRequired = MedicalExamRequired,
            Method = Method,
            NatureOfCall = NatureOfCall,
            PccSummary = PccSummary,
            PoliceForce = PoliceForce,
            PoliceInvestigation = PoliceInvestigation,
            PoliceNotifiedDate = PoliceNotifiedDate?.ToString(dateFormat),
            PoliceReportNumber = PoliceReportNumber,
            PreferredContactMethod = PreferredContactMethod,
            ProtectionResponse = ProtectionResponse,
            Resolution = Resolution,
            ResponsePriority = ResponsePriority,
            RestrictedFlag = RestrictedFlag.AsTruthyChar(),
            ServiceOffice = ServiceOffice,
            Status = Status,
            Type = Type.GetDisplayString(),
            TypeOfCaller = TypeOfCaller,
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

        string type = EntityType.Incident.ToString();
        var v2Incidents = FromApiJsonArray(section.Items ?? []);
        var v1Incidents = realm.All<CaseloadItem>().Where(item => item.EntityType == type);

        await RealmExtensions.CommitAsync(realm, () =>
        {
            realm.DeleteByIds<IncidentRecord>(unassignedIds);
            realm.Upsert(v2Incidents);

            MapCaseloadItemRowIds(v2Incidents, v1Incidents);
        });
    }

    // TODO: Remove this once we've fully removed V1 CaseloadItems
    static void MapCaseloadItemRowIds(
        IEnumerable<IncidentRecord> v2Incidents,
        IEnumerable<CaseloadItem> v1Incidents)
    {
        foreach (var v1Incident in v1Incidents)
            v1Incident.RowId = v2Incidents.FirstOrDefault(v2Incident =>
                v2Incident.FileNumber == v1Incident.CaseIncidentNumber)?.Id;
    }
}
