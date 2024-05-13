#if IOS || MACCATALYST
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
#endif

namespace Visitz.Extensions;

public static class ContentViewExtensions
{
    public static readonly double DefaultHeight = 700;

	public static readonly double Unset = -1;
	public static readonly double MediumWidth = 600;
	public static readonly double WideWidth = 850;

    public static ContentPage WrapPageForModal(this ContentView contentView, ViewModalSize size = ViewModalSize.Wide)
    {
        var page = new ContentPage()
        {
            Background = Colors.Transparent,
            Content = contentView,
        };
#if IOS
		var presentationStyle = size switch
		{
			ViewModalSize.Medium => UIModalPresentationStyle.FormSheet,
			ViewModalSize.Fullscreen => UIModalPresentationStyle.FullScreen,
			_ => UIModalPresentationStyle.PageSheet,
		};

		page.On<iOS>().SetModalPresentationStyle(presentationStyle);
#else
		page.HeightRequest = size == ViewModalSize.Fullscreen ? Unset : DefaultHeight ;

		page.WidthRequest = size switch
		{
			ViewModalSize.Medium => MediumWidth,
			ViewModalSize.Fullscreen => Unset,
			_ => WideWidth,
		};
#endif

		return page;
    }
}
