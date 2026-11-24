using RealmBindings;
using Realms;
using VisitzApi.Models.People;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Resources.Localization;

namespace VisitzModel.Models.People;

[GenerateRealmBindings]
public partial class ContactLanguage
    : IRealmObject,
        IApiJson<SubmitContactLanguageJson>,
        IComparable<ContactLanguage>,
        IValidToPublish
{
    public static readonly int CommentsMaxLength = 255;
    public static readonly int OtherLanguageMaxLength = 30;

    public static readonly string LanguageNamesLovKey = "ICM_CON_LANG";
    public static readonly string TypeLovKey = "ICM_LANG_TYPE";

    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
    public string Type { get; set; } = string.Empty;

    [MapTo("SSAPrimaryField")]
    public string Preferred { get; set; } = string.Empty;

    public DateTimeOffset Updated { get; set; } = DateTimeOffset.UtcNow;
    public string TranslatorReq { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;
    public string UpdatedByName { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    public string ParentContactId { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public string LanguageName { get; set; } = string.Empty;
    public string OtherLanguage { get; set; } = string.Empty;
    public string CreatedByName { get; set; } = string.Empty;
    public string ICMType { get; set; } = string.Empty;

    public string DisplayPreferred =>
        PreferredBinding.ParseWordTruthiness() ? GeneralStrings.Preferred : GeneralStrings.Secondary;

    public bool IsValidToPublish =>
        Comments.Length <= CommentsMaxLength
        && OtherLanguage.Length <= OtherLanguageMaxLength
        && LanguageName.Length > 0;

    public ContactLanguage() { }

    public ContactLanguage(GetContactLanguageJson json, string parentContactId)
    {
        Id = json.Id;
        CreatedBy = json.CreatedBy;
        Created = DateTimeOffset.Parse(json.Created);
        UpdatedBy = json.UpdatedBy;
        Type = json.Type;
        Preferred = json.SSAPrimaryField;
        Updated = DateTimeOffset.Parse(json.Updated);
        TranslatorReq = json.TranslatorReq;
        Comments = json.Comments;
        UpdatedByName = json.UpdatedByName;
        ParentContactId = parentContactId;
        LanguageName = json.LanguageName;
        OtherLanguage = json.OtherLanguage;
        CreatedByName = json.CreatedByName;
        ICMType = json.ICMType;
    }

    public SubmitContactLanguageJson ToApiJson(string dateFormat = "s")
    {
        return new()
        {
            TranslatorReq = TranslatorReq,
            Comments = Comments,
            LanguageName = LanguageName,
            OtherLanguage = OtherLanguage,
            ICMType = ICMType,
        };
    }

    public static List<ContactLanguage> FromApiJsonArray(
        IEnumerable<GetContactLanguageJson> jsonArray,
        string parentContactId
    )
    {
        List<ContactLanguage> outList = [];

        if (jsonArray != null)
            foreach (var jsonItem in jsonArray)
                outList.Add(new ContactLanguage(jsonItem, parentContactId));

        return outList;
    }

    public static async Task SynchronizeAsync(
        Realm realm,
        IEnumerable<GetContactLanguageJson> newContactLanguages,
        string parentContactId
    )
    {
        await realm.SynchronizeByQueryAsync(
            incomingItems: FromApiJsonArray(newContactLanguages, parentContactId),
            existingQuery: GetAllByParent(realm, parentContactId)
        );
    }

    public static void RemoveByParent(Realm realm, string parentContactId)
    {
        var contacts = realm.All<IcmContact>().Where(item => item.Id == parentContactId);

        if (contacts.Count() <= 1)
        {
            var contactLanguagesToBeDeleted = realm
                .All<ContactLanguage>()
                .Where(item => item.ParentContactId == parentContactId);

            realm.RemoveRange(contactLanguagesToBeDeleted);
        }
    }

    public static IQueryable<ContactLanguage> GetAllByParent(Realm realm, string parentContactId)
    {
        return realm.All<ContactLanguage>().Where(item => item.ParentContactId == parentContactId);
    }

    public int CompareTo(ContactLanguage? other)
    {
        if (other == null)
            return 1;

        int preferredCompare = PreferredBinding.CompareTo(other.PreferredBinding);
        if (preferredCompare != 0)
            return preferredCompare * -1; // Descending order

        return LanguageNameBinding.CompareTo(other.LanguageNameBinding);
    }
}
