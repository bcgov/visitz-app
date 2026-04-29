using VisitzModel.Extensions;
using VisitzModel.Models.EntityTypes;

namespace VisitzModel.Models.InPersonVisits;

public partial class PersonVisitDraft
{
    public EntityType RelatedEntityTypeBinding
    {
        get => IsValid ? RelatedEntityType : default;
        set
        {
            this.Commit(() => RelatedEntityType = value);
            RaisePropertyChanged(nameof(RelatedEntityType));
        }
    }

    public EntitySubtype RelatedEntitySubtypeBinding
    {
        get => IsValid ? RelatedEntitySubtype : default;
        set
        {
            this.Commit(() => RelatedEntitySubtype = value);
            RaisePropertyChanged(nameof(RelatedEntitySubtype));
        }
    }

    public DateTimeOffset LastUpdatedBinding
    {
        get => IsValid ? LastUpdated : default;
        set
        {
            this.Commit(() => LastUpdated = value);
            RaisePropertyChanged(nameof(LastUpdated));
        }
    }

    public string DraftLocationBinding
    {
        get => IsValid ? DraftLocation : string.Empty;
        set
        {
            this.Commit(() => DraftLocation = value);
            RaisePropertyChanged(nameof(DraftLocation));
        }
    }
}
