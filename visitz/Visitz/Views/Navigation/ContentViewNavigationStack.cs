namespace Visitz.Views.Navigation;

#nullable enable

internal partial class ContentViewNavigationStack : ContentView
{
    readonly Stack<ContentView> _viewStack = new();

    readonly AbsoluteLayout _layout = [];

    public ContentViewNavigationStack()
    {
        Content = _layout;
    }

    public async Task PushAsync(ContentView newView)
    {
        _viewStack.Push(newView);
        _layout.Add(newView);

        AbsoluteLayout.SetLayoutBounds(newView, new Rect(0, 0, 1, 1));
        AbsoluteLayout.SetLayoutFlags(newView, Microsoft.Maui.Layouts.AbsoluteLayoutFlags.All);

        newView.TranslationX = X + Width;

        await newView.TranslateToAsync(X, Y, easing: Easing.CubicInOut);
    }

    public async Task<ContentView?> PopAsync()
    {
        if (_viewStack.TryPop(out ContentView? view) && view != null)
        {
            await view.TranslateToAsync(X + view.Width, Y, easing: Easing.CubicInOut);

            _layout.Remove(view);

            if (view is IDisposable disposable)
                disposable.Dispose();
        }

        return view;
    }
}
