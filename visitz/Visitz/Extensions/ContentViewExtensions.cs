#if IOS || MACCATALYST
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
#endif

namespace Visitz.Extensions;

public static class ContentViewExtensions
{
    public static readonly double Height = 700;
    
    public static readonly double Width = 850;

    public static ContentPage WrapPageForModal(this ContentView contentView)
    {
        var page = new ContentPage()
        {
            Background = Colors.Transparent,
            Content = contentView,
            HeightRequest = Height,
            WidthRequest = Width,
        };

#if IOS
        page.On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.PageSheet);
#endif

        return page;
    }
}
