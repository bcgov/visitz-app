using Realms;
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

public partial class CaseRecord : IRealmObject, IRowMetadata, IBusinessObject, IAssignedMetadata, IApiJson<CaseJson>
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string CreatedBy { get; set; } = string.Empty;

    public string CreatedById { get; set; } = string.Empty;

    public string UpdatedBy { get; set; } = string.Empty;

    public string UpdatedById { get; set; } = string.Empty;

    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.UtcNow;

    public string FileNumber { get; set; } = string.Empty;

    public EntityType EntityType => EntityType.Case;

    public string GivenNames { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string AssignedTo { get; set; } = string.Empty;

    public string AssignedToId { get; set; } = string.Empty;

    public IList<string> Assignees { get; } = null!;

    public string DisplayAssignees
    {
        get
        {
            if (!IsValid)
                return string.Empty;

            return Assignees.Any()
                ? Assignees.Order().Aggregate((acc, assigned) => acc + Environment.NewLine + assigned).Trim()
                : AssignedTo;
        }
    }

    public string Caseload { get; set; } = string.Empty;

    public DateTimeOffset? ClosedDate { get; set; }

    public string CloseReason { get; set; } = string.Empty;

    public string EarlyOpenReason { get; set; } = string.Empty;

    public string IntegrationState { get; set; } = string.Empty;

    public string LegacyFileNumber { get; set; } = string.Empty;

    public string MiddleName { get; set; } = string.Empty;

    public bool MyFSFlag { get; set; }

    public string Name { get; set; } = string.Empty;

    public string ServiceOffice { get; set; } = string.Empty;

    public string Organization { get; set; } = string.Empty;

    public string RegionName { get; set; } = string.Empty;

    public DateTimeOffset? RenewReviewDate { get; set; }

    public DateTimeOffset? ReopenedDate { get; set; }

    public bool RestrictedFlag { get; set; }

    public string Status { get; set; } = string.Empty;

    int TypeInt { get; set; }
    public EntitySubtype EntitySubtype
    {
        get => (EntitySubtype)TypeInt;
        set => TypeInt = (int)value;
    }

    public EntitySubtype EntitySubtypeBinding
    {
        get => IsValid ? EntitySubtype : EntitySubtype.Unknown;
        set
        {
            if (!IsValid)
                return;

            EntitySubtype = value;
            RaisePropertyChanged(nameof(EntitySubtype));
        }
    }

    public string EntitySubtypeInitials => EntitySubtype.GetDisplayInitials();

    public string WorkQueue { get; set; } = string.Empty;

    public BoLocalState? LocalState { get; set; }

    public CaseRecord() { }

    public CaseRecord(CaseJson caseJson, string? currentUsername = null)
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

        if (Assignees != null)
        {
            if (caseJson.Position?.Count > 0)
                foreach (var position in caseJson.Position)
                    Assignees.Add(position.SalesRep);

            if (!string.IsNullOrWhiteSpace(AssignedTo) && !Assignees.Contains(AssignedTo))
                Assignees.Add(AssignedTo);

            if (!string.IsNullOrWhiteSpace(currentUsername) && !Assignees.Contains(currentUsername))
                Assignees.Add(currentUsername);
        }

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

    public static List<CaseRecord> FromApiJsonArray(IEnumerable<CaseJson> jsonArray, string? currentUsername = null)
    {
        List<CaseRecord> outList = [];

        if (jsonArray != null)
            foreach (var jsonItem in jsonArray)
                outList.Add(new CaseRecord(jsonItem, currentUsername));

        return outList;
    }

    public static IEnumerable<TItem> FilterUnsupportedSubtypes<TItem>(IEnumerable<TItem> businessObjects)
        where TItem : IBusinessObject
    {
        return businessObjects.Where(item =>
            item is CaseRecord
            && (
                item.EntitySubtype == EntitySubtype.ChildServices
                || item.EntitySubtype == EntitySubtype.FamilyServices
                || item.EntitySubtype == EntitySubtype.CysnFamilyServices
                || item.EntitySubtype == EntitySubtype.Resource
            )
        );
    }

    public void DeleteDependentData(
        UserIgnoredContentPrefs userIgnoredPrefs,
        Realm? fromRealm = null,
        bool deleteLocalState = true
    )
    {
        fromRealm ??= Realm ?? throw new InvalidOperationException("Managed realm is null");

        NoteItem.RemoveByParentFileNumber(fromRealm, EntityType.Case, FileNumber);
        PersonVisit.RemoveByParent(fromRealm, EntityType.Case, Id);
        IcmContact.RemoveByParent(fromRealm, EntityType.Case, Id);
        SupportNetworkItem.RemoveByParent(fromRealm, EntityType.Case, Id);
        Attachment.RemoveByParent(fromRealm, EntityType.Case, Id, userIgnoredPrefs);

        if (deleteLocalState && LocalState != null)
            fromRealm.Remove(LocalState);
    }

    public static IBusinessObject? GetByDraftItem(Realm realm, IDraftItem draftItem)
    {
        return realm
            .All<CaseRecord>()
            .FirstOrDefault(@case =>
                @case.Id == draftItem.RelatedEntityId || @case.FileNumber == draftItem.RelatedEntityId
            );
    }

    public static IBusinessObject? GetByPersonVisitItem(Realm realm, PersonVisit item)
    {
        return realm
            .All<CaseRecord>()
            .FirstOrDefault(@case => @case.Id == item.ParentId || @case.FileNumber == item.ParentId);
    }

    public static IQueryable<CaseRecord> GetAllByAssignee(Realm realm, string username, bool isAssignedTo = true)
    {
        return GetAllByAssignee<CaseRecord>(realm, username, isAssignedTo);
    }

    public static IQueryable<TItem> GetAllByAssignee<TItem>(Realm realm, string username, bool isAssignedTo = true)
        where TItem : IBusinessObject
    {
        string operation = isAssignedTo ? "ANY" : "NONE";

        return (IQueryable<TItem>)realm.All<CaseRecord>().Filter($"$0 == {operation} {nameof(Assignees)}", username);
    }

    public bool IsAssigned(string username)
    {
        return AssignedTo == username || Assignees.Contains(username);
    }

    public override bool Equals(object? obj)
    {
        return obj is IBusinessObject info ? ((IBusinessObject)this).Equals(info) : base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return ((IBusinessObject)this).MakeHashCode();
    }

    public void RaisePropertyChangedEvent(string propertyName)
    {
        RaisePropertyChanged(propertyName);
    }
}
