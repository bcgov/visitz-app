using RealmBindings;
using Realms;
using VisitzApi.Models.People;
using VisitzModel.Extensions;
using VisitzModel.Formats;
using VisitzModel.Interfaces;
using VisitzModel.Utilities;

namespace VisitzModel.Models.People;

[GenerateRealmBindings]
public partial class ContactMedicalBehavioral
    : IRealmObject,
        IApiJson<SubmitContactMedicalBehavioralJson>,
        IComparable<ContactMedicalBehavioral>,
        IValidToPublish
{
    public static readonly int CommentsMaxLength = 200;
    public static readonly int DiagnosedByMaxLength = 100;
    public static readonly int TreatmentPlanMaxLength = 100;

    public static readonly string ConditionNamesLovKey = "ICM_CON_MED_COND";
    public static readonly string CategoriesLovKey = "ICM_CON_MED_CATEGORY";

    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ContactFirstName { get; set; } = string.Empty;
    public string ContactRowNum { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset Updated { get; set; } = DateTimeOffset.UtcNow;
    public string Comments { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string ParentCaseNum { get; set; } = string.Empty;
    public string CreatedByName { get; set; } = string.Empty;
    public string DiagnosedBy { get; set; } = string.Empty;
    public DateTimeOffset? EndDate { get; set; }
    public string TreatmentPlan { get; set; } = string.Empty;
    public string ContactMiddleName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTimeOffset? DiagnosisDate { get; set; }
    public string UpdatedByName { get; set; } = string.Empty;
    public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
    public string Condition { get; set; } = string.Empty;
    public string ContactLastName { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    public string ParentContactId { get; set; } = string.Empty;
    public DateTimeOffset? StartDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;

    public string? DisplayDiagnosisDate => DiagnosisDateBinding?.ToString(IcmDateFormats.BasicTimestamp);

    public string? DisplayStartDate => StartDateBinding?.ToString(IcmDateFormats.BasicTimestamp);

    public string? DisplayEndDate => EndDateBinding?.ToString(IcmDateFormats.BasicTimestamp);

    public bool IsValidToPublish =>
        Comments.Length <= CommentsMaxLength
        && DiagnosedBy.Length <= DiagnosedByMaxLength
        && TreatmentPlan.Length <= TreatmentPlanMaxLength
        && Category.Length > 0;

    public ContactMedicalBehavioral() { }

    public ContactMedicalBehavioral(GetContactMedicalBehavioralJson json, string parentContactId)
    {
        Id = json.Id;
        ContactFirstName = json.ContactFirstName;
        ContactLastName = json.ContactLastName;
        CreatedBy = json.CreatedBy;
        ContactRowNum = json.ContactRowNum;
        Name = json.Name;
        UpdatedBy = json.UpdatedBy;
        ParentContactId = parentContactId;
        Comments = json.Comments;
        Type = json.Type;
        ParentCaseNum = json.ParentCaseNum;
        CreatedByName = json.CreatedByName;
        DiagnosedBy = json.DiagnosedBy;
        TreatmentPlan = json.TreatmentPlan;
        ContactMiddleName = json.ContactMiddleName;
        Category = json.Category;
        UpdatedByName = json.UpdatedByName;
        Condition = json.Condition;
        Created = DateTimeOffset.Parse(json.Created);
        StartDate = Timestamp.ParseDateTimeOffsetNullable(json.StartDate);
        EndDate = Timestamp.ParseDateTimeOffsetNullable(json.EndDate);
        Updated = DateTimeOffset.Parse(json.Updated);
        DiagnosisDate = Timestamp.ParseDateTimeOffsetNullable(json.DiagnosisDate);
    }

    public SubmitContactMedicalBehavioralJson ToApiJson(string dateFormat = "s")
    {
        return new()
        {
            Comments = Comments,
            DiagnosedBy = DiagnosedBy,
            Category = Category,
            Condition = Condition,
            DiagnosisDate = DiagnosisDate?.ToString(dateFormat) ?? string.Empty,
            EndDate = EndDate?.ToString(dateFormat) ?? string.Empty,
            StartDate = StartDate?.ToString(dateFormat) ?? string.Empty,
            TreatmentPlan = TreatmentPlan,
        };
    }

    public static List<ContactMedicalBehavioral> FromApiJsonArray(
        IEnumerable<GetContactMedicalBehavioralJson> jsonArray,
        string parentContactId
    )
    {
        List<ContactMedicalBehavioral> outList = [];

        if (jsonArray != null)
            foreach (var jsonItem in jsonArray)
                outList.Add(new ContactMedicalBehavioral(jsonItem, parentContactId));

        return outList;
    }

    public static async Task SynchronizeAsync(
        Realm realm,
        IEnumerable<GetContactMedicalBehavioralJson> contactMedicalBehavioral,
        string parentContactId
    )
    {
        await realm.SynchronizeByQueryAsync(
            incomingItems: FromApiJsonArray(contactMedicalBehavioral, parentContactId),
            existingQuery: GetAllByParent(realm, parentContactId)
        );
    }

    public static void RemoveByParent(Realm realm, string parentContactId)
    {
        var contacts = realm.All<IcmContact>().Where(item => item.Id == parentContactId);

        if (contacts.Count() <= 1)
        {
            var contactMedicalBehavioral = realm
                .All<ContactMedicalBehavioral>()
                .Where(item => item.ParentContactId == parentContactId);

            realm.RemoveRange(contactMedicalBehavioral);
        }
    }

    public static IQueryable<ContactMedicalBehavioral> GetAllByParent(Realm realm, string parentContactId)
    {
        return realm.All<ContactMedicalBehavioral>().Where(item => item.ParentContactId == parentContactId);
    }

    public int CompareTo(ContactMedicalBehavioral? other)
    {
        if (other == null)
            return 1;

        int conditionCompare = ConditionBinding.CompareTo(other.ConditionBinding);
        if (conditionCompare != 0)
            return conditionCompare;

        int categoryCompare = CategoryBinding.CompareTo(other.CategoryBinding);
        if (categoryCompare != 0)
            return categoryCompare;

        int commentsCompare = CommentsBinding.CompareTo(other.CommentsBinding);
        if (commentsCompare != 0)
            return commentsCompare;

        return CreatedBinding.CompareTo(other.CreatedBinding);
    }
}
