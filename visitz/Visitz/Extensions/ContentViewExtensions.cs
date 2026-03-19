using Visitz.Views.BaseClasses;
using MauiNavigationPage = Microsoft.Maui.Controls.NavigationPage;
#if IOS || MACCATALYST
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
#endif

#if WINDOWS
using Visitz.Resources.Styles;
using Microsoft.Maui.Layouts;
#endif

namespace Visitz.Extensions;

public static class ContentViewExtensions
{
#if WINDOWS
    public static readonly double DefaultHeight = 0.90;

    public static readonly double Fullscreen = 1.0;
    public static readonly double MediumWidth = 0.50;
    public static readonly double WideWidth = 0.72;
#endif

    public static ContentPage WrapPageForModal(this ContentView contentView, ViewModalSize size = ViewModalSize.Wide)
    {
        var page = new VisitzPage(null) { Background = Colors.Transparent };

        MauiNavigationPage.SetHasNavigationBar(page, false);
        MauiNavigationPage.SetHasBackButton(page, false);

#if IOS || MACCATALYST
        var presentationStyle = size switch
        {
            ViewModalSize.Medium => UIModalPresentationStyle.FormSheet,
            ViewModalSize.Fullscreen => UIModalPresentationStyle.FullScreen,
            _ => UIModalPresentationStyle.PageSheet,
        };

        page.Content = contentView;
        page.On<iOS>().SetModalPresentationStyle(presentationStyle);
#elif WINDOWS
        var wrapper = new AbsoluteLayout { contentView };

        page.Content = wrapper;

        contentView.HorizontalOptions = LayoutOptions.Fill;
        contentView.VerticalOptions = LayoutOptions.Fill;
        AbsoluteLayout.SetLayoutFlags(contentView, AbsoluteLayoutFlags.All);

        if (size != ViewModalSize.Fullscreen)
            contentView.Shadow = VisitzShadows.Level5;

        double middle = 0.5;
        double width = size switch
        {
            ViewModalSize.Medium => MediumWidth,
            ViewModalSize.Fullscreen => Fullscreen,
            _ => WideWidth,
        };
        double height = size == ViewModalSize.Fullscreen ? Fullscreen : DefaultHeight;

        AbsoluteLayout.SetLayoutBounds(contentView, new Rect(middle, middle, width, height));
#endif

        return page;
    }
}
