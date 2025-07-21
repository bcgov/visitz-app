using Realms;
using System.Globalization;
using VisitzApi.Models.Caseload;
using VisitzModel.Extensions;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Interfaces;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Interfaces;
using VisitzModel.Models.Notes;
using VisitzModel.Models.People;
using VisitzModel.Storage;
using VisitzModel.Utilities;

namespace VisitzModel.Models.Caseload;

public partial class ServiceRequestRecord :
    IRealmObject,
    IRowMetadata,
    IBusinessObject,
    IAssignedMetadata,
    IApiJson<ServiceRequestJson>
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

    public EntityType EntityType => EntityType.ServiceRequest;

    public string GivenNames { get; set; }

    public string LastName { get; set; }

    public string AssignedTo { get; set; }

    public string AssignedToId { get; set; }

    public string DisplayAssignees => AssignedTo;

    public string Address { get; set; }

    public string AddressComments { get; set; }

    public string AreAnyOfTheFamilyMembersIndigenous { get; set; }

    public DateTimeOffset? CallDate { get; set; }

    public string CallerAddress { get; set; }

    public string CallerEmail { get; set; }

    public string CallerName { get; set; }

    public string CallerPhone { get; set; }

    public string CellPhone { get; set; }

    public DateTimeOffset? ClosedDate { get; set; }

    public string CreatedByOffice { get; set; }

    public string HomePhone { get; set; }

    public string IntegrationId { get; set; }

    public string Method { get; set; }

    public string NatureOfCall { get; set; }

    public string PccSummary { get; set; }

    public string PreferredContactMethod { get; set; }

    public string Priority { get; set; }

    public string Resolution { get; set; }

    public bool RestrictedFlag { get; set; }

    public string RowId { get; set; }

    public string ServiceOffice { get; set; }

    public string Status { get; set; }

    private int TypeInt { get; set; }
    public EntitySubtype EntitySubtype
    {
        get => (EntitySubtype)TypeInt;
        set => TypeInt = (int)value;
    }

    public string TypeOfCaller { get; set; }

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

    public string DisplayName => ServiceOffice;

    public string FullType => this.GetFullType();

    public IQueryable<IcmContact> Contacts => this.GetContacts();

    public ServiceRequestRecord() { }

    public ServiceRequestRecord(
        ServiceRequestJson json,
        BoLocalState localState = null)
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
        BoLocalState = localState;
        BoLocalState?.SetBusinessObject(this);
    }

    public static List<ServiceRequestRecord> FromApiArray(IEnumerable<ServiceRequestJson> jsonArray)
    {
        List<ServiceRequestRecord> outList = [];

        foreach (var jsonItem in jsonArray)
            outList.Add(new ServiceRequestRecord(jsonItem));

        return outList;
    }

    public static async Task SynchronizeAsync(Realm realm, SectionJson<ServiceRequestJson> section, UserIgnoredContentPrefs userIgnoredPrefs)
    {
        var currentAssignedIds = realm.All<ServiceRequestRecord>().AsEnumerable().Select(sr => sr.Id);
        var unassignedIds = currentAssignedIds.Except(section.AssignedIds);
        var serviceRequests = FromApiArray(section.Items ?? []);

        await RealmExtensions.CommitAsync(realm, () =>
        {
            CascadeDelete(realm, unassignedIds, userIgnoredPrefs);
            realm.Upsert(serviceRequests);
        });
    }

    static void CascadeDelete(Realm realm, IEnumerable<string> unassignedIds, UserIgnoredContentPrefs userIgnoredPrefs)
    {
        foreach (var id in unassignedIds)
        {
            var sr = realm.Find<ServiceRequestRecord>(id);

            NoteItem.RemoveByParentFileNumber(realm, EntityType.ServiceRequest, sr.FileNumber);
            IcmContact.RemoveByParent(realm, EntityType.ServiceRequest, id);
            SupportNetworkItem.RemoveByParent(realm, EntityType.ServiceRequest, id);
            Attachment.RemoveByParent(realm, EntityType.ServiceRequest, id, userIgnoredPrefs);

            realm.Remove(sr);
        }
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
            CallDate = CallDate?.ToString(dateFormat),
            CallerAddress = CallerAddress,
            CallerEmail = CallerEmail,
            CallerName = CallerName,
            CallerPhone = CallerPhone,
            CellPhone = CellPhone,
            ClosedDate = ClosedDate?.ToString(dateFormat),
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

    public static IBusinessObject GetByDraftItem(Realm realm, IDraftItem draftItem)
    {
        return realm
            .All<ServiceRequestRecord>()
            .FirstOrDefault(sr => sr.Id == draftItem.RelatedEntityId
                        || sr.FileNumber == draftItem.RelatedEntityId);
    }

    public static IQueryable<ServiceRequestRecord> GetAllByAssignee(
        Realm realm,
        string username,
        bool invert = false)
    {
        string operation = invert ? "!=" : "==";

        return realm
            .All<ServiceRequestRecord>()
            .Filter($"$0 {operation} {nameof(AssignedTo)}", username);
    }

    public bool IsAssigned(string username)
    {
        return AssignedTo == username;
    }
}
