using VisitzModel;

namespace Visitz;

public class Navigator
{
    public static INavigation Navigation =>
        Application.Current?.Windows[0].Page?.Navigation
        ?? throw new InvalidOperationException(nameof(Navigator) + ": main window is null");

    public static Page CurrentOpenPage
    {
        get
        {
            int last = Navigation.NavigationStack.Count - 1;
            return last >= 0 ? Navigation.NavigationStack[last] : throw new InvalidOperationException("No pages open");
        }
    }

    public static Page? CurrentOpenModal
    {
        get
        {
            int last = Navigation.ModalStack.Count - 1;
            return last >= 0 ? Navigation.ModalStack[last] : null;
        }
    }

    public static async Task GoToPage<T>(Page? fromPage = null, bool modal = false, bool animated = true)
        where T : ContentPage
    {
        fromPage ??= CurrentOpenPage ?? CurrentOpenModal ?? throw new ArgumentNullException(nameof(fromPage));

        ConsoleTrace.TraceMethod(typeof(Navigator), $"Navigating from '{fromPage}' to {typeof(T)}");

        var newPage = ServiceProvider.Current.GetRequiredService<T>();

        if (modal)
            await fromPage.Navigation.PushModalAsync(newPage, animated);
        else
            await fromPage.Navigation.PushAsync(newPage, animated);
    }

    public static async Task PopAllModalsAsync(bool animated)
    {
        while (Navigation.ModalStack.Count > 0)
            await Navigation.PopModalAsync(animated);
    }
}
