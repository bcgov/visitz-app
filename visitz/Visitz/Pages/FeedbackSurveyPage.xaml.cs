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
}
