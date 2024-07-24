using Microsoft.Maui.Graphics.Platform;
using Realms;
using VisitzApi.Models.Attachments;
using VisitzModel.Extensions;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Formats;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Resources.Localization;
using VisitzModel.Storage.Filesystem;
using VisitzModel.Utilities;
using IImage = Microsoft.Maui.Graphics.IImage;

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

	public static async Task<AttachmentDraft> SaveNewPhoto(
		AttachmentFiler filer,
		Realm realm,
		string filename,
		Stream stream,
		byte[] thumbnail = null)
	{
		stream = LimitFilesizeByResize(stream, ImageFormat.Jpeg);
		return await SaveNewFile(filer, realm, filename, stream, thumbnail);
	}

	public static async Task<AttachmentDraft> SaveNewFile(
		AttachmentFiler filer,
		Realm realm,
		string filename,
		Stream stream,
		byte[] thumbnail = null)
	{
		if (stream.Length > Attachment.MaxFilesize)
			ThrowSizeError(stream);

		string fullpath = await filer.SaveFileAsync(stream, filename.GetFileExtension());
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

	static Stream LimitFilesizeByResize(Stream stream, ImageFormat imageFormat)
	{
		if (stream.Length <= Attachment.MaxFilesize)
			return stream;

		var (image, newWidth, newHeight) = GetNewDimensions(stream, imageFormat);

		var downsizedImage = image.Downsize(Math.Max(newWidth, newHeight));

		var downsizedStream = downsizedImage.AsStream(imageFormat);

		ConsoleTrace.TraceMethod(typeof(AttachmentDraft),
			$"Original size '{stream.Length}' ||| Resized size '{downsizedStream.Length}'");

		return downsizedStream;
	}

	static (IImage Image, float NewWidth, float NewHeight) GetNewDimensions(Stream stream, ImageFormat imageFormat)
	{
		stream.Seek(0, SeekOrigin.Begin);

		var image = PlatformImage.FromStream(stream, imageFormat);

		var (newWidth, newHeight) = ResizeImageValues.ResizeByFileSize(
			image.Width,
			image.Height,
			Attachment.MaxFilesize);

		ConsoleTrace.TraceMethod(typeof(AttachmentDraft),
			$"Original w,h ({image.Width},{image.Height}) ||| Resized w,h ({newWidth},{newHeight})");

		return (image, newWidth, newHeight);
	}

	static void ThrowSizeError(Stream stream)
	{
		double tooLargeSize = stream.Length / (double)Sizes.MB;
		throw new ArgumentException(GeneralStrings.FileTooLarge.Format(tooLargeSize), nameof(stream));
	}

	static AttachmentDraft MakeDraft(AttachmentFiler filer, string filename, string relativePath, byte[] thumbnail)
	{
		int dotIndex = filename.LastIndexOf('.');

		var draft = new AttachmentDraft()
		{
			Attachment = new()
			{
				Filename = dotIndex != -1 ? filename[..dotIndex] : filename,
				Extension = dotIndex != -1 ? filename[dotIndex..] : filename,
				RelativePath = relativePath,
				Thumbnail = thumbnail,
			},
		};
		draft.InitDraftWith(filer.CaseloadItem);

		return draft;
	}

	public async Task<SubmitAttachmentEntity> ToSubmitAttachmentEntity(
		AttachmentFiler attachmentFiler,
		IStreamConverter streamConverter = null,
		CancellationToken? token = null)
	{
		token ??= CancellationToken.None;

		await using var attachmentStream = await attachmentFiler.GetAppDataFileAsync(Attachment.RelativePath, token);
		byte[] attachmentBytes;

		if (streamConverter != null)
		{
			var convertedStream = await streamConverter.ConvertAsync(attachmentStream);

			attachmentBytes = new byte[convertedStream.Length];
			await convertedStream.ReadAsync(attachmentBytes.AsMemory(0, attachmentBytes.Length), token.Value);
		}
		else
		{
			attachmentBytes = new byte[attachmentStream.Length];
			await attachmentStream.ReadAsync(attachmentBytes.AsMemory(0, attachmentBytes.Length), token.Value);
		}

		return new()
		{
			EntityNumber = RelatedEntityId,
			EntityType = RelatedEntityType.GetDisplayString(),
			CaseType = RelatedEntitySubtype.GetDisplayString(),
			FormName = IcmFormNames.GenericDocument,
			FileName = Attachment.Filename,
			FormDescription = "",
			FormCategory = "",
			Section13Exists = "",
			InvestigationResponse = "",
			Attachment = new()
			{
				PdfString = Convert.ToBase64String(attachmentBytes),
			}
		};
	}
}
