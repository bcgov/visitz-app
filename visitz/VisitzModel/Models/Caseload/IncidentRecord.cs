using System.Globalization;
using Realms;
using VisitzApi.Models.Caseload;
using VisitzModel.Extensions;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Interfaces;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.CallDetails;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Interfaces;
using VisitzModel.Models.Notes;
using VisitzModel.Models.People;
using VisitzModel.Storage;
using VisitzModel.Utilities;

namespace VisitzModel.Models.Caseload;

public partial class IncidentRecord
    : IRealmObject,
        IRowMetadata,
        IBusinessObject,
        IAssignedMetadata,
        IApiJson<IncidentJson>
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

    public EntityType EntityType => EntityType.Incident;

    public string GivenNames { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string AssignedTo { get; set; } = string.Empty;

    public string AssignedToId { get; set; } = string.Empty;

    public IList<string> Assignees { get; } = null!;

    public string DisplayAssignees =>
        Assignees.Any()
            ? Assignees.Order().Aggregate((acc, assigned) => acc + Environment.NewLine + assigned)
            : AssignedTo;

    public string AddressComments { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string AreAnyOfTheFamilyMembersIndigenous { get; set; } = string.Empty;

    public string CallerAddress { get; set; } = string.Empty;

    public string CallerEmail { get; set; } = string.Empty;

    public string CallerName { get; set; } = string.Empty;

    public string CallerPhone { get; set; } = string.Empty;

    public string Caseload { get; set; } = string.Empty;

    public string CellPhone { get; set; } = string.Empty;

    public DateTimeOffset? ClosedDate { get; set; }

    public string CreatedByOffice { get; set; } = string.Empty;

    public DateTimeOffset? DateReported { get; set; }

    public string HomePhone { get; set; } = string.Empty;

    public string MedicalExamRequired { get; set; } = string.Empty;

    public string Method { get; set; } = string.Empty;

    public string NatureOfCall { get; set; } = string.Empty;

    public string PccSummary { get; set; } = string.Empty;

    public string PoliceForce { get; set; } = string.Empty;

    public string PoliceInvestigation { get; set; } = string.Empty;

    public DateTimeOffset? PoliceNotifiedDate { get; set; }

    public string PoliceReportNumber { get; set; } = string.Empty;

    public string PreferredContactMethod { get; set; } = string.Empty;

    public string ProtectionResponse { get; set; } = string.Empty;

    public string Resolution { get; set; } = string.Empty;

    public string ResponsePriority { get; set; } = string.Empty;

    public bool RestrictedFlag { get; set; }

    public string ServiceOffice { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    private int TypeInt { get; set; }
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

    public string TypeOfCaller { get; set; } = string.Empty;

    public BoLocalState? LocalState { get; set; }

    public string DisplayDate =>
        DateReported?.ToString(IBusinessObject.DisplayDateFormat, CultureInfo.InvariantCulture) ?? "";

    public IncidentRecord() { }

    public IncidentRecord(IncidentJson json, string? currentUsername = null)
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

        if (!string.IsNullOrWhiteSpace(AssignedTo) && !Assignees.Contains(AssignedTo))
            Assignees.Add(AssignedTo);

        if (!string.IsNullOrWhiteSpace(currentUsername) && !Assignees.Contains(currentUsername))
            Assignees.Add(currentUsername);

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
        EntitySubtype = json.Type?.ParseEntitySubtype() ?? EntitySubtype.Unknown;
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
            ClosedDate = ClosedDate?.ToString(dateFormat) ?? string.Empty,
            CreatedByOffice = CreatedByOffice,
            DateReported = DateReported?.ToString(dateFormat) ?? string.Empty,
            HomePhone = HomePhone,
            MedicalExamRequired = MedicalExamRequired,
            Method = Method,
            NatureOfCall = NatureOfCall,
            PccSummary = PccSummary,
            PoliceForce = PoliceForce,
            PoliceInvestigation = PoliceInvestigation,
            PoliceNotifiedDate = PoliceNotifiedDate?.ToString(dateFormat) ?? string.Empty,
            PoliceReportNumber = PoliceReportNumber,
            PreferredContactMethod = PreferredContactMethod,
            ProtectionResponse = ProtectionResponse,
            Resolution = Resolution,
            ResponsePriority = ResponsePriority,
            RestrictedFlag = RestrictedFlag.AsTruthyChar(),
            ServiceOffice = ServiceOffice,
            Status = Status,
            Type = EntitySubtype.GetDisplayString(),
            TypeOfCaller = TypeOfCaller,
        };
    }

    public static List<IncidentRecord> FromApiJsonArray(
        IEnumerable<IncidentJson> jsonArray,
        string? currentUsername = null
    )
    {
        List<IncidentRecord> outList = [];

        if (jsonArray != null)
            foreach (var jsonItem in jsonArray)
                outList.Add(new IncidentRecord(jsonItem, currentUsername));

        return outList;
    }

    public static IEnumerable<TItem> FilterUnsupportedSubtypes<TItem>(IEnumerable<TItem> businessObjects)
        where TItem : IBusinessObject
    {
        return businessObjects.Where(incident =>
            incident is IncidentRecord && incident.EntitySubtype == EntitySubtype.ChildProtection
        );
    }

    public void DeleteDependentData(
        UserIgnoredContentPrefs userIgnoredPrefs,
        Realm? fromRealm = null,
        bool deleteLocalState = true
    )
    {
        fromRealm ??= Realm;
        ArgumentNullException.ThrowIfNull(fromRealm);

        NoteItem.RemoveByParentFileNumber(fromRealm, EntityType.Incident, FileNumber);
        IcmContact.RemoveByParent(fromRealm, EntityType.Incident, Id);
        SupportNetworkItem.RemoveByParent(fromRealm, EntityType.Incident, Id);
        Attachment.RemoveByParent(fromRealm, EntityType.Incident, Id, userIgnoredPrefs);
        AdditionalInformation.RemoveByParent(fromRealm, EntityType.Incident, Id);
        IncidentConcerns.RemoveByParent(fromRealm, Id);
        CallInformation.RemoveByParent(fromRealm, EntityType.Incident, Id);

        if (deleteLocalState && LocalState != null)
            fromRealm.Remove(LocalState);
    }

    public static IBusinessObject? GetByDraftItem(Realm realm, IDraftItem draftItem)
    {
        return realm
            .All<IncidentRecord>()
            .FirstOrDefault(incident =>
                incident.Id == draftItem.RelatedEntityId || incident.FileNumber == draftItem.RelatedEntityId
            );
    }

    public static IQueryable<IncidentRecord> GetAllByAssignee(Realm realm, string username, bool isAssignedTo = true)
    {
        return GetAllByAssignee<IncidentRecord>(realm, username, isAssignedTo);
    }

    public static IQueryable<TItem> GetAllByAssignee<TItem>(Realm realm, string username, bool isAssignedTo = true)
        where TItem : IBusinessObject
    {
        string operation = isAssignedTo ? "ANY" : "NONE";

        return (IQueryable<TItem>)
            realm.All<IncidentRecord>().Filter($"$0 == {operation} {nameof(Assignees)}", username);
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

    public override string ToString()
    {
        return ((IBusinessObject)this).MakeToString();
    }
}
