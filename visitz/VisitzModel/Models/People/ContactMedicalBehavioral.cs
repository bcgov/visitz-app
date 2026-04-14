using Realms;
using VisitzApi.Models.People;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Utilities;

#nullable enable

namespace VisitzModel.Models.People;

public partial class ContactMedicalBehavioral : IRealmObject, IApiJson<ContactMedicalBehavioralJson>
{
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

    public ContactMedicalBehavioral() { }

    public ContactMedicalBehavioral(ContactMedicalBehavioralJson json, string parentContactId)
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

    public ContactMedicalBehavioralJson ToApiJson(string dateFormat = "s")
    {
        return new ContactMedicalBehavioralJson()
        {
            Id = Id,
            Created = Created.ToString(dateFormat) ?? string.Empty,
            CreatedBy = CreatedBy,
            CreatedByName = CreatedByName,
            Updated = Updated.ToString(dateFormat) ?? string.Empty,
            UpdatedBy = UpdatedBy,
            UpdatedByName = UpdatedByName,
            ContactFirstName = ContactFirstName,
            ContactId = ParentContactId,
            ContactLastName = ContactLastName,
            Comments = Comments,
            DiagnosedBy = DiagnosedBy,
            Category = Category,
            Condition = Condition,
            ContactMiddleName = ContactMiddleName,
            ContactRowNum = ContactRowNum,
            DiagnosisDate = DiagnosisDate?.ToString(dateFormat) ?? string.Empty,
            EndDate = EndDate?.ToString(dateFormat) ?? string.Empty,
            Name = Name,
            ParentCaseNum = ParentCaseNum,
            StartDate = StartDate?.ToString(dateFormat) ?? string.Empty,
            TreatmentPlan = TreatmentPlan,
            Type = Type,
        };
    }

    public static List<ContactMedicalBehavioral> FromApiJsonArray(
        IEnumerable<ContactMedicalBehavioralJson> jsonArray,
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
        IEnumerable<ContactMedicalBehavioralJson> contactMedicalBehavioral,
        string parentContactId
    )
    {
        if (contactMedicalBehavioral == null)
            return;

        var incomingContactMedicalBehavioral = FromApiJsonArray(contactMedicalBehavioral, parentContactId);
        var incomingContactMedicalBehavioralIds = incomingContactMedicalBehavioral.Select(item => item.Id);

        var allContactMedicalBehavioral = realm
            .All<ContactMedicalBehavioral>()
            .Where(item => item.ParentContactId == parentContactId);
        var allContactMedicalBehavioralIds = allContactMedicalBehavioral.AsEnumerable().Select(item => item.Id);

        var contactMedicalBehavioralIdsToDelete = allContactMedicalBehavioralIds.Except(
            incomingContactMedicalBehavioralIds
        );
        var contactMedicalBehavioralToDelete = allContactMedicalBehavioral
            .ToList()
            .Where(item => contactMedicalBehavioralIdsToDelete.Contains(item.Id));

        if (!contactMedicalBehavioralToDelete.Any() && !incomingContactMedicalBehavioralIds.Any())
            return;

        await RealmExtensions.CommitAsync(
            realm,
            () =>
            {
                foreach (var item in contactMedicalBehavioralToDelete)
                {
                    if (item != null && item.IsValid)
                        realm.Remove(item);
                }

                realm.Upsert(incomingContactMedicalBehavioral);
            }
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
}
