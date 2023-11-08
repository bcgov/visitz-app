using Visitz.Pages;

namespace Visitz;

public class Navigator
{
    public static INavigation Navigation => Application.Current.MainPage.Navigation;

    public static Page CurrentOpenPage
    {
        get
        {
            int last = Navigation.NavigationStack.Count - 1;
            return last >= 0 ? Navigation.NavigationStack[last] : null;
        }
    }

    public static Page CurrentOpenModal
    {
        get
        {
            int last = Navigation.ModalStack.Count - 1;
            return last >= 0 ? Navigation.ModalStack[last] : null;
        }
    }

    public static async Task GoToPage<T>(
        Page fromPage = null,
        IDictionary<string, object> parameters = null,
        bool modal = false,
        bool animated = true) where T : ContentPage
    {
        fromPage ??= CurrentOpenPage ?? CurrentOpenModal;

        ConsoleTrace.TraceMethod(typeof(Navigator), $"Navigating from '{fromPage}' to {typeof(T)}");

        var newPage = ServiceProvider.Current.GetRequiredService<T>();

        if (newPage is VisitzPage vPage)
            vPage.Parameters = parameters;

        if (modal)
            await fromPage.Navigation.PushModalAsync(newPage, animated);
        else
            await fromPage.Navigation.PushAsync(newPage, animated);
    }
}
