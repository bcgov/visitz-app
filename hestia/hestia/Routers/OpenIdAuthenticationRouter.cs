using System;
using visitz.Views;
using visitz.ViewModels;
using visitz.Services.Localization;

namespace visitz.Routers
{
    /// <summary>
    /// Use Router to move to different screens. Router handles navigation between screen.
    /// </summary>
	public class OpenIdAuthenticationRouter
    {
        private LocalizeExtension _localizer;

        public OpenIdAuthenticationRouter(LocalizeExtension localizer)
        {
            _localizer = localizer;
        }

        public void routeUsing(OpenIdAuthenticationViewModel.Result result)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (result.IsError)
                {
                    _ = Shell.Current.DisplayAlert($"{_localizer.Localize("LoginError")}!",
                        result.ErrorDescription, _localizer.Localize("Ok"));
                }
                else
                {
                    ((AppShell)Shell.Current).
                        GoToAsyncRequest($"..?navigatingBackFromPage={nameof(OpenIdAuthenticationPage)}");
                }
            });
        }
    }
}

