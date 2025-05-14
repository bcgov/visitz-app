using Realms;
using System.Globalization;
using VisitzApi.Models.Caseload;
using VisitzModel.Extensions;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Interfaces;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Interfaces;
using VisitzModel.Models.People;
using VisitzModel.Storage;
using VisitzModel.Utilities;

namespace VisitzModel.Models.Caseload;

public partial class CaseRecord :
    IRealmObject,
    IRowMetadata,
    IBusinessObject,
    IAssignedMetadata,
    IApiJson<CaseJson>
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

    public EntityType EntityType => EntityType.Case;

    public string GivenNames { get; set; }

    public string LastName { get; set; }

    public string AssignedTo { get; set; }

    public string AssignedToId { get; set; }

    public string Caseload { get; set; }

    public DateTimeOffset? ClosedDate { get; set; }

    public string CloseReason { get; set; }

    public string EarlyOpenReason { get; set; }

    public string IntegrationState { get; set; }

    public string LegacyFileNumber { get; set; }

    public string MiddleName { get; set; }

    public bool MyFSFlag { get; set; }

    public string Name { get; set; }

    public string ServiceOffice { get; set; }

    public string Organization { get; set; }

    public string RegionName { get; set; }

    public DateTimeOffset? RenewReviewDate { get; set; }

    public DateTimeOffset? ReopenedDate { get; set; }

    public bool RestrictedFlag { get; set; }

    public string Status { get; set; }

    int TypeInt { get; set; }
    public EntitySubtype EntitySubtype
    {
        get => (EntitySubtype)TypeInt;
        set => TypeInt = (int)value;
    }

    public string WorkQueue { get; set; }

    public string DisplayDate => CreatedDate.ToString(
        IBusinessObject.DisplayDateFormat,
        CultureInfo.InvariantCulture);

    public string DisplayName => IBusinessObjectExtensions.GetDisplayName(this);

    public string FullType => IBusinessObjectExtensions.GetFullType(this);

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
        FileNumber = caseJson.CaseNum;
        GivenNames = caseJson.SubjectContactFirstName;
        LastName = caseJson.SubjectContactLastName;
        AssignedTo = caseJson.AssignedTo;
        AssignedToId = caseJson.AssignedToId;
        Caseload = caseJson.Caseload;
        ClosedDate = Timestamp.ParseDateTimeOffsetNullable(caseJson.ClosedDate);
        CloseReason = caseJson.CloseReason;
        EarlyOpenReason = caseJson.EarlyOpenReason;
        IntegrationState = caseJson.IntegrationState;
        LegacyFileNumber = caseJson.LegacyFileNumber;
        MiddleName = caseJson.MiddleName;
        MyFSFlag = caseJson.MyFSFlag.ParseWordTruthiness();
        Name = caseJson.Name;
        ServiceOffice = caseJson.OfficeName;
        Organization = caseJson.Organization;
        RegionName = caseJson.RegionName;
        RenewReviewDate = Timestamp.ParseDateTimeOffsetNullable(caseJson.RenewReviewDate);
        ReopenedDate = Timestamp.ParseDateTimeOffsetNullable(caseJson.ReopenedDate);
        RestrictedFlag = caseJson.RestrictedFlag.ParseWordTruthiness();
        Status = caseJson.Status;
        EntitySubtype = caseJson.Type.ParseEntitySubtype();
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
            CaseNum = FileNumber,
            SubjectContactFirstName = GivenNames,
            SubjectContactLastName = LastName,
            AssignedTo = AssignedTo,
            AssignedToId = AssignedToId,
            Caseload = Caseload,
            ClosedDate = Timestamp.WriteDateTimeOffset(ClosedDate, dateFormat),
            CloseReason = CloseReason,
            EarlyOpenReason = EarlyOpenReason,
            IntegrationState = IntegrationState,
            LegacyFileNumber = LegacyFileNumber,
            MiddleName = MiddleName,
            MyFSFlag = MyFSFlag.AsTruthyChar(),
            Name = Name,
            OfficeName = ServiceOffice,
            Organization = Organization,
            RegionName = RegionName,
            RenewReviewDate = Timestamp.WriteDateTimeOffset(RenewReviewDate, dateFormat),
            ReopenedDate = Timestamp.WriteDateTimeOffset(ReopenedDate, dateFormat),
            RestrictedFlag = RestrictedFlag.AsTruthyChar(),
            Status = Status,
            Type = EntitySubtype.GetDisplayString(),
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

    public static async Task SynchronizeCasesAsync(Realm realm, SectionJson<CaseJson> section, UserIgnoredContentPrefs userIgnoredPrefs)
    {
        var currentAssignedIds = realm.All<CaseRecord>().AsEnumerable().Select(@case => @case.Id);
        var unassignedIds = currentAssignedIds.Except(section.AssignedIds);

        string type = EntityType.Case.ToString();

        var v2Cases = FromApiJsonArray(section.Items ?? []);
        var v1Cases = realm
            .All<CaseloadItem>()
            .Where(@case => @case.EntityType == type);

        await RealmExtensions.CommitAsync(realm, () =>
        {
            CascadeDelete(realm, unassignedIds, userIgnoredPrefs);
            realm.Upsert(v2Cases);

            // TODO: Remove this foreach loop once we've fully removed V1 CaseloadItems
            foreach (var v1Case in v1Cases)
                v1Case.RowId = v2Cases.FirstOrDefault(v2Case => v2Case.FileNumber == v1Case.CaseIncidentNumber)?.Id;
        });
    }

    static void CascadeDelete(Realm realm, IEnumerable<string> unassignedIds, UserIgnoredContentPrefs userIgnoredPrefs)
    {
        foreach (var id in unassignedIds)
        {
            realm.Remove(realm.Find<CaseRecord>(id));

            // TODO: Remove notes here once we remove V1 CaseloadItem
            PersonVisit.RemoveByParent(realm, EntityType.Case, id);
            IcmContact.RemoveByParent(realm, EntityType.Case, id);
            SupportNetworkItem.RemoveByParent(realm, EntityType.Case, id);
            Attachment.RemoveByParent(realm, EntityType.Case, id, userIgnoredPrefs);
        }
    }

    public static IBusinessObject GetByDraftItem(Realm realm, IDraftItem draftItem)
    {
        return realm
            .All<CaseRecord>()
            .FirstOrDefault(@case => @case.Id == draftItem.RelatedEntityId
                        || @case.FileNumber == draftItem.RelatedEntityId);
    }
}
