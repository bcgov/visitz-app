using Realms;
using VisitzApi.Models;
using VisitzApi.Models.People;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Interfaces;

namespace VisitzModel.Models.People;

public partial class SupportNetworkItem : IRealmObject, IRowMetadata, IParentRecord, IApiJson<SubmitSupportNetworkJson>
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string CreatedBy { get; set; } = string.Empty;

    public string CreatedById { get; set; } = string.Empty;

    public string UpdatedBy { get; set; } = string.Empty;

    public string UpdatedById { get; set; } = string.Empty;

    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.UtcNow;

    public string ParentId { get; set; } = string.Empty;

    private int ParentTypeInt { get; set; }

    public EntityType ParentType
    {
        get => (EntityType)ParentTypeInt;
        set => ParentTypeInt = (int)value;
    }

    public string Active { get; set; } = string.Empty;

    public bool IsActive => ActiveBinding.ParseWordTruthiness();

    public string Address { get; set; } = string.Empty;

    public string AgencyName { get; set; } = string.Empty;

    public string ParentRecordId { get; set; } = string.Empty;

    public string CellPhoneNumber { get; set; } = string.Empty;

    public string Comments { get; set; } = string.Empty;

    public string EmergencyContact { get; set; } = string.Empty;

    public string EntityId { get; set; } = string.Empty;

    public string EntityName { get; set; } = string.Empty;

    public string IcmSncCaseConFlag { get; set; } = string.Empty;

    public string IcmSncSrConFlag { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Relationship { get; set; } = string.Empty;

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
        AgencyName = json.Agency;
        CellPhoneNumber = json.Cell;
        Comments = json.Comments;
        EntityId = json.EntityId;
        EntityName = json.EntityName;
        Name = json.Name;
        PhoneNumber = json.Phone;
        Relationship = json.Relationship;
    }

    public SubmitSupportNetworkJson ToApiJson(string dateFormat = "s")
    {
        return new SubmitSupportNetworkJson()
        {
            Active = Active,
            Address = Address,
            AgencyName = AgencyName,
            Cell = CellPhoneNumber,
            Comments = Comments,
            Name = Name,
            Phone = PhoneNumber,
            Relationship = Relationship,
        };
    }

    public static IEnumerable<SupportNetworkItem> FromApiArray(
        IEnumerable<SupportNetworkJson> items,
        string parentId,
        EntityType type
    )
    {
        List<SupportNetworkItem> outList = [];

        foreach (var supportNetworkJson in items)
            outList.Add(new SupportNetworkItem(supportNetworkJson, parentId, type));

        return outList;
    }

    public static async Task SynchronizeAsync(
        Realm realm,
        IEnumerable<SupportNetworkJson> items,
        string parentId,
        EntityType type
    )
    {
        var incomingSupportNetworkItems = FromApiArray(items, parentId, type);
        var incomingSupportNetworkItemIds = incomingSupportNetworkItems.Select(item => item.Id);
        var supportNetworks = realm
            .All<SupportNetworkItem>()
            .Where(item => item.ParentId == parentId && item.ParentTypeInt == (int)type)
            .ToList();
        var supportNetworkIds = supportNetworks.Select(item => item.Id);

        var networkItemIdsToDelete = supportNetworkIds.Except(incomingSupportNetworkItemIds);
        var networkItemsToDelete = supportNetworks.Where(item => networkItemIdsToDelete.Contains(item.Id));

        await RealmExtensions.CommitAsync(
            realm,
            () =>
            {
                foreach (var item in supportNetworks)
                {
                    if (item != null && item.IsValid)
                        realm.Remove(item);
                }
                realm.Upsert(incomingSupportNetworkItems);
            }
        );
    }

    public static IQueryable<SupportNetworkItem> GetSupportNetworkByIdType(Realm realm, string id, EntityType type)
    {
        return realm
            .All<SupportNetworkItem>()
            .Where(item => item.EntityId == id && item.ParentTypeInt == (int)type)
            .Filter($"TRUEPREDICATE SORT({nameof(Active)} DESC, {nameof(Name)} ASC)");
    }

    public static void RemoveByParent(Realm realm, EntityType type, string parentId)
    {
        var networkItems = realm
            .All<SupportNetworkItem>()
            .Where(item => item.ParentId == parentId && item.ParentTypeInt == (int)type);

        realm.RemoveRange(networkItems);
    }
}
