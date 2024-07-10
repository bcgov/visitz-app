using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.Snackbar;
using VisitzModel.Extensions;
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

		attachmentFiler = await VisitzFiles.GetAsync(CaseloadItem);

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

	[RelayCommand]
	public static void DeleteAttachmentDraft(Attachment attachment)
	{
		if (attachment.HasDraft)
			_ = PromptDiscardAttachmentAsync(attachment);
	}

	static async Task PromptDiscardAttachmentAsync(Attachment attachment)
	{
		bool shouldDiscard = await Navigator.CurrentOpenPage.DisplayAlert(
			LocalizedStrings.DiscardDraft,
			LocalizedStrings.DiscardAttachmentDraftDescription,
			LocalizedStrings.Discard,
			LocalizedStrings.Cancel);

		if (shouldDiscard)
		{
			string filename = attachment.Filename;

			await attachment.DeleteAsync();
			await Navigator.Navigation.PopModalAsync();

			SnackbarHandler.ShowText(LocalizedStrings.FileDeleted.Format(filename));
		}
	}

	[RelayCommand]
	public static void PublishAttachmentDraft(Attachment attachment)
	{
		// TODO
	}
}
