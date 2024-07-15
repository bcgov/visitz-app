using Realms;
using VisitzModel.Extensions;
using VisitzModel.Formats;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Resources.Localization;
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
		if (stream.Length > Attachment.MaxFilesize)
		{
			double tooLargeSize = stream.Length / (double)Sizes.MB;
			throw new ArgumentException(GeneralStrings.FileTooLarge.Format(tooLargeSize), nameof(stream));
		}

		string fullpath = await filer.SaveFileAsync(stream, new FileInfo(filename).Extension);
		var draft = MakeDraft(filer, filename, fullpath, thumbnail);

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

	static AttachmentDraft MakeDraft(AttachmentFiler filer, string filename, string fullpath, byte[] thumbnail)
	{
		var draft = new AttachmentDraft()
		{
			Attachment = new()
			{
				Filename = filename,
				Fullpath = fullpath,
				Thumbnail = thumbnail,
			},
		};
		draft.InitDraftWith(filer.CaseloadItem);

		return draft;
	}
}
