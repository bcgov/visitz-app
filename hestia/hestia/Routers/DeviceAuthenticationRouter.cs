using System;
using hestia.Services;
using hestia.Views;
using hestia.Services.Localization;

namespace hestia.Routers
{
    /// <summary>
    /// Use Router to move to different screens. Router handles navigation between screen.
    /// </summary>
	public class DeviceAuthenticationRouter
    {
        private LocalizeExtension _localizer;

        public DeviceAuthenticationRouter(LocalizeExtension localizer)
        {
            _localizer = localizer;
        }

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
                    _ = Shell.Current.DisplayAlert($"{_localizer.Localize("UnprotectedDevice")}!",
                        _localizer.Localize("SecureDeviceAndTryAgain"), _localizer.Localize("Ok"));
                    break;
                case DeviceAuthenticator.Result.Successful:
                    navigateBack();
                    break;
                case DeviceAuthenticator.Result.Failure:
                    // TODO: Instead of this error modal: using dynamic visibility, show message in ContentPage prompting for auth, show button to open DeviceAuthenticator
                    _ = Shell.Current.DisplayAlert(_localizer.Localize("AccessDenied"),
                        _localizer.Localize("PleaseTryAgain"), _localizer.Localize("Ok"));
                    break;
            }
        }

        private void navigateBack()
        {
            ((AppShell)Shell.Current).
                        GoToAsyncRequest($"..?navigatingBackFromPage={nameof(DeviceAuthenticationPage)}");
        }
    }
}

