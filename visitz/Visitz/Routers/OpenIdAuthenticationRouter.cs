using Visitz.Views;
using Visitz.ViewModels;
using Visitz.Services.Localization;

namespace Visitz.Routers
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
                    ((VisitzShell)Shell.Current).
                        GoToAsyncRequest($"..?navigatingBackFromPage={nameof(OpenIdAuthenticationPage)}");
                }
            });
        }
    }
}

