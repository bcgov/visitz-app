using Realms;
using VisitzApi.Models;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Interfaces;

namespace VisitzModel.Models.People;

public partial class SupportNetworkItem : IRealmObject, IRowMetadata, IApiJson<SupportNetworkJson>, IParentRecord
{
    [PrimaryKey]
    public string Id { get; set; }

    public string CreatedBy { get; set; }

    public string CreatedById { get; set; }

    public string UpdatedBy { get; set; }

    public string UpdatedById { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset UpdatedDate { get; set; }

    public string ParentId { get; set; }

    private int ParentTypeInt { get; set; }

    public EntityType ParentType
    {
        get => (EntityType)ParentTypeInt;
        set => ParentTypeInt = (int)value;
    }

    public string Active { get; set; }

    public string Address { get; set; }

    public string AgencyName { get; set; }

    public string ParentRecordId { get; set; }

    public string CellPhoneNumber { get; set; }

    public string Comments { get; set; }

    public string EmergencyContact { get; set; }

    public string EntityId { get; set; }

    public string EntityName { get; set; }

    public string IcmSncCaseConFlag { get; set; }

    public string IcmSncSrConFlag { get; set; }

    public string Name { get; set; }

    public string PhoneNumber { get; set; }

    public string Relationship { get; set; }

    public SupportNetworkItem() { }

    public SupportNetworkItem(SupportNetworkJson json, string parentId, EntityType type)
    {
        Id = json.Id;
        CreatedBy = json.CreatedBy;
        CreatedById = json.CreatedById;
        UpdatedBy = json.UpdatedBy;
        UpdatedById = json.UpdatedById;
        CreatedDate = DateTimeOffset.Parse(json.CreatedDate);
        UpdatedDate = DateTimeOffset.Parse(json.UpdatedDate);
        ParentId = parentId;
        ParentType = type;
        Active = json.Active;
        Address = json.Address;
        AgencyName = json.AgencyName;
        CellPhoneNumber = json.CellPhoneNumber;
        Comments = json.Comments;
        EmergencyContact = json.EmergencyContact;
        EntityId = json.EntityId;
        EntityName = json.EntityName;
        IcmSncCaseConFlag = json.ICMSNCCaseConFlag;
        IcmSncSrConFlag = json.ICMSNCSRConFlag;
        Name = json.Name;
        PhoneNumber = json.PhoneNumber;
        Relationship = json.Relationship;
    }

    public SupportNetworkJson ToApiJson(string dateFormat = "s")
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
            Active = Active,
            Address = Address,
            AgencyName = AgencyName,
            CellPhoneNumber = CellPhoneNumber,
            Comments = Comments,
            EmergencyContact = EmergencyContact,
            EntityId = EntityId,
            EntityName = EntityName,
            ICMSNCCaseConFlag = IcmSncCaseConFlag,
            ICMSNCSRConFlag = IcmSncSrConFlag,
            Name = Name,
            PhoneNumber = PhoneNumber,
            Relationship = Relationship,
        };
    }

    public static IEnumerable<SupportNetworkItem> FromApiArray(
        IEnumerable<SupportNetworkJson> items,
        string parentId,
        EntityType type)
    {
        List<SupportNetworkItem> outList = [];

        foreach (var supportNetworkJson in items)
            outList.Add(new SupportNetworkItem(supportNetworkJson, parentId, type));

        return outList;
    }

    public static async Task SaveSupportNetworkItemsAsync(
        Realm realm,
        IEnumerable<SupportNetworkJson> items,
        string parentId,
        EntityType type)
    {
        await RealmExtensions.CommitAsync(realm, () => realm.Upsert(FromApiArray(items, parentId, type)));
    }

    public static IQueryable<SupportNetworkItem> GetSupportNetworkByCaseId(Realm realm, string caseId)
    {
        return realm.All<SupportNetworkItem>()
            .Where(item => item.EntityId == caseId)
            .Filter($"TRUEPREDICATE SORT({nameof(Name)} DESC)");
    }
}
