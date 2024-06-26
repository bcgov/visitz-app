using Visitz.Views.BaseClasses;

namespace Visitz.Views.Entity.Attachments;

public partial class TakePhotoView : BaseContentView
{
	public TakePhotoView()
	{
		InitializeComponent();
	}

	protected override async void Creating()
	{
		base.Creating();

		await Camera.StartCameraPreview(CancellationToken.None);
	}

	protected override void Destroying()
	{
		base.Destroying();

		Camera.StopCameraPreview();
	}

	private void TakePictureButton_Clicked(object sender, EventArgs e)
	{

	}
}
