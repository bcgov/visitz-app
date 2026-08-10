namespace Visitz.Views.Navigation;

public class Tab(string label, Func<ContentView> viewBuilder) : IDisposable
{
    public string Label { get; private set; } = label;

    public Func<ContentView> ViewBuilder { get; private set; } = viewBuilder;

    ContentView? _contentView;
    private bool disposedValue;

    public ContentView ContentView
    {
        get
        {
            TryBuildView();
            return _contentView ?? throw new InvalidOperationException("ContentView should not be null");
        }
    }

    public void TryBuildView()
    {
        _contentView ??= ViewBuilder();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (_contentView is IDisposable disposable)
                    disposable?.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
