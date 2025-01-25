using Realms;
using VisitzApi.Models;
using VisitzApi.Models.Caseload;
using VisitzModel.Extensions;
using VisitzModel.Utilities;

namespace VisitzModel.Models.Caseload;

public partial class CaseRecord : IRealmObject, IRowMetadata, IAssignedMetadata
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

    public string Type { get; set; }

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
        Type = caseJson.Type;
        WorkQueue = caseJson.WorkQueue;
    }

    public CaseJson ToCaseJson(string dateFormat = "s")
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
            Type = Type,
            WorkQueue = WorkQueue,
        };
    }

    public static List<CaseRecord> FromCasesJson(IEnumerable<CaseJson> casesJson)
    {
        List<CaseRecord> outList = [];

        foreach (var caseJson in casesJson)
            outList.Add(new CaseRecord(caseJson));

        return outList;
    }

    public static async Task SynchronizeCasesAsync(Realm realm, SectionJson<CaseJson> casesSection)
    {
        var currentAssignedIds = realm.All<CaseRecord>().AsEnumerable().Select(@case => @case.Id);
        var unassignedIds = currentAssignedIds.Except(casesSection.AssignedIds);

        await RealmExtensions.CommitAsync(realm, () =>
        {
            realm.DeleteByIds<CaseRecord>(unassignedIds);
            realm.Upsert(FromCasesJson(casesSection.Items));
        });
    }
}
