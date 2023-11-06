using Visitz.Pages;

namespace Visitz;

public class Navigator
{
    public static async Task GoToPage<T>(
    Page fromPage = null,
    IDictionary<string, object> parameters = null,
    bool modal = false,
    bool animated = true) where T : VisitzPage
    {
        fromPage ??= VisitzApp.CurrentOpenPage ?? VisitzApp.CurrentOpenModal;

        ConsoleTrace.TraceMethod(typeof(Navigator), $"Navigating from '{fromPage}' to {typeof(T)}");

        var newPage = ServiceProvider.Current.GetRequiredService<T>();

        newPage.Parameters = parameters;

        if (modal)
            await fromPage.Navigation.PushModalAsync(newPage, animated);
        else
            await fromPage.Navigation.PushAsync(newPage, animated);
    }
}
