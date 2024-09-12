using CommunityToolkit.Mvvm.ComponentModel;
using Realms;
using Visitz.Extensions;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;
using VisitzModel.Models;
using VisitzModel.Storage.Filesystem;

namespace Visitz.Views.Entity.Attachments;

internal partial class AttachmentsViewModel : VisitzViewModel, ICaseloadItemHolder
{
	[ObservableProperty]
	public CaseloadItem caseloadItem;

	Realm AttachmentsRealm { get; set; }

	AttachmentFiler attachmentFiler;

	public override async void Create()
	{
		base.Create();

		AttachmentsRealm = await VisitzRealms.GetAttachmentDraftsRealmAsync();
		attachmentFiler = await VisitzFiles.GetAsync(CaseloadItem);
	}

	public async Task SaveFile(FileResult fileResult)
	{
		string extension = fileResult.FileName.GetFileExtension();
		await using Stream stream = await fileResult.OpenReadAsync();

		if (Attachment.AllowedImageTypes.Contains(extension.ToLowerInvariant()))
		{
			byte[] thumbnail = await stream.MakeThumbnail(ThumbnailSize).AsBytesAsync(ImageFormat.Jpeg);
			await AttachmentDraft.SaveNewPhoto(attachmentFiler, AttachmentsRealm, fileResult.FileName, stream, thumbnail);
		}
		else
			await AttachmentDraft.SaveNewFile(attachmentFiler, AttachmentsRealm, fileResult.FileName, stream);
	}
}
