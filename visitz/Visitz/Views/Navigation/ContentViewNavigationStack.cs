namespace Visitz.Views.Navigation;

#nullable enable

internal partial class ContentViewNavigationStack : ContentView
{
    readonly Stack<ContentView> _viewStack = new();

    public async Task PushAsync(ContentView newView)
    {
        _viewStack.Push(newView);

        newView.TranslationX = X + Width;

        await newView.TranslateToAsync(X, Y, easing: Easing.CubicInOut);
    }

    public async Task<ContentView> PopAsync()
    {
        ContentView view = _viewStack.Peek();

        await view.TranslateToAsync(X + view.Width, Y, easing: Easing.CubicInOut);

        return _viewStack.Pop();
    }
}
