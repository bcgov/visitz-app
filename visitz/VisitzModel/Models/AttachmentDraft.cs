using Realms;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;

namespace VisitzModel.Models;

public partial class AttachmentDraft : IRealmObject, IDraftItem
{
	public string RelatedEntityId { get; set; }

	private int RelatedEntityTypeInt { get; set; }
	public EntityType RelatedEntityType
	{
		get => (EntityType)RelatedEntityTypeInt;
		set => RelatedEntityTypeInt = (int)value;
	}

	private int RelatedEntitySubtypeInt { get; set; }
	public EntitySubtype RelatedEntitySubtype
	{
		get => (EntitySubtype)RelatedEntitySubtypeInt;
		set => RelatedEntitySubtypeInt = (int)value;
	}

	public DateTimeOffset DraftCreated { get; set; } = DateTimeOffset.Now;

	public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.Now;

	public string Preview => Attachment.Filename;

	public string DraftLocation { get; set; }

	public Attachment Attachment { get; set; }

	public static AttachmentDraft Make(string filename, string fullpath, byte[] thumbnail = null)
	{
		return new()
		{
			Attachment = new()
			{
				Filename = filename,
				Fullpath = fullpath,
				Thumbnail = thumbnail,
			},
		};
	}
}
