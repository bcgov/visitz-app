using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Oidc;
using System.Diagnostics;
using System.Net;
using Visitz.Controls;
using Visitz.Resources.Localization;
using Visitz.Settings;

namespace Visitz.Pages;

public partial class WebViewPage
{
	const string _logoutResponse = "/logout_response";

	readonly Uri _baseRedirectUri = new(new AppSettings().Oidc.RedirectUri);

	Action SessionAction { get; set; }

	partial void Setup()
	{
		MainWebView.Loaded += MainWebView_Loaded;
		CloseButton.Closing += CloseButton_Closing;
	}

	private static async Task<CoreWebView2> GetCoreWebView(WebView webView)
	{
		var winWebView = webView.Handler.PlatformView as WebView2;

		await winWebView.EnsureCoreWebView2Async();

		if (winWebView.CoreWebView2.Settings is CoreWebView2Settings settings)
		{
			settings.IsZoomControlEnabled = false;
#if DEBUG
			settings.AreDevToolsEnabled = true;
#else
			settings.AreDevToolsEnabled = false;
#endif
		}

		return winWebView.CoreWebView2;
	}

	private async void MainWebView_Loaded(object sender, EventArgs e)
	{
		var webView = sender as WebView;
		var coreWebView = await GetCoreWebView(webView);

		coreWebView.NavigationCompleted += CompleteRedirectNavigation;
		coreWebView.NavigationStarting += (sender, args) =>
		{
			if (args.Uri.StartsWith(_baseRedirectUri.Scheme, StringComparison.InvariantCultureIgnoreCase))
			{
				SessionAction = async () => await PerformCustomSchemeRedirect(args.Uri);
			}
			else if (args.Uri.Contains(_logoutResponse, StringComparison.InvariantCultureIgnoreCase))
			{
				SessionAction = async () => await ForceLogout(sender);
			}
		};

		coreWebView.ProcessFailed += async (_, args) =>
		{
			await DisplayAlert(LocalizedStrings.Error,
				args.Reason + "\n\n" + args.ProcessDescription,
				LocalizedStrings.Ok);

			await Navigator.Navigation.PopModalAsync();
		};

		webView.Source = ViewModel.AuthUri;
	}

	public void CompleteRedirectNavigation(CoreWebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
	{
		if (SessionAction != null && args.HttpStatusCode >= ((int)HttpStatusCode.InternalServerError))
		{
			SessionAction();
			SessionAction = null;
		}
	}

	private static async Task PerformCustomSchemeRedirect(string uri)
	{
		Process.Start(new ProcessStartInfo
		{
			FileName = uri,
			UseShellExecute = true,
		});

		await Navigator.Navigation.PopModalAsync();
	}

	// A workaround implementation to forcibly logout the user on Windows.
	// This, along with the rest of the Windows OIDC workarounds, may not
	// be needed once https://github.com/microsoft/WindowsAppSDK/issues/441
	// is fixed.
	private async Task ForceLogout(CoreWebView2 coreWebView)
	{
		coreWebView.CookieManager.DeleteAllCookies();

		await CancelTokenSource?.CancelAsync();
		await OidcSession.LocalLogoutAsync();
		await Navigator.Navigation.PopModalAsync();
	}

	private void CloseButton_Closing(object sender, ClosingEventArgs e)
	{
		CancelTokenSource?.Cancel();
	}
}
