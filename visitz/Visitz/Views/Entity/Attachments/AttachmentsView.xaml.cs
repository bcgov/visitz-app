using Visitz.Extensions;
using Visitz.Views.BaseClasses;
using VisitzModel.Models;

namespace Visitz.Views.Entity.Attachments;

public partial class AttachmentsView : ViewModelContentView, ICaseloadItemHolder
{
	public CaseloadItem CaseloadItem { get; set; }

	public AttachmentsView() : base(ServiceProvider.GetService<AttachmentsViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}

	private void AddPhotos_Clicked(object sender, EventArgs e)
	{
		_ = OpenTakePhotoView();
	}

	private async Task OpenTakePhotoView()
	{
		TakePhotoView photoView = new() { CaseloadItem = CaseloadItem, };
		await Navigator.Navigation.PushModalAsync(photoView.WrapPageForModal(ViewModalSize.Fullscreen));
	}
}
