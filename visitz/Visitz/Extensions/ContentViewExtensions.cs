#if IOS || MACCATALYST
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
#endif

namespace Visitz.Extensions;

public static partial class ContentViewExtensions
{
    public static readonly double Height = 700;

	public static readonly double MediumWidth = 600;
	public static readonly double WideWidth = 850;

    public static ContentPage WrapPageForModal(this ContentView contentView, ViewModalSize size = ViewModalSize.Wide)
    {
        var page = new ContentPage()
        {
            Background = Colors.Transparent,
            Content = contentView,
            HeightRequest = Height,
        };
#if IOS
		var presentationStyle = size switch
		{
			ViewModalSize.Medium => UIModalPresentationStyle.FormSheet,
			_ => UIModalPresentationStyle.PageSheet,
		};

		page.On<iOS>().SetModalPresentationStyle(presentationStyle);
#else
		page.WidthRequest = size switch
		{
			ViewModalSize.Medium => MediumWidth,
			_ => WideWidth,
		};
#endif

		return page;
    }
}
