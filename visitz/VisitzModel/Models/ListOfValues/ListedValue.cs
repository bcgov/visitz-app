using Realms;
using VisitzApi.Models.ListOfValues;
using VisitzModel.Extensions;

namespace VisitzModel.Models.ListOfValues;

public partial class ListedValue : IRealmObject, IComparable<ListedValue>
{
    [PrimaryKey]
    public string CompositePk { get; set; } = Guid.NewGuid().ToString();

    public bool Active { get; set; }

    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int? OrderBy { get; set; }

    public string Parent { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public ListedValue() { }

    public ListedValue(ListedValueJson json)
    {
        CompositePk = MakeCompositePk(json.Type, json.Id);
        Active = json.Active.ParseWordTruthiness();
        Id = json.Id;
        Name = json.Name;
        OrderBy = int.TryParse(json.OrderBy, out int result) ? result : null;
        Parent = json.Parent;
        Type = json.Type;
        Value = json.Value;
    }

    static string MakeCompositePk(string type, string id) => $"{type}|{id}";

    public static IQueryable<ListedValue> GetAllByType(Realm realm, string type)
    {
        return realm.All<ListedValue>().Where(lv => lv.Type == type);
    }

    public static async Task SynchronizeAsync(Realm realm, string type, IEnumerable<ListedValue> valueList)
    {
        await realm.SynchronizeByQueryAsync(incomingItems: valueList, existingQuery: GetAllByType(realm, type));
    }

    public override string ToString()
    {
        return $"{CompositePk} | {Name} | {Value}";
    }

    public int CompareTo(ListedValue? other)
    {
        if (other == null)
            return 1;

        int orderByComparison = OrderBy?.CompareTo(other.OrderBy) ?? 0;
        if (orderByComparison != 0)
            return orderByComparison;

        return string.Compare(Name, other.Name, StringComparison.Ordinal);
    }
}
