namespace Visitz.Extensions;

public static class INavigationExtensions
{
    public static async Task PushModalAsync(
        this INavigation navigation,
        ContentView contentView,
        ViewModalSize size = ViewModalSize.Wide,
        bool animated = true)
    {
        await navigation.PushModalAsync(contentView.WrapPageForModal(size), animated);
    }
}
