using Visitz.Extensions;
using Visitz.Views.BaseClasses;
using VisitzModel.Models;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity.Attachments;

public partial class AttachmentsView : ViewModelContentView, ICaseloadItemHolder, IFocusDraftItem
{
	static readonly IEnumerable<string> AllowedTypes = Attachment.AllowedImageTypes
		.Concat(Attachment.AllowedDocumentTypes);

	new AttachmentsViewModel ViewModel => base.ViewModel as AttachmentsViewModel;

	public CaseloadItem CaseloadItem
	{
		get => ViewModel.CaseloadItem;
		set => AttachmentsList.CaseloadItem = ViewModel.CaseloadItem = value;
	}

	public IDraftItem FocusedDraftItem
	{
		get => AttachmentsList.FocusedDraftItem;
		set => AttachmentsList.FocusedDraftItem = value;
	}

	public AttachmentsView() : base(ServiceProvider.GetService<AttachmentsViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}

	private async void AddPhotos_Clicked(object sender, EventArgs e)
	{
		await OpenTakePhotoView();
	}

	private async void Browse_Clicked(object sender, EventArgs e)
	{
		var result = await FilePicker.Default.PickAsync(new()
		{
			FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>()
			{
				{ DevicePlatform.WinUI, AllowedTypes },
			}),
		});

		await SaveFile(result);
	}

	private async Task SaveFile(FileResult result)
	{
		if (result == null)
			return;

		try
		{
			await ViewModel.SaveFile(result);
		}
		catch (Exception ex)
		{
			await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
		}
	}

	protected override void Destroying()
	{
		base.Destroying();

		AttachmentsList.Destroy();
	}

	private async Task OpenTakePhotoView()
	{
		TakePhotoView photoView = new() { CaseloadItem = CaseloadItem, };
		await Navigator.Navigation.PushModalAsync(photoView.WrapPageForModal(ViewModalSize.Fullscreen));
	}
}
