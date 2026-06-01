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

public partial class ServiceRequestRecord
    : IRealmObject,
        IRowMetadata,
        IBusinessObject,
        IAssignedMetadata,
        IApiJson<ServiceRequestJson>
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

    public EntityType EntityType => EntityType.ServiceRequest;

    public string GivenNames { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string AssignedTo { get; set; } = string.Empty;

    public string AssignedToId { get; set; } = string.Empty;

    public IList<string> Assignees { get; } = null!;

    public string DisplayAssignees => AssignedTo;

    public string Address { get; set; } = string.Empty;

    public string AddressComments { get; set; } = string.Empty;

    public string AreAnyOfTheFamilyMembersIndigenous { get; set; } = string.Empty;

    public DateTimeOffset? CallDate { get; set; }

    public string CallerAddress { get; set; } = string.Empty;

    public string CallerEmail { get; set; } = string.Empty;

    public string CallerName { get; set; } = string.Empty;

    public string CallerPhone { get; set; } = string.Empty;

    public string CellPhone { get; set; } = string.Empty;

    public DateTimeOffset? ClosedDate { get; set; }

    public string CreatedByOffice { get; set; } = string.Empty;

    public string HomePhone { get; set; } = string.Empty;

    public string IntegrationId { get; set; } = string.Empty;

    public string Method { get; set; } = string.Empty;

    public string NatureOfCall { get; set; } = string.Empty;

    public string PccSummary { get; set; } = string.Empty;

    public string PreferredContactMethod { get; set; } = string.Empty;

    public string Priority { get; set; } = string.Empty;

    public string Resolution { get; set; } = string.Empty;

    public bool RestrictedFlag { get; set; }

    public string RowId { get; set; } = string.Empty;

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

    public string DisplayDate => CreatedDate.ToString(IBusinessObject.DisplayDateFormat, CultureInfo.InvariantCulture);

    public string DisplayName => ServiceOffice;

    public ServiceRequestRecord() { }

    public ServiceRequestRecord(ServiceRequestJson json, BoLocalState? localState = null)
    {
        Id = json.Id;
        CreatedBy = json.CreatedBy;
        CreatedById = json.CreatedById;
        UpdatedBy = json.UpdatedBy;
        UpdatedById = json.UpdatedById;
        CreatedDate = DateTimeOffset.Parse(json.CreatedDate);
        UpdatedDate = DateTimeOffset.Parse(json.UpdatedDate);
        FileNumber = json.ServiceRequestNumber;
        GivenNames = json.GivenNames;
        LastName = json.LastName;
        AssignedTo = json.AssignedTo;
        AssignedToId = json.AssignedToId;

        if (Assignees != null && !string.IsNullOrWhiteSpace(AssignedTo) && !Assignees.Contains(AssignedTo))
            Assignees.Add(AssignedTo);

        Address = json.Address;
        AddressComments = json.AddressComments;
        AreAnyOfTheFamilyMembersIndigenous = json.AreAnyOfTheFamilyMembersIndigenous;
        CallDate = Timestamp.ParseDateTimeOffsetNullable(json.CallDate);
        CallerAddress = json.CallerAddress;
        CallerEmail = json.CallerEmail;
        CallerName = json.CallerName;
        CallerPhone = json.CallerPhone;
        CellPhone = json.CellPhone;
        ClosedDate = Timestamp.ParseDateTimeOffsetNullable(json.ClosedDate);
        CreatedByOffice = json.CreatedByOffice;
        HomePhone = json.HomePhone;
        IntegrationId = json.IntegrationId;
        Method = json.Method;
        NatureOfCall = json.NatureOfCall;
        PccSummary = json.PccSummary;
        PreferredContactMethod = json.PreferredContactMethod;
        Priority = json.Priority;
        Resolution = json.Resolution;
        RestrictedFlag = json.RestrictedFlag.ParseWordTruthiness();
        RowId = json.RowId;
        ServiceOffice = json.ServiceOffice;
        Status = json.Status;
        EntitySubtype = json.Type?.ParseEntitySubtype() ?? EntitySubtype.Unknown;
        TypeOfCaller = json.TypeOfCaller;
    }

    public static List<ServiceRequestRecord> FromApiArray(IEnumerable<ServiceRequestJson> jsonArray)
    {
        List<ServiceRequestRecord> outList = [];

        foreach (var jsonItem in jsonArray)
            outList.Add(new ServiceRequestRecord(jsonItem));

        return outList;
    }

    public void DeleteDependentData(
        UserIgnoredContentPrefs userIgnoredPrefs,
        Realm? fromRealm = null,
        bool deleteLocalState = true
    )
    {
        fromRealm ??= Realm;
        ArgumentNullException.ThrowIfNull(fromRealm);

        NoteItem.RemoveByParentFileNumber(fromRealm, EntityType.ServiceRequest, FileNumber);
        IcmContact.RemoveByParent(fromRealm, EntityType.ServiceRequest, Id);
        SupportNetworkItem.RemoveByParent(fromRealm, EntityType.ServiceRequest, Id);
        Attachment.RemoveByParent(fromRealm, EntityType.ServiceRequest, Id, userIgnoredPrefs);
        CallInformation.RemoveByParent(fromRealm, EntityType.ServiceRequest, Id);
        AdditionalInformation.RemoveByParent(fromRealm, EntityType.ServiceRequest, Id);

        if (deleteLocalState && LocalState != null)
            fromRealm.Remove(LocalState);
    }

    public ServiceRequestJson ToApiJson(string dateFormat = "s")
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
            ServiceRequestNumber = FileNumber,
            GivenNames = GivenNames,
            LastName = LastName,
            AssignedTo = AssignedTo,
            AssignedToId = AssignedToId,
            Address = Address,
            AddressComments = AddressComments,
            AreAnyOfTheFamilyMembersIndigenous = AreAnyOfTheFamilyMembersIndigenous,
            CallDate = CallDate?.ToString(dateFormat) ?? string.Empty,
            CallerAddress = CallerAddress,
            CallerEmail = CallerEmail,
            CallerName = CallerName,
            CallerPhone = CallerPhone,
            CellPhone = CellPhone,
            ClosedDate = ClosedDate?.ToString(dateFormat) ?? string.Empty,
            CreatedByOffice = CreatedByOffice,
            HomePhone = HomePhone,
            IntegrationId = IntegrationId,
            Method = Method,
            NatureOfCall = NatureOfCall,
            PccSummary = PccSummary,
            PreferredContactMethod = PreferredContactMethod,
            Priority = Priority,
            Resolution = Resolution,
            RestrictedFlag = RestrictedFlag.AsTruthyChar(),
            RowId = RowId,
            ServiceOffice = ServiceOffice,
            Status = Status,
            Type = EntitySubtype.GetDisplayString(),
            TypeOfCaller = TypeOfCaller,
        };
    }

    public static IBusinessObject? GetByDraftItem(Realm realm, IDraftItem draftItem)
    {
        return realm
            .All<ServiceRequestRecord>()
            .FirstOrDefault(sr => sr.Id == draftItem.RelatedEntityId || sr.FileNumber == draftItem.RelatedEntityId);
    }

    public void RaisePropertyChangedEvent(string propertyName)
    {
        RaisePropertyChanged(propertyName);
    }

    public override bool Equals(object? obj)
    {
        return obj is IBusinessObject info ? ((IBusinessObject)this).Equals(info) : base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return ((IBusinessObject)this).MakeHashCode();
    }

    public override string ToString()
    {
        return ((IBusinessObject)this).MakeToString();
    }
}
