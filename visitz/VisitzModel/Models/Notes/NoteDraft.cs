using Realms;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Resources.Localization;

namespace VisitzModel.Models.Notes;

public partial class NoteDraft : IRealmObject, IDraftItem
{
    // Only allow one note draft per parent entity.
    [PrimaryKey]
    public string ParentEntityId { get; set; } = Guid.NewGuid().ToString();

    public string RelatedEntityId
    {
        get => ParentEntityId;
        set { }
    }

    internal int RelatedEntityTypeInt { get; set; } = (int)EntityType.Unknown;

    public EntityType RelatedEntityType
    {
        get => (EntityType)RelatedEntityTypeInt;
        set => RelatedEntityTypeInt = (int)value;
    }

    internal int RelatedEntitySubtypeInt { get; set; } = (int)EntitySubtype.Unknown;

    public EntitySubtype RelatedEntitySubtype
    {
        get => (EntitySubtype)RelatedEntitySubtypeInt;
        set => RelatedEntitySubtypeInt = (int)value;
    }

    public string Draft { get; set; } = string.Empty;

    public DateTimeOffset DraftCreated { get; set; } = DateTimeOffset.Now;

    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.Now;

    public string Preview => GeneralStrings.Note;

    public string DraftLocation { get; set; } = string.Empty;

    public static string MakeId(string parentEntityId)
    {
        return $"{parentEntityId}";
    }

    public static NoteDraft? FindByEntityId(Realm realm, string entityId)
    {
        return realm.Find<NoteDraft>(MakeId(entityId));
    }

    public static async Task Delete(Realm realm, string entityNumber)
    {
        var draft = FindByEntityId(realm, entityNumber);

        if (draft != null)
            await realm.WriteAsync(() => realm.Remove(draft));
    }
}
