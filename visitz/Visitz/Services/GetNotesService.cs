using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzModel.Models;
using VisitzModel.Utilities;

namespace Visitz.Services
{
	public class GetNotesService(Vpi vpi) : VisitzApiService(vpi)
	{
		static readonly EagerActionQueue actionQueue = new();

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

			await actionQueue.EnqueueAsync(async () =>
			{
				using var realm = await VisitzRealms.GetIcmDataRealmAsync();
				await NoteItem.UpsertNotesAsync(realm, id, entityType, newNotes);
			});

			ResultCode = Result.Successful;
        }
	}
}
