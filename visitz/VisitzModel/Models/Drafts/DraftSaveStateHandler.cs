using VisitzModel.Events;
using VisitzModel.Utilities;

namespace VisitzModel.Models.Drafts;

public class DraftSaveStateHandler(TimeSpan? delay = null)
{
    readonly Debouncer _debouncer = new(delay ?? Debouncer.AvgStoppedTypingDelay);

    public event EventHandler<DraftSaveStatusEventArgs> SaveStateChanged;

    void UpdateState(DraftSaveState newState)
    {
        SaveStateChanged?.Invoke(this, new DraftSaveStatusEventArgs(newState));
    }

    public void Clear()
    {
        _debouncer.Cancel();
        UpdateState(DraftSaveState.None);
    }

    public async Task Saving(bool changeToSavedAfterDelay = true)
    {
        UpdateState(DraftSaveState.Saving);

        if (changeToSavedAfterDelay)
            await _debouncer.Debounce(() => UpdateState(DraftSaveState.Saved));
    }

    public void Saved()
    {
        _debouncer.Cancel();
        UpdateState(DraftSaveState.Saved);
    }
}
