using CommunityToolkit.Mvvm.ComponentModel;
using Realms;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;
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
		attachmentFiler = await VisitzFiles.GetAsync(
			CaseloadItem.EntityType.ParseEntityType(),
			CaseloadItem.CaseIncidentNumber,
			CaseloadItem.KeyPlayer.FirstName,
			CaseloadItem.KeyPlayer.LastName);
	}

	public async Task SaveFile(FileResult fileResult)
	{
		string extension = fileResult.FileName.GetFileExtension();
		await using Stream stream = await fileResult.OpenReadAsync();

		if (Attachment.AllowedImageTypes.Contains(extension.ToLowerInvariant()))
			await AttachmentDraft.SaveNewPhoto(CaseloadItem, attachmentFiler, AttachmentsRealm, fileResult.FileName, stream);
		else
			await AttachmentDraft.SaveNewFile(CaseloadItem, attachmentFiler, AttachmentsRealm, fileResult.FileName, stream);
	}
}
