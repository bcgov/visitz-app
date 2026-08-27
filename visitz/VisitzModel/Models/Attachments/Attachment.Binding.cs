using VisitzModel.Extensions;
using VisitzModel.Models.EntityTypes;

namespace VisitzModel.Models.Attachments;

public partial class Attachment
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

    public byte[]? ThumbnailBinding
    {
        get => IsValid ? Thumbnail : default;
        set
        {
            this.Commit(() => Thumbnail = value);
            RaisePropertyChanged(nameof(ThumbnailBinding));
        }
    }

    public string RelativePathBinding
    {
        get => IsValid ? RelativePath : string.Empty;
        set
        {
            this.Commit(() => RelativePath = value);
            RaisePropertyChanged(nameof(RelativePathBinding));
            RaisePropertyChanged(nameof(FileExistsLocally));
        }
    }

    public string FilenameBinding
    {
        get => IsValid ? Filename : string.Empty;
        set
        {
            if (Filename != value)
            {
                this.Commit(() => Filename = value);
                RaisePropertyChanged(nameof(FilenameBinding));
            }
        }
    }

    public string ExtensionBinding
    {
        get => IsValid ? Extension : string.Empty;
        set
        {
            if (Extension != value)
            {
                this.Commit(() => Extension = value);
                RaisePropertyChanged(nameof(ExtensionBinding));
            }
        }
    }

    public DateTimeOffset CreatedDateBinding
    {
        get => IsValid ? CreatedDate : DateTimeOffset.MinValue;
        set
        {
            if (CreatedDate != value)
            {
                this.Commit(() => CreatedDate = value);
                RaisePropertyChanged(nameof(CreatedDateBinding));
            }
        }
    }
}
