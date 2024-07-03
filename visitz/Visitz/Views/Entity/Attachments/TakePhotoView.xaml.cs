using CommunityToolkit.Maui.Views;
using Visitz.Animations;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using VisitzModel;
using VisitzModel.Models;

namespace Visitz.Views.Entity.Attachments;

public partial class TakePhotoView : ViewModelContentView, ICaseloadItemHolder
{
	readonly VisibilityAnimation SnapshotFade = new(showView: false);

	new TakePhotoViewModel ViewModel => base.ViewModel as TakePhotoViewModel;

	public CaseloadItem CaseloadItem
	{
		get => ViewModel.CaseloadItem;
		set => ViewModel.CaseloadItem = value;
	}

	public TakePhotoView() : base(ServiceProvider.GetService<TakePhotoViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}

	protected override async void Creating()
	{
		base.Creating();

		Camera.MediaCaptured += Camera_MediaCaptured;
		Camera.MediaCaptureFailed += Camera_MediaCaptureFailed;

		await InitCamera();
	}

	async Task InitCamera()
	{
		try
		{
			await Camera.StartCameraPreview(ViewModel.Token);
		}
		catch (TaskCanceledException ex)
		{
			ConsoleTrace.TraceMethod(this, ex);
		}
		catch (Exception ex)
		{
			ConsoleTrace.TraceMethod(this, ex);

			await Navigator.CurrentOpenPage.DisplayAlert(
				LocalizedStrings.Error,
				ex.Message + " => " + ex.StackTrace,
				LocalizedStrings.Ok);
		}
	}

	protected override void Destroying()
	{
		base.Destroying();

		Camera.StopCameraPreview();
		Camera.Handler.DisconnectHandler();

		Camera.MediaCaptured -= Camera_MediaCaptured;
		Camera.MediaCaptureFailed -= Camera_MediaCaptureFailed;
	}

	private void TakePictureButton_Clicked(object sender, EventArgs e)
	{
		_ = AnimateSnapshotAsync();
	}

	private async Task AnimateSnapshotAsync()
	{
		SnapshotLayer.IsVisible = true;
		await Task.Delay(150);
		await SnapshotFade.Animate(SnapshotLayer, CancellationToken.None);
	}

	private async void Camera_MediaCaptured(object sender, MediaCapturedEventArgs e)
	{
		try
		{
			await ViewModel.SavePicture(e.Media);
		}
		catch (Exception ex)
		{
			ConsoleTrace.TraceMethod(this, ex);
		}
	}

	private void Camera_MediaCaptureFailed(object sender, MediaCaptureFailedEventArgs e)
	{
		ConsoleTrace.TraceMethod(this);
	}
}
