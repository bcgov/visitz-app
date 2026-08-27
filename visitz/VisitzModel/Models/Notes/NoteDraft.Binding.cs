using VisitzModel.Extensions;
using VisitzModel.Models.EntityTypes;

namespace VisitzModel.Models.Notes;

public partial class NoteDraft
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

    public string DraftBinding
    {
        get => IsValid ? Draft : string.Empty;
        set
        {
            bool canSet = !value?.ContainsUnicodeSurrogatesAndOtherSymbols() ?? true;

            if (canSet)
            {
                this.Commit(() => Draft = value ?? string.Empty);
                RaisePropertyChanged(nameof(DraftBinding));
                LastUpdatedBinding = DateTimeOffset.Now;
            }
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
