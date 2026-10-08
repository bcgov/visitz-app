using Realms;
using VisitzApi.Models.Visits;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Interfaces;
using VisitzModel.Resources.Localization;

namespace VisitzModel.Models.InPersonVisits;

public partial class PersonVisit : IRealmObject, IApiJson<PostVisitJson>, IParentRecord, IEquatable<PersonVisit>
{
    static readonly string s_defaultType = "In Person Child Youth";

    static readonly string s_defaultSortPredicate =
        $"TRUEPREDICATE SORT({nameof(DateOfVisit)} DESC, {nameof(Created)} DESC)";

    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string ParentId { get; set; } = string.Empty;

    internal int ParentTypeInt { get; set; } = (int)EntityType.Case;

    public EntityType ParentType
    {
        get => (EntityType)ParentTypeInt;
        set => ParentTypeInt = (int)value;
    }

    public string Name { get; set; } = string.Empty;

    public string VisitDescription { get; set; } = string.Empty;

    public string Type { get; set; } = s_defaultType;

    public DateTimeOffset DateOfVisit { get; set; } = DateTimeOffset.Now;

    public IList<string> VisitDetails { get; } = null!; // Realm inits this automatically

    public string LoginName { get; set; } = string.Empty;

    public DateTimeOffset Created { get; set; }

    public DateTimeOffset Updated { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public string UpdatedBy { get; set; } = string.Empty;

    public DateTimeOffset DueDate => DateOfVisitBinding.Date.AddDays((int)VisitDaysThreshold.Info);

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

    public string FirstVisitDetail => VisitDetailsBinding.FirstOrDefault() ?? "";

    public string VisitDetailDisplay =>
        VisitDetailsBinding.Order().Aggregate("", (accum, next) => $"{accum}{Environment.NewLine}• {next}").Trim();

    [Ignored]
    public int SortOrder => DueDateDaysRemaining;

    public PersonVisit() { }

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

    public static async Task SynchronizeAsync(Realm realm, IEnumerable<VisitJson> visits, string parentCaseId)
    {
        await realm.SynchronizeByQueryAsync(
            incomingItems: FromApiArray(visits),
            existingQuery: GetVisitsByCaseId(realm, parentCaseId)
        );
    }

    public static IQueryable<PersonVisit> GetVisitsByCaseId(Realm realm, string parentCaseId)
    {
        return realm.All<PersonVisit>().Where(person => person.ParentId == parentCaseId).Filter(s_defaultSortPredicate);
    }

    public static void RemoveByParent(Realm realm, EntityType type, string parentId)
    {
        var visitItems = realm
            .All<PersonVisit>()
            .Where(item => item.ParentId == parentId && item.ParentTypeInt == (int)type);

        realm.RemoveRange(visitItems);
    }

    /// <summary>
    /// Gets an <see cref="IQueryable"/> representing the latest policy visit for every distinct parent record
    /// currently in the database.
    /// </summary>
    /// <param name="realm"></param>
    /// <returns></returns>
    public static IQueryable<PersonVisit> GetLatestPolicyVisitsPerParentRecord(Realm realm)
    {
        // As of October 2026, only private visits reset the 90-day policy counter.
        List<string> privateStrings =
        [
            PersonVisitDetails.Api_PrivateVisitAge0_5,
            PersonVisitDetails.Api_PrivateVisitInHome,
            PersonVisitDetails.Api_PrivateVisitMedicalSupportNeeds,
            PersonVisitDetails.Api_PrivateVisitNotInHome,
        ];
        privateStrings = privateStrings.Select(s => $"'{s}'").ToList();

        string privateDetails = $"{{ {string.Join(", ", privateStrings)} }}";

        return realm
            .All<PersonVisit>()
            .Filter(s_defaultSortPredicate)
            .Filter($"TRUEPREDICATE DISTINCT({nameof(ParentId)})")
            .Filter($"ANY {nameof(VisitDetails)} == ANY {privateDetails}");
    }

    /// <summary>
    /// <para>Gets the latest visit for a given parent case as per policy requirements.</para>
    /// </summary>
    /// <param name="realm"></param>
    /// <param name="parentCaseId"></param>
    /// <returns></returns>
    public static PersonVisit? GetLatestPolicyVisit(Realm realm, string parentCaseId)
    {
        return GetLatestPolicyVisitsPerParentRecord(realm).FirstOrDefault(visit => visit.ParentId == parentCaseId);
    }

    /// <summary>
    /// A predicate to check if a given visit is "upcoming" (are we approaching its due date or not).
    /// </summary>
    /// <param name="visit"></param>
    /// <returns></returns>
    public static bool IsUpcomingVisit(PersonVisit visit) =>
        visit != null && visit.CurrentDueDateThreshold <= VisitDaysThreshold.Warning;

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

    public bool Equals(PersonVisit? other)
    {
        return ReferenceEquals(this, other) || Id == other?.Id;
    }

    public override int GetHashCode()
    {
#pragma warning disable SS008 // GetHashCode() refers to mutable or static member
        // Id is not meant to be changed
        return Id.GetHashCode();
#pragma warning restore SS008 // GetHashCode() refers to mutable or static member
    }
}
