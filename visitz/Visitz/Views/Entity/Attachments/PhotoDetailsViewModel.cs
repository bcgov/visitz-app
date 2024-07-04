using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Models;
using VisitzModel.Storage.Filesystem;

namespace Visitz.Views.Entity.Attachments;

internal partial class PhotoDetailsViewModel : VisitzViewModel, ICaseloadItemHolder
{
	[ObservableProperty]
	public Attachment attachment;

	[ObservableProperty]
	public ImageSource detailImage;

	[ObservableProperty]
	public CaseloadItem caseloadItem;

	AttachmentFiler attachmentFiler;

	public override async void Create()
	{
		base.Create();

		attachmentFiler = await VisitzFiles.GetAsync(AttachmentFiler.PicturesPath, CaseloadItem);

		DetailImage = ImageSource.FromStream(GetPhoto);
	}

	async Task<Stream> GetPhoto(CancellationToken token)
	{
		return await attachmentFiler.GetAppDataFileAsync(Attachment.Fullpath, token);
	}
	
	public override void Destroy()
	{
		base.Destroy();
	}
}
