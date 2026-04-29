using System.Collections.Concurrent;

namespace VisitzModel.Utilities;

public class EagerActionQueue(CancellationToken? cancellationToken = null)
{
    readonly CancellationToken cancelToken = cancellationToken ?? CancellationToken.None;
    readonly ConcurrentQueue<(TaskCompletionSource, Func<Task>)> taskQueue = new();
    Task? writeFromQueue;

    public bool HasCompleted => writeFromQueue?.IsCompleted ?? true;

    public Task EnqueueAsync(Func<Task> task)
    {
        TaskCompletionSource tcs = new();

        taskQueue.Enqueue((tcs, task));

        if (HasCompleted)
            writeFromQueue = CreateWriteFromQueueTaskAsync();

        return tcs.Task;
    }

    async Task CreateWriteFromQueueTaskAsync()
    {
        while (!taskQueue.IsEmpty)
        {
            cancelToken.ThrowIfCancellationRequested();

            if (taskQueue.TryDequeue(out (TaskCompletionSource, Func<Task>) tuple))
            {
                var (tcs, task) = tuple;

                try
                {
                    await task();
                    tcs.TrySetResult();
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }
        }
    }
}
