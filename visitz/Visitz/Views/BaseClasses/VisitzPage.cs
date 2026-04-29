using Microsoft.Extensions.Logging;
using Visitz.Extensions;
using VisitzModel;
#if IOS
using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
#endif

namespace Visitz.Views.BaseClasses;

#nullable enable

public partial class VisitzPage<TView, TViewModel> : ContentPage, IDisposable
    where TView : ContentPage
    where TViewModel : VisitzViewModel
{
    bool _disposed;

    protected virtual ILogger<TView> Logger { get; set; }

    protected TViewModel ViewModel { get; set; }

    protected Task? ViewModelInit { get; private set; }

    protected Window CurrentWindow => Window ?? GetParentWindow();

    public VisitzPage(TViewModel visitzViewModel, ILogger<TView> logger)
        : base()
    {
        Logger = logger;
        ViewModel = visitzViewModel;

        NavigationPage.SetHasBackButton(this, false);
        NavigationPage.SetHasNavigationBar(this, false);
    }

    protected override async void OnParentChanging(ParentChangingEventArgs args)
    {
        base.OnParentChanging(args);

        var isCreating = args.AttachingToParent();
        var isDestroying = args.DetachingFromParent();

        try
        {
            if (isCreating)
                OnCreated();
            else if (isDestroying)
                Dispose();
        }
        catch (Exception ex)
        {
            await this.DisplayErrorAlert(ex);
            throw;
        }
    }

    protected virtual void OnCreated()
    {
        Logger.TraceMethod(this);

        ViewModelInit = ViewModel.StartInitAsync();
    }

    protected virtual void OnDestroyed()
    {
        Logger.TraceMethod(this);

        Behaviors.Clear();

        ViewModel.Dispose();

        if (Content.FindFirstDisposable() is IDisposable disposable)
            disposable.Dispose();
    }

    protected override bool OnBackButtonPressed()
    {
        ConsoleTrace.TraceMethod(this);
        return base.OnBackButtonPressed();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                OnDestroyed();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
