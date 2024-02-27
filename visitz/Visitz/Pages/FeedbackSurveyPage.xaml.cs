using Visitz.Settings;

#if IOS
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
#endif

namespace Visitz.Pages;

public partial class FeedbackSurveyPage : ContentPage
{
	public FeedbackSurveyPage()
	{
#if IOS
		On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.FormSheet);
#endif

		InitializeComponent();
	}

	private async void StartSurvey_Clicked(object sender, EventArgs e)
	{
		var feedbackUri = new Uri(new AppSettings().ContactInfo.FeedbackSurveyUrl);

		await Navigator.Navigation.PopModalAsync();

		await Browser.Default.OpenAsync(feedbackUri, new BrowserLaunchOptions
		{
			LaunchMode = BrowserLaunchMode.SystemPreferred,
			TitleMode = BrowserTitleMode.Hide,
			Flags = BrowserLaunchFlags.PresentAsFormSheet,
		});
	}
}
