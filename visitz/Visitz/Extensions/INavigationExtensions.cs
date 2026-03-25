namespace Visitz.Extensions;

public static class INavigationExtensions
{
    public static async Task PushModalAsync(
        this INavigation navigation,
        ContentView contentView,
        ViewModalSize size = ViewModalSize.Wide,
        bool animated = true
    )
    {
        await navigation.PushModalAsync(contentView.WrapPageForModal(size), animated);
    }

    public static async Task PushAsync(
        this INavigation navigation,
        ContentView contentView,
        ViewModalSize size = ViewModalSize.Fullscreen,
        bool animated = true
    )
    {
        await navigation.PushAsync(contentView.WrapPageForModal(size), animated);
    }
}
