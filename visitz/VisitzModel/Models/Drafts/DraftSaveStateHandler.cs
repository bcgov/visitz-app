using VisitzModel.Events;
using VisitzModel.Utilities;

namespace VisitzModel.Models.Drafts;

public class DraftSaveStateHandler(TimeSpan? delay = null) : IDisposable
{
    Debouncer? _debouncer = new(delay ?? Debouncer.AvgStoppedTypingDelay);
    bool _disposed;

    public event EventHandler<DraftSaveStatusEventArgs>? SaveStateChanged;

    void UpdateState(DraftSaveState newState)
    {
        SaveStateChanged?.Invoke(this, new DraftSaveStatusEventArgs(newState));
    }

    public void Clear()
    {
        _debouncer?.Cancel();
        UpdateState(DraftSaveState.None);
    }

    public async Task Saving(bool changeToSavedAfterDelay = true)
    {
        UpdateState(DraftSaveState.Saving);

        if (changeToSavedAfterDelay && _debouncer != null)
            await _debouncer.Debounce(() => UpdateState(DraftSaveState.Saved));
    }

    public void Saved()
    {
        _debouncer?.Cancel();
        UpdateState(DraftSaveState.Saved);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _debouncer?.Dispose();
                _debouncer = null;
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
