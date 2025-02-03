using Realms;
using VisitzApi.Models.Caseload;
using VisitzModel.Extensions;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Interfaces;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Interfaces;
using VisitzModel.Utilities;

namespace VisitzModel.Models.Caseload;

public partial class CaseRecord : IRealmObject, IRowMetadata, IAssignedMetadata, IApiJson<CaseJson>
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

    public string Caseload { get; set; }

    public string CaseNum { get; set; }

    public DateTimeOffset? ClosedDate { get; set; }

    public string CloseReason { get; set; }

    public string EarlyOpenReason { get; set; }

    public string IntegrationState { get; set; }

    public string LegacyFileNumber { get; set; }

    public string MiddleName { get; set; }

    public bool MyFSFlag { get; set; }

    public string Name { get; set; }

    public string OfficeName { get; set; }

    public string Organization { get; set; }

    public string RegionName { get; set; }

    public DateTimeOffset? RenewReviewDate { get; set; }

    public DateTimeOffset? ReopenedDate { get; set; }

    public bool RestrictedFlag { get; set; }

    public string Status { get; set; }

    public string SubjectContactFirstName { get; set; }

    public string SubjectContactLastName { get; set; }

    int TypeInt { get; set; }
    public EntitySubtype Type
    {
        get => (EntitySubtype)TypeInt;
        set => TypeInt = (int)value;
    }

    public string WorkQueue { get; set; }

    public CaseRecord() { }

    public CaseRecord(CaseJson caseJson)
    {
        Id = caseJson.Id;
        CreatedBy = caseJson.CreatedBy;
        CreatedById = caseJson.CreatedById;
        UpdatedBy = caseJson.UpdatedBy;
        UpdatedById = caseJson.UpdatedById;
        CreatedDate = DateTimeOffset.Parse(caseJson.CreatedDate);
        UpdatedDate = DateTimeOffset.Parse(caseJson.UpdatedDate);
        AssignedTo = caseJson.AssignedTo;
        AssignedToId = caseJson.AssignedToId;
        Caseload = caseJson.Caseload;
        CaseNum = caseJson.CaseNum;
        ClosedDate = Timestamp.ParseDateTimeOffsetNullable(caseJson.ClosedDate);
        CloseReason = caseJson.CloseReason;
        EarlyOpenReason = caseJson.EarlyOpenReason;
        IntegrationState = caseJson.IntegrationState;
        LegacyFileNumber = caseJson.LegacyFileNumber;
        MiddleName = caseJson.MiddleName;
        MyFSFlag = caseJson.MyFSFlag.ParseWordTruthiness();
        Name = caseJson.Name;
        OfficeName = caseJson.OfficeName;
        Organization = caseJson.Organization;
        RegionName = caseJson.RegionName;
        RenewReviewDate = Timestamp.ParseDateTimeOffsetNullable(caseJson.RenewReviewDate);
        ReopenedDate = Timestamp.ParseDateTimeOffsetNullable(caseJson.ReopenedDate);
        RestrictedFlag = caseJson.RestrictedFlag.ParseWordTruthiness();
        Status = caseJson.Status;
        SubjectContactFirstName = caseJson.SubjectContactFirstName;
        SubjectContactLastName = caseJson.SubjectContactLastName;
        Type = caseJson.Type.ParseEntitySubtype();
        WorkQueue = caseJson.WorkQueue;
    }

    public CaseJson ToApiJson(string dateFormat = "s")
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
            Caseload = Caseload,
            CaseNum = CaseNum,
            ClosedDate = Timestamp.WriteDateTimeOffset(ClosedDate, dateFormat),
            CloseReason = CloseReason,
            EarlyOpenReason = EarlyOpenReason,
            IntegrationState = IntegrationState,
            LegacyFileNumber = LegacyFileNumber,
            MiddleName = MiddleName,
            MyFSFlag = MyFSFlag.AsTruthyChar(),
            Name = Name,
            OfficeName = OfficeName,
            Organization = Organization,
            RegionName = RegionName,
            RenewReviewDate = Timestamp.WriteDateTimeOffset(RenewReviewDate, dateFormat),
            ReopenedDate = Timestamp.WriteDateTimeOffset(ReopenedDate, dateFormat),
            RestrictedFlag = RestrictedFlag.AsTruthyChar(),
            Status = Status,
            SubjectContactFirstName = SubjectContactFirstName,
            SubjectContactLastName = SubjectContactLastName,
            Type = Type.GetDisplayString(),
            WorkQueue = WorkQueue,
        };
    }

    public static List<CaseRecord> FromApiJsonArray(IEnumerable<CaseJson> jsonArray)
    {
        List<CaseRecord> outList = [];

        foreach (var jsonItem in jsonArray)
            outList.Add(new CaseRecord(jsonItem));

        return outList;
    }

    public static async Task SynchronizeCasesAsync(Realm realm, SectionJson<CaseJson> section)
    {
        var currentAssignedIds = realm.All<CaseRecord>().AsEnumerable().Select(@case => @case.Id);
        var unassignedIds = currentAssignedIds.Except(section.AssignedIds);

        await RealmExtensions.CommitAsync(realm, () =>
        {
            realm.DeleteByIds<CaseRecord>(unassignedIds);
            realm.Upsert(FromApiJsonArray(section.Items));
        });
    }
}
