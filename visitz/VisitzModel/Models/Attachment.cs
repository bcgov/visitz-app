using Realms;
using VisitzModel.Extensions;
using VisitzModel.Formats;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Storage.Filesystem;

namespace VisitzModel.Models;

public partial class Attachment : IRealmObject, IRecordInfo
{
	public static readonly int MaxFilesize = 5 * Sizes.MB;
	public static readonly int ThumbnailSize = 200;

	public static readonly IEnumerable<string> AllowedImageTypes = [".jpg", ".jpeg"];
	public static readonly IEnumerable<string> AllowedDocumentTypes = [".pdf"];

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
	/// Relative path to file on virtualized file system. File name will be different than <see cref="Filename"/>.
	/// </summary>
	public string RelativePath { get; set; }

	/// <summary>
	/// Virtual name of the attachment as stored in ICM, without the file type extension.
	/// </summary>
	public string Filename { get; set; }

	/// <summary>
	/// The file type extension including the dot '.'
	/// </summary>
	public string Extension { get; set; }

	[Backlink(nameof(AttachmentDraft.Attachment))]
	public IQueryable<AttachmentDraft> AttachmentDrafts { get; }

#pragma warning disable RLM025 // RealmObject/EmbeddedObject properties usually indicate a relationship
	public AttachmentDraft Draft => AttachmentDrafts.FirstOrDefault();
#pragma warning restore RLM025 // RealmObject/EmbeddedObject properties usually indicate a relationship

	public bool HasDraft => Draft != null;

	public static async Task DeleteAsync(Realm realm, Attachment attachment)
	{
		string fullpath = AttachmentFiler.GetFullPath(attachment.RelativePath);

		if (File.Exists(fullpath))
			File.Delete(fullpath);

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
