namespace Visitz.Extensions;

public static class IViewExtensions
{
    public static IEnumerable<IDisposable> FindDisposables(this IView view)
    {
        List<IDisposable> disposables = [];

        if (view is IDisposable disposableView)
            disposables.Add(disposableView);

        if (view is Layout layout)
            foreach (var child in layout.Children)
                if (child is IDisposable disposable)
                    disposables.Add(disposable);
                else
                    return FindDisposables(view).Concat(disposables);

        return disposables;
    }
}
