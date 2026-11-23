using RealmBindings;
using Realms;
using VisitzModel.Interfaces;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Resources.Localization;

namespace VisitzModel.Models.People;

[GenerateRealmBindings]
public partial class ContactEducationDraft : IRealmObject, IDraftItem, IValidToPublish
{
    [PrimaryKey]
    public string DraftEntityId { get; set; } = Guid.NewGuid().ToString();

    public ContactEducation? ContactEducation { get; set; } = new();

    public DateTimeOffset DraftCreated { get; set; } = DateTimeOffset.Now;

    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.Now;

    public string Preview => GeneralStrings.EducationInfo;

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

    public bool IsValidToPublish => ContactEducation?.IsValidToPublish ?? false;

    public ContactEducationDraft() { }

    public ContactEducationDraft(IBusinessObject businessObject)
    {
        this.InitDraftWith(businessObject);
    }
}
