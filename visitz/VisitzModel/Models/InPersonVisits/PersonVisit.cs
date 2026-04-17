using System.Diagnostics.CodeAnalysis;
using Realms;
using VisitzApi.Models.Visits;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Interfaces;

namespace VisitzModel.Models.InPersonVisits;

#nullable enable

public partial class PersonVisit
    : IRealmObject,
        IApiJson<PostVisitJson>,
        IParentRecord,
        IComparable<ITodoItem>,
        IEquatable<PersonVisit>,
        IEqualityComparer<PersonVisit>
{
    static readonly string _defaultType = "In Person Child Youth";

    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string ParentId { get; set; } = string.Empty;

    private int ParentTypeInt { get; set; } = (int)EntityType.Case;

    public EntityType ParentType
    {
        get => (EntityType)ParentTypeInt;
        set => ParentTypeInt = (int)value;
    }

    public string Name { get; set; } = string.Empty;

    public string VisitDescription { get; set; } = string.Empty;

    public string Type { get; set; } = _defaultType;

    public DateTimeOffset DateOfVisit { get; set; } = DateTimeOffset.Now;

    public IList<string> VisitDetails { get; } = null!; // Realm inits this automatically

    public string LoginName { get; set; } = string.Empty;

    public DateTimeOffset Created { get; set; }

    public DateTimeOffset Updated { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public string UpdatedBy { get; set; } = string.Empty;

    public DateTimeOffset DueDate =>
        IsValid ? DateOfVisit.Date.AddDays((int)VisitDaysThreshold.Info) : DateTimeOffset.MinValue;
    public int DueDateDaysRemaining => (DueDate.Date - DateTimeOffset.Now.Date).Days;

    public VisitDaysThreshold CurrentDueDateThreshold
    {
        get
        {
            if (DueDateDaysRemaining <= (int)VisitDaysThreshold.Critical)
                return VisitDaysThreshold.Critical;
            else if (DueDateDaysRemaining <= (int)VisitDaysThreshold.Danger)
                return VisitDaysThreshold.Danger;
            else if (DueDateDaysRemaining <= (int)VisitDaysThreshold.Warning)
                return VisitDaysThreshold.Warning;
            else
                return VisitDaysThreshold.Info;
        }
    }

    public string FirstVisitDetail => VisitDetails?.FirstOrDefault() ?? "";

    [Ignored]
    public int SortOrder => DueDateDaysRemaining;

    public PersonVisit() { }

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
            DateOfVisit = DateOfVisit.UtcDateTime.ToString(dateFormat),
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

    public static async Task SynchronizeAsync(Realm realm, IEnumerable<VisitJson> visits, string parentId)
    {
        await RealmExtensions.CommitAsync(
            realm,
            () =>
            {
                var incomingVisits = FromApiArray(visits);
                var existingVisits = GetVisitsByCaseId(realm, parentId).ToList();
                var visitsToDelete = existingVisits.Except(incomingVisits);

                foreach (var item in visitsToDelete)
                {
                    if (item != null && item.IsValid)
                        realm.Remove(item);
                }
                realm.Upsert(incomingVisits);
            }
        );
    }

    public static IQueryable<PersonVisit> GetVisitsByCaseId(Realm realm, string caseId)
    {
        return realm
            .All<PersonVisit>()
            .Where(person => person.ParentId == caseId)
            .Filter($"TRUEPREDICATE SORT({nameof(DateOfVisit)} DESC, {nameof(Created)} DESC)");
    }

    public static void RemoveByParent(Realm realm, EntityType type, string parentId)
    {
        var visitItems = realm
            .All<PersonVisit>()
            .Where(item => item.ParentId == parentId && item.ParentTypeInt == (int)type);

        realm.RemoveRange(visitItems);
    }

    public static IEnumerable<PersonVisit?> GetUpcomingVisits(Realm realm)
    {
        var latestVisitsPerCase = realm
            .All<PersonVisit>()
            .Filter($"TRUEPREDICATE SORT({nameof(DateOfVisit)} DESC, {nameof(Created)} DESC)")
            .Filter($"TRUEPREDICATE DISTINCT({nameof(ParentId)})")
            .AsEnumerable()
            .Where(item => item != null && item.CurrentDueDateThreshold <= VisitDaysThreshold.Warning);

        return latestVisitsPerCase;
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

    public int CompareTo(ITodoItem? other)
    {
        return other == null ? 1 : SortOrder.CompareTo(other.SortOrder);
    }

    public bool Equals(PersonVisit? other)
    {
        return Equals(this, other);
    }

    public bool Equals(PersonVisit? x, PersonVisit? y)
    {
        return x?.Id == y?.Id;
    }

    public int GetHashCode([DisallowNull] PersonVisit obj)
    {
        return obj.Id.GetHashCode();
    }
}
