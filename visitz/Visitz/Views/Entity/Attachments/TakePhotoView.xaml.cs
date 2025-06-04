using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using Visitz.Animations;
using Visitz.Device;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using Visitz.Views.Snackbar;
using VisitzModel;
using VisitzModel.Interfaces;
using VisitzModel.Models.Caseload;

namespace Visitz.Views.Entity.Attachments;

public partial class TakePhotoView : ViewModelContentView, IBusinessObjectHolder
{
    readonly VisibilityAnimation SnapshotFade = new(showView: false);

    new TakePhotoViewModel ViewModel => base.ViewModel as TakePhotoViewModel;

    public IBusinessObject BusinessObject
    {
        get => ViewModel.BusinessObject;
        set => ViewModel.BusinessObject = value;
    }

    public TakePhotoView() : base(ServiceProvider.GetService<TakePhotoViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        Unloaded += TakePhotoView_Unloaded;
        Camera.MediaCaptured += Camera_MediaCaptured;
        Camera.MediaCaptureFailed += Camera_MediaCaptureFailed;

        await InitCamera();
    }

    bool disposed;
    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            Camera.StopCameraPreview();
            Camera.Handler.DisconnectHandler();

            Camera.MediaCaptured -= Camera_MediaCaptured;
            Camera.MediaCaptureFailed -= Camera_MediaCaptureFailed;

            ViewModel.Dispose();

            Unloaded -= TakePhotoView_Unloaded;

            disposed = true;
        }
        base.Dispose(disposing);
    }

    private void TakePhotoView_Unloaded(object sender, EventArgs e)
    {
        Dispose();
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

    public static async Task TryOpenWithPermissionsAsync(IBusinessObject businessObject)
    {
        var status = await DevicePermissions.PromptEnsureCameraAsync();

        if (status == PermissionStatus.Granted)
        {
            TakePhotoView photoView = new() { BusinessObject = businessObject, };
            await Navigator.Navigation.PushModalAsync(photoView, ViewModalSize.Fullscreen);
        }
        else
        {
            SnackbarHandler.ShowTextWithDetails(
                LocalizedStrings.NoCameraPermissionsPrompt,
                LocalizedStrings.NoCameraPermissionsPrompt,
                LocalizedStrings.NoCameraPermissionsDetailMessage);
        }
    }

    [RelayCommand]
    public async Task TakePicture()
    {
        try
        {
            _ = AnimateSnapshotAsync();
            CameraRollButton.IsEnabled = false;
            await Camera.CaptureImage(CancellationToken.None);
        }
        catch (Exception ex)
        {
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
            ConsoleTrace.TraceMethod(this, ex);
        }
        finally
        {
            CameraRollButton.IsEnabled = true;
        }
    }
}
