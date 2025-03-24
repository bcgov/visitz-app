using Realms;
using System.Text.Json.Nodes;
using VisitzApi.Models.Caseload;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models.Interfaces;
using VisitzModel.Utilities;
using static System.Collections.Specialized.BitVector32;

namespace VisitzModel.Models.Caseload;

public partial class MemoRecord : IRealmObject, IRowMetadata, IAssignedMetadata, IApiJson<MemoJson>
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

    public string Address { get; set; }

    public string AddressComments { get; set; }

    public string AreAnyOfTheFamilyMembersIndigenous { get; set; }

    public DateTimeOffset? CallDate { get; set; }

    public DateTimeOffset? CallTime { get; set; }

    public string CallerAddress { get; set; }

    public string CallerEmail { get; set; }

    public string CallerName { get; set; }

    public string CallerPhone { get; set; }

    public string CellPhone { get; set; }

    public DateTimeOffset? ClosedDate { get; set; }

    public string CreatedByOffice { get; set; }

    public string GivenNames { get; set; }

    public string HomePhone { get; set; }

    public string LastName { get; set; }

    public string MedicalExamRequired { get; set; }

    public string MemoNumber { get; set; }

    public string MemoType { get; set; }

    public string Method { get; set; }

    public string NatureOfCall { get; set; }

    public string PccSummary { get; set; }

    public string PoliceForce { get; set; }

    public string PoliceInvestigation { get; set; }

    public DateTimeOffset? PoliceNotifiedDate { get; set; }

    public string PoliceReportNumber { get; set; }

    public string PreferredContactMethod { get; set; }

    public string RecordedBy { get; set; }

    public string Resolution { get; set; }

    public bool RestrictedFlag { get; set; }

    public string ServiceOffice { get; set; }

    public string Status { get; set; }

    public string TypeOfCaller { get; set; }

    public string Urgent { get; set; }

    public MemoRecord() { }

    public MemoRecord(MemoJson json)
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
        Address = json.Address;
        AddressComments = json.AddressComments;
        AreAnyOfTheFamilyMembersIndigenous = json.AreAnyOfTheFamilyMembersIndigenous;
        CallDate = Timestamp.ParseDateTimeOffsetNullable(json.CallDate);
        CallTime = Timestamp.ParseDateTimeOffsetNullable(json.CallTime);
        CallerAddress = json.CallerAddress;
        CallerEmail = json.CallerEmail;
        CallerName = json.CallerName;
        CallerPhone = json.CallerPhone;
        CellPhone = json.CellPhone;
        ClosedDate = Timestamp.ParseDateTimeOffsetNullable(json.ClosedDate);
        CreatedByOffice = json.CreatedByOffice;
        GivenNames = json.GivenNames;
        HomePhone = json.HomePhone;
        LastName = json.LastName;
        MedicalExamRequired = json.MedicalExamRequired;
        MemoNumber = json.MemoNumber;
        MemoType = json.MemoType;
        Method = json.Method;
        NatureOfCall = json.NatureOfCall;
        PccSummary = json.PccSummary;
        PoliceForce = json.PoliceForce;
        PoliceInvestigation = json.PoliceInvestigation;
        PoliceNotifiedDate = Timestamp.ParseDateTimeOffsetNullable(json.PoliceNotifiedDate);
        PoliceReportNumber = json.PoliceReportNumber;
        PreferredContactMethod = json.PreferredContactMethod;
        RecordedBy = json.RecordedBy;
        Resolution = json.Resolution;
        RestrictedFlag = json.RestrictedFlag.ParseWordTruthiness();
        ServiceOffice = json.ServiceOffice;
        Status = json.Status;
        TypeOfCaller = json.TypeOfCaller;
        Urgent = json.Urgent;
    }

    public static List<MemoRecord> FromApiArray(IEnumerable<MemoJson> jsonArray)
    {
        List<MemoRecord> outList = [];
        
        foreach (var jsonItem in jsonArray)
            outList.Add(new MemoRecord(jsonItem));

        return outList;
    }

    public static async Task SynchronizeAsync(Realm realm, SectionJson<MemoJson> section)
    {
        var currentAssignedIds = realm.All<MemoRecord>().AsEnumerable().Select(memo => memo.Id);
        var unassignedIds = currentAssignedIds.Except(section.AssignedIds);
        var memos = FromApiArray(section.Items ?? []);

        await RealmExtensions.CommitAsync(realm, () =>
        {
            realm.DeleteByIds<MemoRecord>(currentAssignedIds);
            realm.Upsert(memos);
        });
    }

    public MemoJson ToApiJson(string dateFormat = "s")
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
            Address = Address,
            AddressComments = AddressComments,
            AreAnyOfTheFamilyMembersIndigenous = AreAnyOfTheFamilyMembersIndigenous,
            CallDate = CallDate?.ToString(dateFormat),
            CallTime = CallTime?.ToString(dateFormat),
            CallerAddress = CallerAddress,
            CallerEmail = CallerEmail,
            CallerName = CallerName,
            CallerPhone = CallerPhone,
            CellPhone = CellPhone,
            ClosedDate = ClosedDate?.ToString(dateFormat),
            CreatedByOffice = CreatedByOffice,
            GivenNames = GivenNames,
            HomePhone = HomePhone,
            LastName = LastName,
            MedicalExamRequired = MedicalExamRequired,
            MemoNumber = MemoNumber,
            MemoType = MemoType,
            Method = Method,
            NatureOfCall = NatureOfCall,
            PccSummary = PccSummary,
            PoliceForce = PoliceForce,
            PoliceInvestigation = PoliceInvestigation,
            PoliceNotifiedDate = PoliceNotifiedDate?.ToString(dateFormat),
            PoliceReportNumber = PoliceReportNumber,
            PreferredContactMethod = PreferredContactMethod,
            RecordedBy = RecordedBy,
            Resolution = Resolution,
            RestrictedFlag = RestrictedFlag.AsTruthyChar(),
            ServiceOffice = ServiceOffice,
            Status = Status,
            TypeOfCaller = TypeOfCaller,
            Urgent = Urgent,
        };
    }
}
