using Syncfusion.Maui.Toolkit.Shimmer;
using Visitz.Views.BaseClasses;
using VisitzModel.Utilities;

namespace Visitz.Views.Navigation;

#nullable enable

/// <summary>
/// Provides support for asynchronous lazy initialization of BaseContentViews. Loads once per BindingContext change.
/// </summary>
/// <typeparam name="T"></typeparam>
public partial class AsyncLazyContentView<T> : ContentView, IDisposable
    where T : BaseContentView
{
    readonly AsyncLazy<T> _lazyInit;

    BaseContentView? BaseContentView { get; set; }

    readonly SfShimmer _shimmer = new() { Type = ShimmerType.Feed };

    bool loaded;

    public AsyncLazyContentView(AsyncLazy<T> lazy)
    {
        _lazyInit = lazy;

        Content = _shimmer;
    }

    protected override async void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (!loaded)
        {
            await LoadContent();
            loaded = true;
        }
    }

    async Task LoadContent()
    {
        BaseContentView = await _lazyInit.Value;

        await BaseContentView.StartInitAsync();

        _shimmer.Content = BaseContentView;
    }

    private bool disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
                BaseContentView?.Dispose();

            disposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
