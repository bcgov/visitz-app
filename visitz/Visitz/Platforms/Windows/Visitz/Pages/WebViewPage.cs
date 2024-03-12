using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using Visitz.Controls;
using Visitz.Resources.Localization;
using Visitz.Settings;

namespace Visitz.Pages;

public partial class WebViewPage
{
	readonly Uri _baseRedirectUri = new(new AppSettings().Oidc.RedirectUri);

	string _redirectUri;

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

		coreWebView.NavigationStarting += (sender, args) =>
		{
			if (args.Uri.StartsWith(_baseRedirectUri.Scheme))
			{
				_redirectUri = args.Uri;
				sender.NavigationCompleted += CompleteRedirectNavigation;
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

	public void CompleteRedirectNavigation(CoreWebView2 sender, CoreWebView2NavigationCompletedEventArgs _args)
	{
		Process.Start(new ProcessStartInfo
		{
			FileName = _redirectUri,
			UseShellExecute = true,
		});

		_ = Navigator.Navigation.PopModalAsync();

		sender.NavigationCompleted -= CompleteRedirectNavigation;
	}

	private void CloseButton_Closing(object sender, ClosingEventArgs e)
	{
		CancelTokenSource?.Cancel();
	}
}
