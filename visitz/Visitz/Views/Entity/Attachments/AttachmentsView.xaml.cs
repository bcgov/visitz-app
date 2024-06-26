using Visitz.Extensions;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Entity.Attachments;

public partial class AttachmentsView : ViewModelContentView
{
	public AttachmentsView() : base(ServiceProvider.GetService<AttachmentsViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}

	private void AddPhotos_Clicked(object sender, EventArgs e)
	{
		_ = OpenTakePhotoView();
	}

	private static async Task OpenTakePhotoView()
	{
		await Navigator.Navigation.PushModalAsync(new TakePhotoView().WrapPageForModal(ViewModalSize.Fullscreen));
	}
}
