using Realms;
using VisitzApi.Models.People;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Utilities;

namespace VisitzModel.Models.People;

#nullable enable
public partial class ContactLanguage : IRealmObject, IApiJson<ContactLanguageJson>
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTimeOffset? Created { get; set; }
    public string Type { get; set; } = string.Empty;
    public string SSAPrimaryField { get; set; } = string.Empty;
    public DateTimeOffset? Updated { get; set; }
    public string TranslatorReq { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;
    public string UpdatedByName { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    public string ContactId { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public string LanguageName { get; set; } = string.Empty;
    public string OtherLanguage { get; set; } = string.Empty;
    public string CreatedByName { get; set; } = string.Empty;
    public string ICMType { get; set; } = string.Empty;
    public string ParentId { get; set; } = string.Empty;
    private int ParentTypeInt { get; set; } = (int)EntityType.Unknown;
    public EntityType ParentType
    {
        get => (EntityType)ParentTypeInt;
        set => ParentTypeInt = (int)value;
    }

    public ContactLanguage() { }

    public ContactLanguage(ContactLanguageJson json, EntityType type, string parentId)
    {
        Id = json.Id;
        CreatedBy = json.CreatedBy;
        Created = Timestamp.ParseDateTimeOffsetNullable(json.Created);
        UpdatedBy = json.UpdatedBy;
        Type = json.Type;
        SSAPrimaryField = json.SSAPrimaryField;
        Updated = Timestamp.ParseDateTimeOffsetNullable(json.Updated);
        TranslatorReq = json.TranslatorReq;
        Comments = json.Comments;
        UpdatedByName = json.UpdatedByName;
        ContactId = json.ContactId;
        LanguageName = json.LanguageName;
        OtherLanguage = json.OtherLanguage;
        CreatedByName = json.CreatedByName;
        ICMType = json.ICMType;
        ParentType = type;
        ParentId = parentId;
    }

    public ContactLanguageJson ToApiJson(string dateFormat = "s")
    {
        return new()
        {
            Id = Id,
            CreatedBy = CreatedBy,
            Created = Created?.ToString(dateFormat) ?? string.Empty,
            UpdatedBy = UpdatedBy,
            Type = Type,
            SSAPrimaryField = SSAPrimaryField,
            Updated = Updated?.ToString(dateFormat) ?? string.Empty,
            TranslatorReq = TranslatorReq,
            Comments = Comments,
            UpdatedByName = UpdatedByName,
            ContactId = ContactId,
            LanguageName = LanguageName,
            OtherLanguage = OtherLanguage,
            CreatedByName = CreatedByName,
            ICMType = ICMType,
        };
    }

    public static List<ContactLanguage> FromApiJsonArray(
        IEnumerable<ContactLanguageJson> jsonArray,
        EntityType type,
        string parentId
    )
    {
        List<ContactLanguage> outList = [];

        if (jsonArray != null)
            foreach (var jsonItem in jsonArray)
                outList.Add(new ContactLanguage(jsonItem, type, parentId));

        return outList;
    }

    public static async Task SynchronizeAsync(
        Realm realm,
        IEnumerable<ContactLanguageJson> newContactLanguages,
        string parentId,
        EntityType type
    )
    {
        if (newContactLanguages == null)
            return;

        var incomingContactLanguages = FromApiJsonArray(newContactLanguages, type, parentId);
        var incomingContactLanguageIds = incomingContactLanguages.Select(item => item.Id);

        var allContactLanguages = realm
            .All<ContactLanguage>()
            .Where(item => item.ParentId == parentId && item.ParentTypeInt == (int)type);
        var allContactLanguageIds = allContactLanguages.AsEnumerable().Select(item => item.Id);

        var contactLanguageIdsToDelete = allContactLanguageIds.Except(incomingContactLanguageIds);
        var contactLanguagesToDelete = allContactLanguages
            .ToList()
            .Where(item => contactLanguageIdsToDelete.Contains(item.Id));

        if (!contactLanguageIdsToDelete.Any() && !incomingContactLanguageIds.Any())
            return;

        await RealmExtensions.CommitAsync(
            realm,
            () =>
            {
                foreach (var item in contactLanguagesToDelete)
                {
                    if (item != null && item.IsValid)
                        realm.Remove(item);
                }
                realm.Upsert(incomingContactLanguages);
            }
        );
    }

    public static void RemoveByParent(Realm realm, EntityType type, string parentId)
    {
        var visitItems = realm
            .All<ContactLanguage>()
            .Where(item => item.ParentId == parentId && item.ParentTypeInt == (int)type);
        realm.RemoveRange(visitItems);
    }
}
