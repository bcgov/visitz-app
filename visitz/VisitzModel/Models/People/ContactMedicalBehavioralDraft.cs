using RealmBindings;
using Realms;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;

namespace VisitzModel.Models.People;

[GenerateRealmBindings]
public partial class ContactMedicalBehavioralDraft : IRealmObject, IDraftItem
{
    [PrimaryKey]
    public string DraftEntityId { get; set; } = Guid.NewGuid().ToString();

    public ContactMedicalBehavioral? ContactMedicalBehavioral { get; set; } = new();

    public DateTimeOffset DraftCreated { get; set; } = DateTimeOffset.Now;

    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.Now;

    public DateTimeOffset LastUpdatedBinding { get; set; }

    public string Preview => ContactMedicalBehavioral?.ConditionBinding ?? string.Empty;

    public string DraftLocation { get; set; } = string.Empty;

    public string RelatedEntityId { get; set; } = string.Empty;

    internal int RelatedEntityTypeInt { get; set; } = (int)EntityType.Case;

    public EntityType RelatedEntityType
    {
        get => (EntityType)RelatedEntityTypeInt;
        set => RelatedEntityTypeInt = (int)value;
    }

    internal int RelatedEntitySubtypeInt { get; set; } = (int)EntitySubtype.ChildServices;

    public EntitySubtype RelatedEntitySubtype
    {
        get => (EntitySubtype)RelatedEntitySubtypeInt;
        set => RelatedEntitySubtypeInt = (int)value;
    }
}
