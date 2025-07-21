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
using VisitzModel.Models.Notes;
using VisitzModel.Models.People;
using VisitzModel.Storage;
using VisitzModel.Utilities;

namespace VisitzModel.Models.Caseload;

public partial class CaseRecord :
    IRealmObject,
    IRowMetadata,
    IBusinessObject,
    IAssignedMetadata,
    IApiJson<CaseJson>,
    IEquatable<CaseRecord>
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

    public IList<string> Assignees { get; }

    public string DisplayAssignees => Assignees.Any()
        ? Assignees.Order().Aggregate((acc, assigned) => acc + Environment.NewLine + assigned).Trim()
        : AssignedTo;

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

    private BoLocalState BoLocalState { get; set; }
    public BoLocalState LocalState
    {
        get
        {
            if (BoLocalState == null)
                this.Commit(() => BoLocalState = this.FindOrMakeLocalState());

            return BoLocalState;
        }
    }

    public string DisplayDate => CreatedDate.ToString(
        IBusinessObject.DisplayDateFormat,
        CultureInfo.InvariantCulture);

    public string DisplayName => this.GetDisplayName();

    public string FullType => this.GetFullType();

    public IQueryable<IcmContact> Contacts => this.GetContacts();

    public CaseRecord() { }

    public CaseRecord(
        CaseJson caseJson,
        BoLocalState localState = null,
        string currentUsername = null)
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

        if (caseJson.Position?.Count > 0)
            foreach (var position in caseJson.Position)
                Assignees.Add(position.SalesRep);

        if (!string.IsNullOrWhiteSpace(AssignedTo)
            && !Assignees.Contains(AssignedTo))
            Assignees.Add(AssignedTo);

        if (!string.IsNullOrWhiteSpace(currentUsername)
            && !Assignees.Contains(currentUsername))
            Assignees.Add(currentUsername);

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
        BoLocalState = localState;
        BoLocalState?.SetBusinessObject(this);
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

    public static List<CaseRecord> FromApiJsonArray(
        IEnumerable<CaseJson> jsonArray,
        BoLocalState localState,
        string currentUsername = null)
    {
        List<CaseRecord> outList = [];

        if (jsonArray != null)
            foreach (var jsonItem in jsonArray)
                outList.Add(new CaseRecord(jsonItem, localState, currentUsername));

        return outList;
    }

    static IEnumerable<CaseRecord> FilterUnsupportedSubtypes(IEnumerable<CaseRecord> cases)
    {
        return cases.Where(@case => @case.EntitySubtype == EntitySubtype.ChildServices
                        || @case.EntitySubtype == EntitySubtype.FamilyServices);
    }

    public static async Task SynchronizeAsync(
        Realm realm,
        IEnumerable<CaseRecord> newOfficeCases,
        UserIgnoredContentPrefs userIgnoredPrefs,
        string currentUsername,
        bool isPersonalCaseload)
    {
        if (newOfficeCases == null)
            return;

        bool isOfficeCaseload = !isPersonalCaseload;
        var incomingCases = FilterUnsupportedSubtypes(newOfficeCases);
        var currentAssigned = GetAllByAssignee(realm, currentUsername, isOfficeCaseload).ToList();
        var unassigned = currentAssigned.Except(incomingCases);

        await RealmExtensions.CommitAsync(realm, () =>
        {
            CascadeDelete(realm, unassigned, userIgnoredPrefs);
            realm.Upsert(incomingCases);
        });
    }
    
    public static Task SynchronizeAsync(
        Realm realm,
        IEnumerable<CaseJson> newOfficeCases,
        UserIgnoredContentPrefs userIgnoredPrefs,
        string currentUsername,
        bool isPersonalCaseload,
        BoLocalState localState)
    {
        return SynchronizeAsync(
            realm,
            FromApiJsonArray(newOfficeCases, localState, currentUsername),
            userIgnoredPrefs,
            currentUsername,
            isPersonalCaseload);
    }

    static void CascadeDelete(
        Realm realm,
        IEnumerable<CaseRecord> unassigned,
        UserIgnoredContentPrefs userIgnoredPrefs)
    {
        foreach (var @case in unassigned)
        {
            NoteItem.RemoveByParentFileNumber(realm, EntityType.Case, @case.FileNumber);
            PersonVisit.RemoveByParent(realm, EntityType.Case, @case.Id);
            IcmContact.RemoveByParent(realm, EntityType.Case, @case.Id);
            SupportNetworkItem.RemoveByParent(realm, EntityType.Case, @case.Id);
            Attachment.RemoveByParent(realm, EntityType.Case, @case.Id, userIgnoredPrefs);

            realm.Remove(@case.BoLocalState);
            realm.Remove(@case);
        }
    }

    public static IBusinessObject GetByDraftItem(Realm realm, IDraftItem draftItem)
    {
        return realm
            .All<CaseRecord>()
            .FirstOrDefault(@case => @case.Id == draftItem.RelatedEntityId
                        || @case.FileNumber == draftItem.RelatedEntityId);
    }

    public static IBusinessObject GetByPersonVisitItem(Realm realm, PersonVisit item)
    {
        return realm
            .All<CaseRecord>()
            .FirstOrDefault(@case => @case.Id == item.ParentId
                        || @case.FileNumber == item.ParentId);
    }

    public static IQueryable<CaseRecord> GetAllByAssignee(
        Realm realm,
        string username,
        bool invert = false)
    {
        string operation = invert ? "NONE" : "ANY";

        return realm
            .All<CaseRecord>()
            .Filter($"$0 == {operation} {nameof(Assignees)}", username);
    }

    public bool IsAssigned(string username)
    {
        return AssignedTo == username || Assignees.Contains(username);
    }

    public bool Equals(CaseRecord other)
    {
        return other != null
            && Id == other.Id
            && EntityType == other.EntityType;
    }

    public override bool Equals(object obj)
    {
        return obj is CaseRecord info ? Equals(info) : base.Equals(obj);
    }

    public override int GetHashCode()
    {
#pragma warning disable SS008 // GetHashCode() refers to mutable or static member
        return EntityType.GetHashCode() * Id.GetHashCode();
#pragma warning restore SS008 // GetHashCode() refers to mutable or static member
    }
}
