using Realms;
using VisitzModel.Extensions;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Storage;

namespace VisitzModel.Models;

public partial class Attachment : IRealmObject, IRecordInfo
{
	public string RelatedEntityId { get; set; }

	private int RelatedEntityTypeInt { get; set; } = (int)EntityType.Unknown;
	public EntityType RelatedEntityType
	{
		get => (EntityType)RelatedEntityTypeInt;
		set => RelatedEntityTypeInt = (int)value;
	}

	private int RelatedEntitySubtypeInt { get; set; } = (int)EntitySubtype.Unknown;
	public EntitySubtype RelatedEntitySubtype
	{
		get => (EntitySubtype)RelatedEntitySubtypeInt;
		set => RelatedEntitySubtypeInt = (int)value;
	}

	public byte[] Thumbnail { get; set; }

	/// <summary>
	/// Real path to file on file system. File name will be different than <see cref="Filename"/>.
	/// </summary>
	public string Fullpath { get; set; }

	/// <summary>
	/// Virtual name of the attachment as stored in ICM.
	/// </summary>
	public string Filename { get; set; }

	[Backlink(nameof(AttachmentDraft.Attachment))]
	public IQueryable<AttachmentDraft> AttachmentDrafts { get; }

#pragma warning disable RLM025 // RealmObject/EmbeddedObject properties usually indicate a relationship
	public AttachmentDraft Draft => AttachmentDrafts.FirstOrDefault();
#pragma warning restore RLM025 // RealmObject/EmbeddedObject properties usually indicate a relationship

	public bool HasDraft => Draft != null;

	public static async Task DeleteAsync(Realm realm, Attachment attachment)
	{
		if (File.Exists(attachment.Fullpath))
			File.Delete(attachment.Fullpath);

		await attachment.CommitAsync(() =>
		{
			if (attachment.HasDraft)
				realm.Remove(attachment.Draft);

			realm.Remove(attachment);
		});
	}

	public async Task DeleteAsync()
	{
		await DeleteAsync(Realm, this);
	}
}
