using Realms;
using VisitzApi.Models.Visits;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Interfaces;
using VisitzModel.Resources.Localization;

namespace VisitzModel.Models.InPersonVisits;

public partial class PersonVisit : IRealmObject, IApiJson<PostVisitJson>, IParentRecord
{
    static readonly string _defaultType = "In Person Child Youth";
    static readonly char DetailsDelimiter = '-';

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

    public string VisitDetailsValue { get; set; }

    public string VisitDetailsGroup { get; set; }

    public string LoginName { get; set; }

    public DateTimeOffset Created { get; set; }

    public DateTimeOffset Updated { get; set; }

    public string CreatedBy { get; set; }

    public string UpdatedBy { get; set; }

    public string CombinedVisitDetails => MakeDetailsValue(VisitDetailsGroup, VisitDetailsValue);

    public PersonVisit() { }

    public PersonVisit(CaseRecord @case)
    {
        ParentId = @case.Id;
    }

    public PersonVisit(VisitJson json)
    {
        Id = json.Id;
        ParentId = json.ParentId;
        Name = json.Name;
        VisitDescription = json.VisitDescription;
        Type = json.Type;
        DateOfVisit = DateTimeOffset.Parse(json.DateOfVisit);
        VisitDetailsValue = json.VisitDetailsValue;

        var (group, value) = SplitDetailsValue(VisitDetailsValue);
        VisitDetailsGroup = group;
        VisitDetailsValue = value;

        LoginName = json.LoginName;
        Created = DateTimeOffset.Parse(json.Created);
        Updated = DateTimeOffset.Parse(json.Updated);
        CreatedBy = json.CreatedBy;
        UpdatedBy = json.UpdatedBy;
    }

    public PostVisitJson ToApiJson(string dateFormat = "s")
    {
        return new()
        {
            DateOfVisit = DateOfVisit,
            VisitDescription = VisitDescription,
            VisitDetailsValue = MakeDetailsValue(VisitDetailsGroup, VisitDetailsValue),
        };
    }

    static string MakeDetailsValue(string group, string value)
    {
        if (group.StartsWith(PersonVisitDetails.Type_PrivateVisit))
            return $"{group} {value}";
        else
            return $"{group} {DetailsDelimiter} {value}";
    }

    static (string Group, string Value) SplitDetailsValue(string detailsValue)
    {
        string privateVisit = PersonVisitDetails.Type_PrivateVisit;

        if (detailsValue.StartsWith(privateVisit))
            return (privateVisit.Trim(), detailsValue[privateVisit.Length..].Trim());
        else
        {
            string[] split = detailsValue.Split(DetailsDelimiter);
            return (split[0].Trim(), split[1].Trim());
        }
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
