namespace Visitz.Device;
public static class DevicePermissions
{
    public async static Task<PermissionStatus> PromptEnsureCameraAsync()
    {
        PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status == PermissionStatus.Unknown)
        {
            status = await Permissions.RequestAsync<Permissions.Camera>();
        }
        return status;
    }
}
