using Realms;
using VisitzApi.Models.Visits;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Interfaces;

namespace VisitzModel.Models.InPersonVisits;

public partial class PersonVisit : IRealmObject, IApiJson<PostVisitJson>, IParentRecord
{
    static readonly string _defaultType = "In Person Child Youth";

    [PrimaryKey]
    public string Id { get; set; }

    public string ParentId { get; set; }

    private int ParentTypeInt { get; set; } = (int)EntityType.Case;

    public EntityType ParentType
    {
        get => (EntityType)ParentTypeInt;
        set => ParentTypeInt = (int)value;
    }

    public string Name { get; set; }

    public string VisitDescription { get; set; }

    public string Type { get; set; } = _defaultType;

    public DateTimeOffset DateOfVisit { get; set; } = DateTimeOffset.Now;

    public IList<string> VisitDetails { get; }

    public string LoginName { get; set; }

    public DateTimeOffset Created { get; set; }

    public DateTimeOffset Updated { get; set; }

    public string CreatedBy { get; set; }

    public string UpdatedBy { get; set; }

    public string FirstVisitDetail => VisitDetails?.FirstOrDefault() ?? "";

    public PersonVisit() { }

    public PersonVisit(CaseRecord @case)
    {
        ParentId = @case.Id;
    }

    public PersonVisit(params string[] visitDetails)
    {
        foreach (var detail in visitDetails)
            VisitDetails.Add(detail);
    }

    public PersonVisit(VisitJson json)
    {
        Id = json.Id;
        ParentId = json.ParentId;
        Name = json.Name;
        VisitDescription = json.VisitDescription;
        Type = json.Type;
        DateOfVisit = DateTimeOffset.Parse(json.DateOfVisit);

        foreach (var item in json.VisitDetails ?? [])
            VisitDetails.Add(item.VisitDetailValue);

        LoginName = json.LoginName;
        Created = DateTimeOffset.Parse(json.Created);
        Updated = DateTimeOffset.Parse(json.Updated);
        CreatedBy = json.CreatedBy;
        UpdatedBy = json.UpdatedBy;
    }

    public PostVisitJson ToApiJson(string dateFormat = "s")
    {
        PostVisitJson jsonVisit = new()
        {
            DateOfVisit = DateOfVisit,
            VisitDescription = VisitDescription,
            VisitDetails = [],
        };

        foreach (var item in VisitDetails)
            jsonVisit.VisitDetails.Add(new() { VisitDetailValue = item });

        return jsonVisit;
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

    public static IQueryable<PersonVisit> GetVisitsByCaseId(Realm realm, string caseId)
    {
        return realm.All<PersonVisit>()
            .Where(person => person.ParentId == caseId)
            .Filter($"TRUEPREDICATE SORT({nameof(DateOfVisit)} DESC, {nameof(Created)} DESC)");
    }

    public static void RemoveByParent(Realm realm, EntityType type, string parentId)
    {
        var visitItems = realm.All<PersonVisit>()
            .Where(item => item.ParentId == parentId && item.ParentTypeInt == (int)type);

        realm.RemoveRange(visitItems);
    }

    public void ToggleVisitDetail(string detail, bool add)
    {
        if (!IsValid)
            return;

        this.Commit(() =>
        {
            if (add && !VisitDetails.Contains(detail))
                VisitDetails.Add(detail);
            else if (!add && VisitDetails.Contains(detail))
                VisitDetails.Remove(detail);
        });

        RaisePropertyChanged(nameof(VisitDetails));
    }
}
