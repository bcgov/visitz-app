using VisitzModel.Extensions;
using VisitzModel.Models.EntityTypes;

namespace VisitzModel.Models;

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

	public byte[] ThumbnailBinding
	{
		get => IsValid ? Thumbnail : default;
		set
		{
			this.Commit(() =>  Thumbnail = value);
			RaisePropertyChanged(nameof(ThumbnailBinding));
		}
	}

	public string FullpathBinding
	{
		get => IsValid ? Fullpath : default;
		set
		{
			this.Commit(() => Fullpath = value);
			RaisePropertyChanged(nameof(FullpathBinding));
			RaisePropertyChanged(nameof(FilenameBinding));
		}
	}

	public string FilenameBinding
	{
		get => IsValid ? Filename : default;
		set
		{
			this.Commit(() => Filename = value);
			RaisePropertyChanged(nameof(FullpathBinding));
			RaisePropertyChanged(nameof(FilenameBinding));
		}
	}
}
