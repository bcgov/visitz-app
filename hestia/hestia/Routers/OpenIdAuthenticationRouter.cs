using System;
using hestia.Views;
using hestia.ViewModels;

namespace hestia.Routers
{
    /// <summary>
    /// Use Router to move to different screens. Router handles navigation between screen.
    /// </summary>
	public class OpenIdAuthenticationRouter
	{
		public void routeUsing(OpenIdAuthenticationViewModel.Result result)
		{
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (result.IsError)
                {
                    _ = Shell.Current.DisplayAlert("Login Error!",
                        result.ErrorDescription, "Ok");
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

