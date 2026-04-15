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

    public async Task<ContentView> PopAsync()
    {
        ContentView view = _viewStack.Peek();

        await view.TranslateToAsync(X + view.Width, Y, easing: Easing.CubicInOut);

        _viewStack.Pop();
        _layout.Remove(view);

        return view;
    }
}
