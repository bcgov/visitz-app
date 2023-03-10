using System;
using hestia.Services;
using hestia.Views;

namespace hestia.Routers
{
    /// <summary>
    /// Use Router to move to different screens. Router handles navigation between screen.
    /// </summary>
	public class DeviceAuthenticationRouter
    {
        public void RouteUsing(DeviceAuthenticator.Result result)
        {
            if (!MainThread.IsMainThread)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    RouteUsing(result);
                });
                return;
            }
            switch (result)
            {
                case DeviceAuthenticator.Result.NotConfigured:
                    _ = Shell.Current.DisplayAlert("Unprotected Device!",
                        "Please secure your device by setting up device lock and try again.", "Ok");
                    break;
                case DeviceAuthenticator.Result.Successful:
                    ((AppShell)Shell.Current).
                        GoToAsyncRequest($"..?navigatingBackFromPage={nameof(DeviceAuthenticationPage)}");
                    break;
                case DeviceAuthenticator.Result.Failure:
                    _ = Shell.Current.DisplayAlert("Access Denied",
                        "Please try again.", "Ok");
                    break;
            }
        }
    }
}

