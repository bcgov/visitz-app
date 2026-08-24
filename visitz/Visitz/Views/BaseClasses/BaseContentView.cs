using Microsoft.Extensions.Logging;
using Visitz.Extensions;
using Visitz.Views.Entity;

namespace Visitz.Views.BaseClasses;

public abstract class BaseContentView : ContentView, IDisposable, IAsyncInitialize
{
    private bool _disposedValue;

    private bool loaded;

    protected ILogger Logger { get; }

    public Task? InitTask { get; private set; }

    public string Title { get; protected set; } = "";

    public BaseContentView(string title = "")
    {
        Logger = MakeLogger();
        Title = title;

        Loaded += BaseContentView_Loaded;
    }

    protected virtual ILogger MakeLogger()
    {
        return ServiceProvider.GetService<ILoggerFactory>().CreateLogger(GetType());
    }

    protected override async void OnHandlerChanging(HandlerChangingEventArgs args)
    {
        base.OnHandlerChanging(args);

        if (args.AttachingToHandler())
        {
            try
            {
                await StartInitAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
                await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
                throw;
            }
        }
        else if (args.DetachingFromHandler())
            Dispose();
    }

    private async void BaseContentView_Loaded(object? sender, EventArgs e)
    {
        try
        {
            if (!loaded)
            {
                await OnFirstLoadAsync();
                loaded = true;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message, ex);
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
            throw;
        }
    }

    protected virtual Task OnFirstLoadAsync()
    {
        Logger.TraceMethod(this);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual Task InitAsync()
    {
        Logger.TraceMethod(this);

        return Task.CompletedTask;
    }

    public async Task StartInitAsync()
    {
        await (InitTask ??= InitAsync());
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
            return;

        if (disposing)
            Logger.TraceMethod(this);

        _disposedValue = true;
    }
}
