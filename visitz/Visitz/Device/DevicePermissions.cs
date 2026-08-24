using Microsoft.Extensions.Logging;

namespace Visitz.Device;

#if WINDOWS
using Visitz.Extensions;
using Windows.Media.Capture;
using Windows.Security.Authorization.AppCapabilityAccess;
#endif

public class DevicePermissions
{
    public static ILogger Logger { get; set; } = ServiceProvider.GetService<ILogger<DevicePermissions>>();

    public static async Task<PermissionStatus> PromptEnsureCameraAsync()
    {
        //Checking camera access for non Window devices
        PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Camera>();

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
        catch (Exception ex)
        {
            // Other error (camera missing, in use, etc.)
            status = PermissionStatus.Unknown;
            Logger.LogException(ex, $"Error using camera/microphone: {ex.Message}");
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
