using Realms;
using VisitzApi.Models.Visits;
using VisitzModel.Extensions;

namespace VisitzModel.Models;

public partial class PersonVisit : IRealmObject, IApiJson<VisitJson>
{
    static readonly string _defaultType = "In Person Child Youth";

    [PrimaryKey]
    public string Id { get; set; }

    public string ParentId { get; set; }

    public string Name { get; set; }

    public string VisitDescription { get; set; }

    public string Type { get; set; } = _defaultType;

    public DateTimeOffset DateOfVisit { get; set; }

    public string VisitDetailsValue { get; set; }

    public string VisitDetailsGroup { get; set; }

    public string LoginName { get; set; }

    public DateTimeOffset Created { get; set; }

    public DateTimeOffset Updated { get; set; }

    public string CreatedBy { get; set; }

    public string UpdatedBy { get; set; }

    public PersonVisit() { }

    public PersonVisit(VisitJson json)
    {
        Id = json.Id;
        ParentId = json.ParentId;
        Name = json.Name;
        VisitDescription = json.VisitDescription;
        Type = json.Type;
        DateOfVisit = DateTimeOffset.Parse(json.DateOfVisit);
        VisitDetailsValue = json.VisitDetailsValue;
        LoginName = json.LoginName;
        Created = DateTimeOffset.Parse(json.Created);
        Updated = DateTimeOffset.Parse(json.Updated);
        CreatedBy = json.CreatedBy;
        UpdatedBy = json.UpdatedBy;
    }

    public VisitJson ToApiJson(string dateFormat = "s")
    {
        return new()
        {
            Id = Id,
            ParentId = ParentId,
            Name = Name,
            VisitDescription = VisitDescription,
            Type = Type,
            DateOfVisit = DateOfVisit.ToString(dateFormat),
            VisitDetailsValue = VisitDetailsValue,
            LoginName = LoginName,
            Created = Created.ToString(dateFormat),
            Updated = Updated.ToString(dateFormat),
            CreatedBy = CreatedBy,
            UpdatedBy = UpdatedBy,
        };
    }

    public static IEnumerable<PersonVisit> FromApiArray(IEnumerable<VisitJson> visits)
    {
        List<PersonVisit> outList = [];

        foreach (var jsonItem in visits)
            outList.Add(new PersonVisit(jsonItem));

        return outList;
    }

    public static async Task SaveVisitsAsync(Realm realm, IEnumerable<VisitJson> visits)
    {
        await RealmExtensions.CommitAsync(realm, () => realm.Upsert(FromApiArray(visits)));
    }
}
