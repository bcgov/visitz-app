using Visitz.Animations;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Entity.Attachments;

public partial class TakePhotoView : BaseContentView
{
	readonly VisibilityAnimation SnapshotFade = new(showView: false);

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
		_ = AnimateSnapshot();
	}

	private async Task AnimateSnapshot()
	{
		SnapshotLayer.IsVisible = true;
		await Task.Delay(150);
		await SnapshotFade.Animate(SnapshotLayer, CancellationToken.None);
	}
}
