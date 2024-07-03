using Visitz.Extensions;
using Visitz.Views.BaseClasses;
using VisitzModel.Models;

namespace Visitz.Views.Entity.Attachments;

public partial class AttachmentsView : ViewModelContentView, ICaseloadItemHolder
{
	new AttachmentsViewModel ViewModel => base.ViewModel as AttachmentsViewModel;

	public CaseloadItem CaseloadItem
	{
		get => ViewModel.CaseloadItem;
		set => AttachmentsList.CaseloadItem = ViewModel.CaseloadItem = value;
	}

	public AttachmentsView() : base(ServiceProvider.GetService<AttachmentsViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}

	private void AddPhotos_Clicked(object sender, EventArgs e)
	{
		_ = OpenTakePhotoView();
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
