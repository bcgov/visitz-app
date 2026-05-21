/*
    Code adapted from Ionix's StackOverflow answer: https://stackoverflow.com/a/66491564
 */

namespace VisitzModel.Utilities;

public sealed partial class Debouncer(TimeSpan? delay) : IDisposable
{
    public static readonly TimeSpan AvgStoppedTypingDelay = TimeSpan.FromMilliseconds(700);

    private readonly TimeSpan _delay = delay ?? TimeSpan.FromSeconds(2);
    private CancellationTokenSource? previousCancellationToken = null;

    public async Task Debounce(Action action)
    {
        _ = action ?? throw new ArgumentNullException(nameof(action));
        Cancel();
        previousCancellationToken = new CancellationTokenSource();
        try
        {
            await Task.Delay(_delay, previousCancellationToken.Token);
            await Task.Run(action, previousCancellationToken.Token);
        }
        catch { } // can swallow exception as nothing more to do if task cancelled/token disposed
    }

    public void Cancel()
    {
        if (previousCancellationToken == null || previousCancellationToken.IsCancellationRequested)
            return;

        try
        {
            previousCancellationToken.Cancel();
            previousCancellationToken.Dispose();
        }
        catch { } // discard exception, nothing to handle
    }

    public void Dispose() => Cancel();
}
