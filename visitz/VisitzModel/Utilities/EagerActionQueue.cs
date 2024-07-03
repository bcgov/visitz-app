using System.Collections.Concurrent;

namespace VisitzModel.Utilities;

public class EagerActionQueue(CancellationToken? cancellationToken = null)
{
	readonly CancellationToken cancelToken = cancellationToken ?? CancellationToken.None;
	readonly ConcurrentQueue<Task> taskQueue = new();
	Task writeFromQueue;

	public bool HasCompleted => writeFromQueue?.IsCompleted ?? true;

	public Task EnqueueAsync(Action action)
	{
		Task task = new(action, TaskCreationOptions.PreferFairness);

		taskQueue.Enqueue(task);

		if (HasCompleted)
			writeFromQueue = CreateWriteFromQueueTaskAsync();

		return task;
	}

	async Task CreateWriteFromQueueTaskAsync()
	{
		while (!taskQueue.IsEmpty)
		{
			cancelToken.ThrowIfCancellationRequested();

			if (taskQueue.TryDequeue(out Task task))
			{
				task.Start();
				await task.WaitAsync(cancelToken);
			}
		}
	}
}
