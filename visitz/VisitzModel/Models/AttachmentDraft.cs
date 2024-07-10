using Realms;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Storage.Filesystem;

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

	public static async Task<AttachmentDraft> SaveNew(
		AttachmentFiler filer,
		Realm realm,
		string filename,
		Stream stream,
		byte[] thumbnail = null)
	{
		string obfuscatedName = Guid.NewGuid().ToString() + new FileInfo(filename).Extension;
		string fullpath = await filer.SaveFileAsync(stream, obfuscatedName);

		var draft = new AttachmentDraft()
		{
			Attachment = new()
			{
				Filename = filename,
				Fullpath = fullpath,
				Thumbnail = thumbnail,
			},
		};
		draft.InitWith(filer.CaseloadItem);

		try
		{
			await realm.WriteAsync(() => realm.Add(draft));
		}
		catch
		{
			if (File.Exists(fullpath))
				File.Delete(fullpath);

			throw;
		}

		return draft;
	}
}
