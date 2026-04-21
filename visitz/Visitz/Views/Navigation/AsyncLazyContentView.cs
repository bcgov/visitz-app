using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Shimmer;
using Visitz.Extensions;
using Visitz.Resources.Styles;
using Visitz.Views.BaseClasses;
using VisitzModel.Utilities;

namespace Visitz.Views.Navigation;

#nullable enable

/// <summary>
/// Provides support for asynchronous lazy initialization of BaseContentViews. Loads once per BindingContext change.
/// </summary>
/// <typeparam name="TLazyBaseContentView"></typeparam>
public partial class AsyncLazyContentView<TLazyBaseContentView> : ContentView, IDisposable, ILoadAsync
    where TLazyBaseContentView : BaseContentView
{
    readonly SemaphoreSlim? _semaphore;
    readonly AsyncLazy<TLazyBaseContentView> _lazyInit;
    readonly bool _loadOnBindingContextChanged;

    BaseContentView? BaseContentView { get; set; }

    readonly SfShimmer _DefaultShimmer = new()
    {
        Type = ShimmerType.Feed,
        Margin = VisitzDimensions.DefaultMargin,
        MaximumHeightRequest = 300,
        MaximumWidthRequest = 300,
    };

    bool loaded;

    public AsyncLazyContentView(
        AsyncLazy<TLazyBaseContentView> lazy,
        View? loadingView = null,
        SemaphoreSlim? semaphore = null,
        bool loadOnBindingContextChanged = true
    )
    {
        _lazyInit = lazy;
        _semaphore = semaphore;
        _loadOnBindingContextChanged = loadOnBindingContextChanged;

        ControlTemplate = new() { LoadTemplate = () => loadingView ?? _DefaultShimmer };
    }

    protected override async void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        try
        {
            if (_loadOnBindingContextChanged)
                await LoadAsync();
        }
        catch (Exception ex)
        {
            try
            {
                ServiceProvider
                    .GetService<ILogger<AsyncLazyContentView<TLazyBaseContentView>>>()
                    .LogError(ex, ex.Message);
            }
            catch { }
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
        }
    }

    public async Task LoadAsync()
    {
        if (_semaphore != null)
            await _semaphore.WaitAsync();

        if (loaded)
            return;

        try
        {
            BaseContentView = await _lazyInit.Value;
        }
        finally
        {
            try
            {
                _semaphore?.Release();
            }
            catch { }
        }

        await BaseContentView.StartInitAsync();

        ControlTemplate = null;
        Content = BaseContentView;

        BaseContentView.Opacity = 0.0d;
        await BaseContentView.FadeToAsync(1.0d);

        loaded = true;
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
