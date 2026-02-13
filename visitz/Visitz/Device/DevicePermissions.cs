namespace Visitz.Device;
#if WINDOWS
using Windows.Security.Authorization.AppCapabilityAccess;
using Windows.Media.Capture;
#endif

public static class DevicePermissions
{
    public async static Task<PermissionStatus> PromptEnsureCameraAsync()
    {
        //Checking camera access for non Window devices
        PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Camera>();

        //Checking camera and microphone access for Window devices
        #if WINDOWS
        try
        {
            var mediaCapture = new MediaCapture();

            //Configuring both audio and video to check microphone and camera access
            var settings = new MediaCaptureInitializationSettings
            {
                StreamingCaptureMode = StreamingCaptureMode.AudioAndVideo,
            };

            await mediaCapture.InitializeAsync(settings);
            status = PermissionStatus.Granted;
        }
        catch (UnauthorizedAccessException)
        {
            status = PermissionStatus.Denied;
        }
        catch
        {
            // Other error (camera missing, in use, etc.)
            status = PermissionStatus.Unknown;
        }
        #else

        if (status == PermissionStatus.Unknown)
        {
            status = await Permissions.RequestAsync<Permissions.Camera>();
        }
        #endif

        return status;
    }
}
