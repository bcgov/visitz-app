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
/// <typeparam name="T"></typeparam>
public partial class AsyncLazyContentView<T> : ContentView, IDisposable
    where T : BaseContentView
{
    readonly SemaphoreSlim _semaphore = new(1);
    readonly AsyncLazy<T> _lazyInit;

    BaseContentView? BaseContentView { get; set; }

    readonly SfShimmer _DefaultShimmer = new()
    {
        Type = ShimmerType.Feed,
        Margin = VisitzDimensions.DefaultMargin,
        MaximumHeightRequest = 300,
        MaximumWidthRequest = 300,
    };

    bool loaded;

    public AsyncLazyContentView(AsyncLazy<T> lazy, View? loadingView = null)
    {
        _lazyInit = lazy;

        ControlTemplate = new() { LoadTemplate = () => loadingView ?? _DefaultShimmer };

        Loaded += AsyncLazyContentView_Loaded;
    }

    private async void AsyncLazyContentView_Loaded(object? sender, EventArgs e) { }

    protected override async void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        try
        {
            await LoadContent();
        }
        catch (Exception ex)
        {
            try
            {
                ServiceProvider.GetService<ILogger<AsyncLazyContentView<T>>>().LogError(ex, ex.Message);
            }
            catch { }
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
        }
    }

    async Task LoadContent()
    {
        await _semaphore.WaitAsync();

        try
        {
            if (loaded)
                return;

            BaseContentView = await _lazyInit.Value;

            await BaseContentView.StartInitAsync();

            ControlTemplate = null;
            Content = BaseContentView;

            BaseContentView.Opacity = 0.0d;
            await BaseContentView.FadeToAsync(1.0d);

            loaded = true;
        }
        finally
        {
            try
            {
                _semaphore.Release();
            }
            catch { }
        }
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
