using System.Collections.Concurrent;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzModel.Models;

namespace Visitz.Services
{
	public class GetNotesService(Vpi vpi) : VisitzApiService(vpi)
	{
		static readonly ConcurrentQueue<Task> notesQueue = new();
		static Task writeFromQueue;

		static Task EnqueueNotesTaskAsync(Action action)
		{
			Task task = new(action, TaskCreationOptions.PreferFairness);

			notesQueue.Enqueue(task);

			if (writeFromQueue == null || writeFromQueue.IsCompleted)
				writeFromQueue = CreateWriteFromQueueTask();

			return task;
		}

		// Disable warning because we're using Task functionality without 'await'
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
		static async Task CreateWriteFromQueueTask()
		{
			while (!notesQueue.IsEmpty)
				if (notesQueue.TryDequeue(out Task task))
					task.Start();
		}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        public static string MakeId(string caseIncidentId)
        {
            return nameof(GetNotesService) + caseIncidentId;
        }

        public static StartServiceMessage MakeStartMessage(string caseIncidentId, string entityType)
        {
            return MakeStartMessage((caseIncidentId, entityType));
        }

        public static StartServiceMessage MakeStartMessage(ValueTuple<string, string> idEntityItem)
        {
            return new StartServiceMessage()
            {
                ServiceId = MakeId(idEntityItem.Item1),
                ServiceType = typeof(GetNotesService),
                Payload = idEntityItem
            };
        }

        private ValueTuple<string, string> PayloadTuple => (ValueTuple<string, string>)Payload;

        public override string GetId()
        {
            var (caseIncidentId, _) = PayloadTuple;
            return MakeId(caseIncidentId);
        }

        protected override async Task RunApiServiceAsync()
        {
            await GetNotesAsync();
        }

        private async Task GetNotesAsync()
        {
            var (id, entityType) = PayloadTuple;

            var notesFromApi = await Vpi.GetNotesAsync(id, entityType);
            var newNotes = NoteItem.FromApiEntities(id, notesFromApi);

			await EnqueueNotesTaskAsync(async () =>
			{
				using var realm = await VisitzRealms.GetIcmDataRealmAsync();
				await NoteItem.UpsertNotesAsync(realm, id, entityType, newNotes);
			});

			ResultCode = Result.Successful;
        }
	}
}
