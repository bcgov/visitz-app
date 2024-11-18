using CommunityToolkit.Maui.Views;
using Visitz.Animations;
using Visitz.Device;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using Visitz.Views.Snackbar;
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

        Unloaded += TakePhotoView_Unloaded;
        Camera.MediaCaptured += Camera_MediaCaptured;
        Camera.MediaCaptureFailed += Camera_MediaCaptureFailed;

        await InitCamera();
    }

    private void TakePhotoView_Unloaded(object sender, EventArgs e)
    {
        Camera.StopCameraPreview();
        Camera.Handler.DisconnectHandler();

        Camera.MediaCaptured -= Camera_MediaCaptured;
        Camera.MediaCaptureFailed -= Camera_MediaCaptureFailed;

        ViewModel.Destroy();

        Unloaded -= TakePhotoView_Unloaded;
    }

    async Task InitCamera()
    {
        try
        {
            await Camera.StartCameraPreview(CancellationToken.None);
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

    private async void TakePictureButton_Clicked(object sender, EventArgs e)
    {
        _ = AnimateSnapshotAsync();

        await Camera.CaptureImage(CancellationToken.None);
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
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
            ConsoleTrace.TraceMethod(this, ex);
        }
    }

    private void Camera_MediaCaptureFailed(object sender, MediaCaptureFailedEventArgs e)
    {
        ConsoleTrace.TraceMethod(this);
        // TODO: Show error when info is added to MediaCaptureFailedEventArgs
        // await Navigator.CurrentOpenPage.DisplayErrorAlert(e...);
    }

    private async void CameraRollButton_Clicked(object sender, EventArgs e)
    {
        await Navigator.Navigation.PopModalAsync();
    }

    public static async Task TryOpenWithPermissionsAsync(CaseloadItem CaseloadItem)
    {
        var status = await DevicePermissions.PromptEnsureCameraAsync();

        if (status == PermissionStatus.Granted)
        {
            TakePhotoView photoView = new() { CaseloadItem = CaseloadItem, };
            await Navigator.Navigation.PushModalAsync(photoView.WrapPageForModal(ViewModalSize.Fullscreen));
        }
        else
        {
            SnackbarHandler.ShowTextWithDetails(
                LocalizedStrings.NoCameraPermissionsPrompt,
                LocalizedStrings.NoCameraPermissionsPrompt,
                LocalizedStrings.NoCameraPermissionsDetailMessage);
        }
    }
}
